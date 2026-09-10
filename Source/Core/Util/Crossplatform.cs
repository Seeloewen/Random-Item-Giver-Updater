using Avalonia.Controls;
using MsBox.Avalonia.Enums;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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

        public static async Task<ButtonResult> Dialog(Window wnd, string message, string header = "Notification", ButtonEnum btn = ButtonEnum.Ok, Icon icon = Icon.Info)
        {
            return await wnd.MsgBox(message, header, btn, icon);
        }
    }
}
