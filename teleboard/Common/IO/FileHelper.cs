using System;
using System.IO;

namespace Teleboard.Common.IO
{
    public static class FileHelper
    {
        public static string GetFileMimeTypeFromData(Stream fileStream)
        {
            byte[] bytes = new byte[4];
            fileStream.Read(bytes, 0, 4);
            fileStream.Seek(0, SeekOrigin.Begin);

            string signature = BitConverter.ToString(bytes).Replace("-", string.Empty);

            return GetMimeType(signature);
        }

        private static string GetMimeType(string signature)
        {
            switch (signature)
            {
                case "89504E47":
                    return "image/png";
                case "47494638":
                    return "image/gif";
                case "FFD8FFE1":
                case "FFD8FFE0":
                case "FFD8FFDB":
                    return "image/jpeg";
                case "424D8644":
                case "424DA6C5":
                case "424DC6E4":
                case "424D1EBB":
                    return "image/bmp";
                case "00018":
                case "00000018":
                case "00020":
                case "00000020":
                    return "video/mp4";
                default:
                    return "Unknown";
            }
        }
    }
}