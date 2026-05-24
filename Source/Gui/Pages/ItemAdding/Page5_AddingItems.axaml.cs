using Avalonia.Controls;
using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Gui.Menus;

namespace RandomItemGiverUpdater.Gui.Pages.ItemAdding
{
    public partial class Page5_AddingItems : UserControl, IWizardPage
    {
        private wndAddItems wndAddItems;

        public Page5_AddingItems()
        {
            InitializeComponent();
        }

        public void Execute()
        {
            RIGU.itemAdding.AddItems();
        }

        public void UpdateProgress(double pbValue, int percentage, int totalItems, int finishedLootTables)
        {
            //Report status of worker
            pbAddingItems.Value = pbValue;
            tblAddingItemsProgress.Text = $"Adding items... (Item {percentage}/{totalItems} - Loot Table {finishedLootTables}/{RIGU.core.currentDatapack.GetLootTableAmount()})";
        }
    }
}
