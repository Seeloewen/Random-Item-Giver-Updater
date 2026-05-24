using Avalonia.Controls;
using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Gui.Menus;

namespace RandomItemGiverUpdater.Gui.Pages.ItemRemover
{
    public partial class Page4_Removing : UserControl, IWizardPage
    {
        private wndRemoveItems wndRemoveItems;

        public Page4_Removing()
        {
            InitializeComponent();
        }

        public void Execute()
        {
            RIGU.itemRemover.Run();
        }

        public void UpdateProgress(double pbValue, int percentage, int totalItems, int finishedLootTables)
        {
            //Report status of worker
            pbItemRemoving.Value = pbValue;
            tblItemRemovingProgress.Text = $"Removing items... (Item {percentage}/{totalItems} - Loot Table {finishedLootTables}/{RIGU.core.currentDatapack.GetLootTableAmount()})";
        }
    }
}