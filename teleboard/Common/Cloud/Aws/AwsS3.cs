//using Amazon;
//using Amazon.S3;
//using Amazon.S3.Model;
//using System.IO;
//using System.Threading.Tasks;
//using Teleboard.Common.Configuration;

//namespace Teleboard.Common.Cloud.Aws
//{
//    public static class AwsS3
//    {
//        public static async Task PutObjectAsync(string key, Stream inputStream)
//        {
//            using (var client = CreateClient())
//            {
//                await client.PutObjectAsync(new PutObjectRequest()
//                {
//                    Key = key,
//                    InputStream = inputStream,
//                    BucketName = AppConfiguration.AwsS3BucketName,
//                });
//            }
//        }

//        public static async Task PutObjectAsync(string key, string fileName)
//        {
//            using (var client = CreateClient())
//            {
//                await client.PutObjectAsync(new PutObjectRequest()
//                {
//                    Key = key,
//                    FilePath = fileName,
//                    BucketName = AppConfiguration.AwsS3BucketName,
//                });
//            }
//        }

//        public static async Task DeleteObjectAsync(string key)
//        {
//            using (var client = CreateClient())
//            {
//                await client.DeleteObjectAsync(new DeleteObjectRequest()
//                {
//                    Key = key,
//                    BucketName = AppConfiguration.AwsS3BucketName,
//                });
//            }
//        }

//        public static string GetObjectUrl(string key)
//        {
//            return Path.Combine(AppConfiguration.AwsS3Url, AppConfiguration.AwsS3BucketName, key)
//                .Replace(@"\", "/");
//        }

//        private static AmazonS3Client CreateClient()
//        {
//            return new AmazonS3Client(AppConfiguration.AwsS3AccessKeyID, AppConfiguration.AwsS3SecretAccessKey, RegionEndpoint.EUWest2);
//        }
//    }
//}
