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
        private ItemAddingCore itemAddingCore;
        private int currentPage = 1;

        private TextBlock tblLoadingItems = new TextBlock();

        private IWizardPage[] pages = new IWizardPage[6];

        //-- Constructor --//

        public wndAddItems()
        {
            InitializeComponent();
            InitUI();

            itemAddingCore = RIGU.itemAddingCore;
            DataContext = itemAddingCore;
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


        //-- Add To Loot Table Entry Event Handlers --//

        private void rbtnAllLootTables_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radiobutton)
            {
                //Get the canvas which the radiobutton is in
                Canvas canvas = SeeloewenLib.Tools.FindVisualParent<Canvas>(radiobutton);

                if (canvas.DataContext is AddingEntry item)
                {
                    //Disable edit button
                    Button button = canvas.FindName("btnEditCertainLootTables") as Button;
                    button.IsEnabled = false;

                    //Set checkstate on variable that gets accessed by the item adding thread
                    item.defaultLootTables = true;
                }
            }
        }

        private void rbtnCertainLootTables_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radiobutton)
            {
                //Get the canvas which the radiobutton is in
                Canvas canvas = SeeloewenLibTools.FindVisualParent<Canvas>(radiobutton);

                if (canvas.DataContext is AddingEntry item)
                {
                    //Enable edit button
                    Button button = canvas.FindName("btnEditCertainLootTables") as Button;
                    button.IsEnabled = true;

                    //Set checkstate on variable that gets accessed by the item adding thread
                    item.defaultLootTables = false;
                }
            }
        }

        private void tbItemName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textbox && textbox.DataContext is AddingEntry item)
            {
                //Set checkstate on variable that gets accessed by the item adding thread
                item.id = textbox.Text;
                item.name = $"{item.prefix}:{item.id}";
            }
        }

        private void tbItemPrefix_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textbox && textbox.DataContext is AddingEntry item)
            {
                //Set checkstate on variable that gets accessed by the item adding thread
                item.id = textbox.Text;
                item.name = $"{item.prefix}:{item.id}";
            }
        }

        private void btnNbtComponentEditor_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btnNbtComponentEditor && btnNbtComponentEditor.DataContext is AddingEntry item)
            {
                if (RIGU.wndMain.datapackUsesLegacyNBT)
                {
                    //Open the legacy nbt editor
                    wndNBTEditor editor = new wndNBTEditor();
                    (ModificationState result, string nbt) = editor.GetFromDialog(item.itemName, item.itemNBT);
                    item.itemNBT = nbt;
                }
                else
                {
                    //Open the item stack component editor
                    wndComponentEditor editor = new wndComponentEditor();
                    (ModificationState result, string component) = editor.GetFromDialog(item.itemName, item.itemStackComponent);
                    item.itemStackComponent = component;
                }

            }
        }
    }
}
