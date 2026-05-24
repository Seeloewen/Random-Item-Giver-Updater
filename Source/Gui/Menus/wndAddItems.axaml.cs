using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Workspace;
using RandomItemGiverUpdater.Gui.Pages.ItemAdding;
using System.ComponentModel;
using Avalonia.Controls;

namespace RandomItemGiverUpdater.Gui.Menus
{
    public partial class wndAddItems : Wizard
    {
        private ItemAdding itemAddingCore;

        public wndAddItems()
        {
            InitializeComponent();

            itemAddingCore = RIGU.itemAdding;
            DataContext = itemAddingCore;
            Init(frWizard, 6);
            InitUI();

            ShowPage(1);
        }

        private void InitUI()
        {
            //Setup pages
            //TODO: Set wndAddItems for Pages
            pages[0] = new Page1_Start();
            pages[1] = new Page2_ItemList();
            pages[2] = new Page3_ItemsAdvanced();
            pages[3] = new Page4_LootTables();
            pages[4] = new Page5_AddingItems();
            pages[5] = new Page6_Finished();

            GetPage<Page1_Start>(1).SetDatapack(RIGU.core.currentDatapack);
            GetPage<Page3_ItemsAdvanced>(3).DataContext = DataContext;
            GetPage<Page4_LootTables>(4).DataContext = DataContext;
        }

        public void UpdateProgress(double pbValue, int percentage, int totalItems, int finishedLootTables)
        {
            GetPage<Page5_AddingItems>(5).UpdateProgress(pbValue, percentage, totalItems, finishedLootTables);
        }

        private void wndAddItem1_Closing(object sender, WindowClosingEventArgs e) => RIGU.core.wndMain.ReloadWorkspace();
    }
}