using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Microsoft.Extensions.Logging;

using ServiceStack.Redis;

using Livescore.Domain.Aggregates.Discussion;
using Livescore.Application.Common.Dto;

namespace Livescore.Infrastructure.InMemory.Listeners.FixtureDiscussionListener {
    public class FixtureDiscussionListener : StreamListener, IFixtureDiscussionListener {
        public FixtureDiscussionListener(
            ILogger<FixtureDiscussionListener> logger,
            IRedisClientsManager redis
        ) : base(logger, redis) { }

        public async IAsyncEnumerable<FixtureDiscussionUpdateDto> ListenForDiscussionUpdates(
            CancellationToken stoppingToken
        ) {
            var commandArgs = new List<object>(64) { "XREAD", "BLOCK", "0", "STREAMS" };
            var commandStreamName = "discussions"; // @@TODO: Config.

            var time = DateTimeOffset.Now.ToUnixTimeMilliseconds() - 6 * 60 * 60 * 1000; // @@TODO: Config.
            var streamPositions = new List<KeyValuePair<string, string>> {
                new(commandStreamName, $"{time}-0")
            };

            await using var client = await _redis.GetClientAsync();

            while (true) {
                RedisText result;
                try {
                    _logger.LogInformation(
                        "Listening on {Streams}",
                        string.Join(", ", streamPositions.Select(streamPosition => streamPosition.Key))
                    );

                    result = await client.CustomAsync(_populateCommandArgs(commandArgs, streamPositions), stoppingToken);
                } catch (OperationCanceledException) {
                    yield break;
                }

                var streams = _parseResult(result);
                if (streams != null) {
                    foreach (var stream in streams) {
                        if (stream.Name == commandStreamName) {
                            foreach (var entry in stream.Entries) {
                                var identifier = entry.NamedValues.First(nv => nv.Key == "identifier").Value;
                                var command = entry.NamedValues.First(nv => nv.Key == "command").Value;
                                if (command == "sub") {
                                    streamPositions.Add(new(identifier, "$"));
                                } else { // unsub
                                    streamPositions.RemoveAll(streamPosition => streamPosition.Key.StartsWith(identifier));
                                }
                            }
                        } else {
                            var discussionEntries = new List<DiscussionEntryDto>(stream.Entries.Count);
                            foreach (var entry in stream.Entries) {
                                discussionEntries.Add(new DiscussionEntryDto {
                                    Id = entry.Id,
                                    UserId = long.Parse(entry.NamedValues.First(nv => nv.Key == nameof(DiscussionEntry.UserId)).Value),
                                    Username = entry.NamedValues.First(nv => nv.Key == nameof(DiscussionEntry.Username)).Value,
                                    Body = entry.NamedValues.First(nv => nv.Key == nameof(DiscussionEntry.Body)).Value
                                });
                            }

                            var streamNameSplit = stream.Name.Split('.');

                            var fixtureDiscussionUpdate = new FixtureDiscussionUpdateDto {
                                FixtureId = long.Parse(streamNameSplit[0].Split(':')[1]),
                                TeamId = long.Parse(streamNameSplit[1].Split(':')[1]),
                                DiscussionId = streamNameSplit[2].Split(':')[1],
                                Entries = discussionEntries
                            };

                            yield return fixtureDiscussionUpdate;
                        }

                        var index = streamPositions.FindIndex(streamPosition => streamPosition.Key == stream.Name);
                        streamPositions[index] = new(stream.Name, stream.Entries.Last().Id);
                    }
                }
            }
        }
    }
}
