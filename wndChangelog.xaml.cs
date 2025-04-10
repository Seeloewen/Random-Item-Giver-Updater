using System.Windows;

namespace Random_Item_Giver_Updater
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
