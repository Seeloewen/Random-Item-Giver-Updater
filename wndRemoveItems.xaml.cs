using SeeloewenLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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

namespace Random_Item_Giver_Updater
{
    public partial class wndRemoveItems : Window
    {
        //General attributes
        public ObservableCollection<itemRemovalEntry> itemRemovalEntries { get; set; } = new ObservableCollection<itemRemovalEntry>();
        public wndSelectLootTables wndSelectLootTables;
        private double removeItemsWorkerProgress = 0;
        private int removeItemsWorkerAddedItems = 0;
        private int removeItemsWorkerAddedItemsLootTables = 0;
        public bool isOpen;
        private bool hasInput;
        private List<string> inputItems = new List<string>();

        //Important references
        MainWindow wndMain = (MainWindow)Application.Current.MainWindow;
        SeeloewenLibTools SeeloewenLibTools = new SeeloewenLibTools();

        //Controls
        public Wizard wzdRemoveItems;
        private BackgroundWorker bgwRemoveItems = new BackgroundWorker();

        //-- Constructor --//

        public wndRemoveItems(bool hasInput, List<string> items)
        {
            InitializeComponent();
            InitializeWizard();
            DataContext = this;
            this.hasInput = hasInput;
            if(hasInput == true)
            {
                inputItems = items;
            }
        }

        //-- Event Handlers --//

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Change open state to true
            isOpen = true;

            //Setup backgroundworker
            bgwRemoveItems.WorkerReportsProgress = true;
            bgwRemoveItems.DoWork += bgwRemoveItems_DoWork;
            bgwRemoveItems.RunWorkerCompleted += bgwRemoveItems_RunWorkerCompleted;
            bgwRemoveItems.ProgressChanged += bgwRemoveItems_ProgressChanged;

            //Input items if it was called with input
            if(hasInput == true)
            {
                if (inputItems.Count > 0)
                {
                    wzdRemoveItems.ShowPage(2);
                    cbIncludesCustomPrefixes.IsChecked = true;
                    foreach (string item in inputItems)
                    {
                        tbItems.AppendText(string.Format("{0}\n", item));
                    }
                }
            }
        }

        private void bgwRemoveItems_DoWork(object s, DoWorkEventArgs args)
        {
            //Reset previous progress
            removeItemsWorkerProgress = 0;
            removeItemsWorkerAddedItems = 0;
            removeItemsWorkerAddedItemsLootTables = 0;

            //Go through each item removal entry and through each loot table in the currently loaded datapack
            foreach (lootTable lootTable in wndMain.lootTableList)
            {
                removeItemsWorkerAddedItems = 0;
                removeItemsWorkerAddedItemsLootTables++;

                foreach (itemRemovalEntry entry in itemRemovalEntries)
                {
                    //If the loot table whitelist of the entry contains the loot table, then remove the item from the loot table
                    if (entry.lootTableWhiteList.Contains(lootTable.fullLootTablePath))
                    {
                        RemoveItem(entry.itemName, entry.itemNBT, lootTable);
                    }

                    //Report worker progress
                    removeItemsWorkerProgress = removeItemsWorkerProgress + (100 / (Convert.ToDouble(itemRemovalEntries.Count * wndMain.lootTableList.Count)));
                    removeItemsWorkerAddedItems++;
                    bgwRemoveItems.ReportProgress(removeItemsWorkerAddedItems, removeItemsWorkerProgress);
                }
            }
        }

        private void bgwRemoveItems_RunWorkerCompleted(object s, RunWorkerCompletedEventArgs args)
        {
            //Go to the next page
            wzdRemoveItems.ShowNextPage();

            foreach (itemRemovalEntry entry in itemRemovalEntries)
            {
                tbRemovedItems.AppendText(entry.itemName + "\n");
            }
        }

        private void bgwRemoveItems_ProgressChanged(object s, ProgressChangedEventArgs progress)
        {
            //Report worker progress to progress bar
            pbItemRemoving.Value = Convert.ToDouble(progress.UserState);

            //Report added items
            tblItemRemovingProgress.Text = string.Format("Adding items... (Item {0}/{1} - Loot Table {2}/{3})", progress.ProgressPercentage, itemRemovalEntries.Count, removeItemsWorkerAddedItemsLootTables, wndMain.lootTableList.Count);
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            //Change open state to false
            isOpen = false;
        }

        //-- Custom Methods --//

        public void InitializeWizard()
        {
            //Create the wizard
            wzdRemoveItems = new Wizard(5, 580, 742, btnContinue, btnBack, Close, codeFinish, new Thickness(0, 0, 0, 0));
            grdRemoveItems.Children.Add(wzdRemoveItems.gbWizard);
            gbStep1.Content = null;
            gbStep2.Content = null;
            gbStep3.Content = null;
            gbStep4.Content = null;
            gbStep5.Content = null;
            wzdRemoveItems.gbWizard.Foreground = new SolidColorBrush(Colors.White);
            wzdRemoveItems.gbWizard.FontSize = 16;

            //Setup the pages
            wzdRemoveItems.pages[0].grdContent.Children.Add(cvsStep1);
            wzdRemoveItems.pages[1].grdContent.Children.Add(cvsStep2);
            wzdRemoveItems.pages[2].grdContent.Children.Add(cvsStep3);
            wzdRemoveItems.pages[3].grdContent.Children.Add(cvsStep4);
            wzdRemoveItems.pages[4].grdContent.Children.Add(cvsStep5);
            wzdRemoveItems.pages[2].requirements = requirementsPage3;
            wzdRemoveItems.pages[3].requirements = requirementsPage4;
            wzdRemoveItems.pages[2].requirementsNotFulfilledMsg = "Please enter items you want to add to the datapack to continue!";
            wzdRemoveItems.pages[3].requirementsNotFulfilledMsg = "No items to remove were found or you have not selected loot tables to remove from!";
            wzdRemoveItems.pages[2].code = codePage3;
            wzdRemoveItems.pages[3].code = codePage4;
            wzdRemoveItems.pages[3].canGoBack = false;
            wzdRemoveItems.pages[3].canContinue = false;
            wzdRemoveItems.pages[4].canGoBack = false;
        }

        private bool requirementsPage3()
        {
            //Checks if the items textbox is empty and only let's you continue if there's something in it
            if (string.IsNullOrEmpty(tbItems.Text) == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool requirementsPage4()
        {
            //Checks if any items to remove were found and only let's you continue if thats the case
            if (itemRemovalEntries.Count() != 0)
            {
                foreach (itemRemovalEntry itemRemovalEntry in itemRemovalEntries)
                {
                    if (string.IsNullOrEmpty(itemRemovalEntry.lootTableWhiteList))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void codePage3()
        {
            //Clear previous content
            itemRemovalEntries.Clear();

            //Get all items into an array
            string[] itemsToRemove = tbItems.Text.Split('\n');

            foreach (lootTable lootTable in wndMain.lootTableList)
            {
                //Load the loot table so it can go through each item
                //Get list of content in file, remove all non-item lines so only items remain
                string[] loadedItems = File.ReadAllLines(lootTable.fullLootTablePath);
                List<string[]> items = new List<string[]>();

                items.Clear();
                foreach (string item in loadedItems)
                {
                    if (item.Contains("\"tag\""))
                    {
                        string nbtFiltered;
                        nbtFiltered = item.Replace(" ", "");
                        nbtFiltered = nbtFiltered.Replace("\"tag\":", "");
                        nbtFiltered = nbtFiltered.Substring(1, nbtFiltered.Length - 2);
                        items[items.Count - 1][1] = nbtFiltered;
                    }
                    else if (!item.Contains("\"tag\"") && !item.Contains("{") && !item.Contains("}") && !item.Contains("[") && !item.Contains("]") && !item.Contains("\"rolls\"") && !item.Contains("\"type\"") && !item.Contains("\"function\"") && item.Contains("\"") && !item.Contains("\"weight\"") && !item.Contains("\"count\"") && !item.Contains("\"min\": 1") && !item.Contains("\"max\": 64") && !item.Contains("\"out\"") && !item.Contains("\"score\""))
                    {
                        string[] itemFiltered = ["", ""];
                        itemFiltered[0] = item.Replace("\"", "");
                        itemFiltered[0] = itemFiltered[0].Replace("name:", "");
                        itemFiltered[0] = itemFiltered[0].Replace(" ", "");
                        itemFiltered[0] = itemFiltered[0].Replace("tag:", "");
                        itemFiltered[0] = itemFiltered[0].Replace(",", "");
                        items.Add(itemFiltered);
                    }
                }

                //Compare each item in the "Items to remove" list with the items in the loot table
                foreach (string itemToRemove in itemsToRemove)
                {
                    string itemToRemoveFinal;

                    if (cbIncludesCustomPrefixes.IsChecked == false)
                    {
                        itemToRemoveFinal = string.Format("minecraft:{0}", itemToRemove).TrimEnd('\r', '\n');
                    }
                    else
                    {
                        itemToRemoveFinal = itemToRemove.TrimEnd('\r', '\n');
                    }

                    foreach (string[] item in items)
                    {

                        //If the items match, create an entry for them
                        if (item[0] == itemToRemoveFinal)
                        {
                            //Check if the item already has an entry
                            bool isAdded = false;
                            foreach (itemRemovalEntry entry in itemRemovalEntries)
                            {
                                //If it does, add the loot table to the entry
                                if (entry.itemName == itemToRemoveFinal && item[1] == entry.itemNBT)
                                {
                                    entry.UpdateLootTables(lootTable);
                                    isAdded = true;
                                }
                            }

                            //If it hasn't already been added, create a new entry and add the loot table
                            if (isAdded == false)
                            {
                                itemRemovalEntries.Add(new itemRemovalEntry(itemToRemoveFinal, item[1].TrimEnd('\r', '\n')));
                                itemRemovalEntries[itemRemovalEntries.Count - 1].UpdateLootTables(lootTable);
                            }
                        }
                    }
                }
            }
        }

        private void codePage4()
        {
            //Begin removing the items
            bgwRemoveItems.RunWorkerAsync();
        }

        private void RemoveItem(string itemName, string itemNBT, lootTable lootTable)
        {
            string[] loadedItems = File.ReadAllLines(lootTable.fullLootTablePath);
            string itemsDone = "";

            if (itemNBT == "") //If the item hasn't got any NBT
            {
                for (int i = 0; i < loadedItems.Count(); i++)
                {
                    bool isAtEnd = false;
                    bool is01item = false;
                    if (loadedItems[i].Contains(string.Format("\"{0}\"", itemName)) && !itemsDone.Contains(string.Format("\"{0}\"", itemName)))
                    {

                        //Check if it's an item in a 01item loot table
                        if (loadedItems[i - 3].Contains("},") && !loadedItems[i - 4].Contains("]"))
                        {
                            is01item = true;
                        }

                        //Delete the item line
                        loadedItems[i] = "";

                        //Delete all lines that come after that
                        int index = 1;
                        bool doLoop = true;
                        while (doLoop == true)
                        {
                            //Go through every line after the item name
                            if ((loadedItems[i + index].Contains("}") && loadedItems[i + index + 1].Contains("}") && loadedItems[i + index + 2].Contains("]") && loadedItems[i + index + 3].Contains("}") && loadedItems[i + index + 4].Contains("]")))
                            {
                                //Stop if it's about to reach the file end, so it doesn't corrupt the loot table (For special loot tables)
                                loadedItems[i + index] = "";
                                loadedItems[i + index + 1] = "";
                                loadedItems[i + index + 2] = "";
                                loadedItems[i + index + 3] = "";

                                doLoop = false;
                                isAtEnd = true;

                            }
                            else if ((loadedItems[i + index].Contains("]") && loadedItems[i + index + 1].Contains("}") && loadedItems[i + index + 2].Contains("]")))
                            {
                                //Stop if it's about to reach the file end, so it doesn't corrupt the loot table
                                doLoop = false;
                                isAtEnd = true;

                                if (is01item == false)
                                {
                                    loadedItems[i + index] = "";
                                    loadedItems[i + index + 1] = "";
                                }

                            }
                            else if (!(loadedItems[i + index].Contains("{") && loadedItems[i + index + 1].Contains("\"type\"") && !loadedItems[i + index].Contains("count") && !loadedItems[i + index].Contains("target")))
                            {
                                //Continue if it hasn't reached the next item yet
                                loadedItems[i + index] = "";
                                index++;
                            }

                            else
                            {
                                //Stop if it reaches an item
                                doLoop = false;
                            }
                        }

                        //Delete all lines that come before that
                        int index2 = 1;
                        bool doLoop2 = true;
                        while (doLoop2 == true)
                        {
                            if (!(loadedItems[i - index2].Contains("},") || loadedItems[i - index2].Contains("\"entries\": [")))
                            {
                                //Continue if it hasn't reached another item or the top of the file
                                loadedItems[i - index2] = "";
                                index2++;
                            }
                            else if (loadedItems[i - index2].Contains("},") || loadedItems[i - index2].Contains("\"entries\": ["))
                            {
                                //Stop if it reaches the top of the file or another item
                                doLoop2 = false;
                                if (isAtEnd)
                                {
                                    //Remove comma to avoid corruption of the loot table
                                    loadedItems[i - index2] = loadedItems[i - index2].Replace("},", "}");
                                }
                            }
                        }

                        itemsDone = string.Format("{0};{1}", itemsDone, string.Format("\"{0}\"", itemName));
                    }
                }
            }
            else //If the item has NBT
            {
                for (int i = 0; i < loadedItems.Count(); i++)
                {
                    bool isAtEnd = false;

                    if (loadedItems[i].Contains(string.Format("\"{0}\"", itemName)) && !itemsDone.Contains(string.Format("\"{0}\"", itemName)))
                    {
                        int index = 1;
                        bool doLoop = true;

                        while (doLoop == true)
                        {
                            if ((loadedItems[i + index].Contains("]") && loadedItems[i + index + 1].Contains("}") && loadedItems[i + index + 2].Contains("]")))
                            {
                                //Stop if it's about to reach the file end, so it doesn't corrupt the loot table
                                doLoop = false;
                            }
                            else if (loadedItems[i + index].Contains(itemNBT))
                            {
                                //Stop the loop
                                doLoop = false;

                                //Delete the item line
                                loadedItems[i] = "";

                                //Delete all lines that come after that
                                int index2 = 1;
                                bool doLoop2 = true;
                                while (doLoop2 == true)
                                {

                                    //Go through every line after the item name
                                    if ((loadedItems[i + index2].Contains("]") && loadedItems[i + index2 + 1].Contains("}") && loadedItems[i + index2 + 2].Contains("]")))
                                    {
                                        //Stop if it's about to reach the file end, so it doesn't corrupt the loot table, but still remove some brackets
                                        loadedItems[i + index2] = "";
                                        loadedItems[i + index2 + 1] = "";
                                        doLoop2 = false;
                                        isAtEnd = true;
                                    }
                                    else if (!(loadedItems[i + index2].Contains("{") && loadedItems[i + index2 + 1].Contains("\"type\"") && !loadedItems[i + index2].Contains("count") && !loadedItems[i + index2].Contains("target")))
                                    {
                                        //Continue if it hasn't reached the next item yet
                                        loadedItems[i + index2] = "";
                                        index2++;
                                    }

                                    else
                                    {
                                        //Stop if it reaches an item
                                        doLoop2 = false;
                                    }
                                }

                                //Delete all lines that come before that
                                int index3 = 1;
                                bool doLoop3 = true;
                                while (doLoop3 == true)
                                {
                                    if (!(loadedItems[i - index3].Contains("},") || loadedItems[i - index3].Contains("\"entries\": [")))
                                    {
                                        //Continue if it hasn't reached another item or the top of the file
                                        loadedItems[i - index3] = "";
                                        index3++;
                                    }
                                    else if (loadedItems[i - index3].Contains("},") || loadedItems[i - index3].Contains("\"entries\": ["))
                                    {
                                        //Stop if it reaches another item or the top of the file
                                        doLoop3 = false;
                                        if (isAtEnd)
                                        {
                                            //If the last item was deleted, remove a comma to avoid corruption
                                            loadedItems[i - index3] = loadedItems[i - index3].Replace("},", "}");
                                        }
                                    }
                                }
                            }
                            else if (loadedItems[i + index].Contains("\"name\""))
                            {
                                doLoop = false;
                            }
                            else
                            {
                                index++;
                            }
                        }

                        itemsDone = string.Format("{0};{1}", itemsDone, string.Format("\"{0}\"", itemName));
                    }
                }
            }

            //Remove empty lines
            loadedItems = loadedItems.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            //Write modified content to file
            File.WriteAllLines(lootTable.fullLootTablePath, loadedItems);
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

        //-- Item Removal Entry Event Handlers --//

        private void btnEditLootTables_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                //Get the canvas which the button is in
                Canvas canvas = SeeloewenLibTools.FindVisualParent<Canvas>(button);

                if (canvas.DataContext is itemRemovalEntry item)
                {
                    //Open loot table selection window
                    wndSelectLootTables = new wndSelectLootTables(item.lootTableCheckList, "Select the Loot Tables, that you want to remove the item from.") { Owner = Application.Current.MainWindow };
                    wndSelectLootTables.Owner = Application.Current.MainWindow;
                    wndSelectLootTables.ShowDialog();

                    //Get loot tables string from loot table selection window
                    item.lootTableWhiteList = "";
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

        private void tblLootTables_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textblock && textblock.DataContext is itemRemovalEntry item)
            {
                //Add all loot tables that the item is in to a list and display that list
                string lootTables = "";
                foreach (lootTable lootTable in item.lootTables)
                {
                    lootTables = string.Format("{0}\n{1}", lootTables, lootTable.fullLootTablePath.Replace(wndMain.currentDatapack, "").Replace("/data/randomitemgiver/loot_tables/", ""));
                }
                MessageBox.Show(lootTables, "List of loot tables", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void tblItemName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textblock && textblock.DataContext is itemRemovalEntry item)
            {
                //Show the full item name in case it's cut off
                MessageBox.Show(item.fullItemName, "Full item name", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is itemRemovalEntry item)
            {
                //Remove the item from the item removal list
                itemRemovalEntries.Remove(item);
            }
        }
    }
}
