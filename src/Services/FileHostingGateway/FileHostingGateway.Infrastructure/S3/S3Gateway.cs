using System;
using System.Threading.Tasks;

using FileHostingGateway.Application.Common.Interfaces;

namespace FileHostingGateway.Infrastructure.S3 {
    public class S3Gateway : IS3Gateway {
        public async Task<string> UploadImage(string filePath) {
            throw new NotImplementedException();
            // /**/user-files/**/image.png
            //var ext = Path.GetExtension(filePath);
            //var putRequest = new PutObjectRequest {
            //    BucketName = bucket,
            //    Key = filePath.Substring(filePath.IndexOf("user-files")),
            //    FilePath = filePath,
            //    ContentType = "image/" + (ext == ".jpg" || ext == ".jpeg" ? "jpeg" : "png")
            //};

            //await s3.PutObjectAsync(putRequest);

            //File.Delete(filePath);

            //return $"https://{putRequest.BucketName}.s3.{region}.amazonaws.com/{putRequest.Key}";
        }
    }
}
