using Avalonia.Controls;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using System.Linq;
using System.Threading.Tasks;
using MsBox.Avalonia.Enums;

namespace RandomItemGiverUpdater.Core.Util
{
    public static class Extensions
    {
        public static async Task<string?> OpenFolderAsync(this IStorageProvider storageProvider, Window window)
        {
            var result = await window.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Choose folder...",
                AllowMultiple = false
            });

            return result.FirstOrDefault()?.Path.LocalPath;
        }

        public static Control FindChild(this Panel panel, string name)
        {
            foreach (var child in panel.Children)
            {
                if (child.Name == name) return child;

                if (child is Panel p)
                {
                    Control c = p.FindChild(name);
                    if (c != null) return c;
                }
            }

            return null;
        }

        public static async Task<ButtonResult> MsgBox(this Window wnd, string text, string header = "Notification", ButtonEnum btn = ButtonEnum.Ok, Icon icon = Icon.Info)
        {
            var box = MessageBoxManager.GetMessageBoxStandard(header, text, btn, icon);
            ButtonResult result = await box.ShowWindowDialogAsync(wnd);

            return result;
        }
    }
}
