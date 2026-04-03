using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Gui.Pages.DuplicateFinder;
using System.Windows;

namespace RandomItemGiverUpdater.Gui.Menus
{
    public partial class wndDuplicateFinder : Wizard
    {
        private DuplicateFinder duplicateFinder;

        public wndDuplicateFinder()
        {
            InitializeComponent();
            duplicateFinder = RIGU.duplicateFinder;
            DataContext = duplicateFinder;

            InitUI();
        }

        private void InitUI()
        {
            //Setup pages
            pages[0] = new Page1_Start(this);
            pages[1] = new Page2_Finished(this);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            duplicateFinder.DisplayItemRemover();
            Close();
        }

        public void UpdateDuplicateDisplay(int amount)
        {
            //Relay the call to the appropriate page
            GetPage<Page2_Finished>(2).UpdateDuplicateDisplay(amount);
        }
    }
}