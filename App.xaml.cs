using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using RandomItemGiverUpdater.Core;

namespace RandomItemGiverUpdater
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            RenderOptions.ProcessRenderMode = RenderMode.Default;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //Initialize application
            Main main = new Main();

            RIGU.Initialize(main);
        }
    }
}
