using RandomItemGiverUpdater.Core;
using System.Diagnostics;
using System.Windows;

namespace RandomItemGiverUpdater.Gui.Menus
{
    public partial class wndAbout : Window
    {
        public wndAbout()
        {
            InitializeComponent();
            SetupControls();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e) => Close();

        private void hlGithub_Click(object sender, RoutedEventArgs e) => Process.Start("https://github.com/seeloewen/random-item-giver-updater");

        private void btnThirdPartyLicenses_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Json.NET (https://github.com/JamesNK/Newtonsoft.Json/)\n\nThe MIT License (MIT)\r\n\r\nCopyright (c) 2007 James Newton-King\r\n\r\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\r\n\r\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.\r\n\r\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.", "Third-Party Licenses", MessageBoxButton.OK, MessageBoxImage.Information);

        private void btnChangelog_Click(object sender, RoutedEventArgs e)
        {
            wndChangelog wndChangelog = new wndChangelog();
            wndChangelog.ShowDialog();
        }

        private void SetupControls()
        {
            //Setup header
            tblHeader.Text = $"Random Item Giver Updater\nVersion {RIGU.VERSION_NUM} ({RIGU.VERSION_DATE})\nMade by Seeloewen";
        }
    }
}
