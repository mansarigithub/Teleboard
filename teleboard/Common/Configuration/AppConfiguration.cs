using System.Configuration;
using System.IO;

namespace Teleboard.Common.Configuration
{
    public class AppConfiguration
    {
        public static string AppStoragePath
        {
            get
            {
                return Get("AppStoragePath");
            }
        }

        public static string AwsS3AccessKeyID
        {
            get
            {
                return Get("AwsS3AccessKeyID");
            }
        }

        public static string AwsS3SecretAccessKey
        {
            get
            {
                return Get("AwsS3SecretAccessKey");
            }
        }

        public static string AppTempDirectory
        {
            get
            {
                return Path.Combine(AppStoragePath, "Temp");
            }
        }

        public static string AwsS3BucketName
        {
            get
            {
                return Get("AwsS3BucketName");
            }
        }

        public static string AwsS3Url
        {
            get
            {
                return Get("AwsS3Url");
            }
        }

        private static string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}