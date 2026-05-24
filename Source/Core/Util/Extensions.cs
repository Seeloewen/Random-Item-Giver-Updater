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
    }
}
