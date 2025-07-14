using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Gui.Pages;
using RandomItemGiverUpdater.Gui.Pages.ItemAdding;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace RandomItemGiverUpdater.Gui.Menus
{
    public partial class wndAddItems : Window
    {
        private ItemAdding itemAddingCore;
        private int currentPage = 1;

        private TextBlock tblLoadingItems = new TextBlock();

        private IWizardPage[] pages = new IWizardPage[6];

        //-- Constructor --//

        public wndAddItems()
        {
            InitializeComponent();

            itemAddingCore = RIGU.itemAdding;
            DataContext = itemAddingCore;

            InitUI();
        }

        private void InitUI()
        {
            //Setup pages
            pages[0] = new Page1_Start(this);
            pages[1] = new Page2_ItemList(this);
            pages[2] = new Page3_ItemsAdvanced(this);
            pages[3] = new Page4_LootTables(this);
            pages[4] = new Page5_AddingItems(this);
            pages[5] = new Page6_Finished(this);

            GetPage<Page1_Start>(1).SetDatapack(RIGU.wndMain.currentDatapack);
        }

        public void ShowNextPage()
        {
            if (currentPage == 6) Close();

            frWizard.Content = pages[++currentPage];
        }

        public void ShowPreviousPage()
        {
            if (currentPage == 1) Close();

            frWizard.Content = pages[--currentPage];
        }

        //Basically a wrapper so I can use numbers from 1 to 6.
        //You need to specify the specific page class to use it properly
        public T GetPage<T>(int i) where T : IWizardPage => (T)pages[i + 1];

        public void UpdateProgress(double pbValue, int percentage, int totalItems, int finishedLootTables)
        {
            GetPage<Page5_AddingItems>(5).UpdateProgress(pbValue, percentage, totalItems, finishedLootTables);
        }

        private void wndAddItem1_Closing(object sender, CancelEventArgs e) => RIGU.wndMain.ReloadLootTable();
    }
}
