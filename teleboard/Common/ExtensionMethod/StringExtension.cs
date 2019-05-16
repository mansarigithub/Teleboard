using System;
using System.IO;
using System.Linq;

namespace Teleboard.Common.ExtensionMethod
{
    public static class StringExtension
    {
        public static bool IsImageExtension(this string fileExtension)
        {
            return fileExtension == "jpeg" ||
                        fileExtension == "jpg" ||
                        fileExtension == "bmp" ||
                        fileExtension == "png" ||
                        fileExtension == "gif";
        }

        public static string ToValidFileNameWithTimeStamp(this string fileName)
        {
            fileName = fileName.Trim().ToLower();
            var name = Path.GetFileNameWithoutExtension(fileName);
            if (name.Length > 50)
                name = name.Substring(0, 50);
            name = string.Concat(name.Select(c => char.IsLetterOrDigit(c) || c == '.' ? c : '_'));
            return string.Format("{0}_{1}{2}",
                DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                name,
                Path.GetExtension(fileName));
        }

        public static string GetThumbnailName(this string fileName, bool preservePath = false)
        {
            //if (preservePath)
            //{
            //    var directory = Path.GetDirectoryName(fileName);
            //    var thumbnailName = $"{Path.GetFileNameWithoutExtension(fileName)}{Path.GetExtension(fileName)}_thumb";
            //    return Path.Combine(directory, thumbnailName).Replace(@"\", "/");
            //}
            //else
            //{
            //    return $"{Path.GetFileNameWithoutExtension(fileName)}{Path.GetExtension(fileName)}_thumb";
            //}
            return $"{fileName}_thumb";
        }

        public static string Cut(this string str, int count)
        {
            return str.Length <= count ? str : $"{str.Substring(0, count)}...";
        }
    }
}
