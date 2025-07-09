using System.Windows;

namespace RandomItemGiverUpdater.Gui.Menus
{
    public partial class wndChangelog : Window
    {

        //-- Constructor --//

        public wndChangelog()
        {
            InitializeComponent();
        }

        //-- Event Handlers --//

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            //Close the window
            Close();
        }
    }
}
