using Newtonsoft.Json.Linq;
using SeeloewenLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RandomItemGiverUpdater.Gui.Menus
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
            if (hasInput == true)
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
            if (hasInput == true)
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
            foreach (lootTable lootTable in RIGU.wndMain.lootTableList)
            {
                removeItemsWorkerAddedItems = 0;
                removeItemsWorkerAddedItemsLootTables++;

                foreach (itemRemovalEntry entry in itemRemovalEntries)
                {
                    //If the loot table whitelist of the entry contains the loot table, then remove the item from the loot table
                    if (entry.lootTableWhiteList.Contains(lootTable.fullLootTablePath))
                    {
                        RemoveItem(entry.itemName, lootTable);
                    }

                    //Report worker progress
                    removeItemsWorkerProgress = removeItemsWorkerProgress + (100 / (Convert.ToDouble(itemRemovalEntries.Count * RIGU.wndMain.lootTableList.Count)));
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
            tblItemRemovingProgress.Text = string.Format("Adding items... (Item {0}/{1} - Loot Table {2}/{3})", progress.ProgressPercentage, itemRemovalEntries.Count, removeItemsWorkerAddedItemsLootTables, RIGU.wndMain.lootTableList.Count);
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
            wzdRemoveItems.pages[2].requirementsNotFulfilledMsg = "Please enter items you want to remove from the datapack to continue!";
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

            //Go through all items to check if a prefix needs to be added
            for (int i = 0; i < itemsToRemove.Length; i++)
            {
                //If custom prefixes is checked but no custom prefix is found OR if custom prefixes is not checked, add the default prefix
                if ((cbIncludesCustomPrefixes.IsChecked == true && !itemsToRemove[i].Contains(':')) || cbIncludesCustomPrefixes.IsChecked == false)
                {
                    itemsToRemove[i] = $"minecraft:{itemsToRemove[i]}";
                }
            }

            foreach (lootTable lootTable in RIGU.wndMain.lootTableList)
            {
                JObject fileObject = JObject.Parse(File.ReadAllText(lootTable.fullLootTablePath));
                JArray itemArray = fileObject.SelectToken("pools[0].entries") as JArray;

                foreach (JObject item in itemArray)
                {
                    string itemName = item["name"].ToString();
                    if (itemsToRemove.Contains(itemName))
                    {
                        //Check if the item already has an entry
                        bool isAdded = false;
                        foreach (itemRemovalEntry entry in itemRemovalEntries)
                        {
                            //If it does, add the loot table to the entry
                            if (entry.itemName == itemName)
                            {
                                entry.UpdateLootTables(lootTable);
                                isAdded = true;
                            }
                        }

                        //If it hasn't already been added, create a new entry and add the loot table
                        if (!isAdded)
                        {
                            itemRemovalEntry newEntry = new itemRemovalEntry(itemName);
                            newEntry.UpdateLootTables(lootTable);
                            itemRemovalEntries.Add(newEntry);
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

        private void RemoveItem(string itemName, lootTable lootTable)
        {
            //Load the item list
            JObject fileObject = JObject.Parse(File.ReadAllText(lootTable.fullLootTablePath));
            JArray items = fileObject.SelectToken("pools[0].entries") as JArray;

            for (int i = 0; i < items.Count; i++)
            {
                //Go through all objects in the list and check if the name matches, if yes, remove it and break
                JObject item = items[i] as JObject;
                if (item["name"].ToString() == itemName)
                {
                    items.Remove(item);
                    break;
                }
            }

            //Write modified content to file
            File.WriteAllText(lootTable.fullLootTablePath, fileObject.ToString());
        }


        private void codeFinish()
        {
            //Reload the currently loaded loot table and close this window
            if (RIGU.wndMain.currentLootTable != "none")
            {
                RIGU.wndMain.LoadLootTable(RIGU.wndMain.currentLootTable);
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
                    lootTables = string.Format("{0}\n{1}", lootTables, lootTable.fullLootTablePath.Replace(RIGU.wndMain.currentDatapack, "").Replace("/data/randomitemgiver/loot_tables/", ""));
                }
                MessageBox.Show(lootTables, "List of loot tables", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void tblItemName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textblock && textblock.DataContext is itemRemovalEntry item)
            {
                //Show the full item name in case it's cut off
                MessageBox.Show(item.itemName, "Full item name", MessageBoxButton.OK, MessageBoxImage.Information);
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
