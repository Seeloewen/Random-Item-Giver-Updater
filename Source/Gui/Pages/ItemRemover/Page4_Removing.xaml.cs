using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Gui.Menus;
using System;
using System.Collections.Generic;
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

namespace RandomItemGiverUpdater.Gui.Pages.ItemRemover
{
    public partial class Page4_Removing : Page, IWizardPage
    {
        private wndRemoveItems wndRemoveItems;

        public Page4_Removing(wndRemoveItems wndRemoveItems)
        {
            InitializeComponent();

            this.wndRemoveItems = wndRemoveItems;
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
