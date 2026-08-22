using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
