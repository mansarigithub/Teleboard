using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using Teleboard.Models;

namespace Teleboard
{
    public class ImageResizer
    {
        public static void ResizeCompressImage(ContentUploadViewModel uploadModel, string fileNameWithPath, int desireWidth, string postFixName)
        {
            // Algorithm simplified for purpose of example.
            Image originalImage = Image.FromStream(uploadModel.FileStream.InputStream, true, true);
            int height = originalImage.Height;
            int width = originalImage.Width;

            if (originalImage.Width > desireWidth)
            {
                height = (originalImage.Height * desireWidth) / originalImage.Width;
                width = desireWidth;
            }

            // Now create a new image
            using (Image newImage = originalImage.GetThumbnailImage(width, height, new Image.GetThumbnailImageAbort(AbortThumbnailImage), IntPtr.Zero))
            {
                newImage.Save(RenameImageFileWithPostFix(fileNameWithPath, postFixName));
            }
        }

        public static string RenameImageFileWithPostFix(string fileNameWithPath, string postFixName)
        {
            var name = fileNameWithPath.ToLower().Replace(Path.GetExtension(fileNameWithPath), postFixName + Path.GetExtension(fileNameWithPath));
            return name;
        }

        private static bool AbortThumbnailImage()
        {
            throw new NotImplementedException();
        }
    }
}