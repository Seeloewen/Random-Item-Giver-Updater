﻿using System;
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
using System.IO;
using System.Security.Policy;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Threading;
using System.ComponentModel;
using System.Threading;
using System.Windows.Media.Animation;
using SeeloewenLib;
using System.Collections.ObjectModel;

namespace Random_Item_Giver_Updater
{

    public partial class MainWindow : Window
    {
        //Lists for items and loot tables
        public ObservableCollection<itemEntry> itemList { get; set; } = new ObservableCollection<itemEntry>();
        private List<lootTableCategory> lootTableCategoryList = new List<lootTableCategory>();
        public List<lootTable> lootTableList = new List<lootTable>();

        //Controls
        private ScrollViewer svLootTables = new ScrollViewer();
        private StackPanel stpLootTables = new StackPanel();
        private System.Windows.Forms.FolderBrowserDialog fbdDatapack = new System.Windows.Forms.FolderBrowserDialog();
        private TextBlock tblLoadingItems = new TextBlock();
        private Canvas cvsLootTableStats = new Canvas();
        private TextBlock tblLootTableStats = new TextBlock();
        private BackgroundWorker bgwEditLootTable = new BackgroundWorker();
        private ProgressBar pbSavingItems = new ProgressBar();

        //General variables for the software
        public string versionNumber = string.Format("Dev");
        public string versionDate = "13.08.2023";
        public string currentLootTable = "none";
        public string currentDatapack = "none";
        private bool calledClose;
        public bool calledNewLootTable;
        public string calledLootTableName;
        public string calledLootTablePath;

        //Windows
        public static wndAddItem wndAddItem;
        public static wndAbout wndAbout;
        public static wndDuplicateFinder wndDuplicateFinder;

        //Buttons
        private Canvas cvsBtnAddItems = new Canvas();
        private Image imgBtnAddItems = new Image();
        private TextBlock tblBtnAddItems = new TextBlock();
        private Canvas cvsBtnDuplicateFinder = new Canvas();
        private Image imgBtnDuplicateFinder = new Image();
        private TextBlock tblBtnDuplicateFinder = new TextBlock();
        private Canvas cvsBtnSave = new Canvas();
        private Image imgBtnSave = new Image();
        private TextBlock tblBtnSave = new TextBlock();
        private Canvas cvsBtnAbout = new Canvas();
        private Image imgBtnAbout = new Image();
        private TextBlock tblBtnAbout = new TextBlock();

        //SeeloewenLib
        SeeloewenLibTools SeeloewenLibTools = new SeeloewenLibTools();


        //-- Constructor --//

        public MainWindow()
        {
            InitializeComponent();

            //Setup controls
            DataContext = this;
            SetupControls();
            SetupButtons();
        }

        //-- Event Handlers --//

        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            //Open add item window
            wndAddItem = new wndAddItem() { Owner = this };
            wndAddItem.Owner = Application.Current.MainWindow;
            if (wndAddItem.isOpen == false)
            {
                //Check if the datapack path exists before opening the window
                if (Directory.Exists(currentDatapack))
                {
                    wndAddItem.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Error: Could not detect datapack. Please make sure the currently selected datapack exists and is valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnBrowseDatapack_Click(object sender, RoutedEventArgs e)
        {
            //Open folder browser dialog to get datapack path
            fbdDatapack.ShowDialog();
            tbDatapack.Text = fbdDatapack.SelectedPath;
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            tbDatapack.Text = "C:/Users/Louis/OneDrive/Desktop/Random Item Giver 1.20 Dev";
            if ((!string.IsNullOrEmpty(tbDatapack.Text) && Directory.Exists(tbDatapack.Text)))
            {
                //If the directory exists and is valid and no other datapack with unsaved changes is loaded, try to load the datapack
                if (lootTableModified() == false)
                {
                    currentLootTable = "none";
                    //itemList.Clear();
                    //stpWorkspace.Children.Clear();
                    currentDatapack = tbDatapack.Text;
                    GetLootTables(currentDatapack);
                }
                else
                {

                    MessageBoxResult result = MessageBox.Show("You have changes in your current loot table, that have not been saved yet. Loading a new loot table will discard all unsaved changes. Do you really want to continue?", "Load new loot table", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            //Discard the current loot table and load a new datapack
                            currentLootTable = "none";
                            //itemList.Clear();
                            //stpWorkspace.Children.Clear();
                            currentDatapack = tbDatapack.Text;
                            GetLootTables(currentDatapack);
                            break;
                    }
                }
            }
            else
            {
                //Otherwise don't even attempt to load the datapack
                MessageBox.Show("Could not load datapack. Please select a valid datapack folder!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSaveLootTable_Click(object sender, RoutedEventArgs e)
        {
            //Check if a loot table is loaded
            if (currentLootTable != "none")
            {
                //Save the current loot table
                SaveCurrentLootTable();
            }
            else
            {
                //Show an error
                MessageBox.Show("Please load a loot table before saving!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void wndMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (currentLootTable != "none")
            {
                if (lootTableModified() == true)
                {
                    //Show warning if there are unsaved changes to the loot table
                    MessageBoxResult result = MessageBox.Show("You still have unsaved modifications in the current loot table.\nDo you want to save the changes before quitting?", "Save changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            //Save the current loot table
                            SaveCurrentLootTable();
                            e.Cancel = true;
                            calledClose = true;
                            break;
                        case MessageBoxResult.Cancel:
                            //Stop the quitting
                            e.Cancel = true;
                            break;
                    }
                }
            }
        }

        private void bgwEditLootTable_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            //Load the file once again
            string[] loadedItems = File.ReadAllLines(currentLootTable);

            //Remove lines that only contain spaces from the file
            List<string> filteredArray = new List<string>();
            foreach (string line in loadedItems)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    filteredArray.Add(line);
                }
            }
            loadedItems = filteredArray.ToArray();


            foreach (itemEntry item in itemList)
            {
                //Check if the item is modified
                if (item.isModified == true && item.isDeleted == true)
                {
                    if (item.itemNBT == "none") //If the item hasn't got any NBT
                    {
                        for (int i = 0; i < loadedItems.Count(); i++)
                        {
                            bool isAtEnd = false;
                            bool is01item = false;
                            if (loadedItems[i].Contains(item.itemName))
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
                            }
                        }
                    }
                    else //If the item has NBT
                    {

                        for (int i = 0; i < loadedItems.Count(); i++)
                        {
                            bool isAtEnd = false;

                            if (loadedItems[i].Contains(item.itemName))
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
                                    else if (loadedItems[i + index].Contains(item.itemNBT))
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
                            }
                        }
                    }
                }

                //Edit the item
                else if (item.isModified == true && item.isDeleted == false)
                {
                    if (item.itemNBT == "none") //If the item has no NBT
                    {
                        //Go through every line and look for the item name. Warning: This means that every item with that name is being replaced, hence why you should make sure to not include duplicates
                        for (int i = 0; i < loadedItems.Count(); i++)
                        {
                            //If item name is found, replace with new item name
                            if (loadedItems[i].Contains(item.itemName))
                            {
                                loadedItems[i] = loadedItems[i].Replace(item.itemName, item.newName);
                            }
                        }
                    }
                    else //If the item has NBT
                    {
                        for (int i = 0; i < loadedItems.Count(); i++)
                        {
                            if (loadedItems[i].Contains(item.itemName))
                            {
                                int index = 1;
                                bool doLoop = true;

                                while (doLoop == true)
                                {
                                    if ((loadedItems[i + index].Contains("]") && loadedItems[i + index + 1].Contains("}") && loadedItems[i + index + 2].Contains("]")))
                                    {
                                        //Stop if it reaches the bottom of the file
                                        doLoop = false;
                                    }
                                    else if (loadedItems[i + index].Contains(item.itemNBT))
                                    {
                                        //If the item has the correct NBT, replace both Item Name and NBT
                                        loadedItems[i] = loadedItems[i].Replace(item.itemName, item.newName);
                                        loadedItems[i + index] = loadedItems[i + index].Replace(item.itemNBT, item.newNBT);
                                        doLoop = false;
                                    }
                                    else if (loadedItems[i + index].Contains("\"name\""))
                                    {
                                        //Stop if it reaches another item
                                        doLoop = false;
                                    }
                                    else
                                    {
                                        index++;
                                    }
                                }
                            }
                        }
                    }
                }
            }


            //Remove empty lines
            loadedItems = loadedItems.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            //Clear file
            using (var writer = new StreamWriter(currentLootTable, false))
            {
                writer.Write("");
            }

            //Append text to file
            int index4 = 0;
            foreach (string line in loadedItems)
            {
                using (var writer = new StreamWriter(currentLootTable, true))
                {
                    //This is done to avoid empty lines at the beginning or end
                    if (index4 == 0)
                    {
                        writer.Write(line);
                    }
                    else
                    {
                        writer.Write("\n" + line);
                    }
                    index4++;
                }

                //Report progress
                bgwEditLootTable.ReportProgress(0, 100 / Convert.ToDouble(loadedItems.Count()));
            }
        }

        private async void bgwEditLootTable_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbSavingItems.Value += Convert.ToDouble(e.UserState);
            await Task.Delay(5);
        }

        private void bgwEditLootTable_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (calledClose == true)
            {
                //Close the app without showing any confirmation
                lootTableModified();
                Close();
            }
            else if (calledNewLootTable == true)
            {
                //Load the loot table
                currentLootTable = string.Format("{0}/{1}", calledLootTablePath, calledLootTableName);
                LoadLootTable(currentLootTable);

                //Show save confirmation
                tblBtnSave.Text = "Saved!";
                MessageBox.Show("Successfully saved the current loot table!", "Save Loot Table", MessageBoxButton.OK, MessageBoxImage.Information);
                btnSave.IsEnabled = true;
                tblBtnSave.Text = "Save Loot Table";
            }
            else if (calledClose == false && calledNewLootTable == false)
            {
                //Reload loot table
                LoadLootTable(currentLootTable);

                //Show save confirmation
                tblBtnSave.Text = "Saved!";
                MessageBox.Show("Successfully saved the current loot table!", "Save Loot Table", MessageBoxButton.OK, MessageBoxImage.Information);
                btnSave.IsEnabled = true;
                tblBtnSave.Text = "Save Loot Table";
            }
        }

        private void btnDuplicateFinder_Click(object sender, RoutedEventArgs e)
        {
            //Show duplicate finder window if a datapack is loaded
            if (currentDatapack != "none")
            {
                wndDuplicateFinder = new wndDuplicateFinder();
                wndDuplicateFinder.Owner = Application.Current.MainWindow;
                wndDuplicateFinder.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please load a datapack before opening Duplicate Finder", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            //Show about window
            wndAbout = new wndAbout();
            wndAbout.Owner = Application.Current.MainWindow;
            wndAbout.ShowDialog();
        }

        //-- Custom Methods --//

        public async void LoadLootTable(string path)
        {

            //Get list of content in file, remove all non-item lines so only items remain
            string[] loadedItems = File.ReadAllLines(currentLootTable);
            List<string> items = new List<string>();

            items.Clear();
            foreach (string item in loadedItems)
            {
                if (item.Contains("\"tag\""))
                {
                    string itemFiltered;
                    itemFiltered = item.Replace(" ", "");
                    itemFiltered = itemFiltered.Replace("\"tag\":", "");
                    itemFiltered = itemFiltered.Substring(1, itemFiltered.Length - 2);
                    items.Add(itemFiltered);
                }
                else if (!item.Contains("\"tag\"") && !item.Contains("{") && !item.Contains("}") && !item.Contains("[") && !item.Contains("]") && !item.Contains("\"rolls\"") && !item.Contains("\"type\"") && !item.Contains("\"function\"") && item.Contains("\"") && !item.Contains("\"weight\"") && !item.Contains("\"count\"") && !item.Contains("\"min\": 1") && !item.Contains("\"max\": 64") && !item.Contains("\"out\"") && !item.Contains("\"score\""))
                {
                    string itemFiltered;
                    itemFiltered = item.Replace("\"", "");
                    itemFiltered = itemFiltered.Replace("name:", "");
                    itemFiltered = itemFiltered.Replace(" ", "");
                    itemFiltered = itemFiltered.Replace("tag:", "");
                    itemFiltered = itemFiltered.Replace(",", "");
                    items.Add(itemFiltered);
                }
            }

            List<string> finalItemList = new List<string>();
            //Check for each item if it has NBT and add it to the string
            for (int i = 0; i < items.Count; i++)
            {
                if (!items[i].Contains("{"))
                {
                    if (i < items.Count - 1)
                    {

                        if (items[i + 1].Contains("{"))
                        {
                            finalItemList.Add(string.Format("{0};{1}", items[i], items[i + 1]));
                        }

                        if (!items[i + 1].Contains("{"))
                        {
                            finalItemList.Add(string.Format("{0};none", items[i]));
                        }
                    }
                    else
                    {
                        {
                            finalItemList.Add(string.Format("{0};none", items[i]));
                        }
                    }
                }
            }

            itemList.Clear();
            int index = 0;
            //Add an entry for all items
            foreach (string item in finalItemList)
            {
                string[] splitItem = item.Split(';');
                itemList.Add(new itemEntry(splitItem[0], splitItem[1], index));
                index++;
            }

            //Show 'loading' message
            tblLoadingItems.Margin = new Thickness(lbItems.ActualWidth / 2 - 150, lbItems.ActualHeight / 2 - 75, 0, 0);
            tblLoadingItems.Text = "Loading items, please wait...\nThis may take a few seconds!";
            await Task.Delay(5); //Allows the UI to update and show the textblock
        }


        private void GetLootTables(string path)
        {
            //Get all categories
            lootTableCategoryList.Clear();
            string[] categories = Directory.GetDirectories(string.Format("{0}/data/randomitemgiver/loot_tables/", path));
            for (int i = 0; i < categories.Length; i++)
            {
                lootTableCategoryList.Add(new lootTableCategory(categories[i].Replace(String.Format("{0}/data/randomitemgiver/loot_tables/", path), ""), categories[i]));
            }

            //Get each loot table
            lootTableList.Clear();
            foreach (lootTableCategory category in lootTableCategoryList)
            {
                string[] lootTables = Directory.GetFiles(category.categoryPath);
                for (int i = 0; i < lootTables.Length; i++)
                {
                    //Add the loot table to the total loot table list
                    lootTableList.Add(new lootTable(lootTables[i].Replace(category.categoryPath, ""), "loottable", category.categoryPath));

                    //Add the loot table to the list of the category
                    category.lootTableList.Add(lootTableList[lootTableList.Count - 1]);
                }
            }

            //Add all loot tables to sidebar display
            stpLootTables.Children.Clear();
            foreach (lootTableCategory category in lootTableCategoryList)
            {
                stpLootTables.Children.Add(category.stpCategory);
            }
        }

        public int GetDatapackVersionNumberLegacy(string path)
        {
            //Read line with pack version from pack.mcmeta file, replace unnecessary characters and return the raw version
            string[] loadedItems = File.ReadAllLines(String.Format("{0}/pack.mcmeta", path));
            string versionString = loadedItems[2];
            versionString = versionString.Replace("    \"pack_format\":", "");
            versionString = versionString.Replace(",", "");
            int version = int.Parse(versionString);
            return version;
        }

        public string GetDatapackMCVersionLegacy(string path)
        {
            //Get datapack version
            int datapackVersion = GetDatapackVersionNumberLegacy(path);

            //Determine version based on datapack version
            if (datapackVersion == 4)
            {
                //Version 1.13 - 1.14.4 (unsupported)
                return "Version: 1.13 - 1.14.4 (unsupported) (Pack format: 4)";
            }
            else if (datapackVersion == 5)
            {
                //Version 1.15 - 1.16.1 (unsupported)
                return "Version: 1.15 - 1.16.1 (unsupported) (Pack format: 5)";
            }
            else if (datapackVersion == 6)
            {
                //Version 1.16.2 - 1.16.5
                return "Version: 1.16.2 - 1.16.5 (Pack format: 6)";
            }
            else if (datapackVersion == 7)
            {
                //Version 1.17 - 1.17.1
                return "Version: 1.17 - 1.17.1 (Pack format: 7)";
            }
            else if (datapackVersion == 8)
            {
                //Version 1.18 - 1.18.1
                return "Version: 1.18 - 1.18.1 (Pack format: 8)";
            }
            else if (datapackVersion == 9)
            {
                //Version 1.18.2
                return "Version: 1.18.2 (Pack format: 9)";
            }
            else if (datapackVersion == 10)
            {
                //Version 1.19 - 1.19.3
                return "Version: 1.19 - 1.19.3 (Pack format: 10)";
            }
            else if (datapackVersion == 11)
            {
                //Version 1.19.4-Snapshot
                return "Version: 1.19.4-Snapshot (Pack format: 11)";
            }
            else if (datapackVersion == 12)
            {
                //Version 1.19.4
                return "Version: 1.19.4 (Pack format: 12)";
            }
            else if (datapackVersion == 13)
            {
                //Version 1.20-Snapshot
                return "Version: 1.20-Snapshot (Pack format: 13)";
            }
            else if (datapackVersion == 14)
            {
                //Version 1.20-Snapshot
                return "Version: 1.20-Snapshot (Pack format: 14)";
            }
            else if (datapackVersion == 15)
            {
                //Version 1.20
                return "Version: 1.20 (Pack format: 15)";
            }
            else
            {
                //Unknown version
                return "Version: Unknown (Pack format: Unknown)";
            }
        }

        public string GetDatapackVersionInfo(string path)
        {
            if (File.Exists(string.Format("{0}\\UPDATER.txt", path)))
            {
                //Use default method of detecting version by file
                string datapackVersion = "unknown";
                string mcVersion = "unknown";
                string versionBranch = "unknown";

                //Go through the file to get the variables
                string[] file = File.ReadAllLines(string.Format("{0}\\UPDATER.txt", path));
                for (int i = 0; i < file.Length; i++)
                {
                    //Only read the line if it's not a comment
                    if (!file[i].Contains("#"))
                    {
                        if (file[i].Contains("datapack_version"))
                        {
                            datapackVersion = file[i].Replace("datapack_version=", "");
                        }
                        else if (file[i].Contains("version_branch"))
                        {
                            versionBranch = file[i].Replace("version_branch=", "");
                        }
                        else if (file[i].Contains("mc_version"))
                        {
                            mcVersion = file[i].Replace("mc_version=", "");
                        }
                    }
                }

                return (string.Format("Version {0} for {1} ({2})", datapackVersion, mcVersion, versionBranch));
            }
            else
            {
                //Use the legacy version of getting the version if the file doesn't exist (Most likely because the datapack is too old)
                return GetDatapackMCVersionLegacy(path);
            }
        }

        public async void SaveCurrentLootTable()
        {
            //Disable save button
            btnSave.IsEnabled = false;
            tblBtnSave.Text = "Saving...";

            //Show 'loading' message
            //stpWorkspace.Children.Clear();
            tblLoadingItems.Margin = new Thickness(lbItems.ActualWidth / 2 - 150, lbItems.ActualHeight / 2 - 75, 0, 0);
            tblLoadingItems.Text = "Saving items, please wait...\nThis may take a few seconds!";
            //stpWorkspace.Children.Add(tblLoadingItems);

            //Show loading bar
            pbSavingItems.Value = 0;
            //stpWorkspace.Children.Add(pbSavingItems);
            pbSavingItems.Margin = new Thickness(lbItems.ActualWidth / 2 - 538, 20, 0, 0);
            await Task.Delay(5); //Allows the UI to update and show the textblock

            //Save the loot table
            bgwEditLootTable.RunWorkerAsync();
        }


        public bool lootTableModified()
        {
            if (calledClose == false)
            {
                bool isModified = false;

                //Check every item in the loot table
                foreach (itemEntry item in itemList)
                {
                    if (item.isModified == true)
                    {
                        //If any item is modified, stop and return
                        isModified = true;
                        break;
                    }
                }

                return isModified;
            }
            else
            {
                return false;
            }
        }

        private void SetupButtons() //Adds Canvas with image and textblock to button
        {
            //btnAbout
            imgBtnAbout.Source = new BitmapImage(new Uri(@"/Random Item Giver Updater;component/Resources/imgAbout.png", UriKind.Relative));
            imgBtnAbout.Margin = new Thickness(5, -10, 0, 0);
            imgBtnAbout.Width = 20;
            imgBtnAbout.Height = 20;
            imgBtnAbout.Stretch = Stretch.UniformToFill;

            tblBtnAbout.Text = "About";
            tblBtnAbout.FontSize = 17;
            tblBtnAbout.Margin = new Thickness(35, -12, 0, 0);

            cvsBtnAbout.Children.Add(imgBtnAbout);
            cvsBtnAbout.Children.Add(tblBtnAbout);
            btnAbout.Content = cvsBtnAbout;

            //btnSave
            imgBtnSave.Source = new BitmapImage(new Uri(@"/Random Item Giver Updater;component/Resources/imgSave.png", UriKind.Relative));
            imgBtnSave.Margin = new Thickness(5, -10, 0, 0);
            imgBtnSave.Width = 20;
            imgBtnSave.Height = 20;
            imgBtnSave.Stretch = Stretch.UniformToFill;

            tblBtnSave.Text = "Save Loot Table";
            tblBtnSave.FontSize = 17;
            tblBtnSave.Margin = new Thickness(35, -12, 0, 0);

            cvsBtnSave.Children.Add(imgBtnSave);
            cvsBtnSave.Children.Add(tblBtnSave);
            btnSave.Content = cvsBtnSave;

            //btnDuplicateFinder
            imgBtnDuplicateFinder.Source = new BitmapImage(new Uri(@"/Random Item Giver Updater;component/Resources/imgDuplicateFinder.png", UriKind.Relative));
            imgBtnDuplicateFinder.Margin = new Thickness(5, -10, 0, 0);
            imgBtnDuplicateFinder.Width = 22;
            imgBtnDuplicateFinder.Height = 22;
            imgBtnDuplicateFinder.Stretch = Stretch.UniformToFill;

            tblBtnDuplicateFinder.Text = "Duplicate Finder";
            tblBtnDuplicateFinder.FontSize = 17;
            tblBtnDuplicateFinder.Margin = new Thickness(35, -12, 0, 0);

            cvsBtnDuplicateFinder.Children.Add(imgBtnDuplicateFinder);
            cvsBtnDuplicateFinder.Children.Add(tblBtnDuplicateFinder);
            btnDuplicateFinder.Content = cvsBtnDuplicateFinder;

            //BtnAddItems
            imgBtnAddItems.Source = new BitmapImage(new Uri(@"/Random Item Giver Updater;component/Resources/imgAddItems.png", UriKind.Relative));
            imgBtnAddItems.Margin = new Thickness(5, -10, 0, 0);
            imgBtnAddItems.Width = 20;
            imgBtnAddItems.Height = 20;
            imgBtnAddItems.Stretch = Stretch.UniformToFill;

            tblBtnAddItems.Text = "Add Items";
            tblBtnAddItems.FontSize = 17;
            tblBtnAddItems.Margin = new Thickness(35, -12, 0, 0);

            cvsBtnAddItems.Children.Add(imgBtnAddItems);
            cvsBtnAddItems.Children.Add(tblBtnAddItems);
            btnAddItems.Content = cvsBtnAddItems;
        }

        private void SetupControls()
        {
            //Add the listbox to the grid
            Grid.SetColumn(lbItems, 1);
            Grid.SetRow(lbItems, 1);
            lbItems.Background = new SolidColorBrush(Color.FromArgb(100, 140, 140, 140));

            //Create loot table list stack panel
            stpLootTables.HorizontalAlignment = HorizontalAlignment.Stretch;
            stpLootTables.VerticalAlignment = VerticalAlignment.Stretch;
            stpLootTables.Children.Clear();

            //Create loot table list scrollviewer
            svLootTables.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            svLootTables.HorizontalAlignment = HorizontalAlignment.Stretch;
            svLootTables.VerticalAlignment = VerticalAlignment.Stretch;
            svLootTables.Content = stpLootTables;
            svLootTables.Background = new SolidColorBrush(Color.FromArgb(100, 50, 50, 50));

            //Add the loot table list scrollviewer to the grid
            Grid.SetColumn(svLootTables, 0);
            Grid.SetRow(svLootTables, 1);
            grdWorkspace.Children.Add(svLootTables);

            //Setup folder browser for datapack
            fbdDatapack.Description = "Select the datapack that you want to edit.";

            //Set version number in header
            tblHeader.Text = String.Format("Random Item Giver Updater\nVersion {0}", versionNumber);

            //Create 'Loading' text for loading items
            tblLoadingItems.Text = "Loading items, please wait...\nThis may take a few seconds!";
            tblLoadingItems.Foreground = new SolidColorBrush(Colors.White);
            tblLoadingItems.FontSize = 24;

            //Create loot table stats canvas and textblock
            cvsLootTableStats.Height = 50;
            cvsLootTableStats.Background = new SolidColorBrush(Color.FromArgb(100, 20, 20, 20));
            tblLootTableStats.Foreground = new SolidColorBrush(Colors.White);
            tblLootTableStats.FontSize = 20;
            tblLootTableStats.FontWeight = FontWeights.DemiBold;
            tblLootTableStats.Margin = new Thickness(10, 10, 0, 0);
            cvsLootTableStats.Children.Add(tblLootTableStats);

            //Create item editing backgroundworker
            bgwEditLootTable.DoWork += bgwEditLootTable_DoWork;
            bgwEditLootTable.ProgressChanged += bgwEditLootTable_ProgressChanged;
            bgwEditLootTable.RunWorkerCompleted += bgwEditLootTable_RunWorkerCompleted;
            bgwEditLootTable.WorkerReportsProgress = true;

            //Create progress bar for saving items
            pbSavingItems.Height = 15;
            pbSavingItems.Width = 300;
        }

        //-- Item Entry Event Handlers --//

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is itemEntry item)
            {
                if (item.isDeleted == false) //Set state to deleted
                {
                    item.isModified = true;
                    item.isDeleted = true;
                    button.Content = "Undo deletion";
                    item.SetIndicatorState(sender);
                }
                else if (item.isDeleted == true) //If the item has been deleted, set state to undeleted
                {
                    //Check if the item has been modified in some other way before setting the modified state to false
                    if (item.itemName == item.newName)
                    {
                        item.isModified = false;
                    }
                    item.isDeleted = false;
                    button.Content = "Delete";
                    item.SetIndicatorState(sender);
                }
            }
        }

        private void btnSaveItemName_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is itemEntry item)
            {
                //Get the parent canvas
                Canvas canvas = SeeloewenLibTools.FindVisualParent<Canvas>(button);

                //Get the necessary controls
                TextBlock textblock = canvas.FindName("tblItemName") as TextBlock;
                TextBox textbox = canvas.FindName("tbItemName") as TextBox;

                //Hide the controls for editing and show the item name
                textbox.Visibility = Visibility.Hidden;
                button.Visibility = Visibility.Hidden;
                textblock.Visibility = Visibility.Visible;
                item.newName = textbox.Text;
                textblock.Text = item.newName;

                //Check if the item name has been changed and change modified state
                if (item.newName != item.itemName)
                {
                    item.isModified = true;
                    item.SetIndicatorState(sender);
                }
                else
                {
                    if (item.isDeleted == false && item.itemNBT == item.newNBT)
                    {
                        item.isModified = false;
                    }
                    item.SetIndicatorState(sender);
                }
            }
        }

        private void btnSaveItemNBT_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is itemEntry item)
            {
                //Get the parent canvas
                Canvas canvas = SeeloewenLibTools.FindVisualParent<Canvas>(button);

                //Get the necessary controls
                TextBlock textblock = canvas.FindName("tblItemNBT") as TextBlock;
                TextBox textbox = canvas.FindName("tbItemNBT") as TextBox;

                //Hide the controls for editing and show the item NBT
                textbox.Visibility = Visibility.Hidden;
                button.Visibility = Visibility.Hidden;
                textblock.Visibility = Visibility.Visible;
                item.newNBT = textbox.Text;
                textblock.Text = string.Format("NBT: {0}", item.newNBT);

                //Check if the item NBT has been changed and change modified state
                if (item.newNBT != item.itemNBT)
                {
                    item.isModified = true;
                    item.SetIndicatorState(sender);
                }
                else
                {
                    if (item.isDeleted == false && item.itemName == item.newName)
                    {
                        item.isModified = false;
                    }
                    item.SetIndicatorState(sender);
                }
            }
        }

        private void tblItemName_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock textblock && textblock.DataContext is itemEntry item)
            {
                //Get the parent canvas
                Canvas canvas = SeeloewenLibTools.FindVisualParent<Canvas>(textblock);

                //Get the item name textbox
                TextBox textBox = canvas.FindName("tbItemName") as TextBox;
                textBox.Visibility = Visibility.Visible;
                textBox.Text = textblock.Text;
                textBox.Focus();

                //Get the item name save button
                Button button = canvas.FindName("btnSaveItemName") as Button;
                button.Visibility = Visibility.Visible;

                //Hide the textblock
                textblock.Visibility = Visibility.Hidden;
            }
        }

        private void tblItemNBT_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock textblock && textblock.DataContext is itemEntry item)
            {
                //Get the parent canvas
                Canvas canvas = SeeloewenLibTools.FindVisualParent<Canvas>(textblock);

                //Get the necessary controls
                TextBox textbox = canvas.FindName("tbItemNBT") as TextBox;
                Button button = canvas.FindName("btnSaveItemNBT") as Button;

                //Show the controls for editing and hide the original NBT
                textbox.Visibility = Visibility.Visible;
                button.Visibility = Visibility.Visible;
                textblock.Visibility = Visibility.Hidden;
                textbox.Text = textblock.Text.Replace("NBT: ", "");
            }
        }

        private void cvsItem_Initialized(object sender, EventArgs e)
        {
            if (sender is Canvas canvas && canvas.DataContext is itemEntry item)
            {
                //Check if the item has NBT, and show it in that case
                if (item.itemNBT != "none")
                {
                    TextBlock textblock = canvas.FindName("tblItemNBT") as TextBlock;
                    textblock.Visibility = Visibility.Visible;
                }
            }
        }
    }

}


