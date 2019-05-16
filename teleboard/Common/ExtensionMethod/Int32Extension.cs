namespace Teleboard.Common.ExtensionMethod
{
    public static class Int32Extension
    {
        public static string ToFileSizeString(this int bytes)
        {
            var kb = (float)bytes / 1024;
            var mb = kb / 1024;

            var sizeString = string.Empty;

            if (kb < 1024)
                sizeString = string.Format("{0:F1} KB", kb);
            else
                sizeString = string.Format("{0:F1} MB", mb);

            return sizeString;
        }
    }
}
