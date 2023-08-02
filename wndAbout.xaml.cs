using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Random_Item_Giver_Updater
{
    public partial class wndAbout : Window
    {

        //Reference to main window
        public MainWindow wndMain = (MainWindow)Application.Current.MainWindow;

        //Windows
        private wndChangelog wndChangelog;

        //-- Constructor --//

        public wndAbout()
        {
            InitializeComponent();
            SetupControls();
        }

        //-- Event Handlers --//

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            //Close the window
            Close();
        }

        private void btnChangelog_Click(object sender, RoutedEventArgs e)
        {
            //Open the changelog
            wndChangelog = new wndChangelog();
            wndChangelog.Owner = Application.Current.MainWindow;
            wndChangelog.ShowDialog();
        }

        private void hlGithub_Click(object sender, RoutedEventArgs e)
        {
            //Open the GitHub website of the Random Item Giver Updater
            Process.Start("https://github.com/seeloewen/random-item-giver-updater");
        }

        //-- Custom Methods --//

        private void SetupControls()
        {
            //Setup header
            tblHeader.Text = string.Format("Random Item Giver Updater\nVersion {0} ({1})\nMade by Seeloewen", wndMain.versionNumber, wndMain.versionDate);
        }
    }
}
