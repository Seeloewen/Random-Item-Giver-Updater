using RandomItemGiverUpdater.Gui.Menus;
using System.Windows.Controls;

namespace RandomItemGiverUpdater.Gui.Pages.ItemAdding
{
    public partial class Page5_AddingItems : Page, IWizardPage
    {
        private wndAddItems wndAddItems;

        public Page5_AddingItems(wndAddItems wndAddItems)
        {
            InitializeComponent();
            this.wndAddItems = wndAddItems;
        }

        public void Execute()
        {
            RIGU.itemAdding.AddItems();
        }

        public void UpdateProgress(double pbValue, int percentage, int totalItems, int finishedLootTables)
        {
            //Report status of worker
            pbAddingItems.Value = pbValue;
            tblAddingItemsProgress.Text = $"Adding items... (Item {percentage}/{totalItems} - Loot Table {finishedLootTables}/{RIGU.wndMain.lootTableList.Count})";
        }
    }
}
