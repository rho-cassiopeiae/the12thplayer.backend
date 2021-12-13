using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using FileHostingGateway.Application.Common.Errors;
using FileHostingGateway.Application.Common.Interfaces;
using FileHostingGateway.Application.Common.Results;
using FileHostingGateway.Application.Commands.UploadVideo;

namespace FileHostingGateway.Infrastructure.Vimeo {
    public class VimeoGateway : IVimeoGateway {
        private readonly IHttpClientFactory _clientFactory;

        private readonly string _vimeoApiBaseAddress;
        private readonly TimeSpan _pollInterval;

        public VimeoGateway(
            IConfiguration configuration,
            IHttpClientFactory clientFactory
        ) {
            _clientFactory = clientFactory;

            _vimeoApiBaseAddress = configuration["Vimeo:BaseAddress"];
            _pollInterval = TimeSpan.FromSeconds(
                configuration.GetValue<int>("Vimeo:PollIntervalSec")
            );
        }

        public async Task<Either<VimeoError, string>> AddProjectFor(long fixtureId, long teamId) {
            var client = _clientFactory.CreateClient("vimeo");

            // @@NOTE: Vimeo allows multiple projects with the same name.
            var projectName = $"f-{fixtureId}-t-{teamId}";

            var request = new HttpRequestMessage(HttpMethod.Get, _vimeoApiBaseAddress + "/me/projects");

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode) {
                return new VimeoError($"Error creating Vimeo project (1): {response.ReasonPhrase}");
            }

            var responseMap = JsonSerializer.Deserialize<IDictionary<string, JsonElement>>(
                await response.Content.ReadAsStringAsync()
            );

            foreach (var projectMap in responseMap["data"].EnumerateArray()) {
                if (projectMap.GetProperty("name").GetString() == projectName) {
                    return projectMap.GetProperty("uri").GetString().Split('/').Last();
                }
            }

            var body = $@"{{
                ""name"": ""{projectName}""
            }}";

            request = new HttpRequestMessage(HttpMethod.Post, _vimeoApiBaseAddress + "/me/projects");
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode) {
                return new VimeoError($"Error creating Vimeo project (2): {response.ReasonPhrase}");
            }

            responseMap = JsonSerializer.Deserialize<IDictionary<string, JsonElement>>(
                await response.Content.ReadAsStringAsync()
            );

            var projectId = responseMap["uri"].GetString().Split('/').Last();

            return projectId;
        }

        public async Task<Either<VimeoError, VideoStreamingInfoDto>> UploadVideo(
            string filePath, string projectId
        ) {
            // wwwroot/user-files/video-reactions/f-123-t-789/random-name.ext

            var client = _clientFactory.CreateClient("vimeo");

            long fileSize = new FileInfo(filePath).Length;
            var body = $@"{{
                ""upload"": {{
                    ""approach"": ""tus"",
                    ""size"": ""{fileSize}""
                }},
                ""name"": ""{Path.GetFileNameWithoutExtension(filePath)}""
            }}";

            var request = new HttpRequestMessage(HttpMethod.Post, _vimeoApiBaseAddress + "/me/videos");
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode) {
                return new VimeoError($"Error uploading file to Vimeo (1): {response.ReasonPhrase}");
            }

            var responseMap = JsonSerializer.Deserialize<IDictionary<string, JsonElement>>(
                await response.Content.ReadAsStringAsync()
            );

            var uploadMap = responseMap["upload"];
            string approach;
            if ((approach = uploadMap.GetProperty("approach").GetString()) != "tus") {
                return new VimeoError($"Error uploading file to Vimeo (2): Invalid approach: {approach}");
            }

            var uploadLink = uploadMap.GetProperty("upload_link").GetString();
            long uploadOffset = 0;
            using (var fs = File.OpenRead(filePath)) {
                var chunkSize = (int) Math.Min(256 * 1024 * 1024, fileSize); // @@NOTE: Vimeo recommends 128-512Mb-sized chunks.
                var buffer = ArrayPool<byte>.Shared.Rent(chunkSize);
                try {
                    while (uploadOffset < fileSize) {
                        fs.Seek(uploadOffset, SeekOrigin.Begin);
                        int n = await fs.ReadAsync(buffer, 0, chunkSize);

                        request = new HttpRequestMessage(HttpMethod.Patch, uploadLink);
                        request.Headers.Add("Tus-Resumable", "1.0.0");
                        request.Headers.Add("Upload-Offset", uploadOffset.ToString());

                        var content = new ByteArrayContent(buffer, 0, n);
                        content.Headers.Add("Content-Type", "application/offset+octet-stream");
                        request.Content = content;

                        response = await client.SendAsync(request);
                        if (!response.IsSuccessStatusCode) {
                            return new VimeoError($"Error uploading file to Vimeo (3): {response.ReasonPhrase}");
                        }

                        uploadOffset = long.Parse(response.Headers.GetValues("Upload-Offset").First());
                    }
                } finally {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }

            var videoId = responseMap["uri"].GetString().Split('/').Last();

            // @@NOTE: There is no "transcode complete" hook, so we have to poll.
            var transcodeComplete = false;
            while (!transcodeComplete) {
                await Task.Delay(_pollInterval);

                request = new HttpRequestMessage(
                    HttpMethod.Get,
                    _vimeoApiBaseAddress + $"/videos/{videoId}?fields=transcode.status"
                );

                response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode) {
                    return new VimeoError($"Error uploading file to Vimeo (4): {response.ReasonPhrase}");
                }

                responseMap = JsonSerializer.Deserialize<IDictionary<string, JsonElement>>(
                    await response.Content.ReadAsStringAsync()
                );

                var transcodeStatus = responseMap["transcode"].GetProperty("status").GetString();
                if (string.Equals(transcodeStatus, "error", StringComparison.OrdinalIgnoreCase)) {
                    return new VimeoError($"Error uploading file to Vimeo (5): Transcoding error");
                } else if (string.Equals(transcodeStatus, "complete", StringComparison.OrdinalIgnoreCase)) {
                    transcodeComplete = true;
                }
            }

            request = new HttpRequestMessage(
                HttpMethod.Put,
                _vimeoApiBaseAddress + $"/me/projects/{projectId}/videos/{videoId}"
            );

            response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode) {
                // @@NOTE: Means that the project no longer exists.
                // @@TODO: Delete video.
                return new VimeoError($"Error uploading file to Vimeo (6): {response.ReasonPhrase}");
            }

            var oldClient = _clientFactory.CreateClient("vimeo-old");
            JsonElement thumbnail;
            while (true) {
                request = new HttpRequestMessage(HttpMethod.Get, $"/video/{videoId}.json");

                response = await oldClient.SendAsync(request);
                if (response.IsSuccessStatusCode) {
                    responseMap = JsonSerializer.Deserialize<IList<IDictionary<string, JsonElement>>>(
                        await response.Content.ReadAsStringAsync()
                    ).First();

                    bool thumbnailReady =
                        responseMap.TryGetValue("thumbnail_large", out thumbnail) ||
                        responseMap.TryGetValue("thumbnail_medium", out thumbnail);

                    if (thumbnailReady) {
                        break;
                    }
                }

                await Task.Delay(_pollInterval);
            }

            return new VideoStreamingInfoDto {
                VideoId = videoId,
                ThumbnailUrl = thumbnail.GetString()
            };
        }
    }
}
