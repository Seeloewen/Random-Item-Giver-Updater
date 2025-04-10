using System.Windows;
using System.Windows.Media;

namespace Random_Item_Giver_Updater
{

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.Default;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //Initialize application
            wndMain wndMain = new wndMain();

            RIGU.Initialize(wndMain);

            wndMain.Show();
        }
    }
}
