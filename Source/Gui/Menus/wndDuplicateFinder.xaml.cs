using Newtonsoft.Json.Linq;
using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Entries;
using RandomItemGiverUpdater.Gui.Pages.DuplicateFinder;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

            SetupControls();

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



        //-- Duplicate Entry Event Handlers --//

        private void btnViewAll_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                //Get the canvas which the button is in
                Canvas canvas = SeeloewenLibTools.FindVisualParent<Canvas>(button);

                //Show a message in which loot tables the duplicate occurs
                if (canvas.DataContext is DuplicateEntry duplicateEntry)
                {
                    MessageBox.Show(string.Format("The duplicate occurs in the following loot tables:\n{0}", duplicateEntry.lootTables.Replace(", ", "\n")), "View all Loot Tables", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void tblItemName_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                //Show the controls for editing and hide the original name
                MessageBox.Show(string.Format("Full name of the item:\n{0}", textBlock.Text), "Full item name", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}