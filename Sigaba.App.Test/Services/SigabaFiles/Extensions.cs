namespace Sigaba.App.Services.SigabaFiles;

internal static class Extensions
{
    extension(string str)
    {
        /// <summary>
        /// Converts a string to a path, splitting on both forward and backward slashes, depending on the current platform. 
        /// This is useful for handling paths in a cross-platform manner.
        /// </summary>
        /// <returns></returns>
        public string AsPath()
        {
            if (str == null)
                return null;
            return Path.Combine(str.Split(['/', '\\']));
        }
    }
}
