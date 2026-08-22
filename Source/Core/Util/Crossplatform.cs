using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RandomItemGiverUpdater.Core.Util
{
    internal class Crossplatform
    {
        public static void OpenUrl(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
        }
    }
}
