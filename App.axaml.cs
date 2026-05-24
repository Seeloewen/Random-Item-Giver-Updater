using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Workspace;

namespace RandomItemGiverUpdater;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var main = new Main();

            RIGU.Initialize(main);

            desktop.MainWindow = main.wndMain;
            main.wndMain.Show();
        }

        base.OnFrameworkInitializationCompleted();
    }
}