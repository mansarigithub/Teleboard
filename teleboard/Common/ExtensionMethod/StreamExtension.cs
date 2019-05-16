using System.IO;

namespace Teleboard.Common.ExtensionMethod
{
    public static class StreamExtension
    {
        public static void SaveAs(this Stream stream, string fileName)
        {
            using (var fileStream = File.Create(fileName))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }
        }
    }
}
