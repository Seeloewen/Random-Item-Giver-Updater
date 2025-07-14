using RandomItemGiverUpdater.Gui.Menus;
using System.Windows;
using System.Windows.Controls;

namespace RandomItemGiverUpdater.Gui.Pages.ItemAdding
{
    public partial class Page2_ItemList : Page, IWizardPage
    {
        private wndAddItems wndAddItems;

        public Page2_ItemList(wndAddItems wndAddItems)
        {
            InitializeComponent();

            this.wndAddItems = wndAddItems;
        }

        public void Execute()
        {
            string[] items = tbItemName.Text.Split('\n');

            //Create an entry for every item
            RIGU.itemAdding.itemEntries.Clear();
            int index = 0;

            for (int i = 0; i < items.Length; i++)
            {
                //Split the item into prefix and item name if checkbox for prefixes is checked
                //If the line actually doesn't contain a prefix, add the item normally with default prefix
                if (cbIncludesPrefixes.IsChecked == true)
                {
                    string[] entry = items[i].Split(':');
                    RIGU.itemAdding.itemEntries.Add(new AddingEntry(entry.Length != 2 ? "minecraft" : entry[0], entry.Length != 2 ? entry[0] : entry[1], index));
                }
                else
                {
                    RIGU.itemAdding.itemEntries.Add(new AddingEntry("minecraft", items[i], index));
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) => wndAddItems.ShowNextPage();

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbItemName.Text))
            {
                wndAddItems.ShowNextPage();
            }
            else
            {
                MessageBox.Show("Please enter some items before continuing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
