using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;
using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Workspace;
using System.Diagnostics;

namespace RandomItemGiverUpdater;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Dispatcher.UIThread.UnhandledException += OnUnhandledException;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var main = new Main();

            RIGU.Initialize(main);

            desktop.MainWindow = main.wndMain;
            main.wndMain.Show();
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var ex = e.Exception;

        string args = "";
        if (ex.InnerException != null)
        {
            args = $"\"Random Item Giver Updater\" \"{RIGU.VERSION_NUM}\" \"{ex.InnerException.Message}\" \"{ex.InnerException.StackTrace}\" \"{Process.GetCurrentProcess().MainModule!.FileName}\"";
        }
        else
        {
            args = $"\"Random Item Giver Updater\" \"{RIGU.VERSION_NUM}\" \"{ex.Message}\" \"{ex.StackTrace}\" \"{Process.GetCurrentProcess().MainModule!.FileName}\"";
        }

        //Display SealCrashHandler with the current exception
        Process.Start(new ProcessStartInfo
        {
            FileName = "SealCrashHandler.exe",
            UseShellExecute = true,
            Arguments = args
        });
    }
}