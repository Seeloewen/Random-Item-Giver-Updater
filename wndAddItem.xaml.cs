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
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Reflection;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using SeeloewenLib;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace Random_Item_Giver_Updater
{
    public partial class wndAddItem : Window
    {
        //General attributes
        public bool isOpen;
        public int currentPage = 1;
        public ObservableCollection<addItemEntry> itemEntries { get; set; } = new ObservableCollection<addItemEntry>();
        double addItemsWorkerProgress;
        int addItemsWorkerAddedItems;
        int addItemsWorkerAddedItemsLootTables;
        DateTime startTime;

        //Important references
        public MainWindow wndMain = (MainWindow)Application.Current.MainWindow;
        private wndSelectLootTables wndSelectLootTables;
        public SeeloewenLibTools SeeloewenLibTools = new SeeloewenLibTools();

        //Controls
        private BackgroundWorker bgwAddItems = new BackgroundWorker();
        private TextBlock tblLoadingItems = new TextBlock();
        public Wizard wzdAddItems;

        //-- Constructor --//

        public wndAddItem()
        {
            InitializeComponent();
            SetupControls();
            CreateWizard();
            DataContext = this;
        }

        //-- Event Handlers --//

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Change open state to true
            isOpen = true;

            //Setup the first page
            tblCurrentlySelectedDatapack.Text = string.Format("Currently selected Datapack: {0}\n{1}", wndMain.currentDatapack, wndMain.GetDatapackVersionInfo(wndMain.currentDatapack));

            //Setup backgroundworker
            bgwAddItems.WorkerReportsProgress = true;
            bgwAddItems.DoWork += bgwAddItems_DoWork;
            bgwAddItems.RunWorkerCompleted += bgwAddItems_RunWorkerCompleted;
            bgwAddItems.ProgressChanged += bgwAddItems_ProgressChanged;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            //Change open state to false
            isOpen = false;
        }

        private void btnAddAdditionalItem_Click(object sender, RoutedEventArgs e)
        {
            //Add a new empty item
            itemEntries.Add(new addItemEntry("minecraft", "", itemEntries.Count));
        }

        private void bgwAddItems_DoWork(object s, DoWorkEventArgs args)
        {
            //Reset previous progress
            addItemsWorkerProgress = 0;
            addItemsWorkerAddedItems = 0;
            addItemsWorkerAddedItemsLootTables = 0;

            //Go through each loot table and add the items
            foreach (lootTable lootTable in wndMain.lootTableList)
            {
                AddItems(string.Format("{0}/{1}", lootTable.lootTablePath, lootTable.lootTableName));
                addItemsWorkerAddedItemsLootTables++;
                addItemsWorkerAddedItems = 0;
            }
        }

        private void bgwAddItems_RunWorkerCompleted(object s, RunWorkerCompletedEventArgs args)
        {
            //Go to the next page
            wzdAddItems.ShowNextPage();
        }

        private void bgwAddItems_ProgressChanged(object s, ProgressChangedEventArgs progress)
        {
            //Report worker progress to progress bar
            pbAddingItems.Value = Convert.ToDouble(progress.UserState);

            //Report added items
            tblAddingItemsProgress.Text = string.Format("Adding items... (Item {0}/{1} - Loot Table {2}/{3})", progress.ProgressPercentage, itemEntries.Count, addItemsWorkerAddedItemsLootTables, wndMain.lootTableList.Count);
        }

        //-- Custom Methods --//


        private void CreateWizard()
        {
            //Create the wizard
            wzdAddItems = new Wizard(6, 580, 742, btnContinue, btnBack, Close, codeFinish, new Thickness(0, 0, 0, 0));
            grdAddItems.Children.Add(wzdAddItems.gbWizard);
            gbStep1.Content = null;
            gbStep2.Content = null;
            gbStep3.Content = null;
            gbStep4.Content = null;
            gbStep5.Content = null;
            gbStep6.Content = null;
            wzdAddItems.gbWizard.Foreground = new SolidColorBrush(Colors.White);
            wzdAddItems.gbWizard.FontSize = 16;

            //Setup the pages
            wzdAddItems.pages[0].grdContent.Children.Add(cvsStep1);
            wzdAddItems.pages[1].grdContent.Children.Add(cvsStep2);
            wzdAddItems.pages[2].grdContent.Children.Add(cvsStep3);
            wzdAddItems.pages[3].grdContent.Children.Add(cvsStep4);
            wzdAddItems.pages[4].grdContent.Children.Add(cvsStep5);
            wzdAddItems.pages[5].grdContent.Children.Add(cvsStep6);

            wzdAddItems.pages[2].code = codePage3;
            wzdAddItems.pages[2].requirements = requirementsPage3;
            wzdAddItems.pages[2].requirementsNotFulfilledMsg = "Please enter items you want to add to the datapack to continue!";
            wzdAddItems.pages[3].code = codePage4;
            wzdAddItems.pages[4].code = codePage5;
            wzdAddItems.pages[4].requirements = requirementsPage5;
            wzdAddItems.pages[4].requirementsNotFulfilledMsg = "You have selected to only add the item to certain loot tables without actually selecting any. Please choose any loot tables before continuing.";
            wzdAddItems.pages[4].canGoBack = false;
            wzdAddItems.pages[4].canContinue = false;
            wzdAddItems.pages[5].code = codePage6;
            wzdAddItems.pages[5].canGoBack = false;
        }
        private bool requirementsPage3()
        {
            //Check if the item name textbox is empty
            if (!string.IsNullOrEmpty(tbItemName.Text))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool requirementsPage5()
        {
            foreach (addItemEntry addItemEntry in itemEntries)
            {
                if (addItemEntry.allLootTablesChecked == false && string.IsNullOrEmpty(addItemEntry.lootTableWhiteList))
                {
                    return false;
                }
            }

            return true;
        }

        private void codePage3()
        {
            //Get all items into an array
            string[] items = tbItemName.Text.Split('\n');

            //Create an entry for every item
            itemEntries.Clear();
            int index = 0;

            if (cbIncludesPrefixes.IsChecked == true)
            {
                //Split the item into prefix and item name if checkbox for prefixes is checked
                foreach (string item in items)
                {
                    string[] line = item.Split(':');
                    if (line.Length != 2)
                    {
                        //If the line actually doesn't contain a prefix, add the item normally with default prefix
                        itemEntries.Add(new addItemEntry("minecraft", line[0], index));
                        index++;
                    }
                    else
                    {
                        //Add the item with prefix and name
                        itemEntries.Add(new addItemEntry(line[0], line[1], index));
                        index++;
                    }
                }
            }
            else
            {
                //Add the line as an item name if checkbox for prefixes is not checked
                foreach (string item in items)
                {
                    itemEntries.Add(new addItemEntry("minecraft", item, index));
                    index++;
                }
            }
        }

        private void codePage4()
        {

        }

        private void codePage5()
        {
            //Set start time for time calculation afterwards
            startTime = DateTime.Now;

            //Add the items
            bgwAddItems.RunWorkerAsync();
        }

        private void codePage6()
        {
            //Add all items to the added items list
            foreach (addItemEntry item in itemEntries)
            {
                tbAddedItemsList.AppendText(string.Format("{0}:{1}\n", item.itemPrefix, item.itemName));
            }

            //Show the elapsed time
            tblElapsedTime.Text = string.Format("Elapsed time: {0}", (DateTime.Now - startTime).ToString(@"hh\:mm\:ss"));
        }

        public void AddItems(string lootTable)
        {
            //Get the array of items
            string lootTableFile = File.ReadAllText(lootTable);
            JObject fileObject = JObject.Parse(lootTableFile);
            JArray items = fileObject.SelectToken("pools[0].entries") as JArray;

            //Create a template item entry and strip the NBT and Components, and possibly the functions part
            string templateString = items[items.Count - 1].ToString();
            ItemEntry template = new ItemEntry(templateString, 0);
            template.RemoveNbtOrComponentBody();

            foreach (addItemEntry entry in itemEntries)
            {
                if (entry.lootTableWhiteList.Contains(lootTable) || entry.allLootTablesChecked)
                {
                    //Create the new item
                    ItemEntry newItem = new ItemEntry(template.itemBody, 0);
                    newItem.SetName(entry.itemName, null);

                    if (entry.HasLegacyNBT())
                    {
                        newItem.SetNBT(entry.itemNBT);
                    }

                    if (entry.HasItemStackComponent())
                    {
                        newItem.SetNBT(entry.itemStackComponent);
                    }

                    items.Add(JObject.Parse(newItem.itemBody));
                }           
            }

            File.WriteAllText(lootTable, fileObject.ToString());
        }            

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                    return typedChild;

                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }

        private void SetupControls()
        {
            //Create 'Loading' text for loading items
            tblLoadingItems.Text = "Processing items, please wait...\nThis may take a few seconds!";
            tblLoadingItems.Foreground = new SolidColorBrush(Colors.White);
            tblLoadingItems.FontSize = 24;
        }

        private void codeFinish()
        {
            //Reload the currently loaded loot table and close this window
            if (wndMain.currentLootTable != "none")
            {
                wndMain.LoadLootTable(wndMain.currentLootTable);
            }
            Close();
        }

        //-- Add Item Entry Event Handlers --//

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                //Get the canvas which the button is in
                Canvas canvas = SeeloewenLibTools.FindVisualParent<Canvas>(button);

                if (canvas.DataContext is addItemEntry item)
                {
                    //Remove the current item from the list
                    itemEntries.Remove(item);
                }
            }
        }

        //-- Add To Loot Table Entry Event Handlers --//

        private void btnEditCertainLootTables_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                //Get the canvas which the button is in
                Canvas canvas = SeeloewenLibTools.FindVisualParent<Canvas>(button);

                if (canvas.DataContext is addItemEntry item)
                {
                    //Open loot table selection window
                    wndSelectLootTables = new wndSelectLootTables(item.lootTableCheckList, "Select the Loot Tables, that you want to add the item to.") { Owner = Application.Current.MainWindow };
                    wndSelectLootTables.Owner = Application.Current.MainWindow;
                    wndSelectLootTables.ShowDialog();

                    //Get loot tables string from loot table selection window
                    foreach (lootTable lootTable in wndSelectLootTables.lootTableList)
                    {
                        if (lootTable.cbAddToLootTable.IsChecked == true)
                        {
                            item.lootTableWhiteList = string.Format("{0}{1}", item.lootTableWhiteList, lootTable.fullLootTablePath);
                        }
                    }
                }
            }
        }

        private void rbtnAllLootTables_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radiobutton)
            {
                //Get the canvas which the radiobutton is in
                Canvas canvas = SeeloewenLibTools.FindVisualParent<Canvas>(radiobutton);

                if (canvas.DataContext is addItemEntry item)
                {
                    //Disable edit button
                    Button button = canvas.FindName("btnEditCertainLootTables") as Button;
                    button.IsEnabled = false;

                    //Set checkstate on variable that gets accessed by the item adding thread
                    item.allLootTablesChecked = true;
                }
            }
        }

        private void rbtnCertainLootTables_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radiobutton)
            {
                //Get the canvas which the radiobutton is in
                Canvas canvas = SeeloewenLibTools.FindVisualParent<Canvas>(radiobutton);

                if (canvas.DataContext is addItemEntry item)
                {
                    //Enable edit button
                    Button button = canvas.FindName("btnEditCertainLootTables") as Button;
                    button.IsEnabled = true;

                    //Set checkstate on variable that gets accessed by the item adding thread
                    item.allLootTablesChecked = false;
                }
            }
        }

        private void tbItemName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textbox)
            {

                if (textbox.DataContext is addItemEntry item)
                {

                    //Set checkstate on variable that gets accessed by the item adding thread
                    item.itemName = textbox.Text;
                }
            }
        }

        private void tbItemPrefix_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textbox)
            {

                if (textbox.DataContext is addItemEntry item)
                {

                    //Set checkstate on variable that gets accessed by the item adding thread
                    item.itemPrefix = textbox.Text;
                }
            }
        }

        private void tbItemNBT_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textbox)
            {

                if (textbox.DataContext is addItemEntry item)
                {

                    //TODO: Replace with NBT/Item Stack Components editor


                    //Set checkstate on variable that gets accessed by the item adding thread
                    item.itemNBT = textbox.Text;
                }
                }
        }
    }
}
