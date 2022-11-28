using System;
using System.IO;
using System.Runtime.InteropServices;

namespace WixSharp.Fluent.Redistributables
{
    internal static class DownloadsFolder
    {
#if WIXSHARP_FLUENT_CACHE_IN_DOWNLOADS
        private static Guid folderDownloads = new Guid("374DE290-123F-4565-9164-39C4925E467B");

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHGetKnownFolderPath(ref Guid id, int flags, IntPtr token, out IntPtr path);

        static DownloadsFolder()
        {
            if (Environment.OSVersion.Version.Major < 6)
                throw new NotSupportedException("Cannot get Downloads folder on XP and older");//this only matters for installer builder

            IntPtr pathPtr = IntPtr.Zero;

            try
            {
                SHGetKnownFolderPath(ref folderDownloads, 0, IntPtr.Zero, out pathPtr);
                DownloadsPath = Marshal.PtrToStringUni(pathPtr);
            }
            catch
            {
                throw new Exception("Cannot find Downloads folder");
            }
            finally
            {
                Marshal.FreeCoTaskMem(pathPtr);
            }
        }
#else
        static DownloadsFolder()
        {
            DownloadsPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\.WixSharp.Fluent\Redistributables\Cache");
            Directory.CreateDirectory(DownloadsPath);
        }
#endif

        internal static string DownloadsPath { get; private set; }
    }
}
