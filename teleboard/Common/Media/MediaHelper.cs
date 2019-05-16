using NReco.VideoConverter;
using System;
using System.Drawing;
using Teleboard.Common.Enum;

namespace Teleboard.Common.Media
{
    public static class MediaHelper
    {
        public static void CreateThumbnail(string fileName, string thumbnailFileName, ResourceType resourceType)
        {
            if (resourceType == ResourceType.Video)
            {
                var ffMpeg = new FFMpegConverter();
                var videoDuration = GetVideoDuration(fileName);
                ffMpeg.GetVideoThumbnail(fileName, thumbnailFileName, (float)videoDuration / 2);
            }
            else if (resourceType == ResourceType.Image)
            {
                using (var image = Image.FromFile(fileName))
                {
                    const int MAX_WIDTH = 250;
                    int width = image.Width > MAX_WIDTH ? MAX_WIDTH : image.Width;
                    int height = image.Width > MAX_WIDTH ?
                        (int)(((float)MAX_WIDTH / image.Width) * image.Height) :
                        image.Height;

                    using (Image newImage = image.GetThumbnailImage(width, height, null, IntPtr.Zero))
                    {
                        newImage.Save(thumbnailFileName, image.RawFormat);
                    }
                }
            }
        }

        public static ResourceType GetResourceType(string mimeType)
        {
            if (mimeType == null)
                return ResourceType.Unknown;
            if (mimeType.StartsWith("video"))
                return ResourceType.Video;
            else if (mimeType.StartsWith("image"))
                return ResourceType.Image;
            else
                return ResourceType.Unknown;
        }

        public static double GetVideoDuration(string fileName)
        {
            try
            {
                var ffProbe = new NReco.VideoInfo.FFProbe();
                return ffProbe.GetMediaInfo(fileName).Duration.TotalSeconds;
            }
            catch
            {
                return 0;
            }
        }

        public static string ToDurationString (int totallSeconds)
        {
            var s = TimeSpan.FromSeconds(totallSeconds);
            return string.Format("{0}:{1}:{2}", s.Hours, s.Minutes, s.Seconds);
        }
    }
}
