using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Workspace.Entries;
using RandomItemGiverUpdater.Gui.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace RandomItemGiverUpdater.Gui.Pages.ItemRemover
{
    public partial class Page5_Finished : UserControl, IWizardPage
    {
        private wndRemoveItems wndRemoveItems;

        public Page5_Finished()
        {
            InitializeComponent();

            this.wndRemoveItems = wndRemoveItems;
        }

        public void Execute()
        {
            foreach (RemovalEntry entry in RIGU.itemRemover.removalEntries)
            {
                tbRemovedItems.Text += entry.name + "\n";
            }
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            RIGU.core.ReloadLootTable();
            wndRemoveItems.ShowNextPage();
        }
    }
}