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
using System.IO;
using System.Security.Policy;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Threading;
using System.ComponentModel;
using System.Threading;
using System.Windows.Media.Animation;
using SeeloewenLib;
using System.Collections.ObjectModel;
using System.Windows.Media.Converters;
using System.Reflection;

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
        private TextBlock tblNoDatapackLoaded = new TextBlock();

        //General variables for the software
        public List<string> lootTableStart = new List<string>();
        public List<string> lootTableEnd = new List<string>();
        public string versionNumber = string.Format("Public Beta 2");
        public string versionDate = "24.02.2024";
        public string currentLootTable = "none";
        public string currentDatapack = "none";
        public bool datapackUsesLegacyNBT = false;
        private bool calledClose;
        public bool calledNewLootTable;
        public string calledLootTableName;
        public string calledLootTablePath;


        //Windows
        public static wndAddItem wndAddItem;
        public static wndRemoveItems wndRemoveItems;
        public static wndAbout wndAbout;
        public static wndDuplicateFinder wndDuplicateFinder;
        public static wndSettings wndSettings;
        public wndNBTEditor wndNBTEditor;

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
        private Canvas cvsBtnRemoveItems = new Canvas();
        private Image imgBtnRemoveItems = new Image();
        private TextBlock tblBtnRemoveItems = new TextBlock();

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

        private void btnRemoveItems_Click(object sender, RoutedEventArgs e)
        {
            //Open remove item window
            wndRemoveItems = new wndRemoveItems(false, null) { Owner = this };
            wndRemoveItems.Owner = Application.Current.MainWindow;
            if (wndRemoveItems.isOpen == false)
            {
                //Check if the datapack path exists before opening the window

                if (Directory.Exists(currentDatapack))
                {
                    wndRemoveItems.ShowDialog();
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
            //tbDatapack.Text = "C:/Users/Louis/OneDrive/Desktop/Random Item Giver 1.20 Dev";
            if ((!string.IsNullOrEmpty(tbDatapack.Text) && Directory.Exists(tbDatapack.Text)))
            {
                //If the directory exists and is valid and no other datapack with unsaved changes is loaded, try to load the datapack
                if (lootTableModified() == false)
                {
                    currentLootTable = "none";
                    currentDatapack = tbDatapack.Text;
                    GetLootTables(currentDatapack);
                    CheckForLegacyNBT();
                }
                else
                {

                    MessageBoxResult result = MessageBox.Show("You have changes in your current loot table, that have not been saved yet. Loading a new loot table will discard all unsaved changes. Do you really want to continue?", "Load new loot table", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            //Discard the current loot table and load a new datapack
                            currentLootTable = "none";
                            currentDatapack = tbDatapack.Text;
                            GetLootTables(currentDatapack);
                            CheckForLegacyNBT();
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

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            //Open settings window
            wndSettings = new wndSettings(this);
            wndSettings.ShowDialog();
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
            //Put start of loot table into new list
            List<string> newFile = [.. lootTableStart];

            //Make sure last construct does not have a comma
            itemList[itemList.Count() - 1].itemBody[itemList[itemList.Count() - 1].itemBody.Count() - 1].Replace(",", "");

            //Put each item entry into the new list
            foreach (itemEntry item in itemList)
            {
                //Check if the item wasn't deleted and add it to the loot table
                if (!item.changes.Contains(itemEntry.changeType.Deleted))
                {
                    foreach (string str in item.itemBody)
                    {
                        newFile.Add(str);
                    }
                }

                //Report progress
                bgwEditLootTable.ReportProgress(0, 100 / Convert.ToDouble(itemList.Count()));
            }

            //Don't forget the end, put that in as well
            foreach (string str in lootTableEnd)
            {
                newFile.Add(str);
            }

            //Remove lines that only contain spaces from the file
            List<string> newFileFinal = new List<string>();
            foreach (string line in newFile)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    newFileFinal.Add(line);
                }
            }

            //Finally write the loot table to the file
            File.WriteAllLines(currentLootTable, newFile);
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
                MessageBox.Show("Error: Could not detect datapack. Please make sure the currently selected datapack exists and is valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            //Hide workplace image
            imgWorkplace.Visibility = Visibility.Hidden;

            //Get list of content in file, remove all non-item lines so only items remain
            itemList.Clear();
            string[] loadedItems = File.ReadAllLines(path);
            List<string> items = new List<string>();

            //Get Loot Table Start
            lootTableStart = GetLootTableStart(loadedItems);

            //Get Loot Table End
            lootTableEnd = GetLootTableEnd(loadedItems);

            items.Clear();
            int index = 0;
            foreach (string item in loadedItems)
            {
                List<string> itemStackComponent = new List<string>();
                List<string> itemBody = new List<string>();
                string itemName = "";
                string itemNBT = "";
                bool hasNBT = false;
                bool hasFunctionBody;

                if (item.Contains("\"name\":"))
                {
                    //Get the item name
                    itemName = item.Replace("\"", "").Replace("name:", "").Replace(" ", "").Replace(",", ""); ;

                    //Get entire item body
                    if (GetItemBody(loadedItems, index) != null)
                    {
                        itemBody = GetItemBody(loadedItems, index);

                        //Get if item has function body
                        if (loadedItems[index + 1].Contains("\"functions\":"))
                        {
                            hasFunctionBody = true;
                        }
                        else
                        {
                            hasFunctionBody = false;
                        }

                        //Get Item Stack Components
                        if (GetItemStackComponents(loadedItems, index) != null)
                        {
                            itemStackComponent = GetItemStackComponents(loadedItems, index);
                        }

                        //Get NBT tag
                        if (GetNBT(loadedItems, index) != null)
                        {
                            hasNBT = true;
                            itemNBT = GetNBT(loadedItems, index);
                        }
                        else
                        {
                            hasNBT = false;
                        }

                        //Add item to item list
                        if (datapackUsesLegacyNBT == true)
                        {
                            if (hasNBT == true)
                            {
                                itemList.Add(new itemEntry(itemName, itemNBT, itemList.Count() + 1, hasFunctionBody, new List<string>(itemBody)));
                            }
                            else
                            {
                                itemList.Add(new itemEntry(itemName, "none", itemList.Count() + 1, hasFunctionBody, new List<string>(itemBody)));
                            }
                        }
                        else
                        {
                            itemList.Add(new itemEntry(itemName, new List<string>(itemStackComponent), itemList.Count() + 1, hasFunctionBody, new List<string>(itemBody)));
                        }

                    }
                    else
                    {
                        MessageBox.Show($"Fatal error: Could not get item body for item {itemName}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                index++;
            }

            //Show 'loading' message
            tblLoadingItems.Margin = new Thickness(lbItems.ActualWidth / 2 - 150, lbItems.ActualHeight / 2 - 75, 0, 0);
            tblLoadingItems.Text = "Loading items, please wait...\nThis may take a few seconds!";
            await Task.Delay(5); //Allows the UI to update and show the textblock
        }

        public List<string> GetItemStackComponents(string[] file, int itemLine)
        {
            List<string> itemStackComponent = new List<string>();
            int index = 1;

            while (true)
            {
                //Search for a component, return null if none is found
                if (file[itemLine + index].Contains("\"components\":"))
                {
                    int openBrackets = 1;
                    int index2 = 0;

                    //Go through the entire item stack component by checking when the last bracket was closed (aka the item stack component ends)
                    while (openBrackets > 0)
                    {
                        if (file[itemLine + index + index2].Contains("{") || file[itemLine + index + index2].Contains("["))
                        {
                            openBrackets++;
                        }
                        else if (file[itemLine + index + index2].Contains("}") || file[itemLine + index + index2].Contains("]"))
                        {
                            openBrackets--;
                        }

                        //Add the line to the component
                        itemStackComponent.Add(file[itemLine + index + index2]);
                        index2++;
                    }

                    //Remove the first and last two parts as they are not part of component
                    itemStackComponent.Remove(itemStackComponent[0]);
                    itemStackComponent.Remove(itemStackComponent[itemStackComponent.Count - 1]);
                    itemStackComponent.Remove(itemStackComponent[itemStackComponent.Count - 2]);
                    return itemStackComponent;
                }
                else if (file[itemLine + index].Contains("\"name\":"))
                {
                    //Stop if it reaches the next item
                    return null;
                }
                else if ((file[itemLine + index].Contains("]") && file[itemLine + index + 1].Contains("}") && file[itemLine + index + 2].Contains("]")))
                {
                    //Stop if it reaches the bottom of the file
                    return null;
                }
                index++;
            }
        }

        public List<string> GetLootTableStart(string[] file)
        {
            List<string> lootTableStart = new List<string>();

            //Get all lines that belong to the start of the loot table
            foreach (string line in file)
            {
                lootTableStart.Add(line);
                if (line.Contains("\"entries\":"))
                {
                    break;
                }
            }

            return lootTableStart;
        }

        public List<string> GetLootTableEnd(string[] file)
        {
            List<string> lootTableEnd = new List<string>();

            //Get all lines that belong to the end of the loot table
            lootTableEnd.Add("      ]");
            lootTableEnd.Add("    }");
            lootTableEnd.Add("  ]");
            lootTableEnd.Add("}");

            return lootTableEnd;
        }

        public List<string> GetItemBody(string[] file, int itemLine)
        {
            //Get start line
            int bodyStartLine = 0;
            int indexStartLine = 0;
            while (true)
            {
                if (file[itemLine - indexStartLine].Contains("{"))
                {
                    bodyStartLine = itemLine - indexStartLine;
                    break;
                }
                else if (file[itemLine - indexStartLine].Contains("\"entries\":"))
                {
                    //Stop if it hits the top of the file
                    break;
                }
                indexStartLine++;
            }

            if (bodyStartLine != 0)
            {
                List<string> itemBody = new List<string>();
                int openBrackets = 0;
                int indexItemBody = 0;

                do
                {
                    //Go through each line and add it to the item body until all brackets are closed (aka the body ends)
                    if (file[bodyStartLine + indexItemBody].Contains(" {") || file[bodyStartLine + indexItemBody].Contains(" ["))
                    {
                        openBrackets++;
                    }
                    else if (file[bodyStartLine + indexItemBody].Contains(" }") || file[bodyStartLine + indexItemBody].Contains(" ]"))
                    {
                        openBrackets--;
                    }

                    itemBody.Add(file[bodyStartLine + indexItemBody]);
                    indexItemBody++;
                }
                while (openBrackets > 0);

                return itemBody;

            }
            else
            {
                return null;
            }
        }

        public string GetNBT(string[] file, int itemLine)
        {
            int index = 1;
            string nbt;
            while (true)
            {
                if (file[itemLine + index].Contains("\"tag\":"))
                {
                    nbt = file[itemLine + index].Replace(" ", "").Replace("\"tag\":", "");
                    nbt = nbt.Substring(1, nbt.Length - 2);
                    return nbt;
                }
                else if (file[itemLine + index].Contains("\"name\":"))
                {
                    //Stop if it reaches the next item
                    return null;
                }
                else if ((file[itemLine + index].Contains(" ]") && file[itemLine + index + 1].Contains(" }") && file[itemLine + index + 2].Contains(" ]")))
                {
                    //Stop if it reaches the bottom of the file
                    return null;
                }
                index++;
            }
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

        public string GetDatapackMCVersionLegacy(string path) //Please redo this using a switch statement ANYTIME soon
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
                    if (item.IsModified() == true)
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

        public void CheckForLegacyNBT()
        {
            //Go through the file to check whether the line specifying the legacy nbt state is present
            bool hasLegacyNBT = true;
            string[] file = File.ReadAllLines(string.Format("{0}\\UPDATER.txt", currentDatapack));
            for (int i = 0; i < file.Length; i++)
            {
                //Check if legacy nbt is present or not
                if (file[i].Contains("uses_legacy_nbt=false"))
                {
                    hasLegacyNBT = false;
                }
                else if (file[i].Contains("uses_legacy_nbt=true"))
                {
                    hasLegacyNBT = true;
                }
            }

            if (hasLegacyNBT == true)
            {
                datapackUsesLegacyNBT = true;
            }
            else
            {
                datapackUsesLegacyNBT = false;
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

            //BtnRemove
            imgBtnRemoveItems.Source = new BitmapImage(new Uri(@"/Random Item Giver Updater;component/Resources/imgRemoveItems.png", UriKind.Relative));
            imgBtnRemoveItems.Margin = new Thickness(5, -10, 0, 0);
            imgBtnRemoveItems.Width = 20;
            imgBtnRemoveItems.Height = 20;
            imgBtnRemoveItems.Stretch = Stretch.UniformToFill;

            tblBtnRemoveItems.Text = "Remove Items";
            tblBtnRemoveItems.FontSize = 17;
            tblBtnRemoveItems.Margin = new Thickness(35, -12, 0, 0);

            cvsBtnRemoveItems.Children.Add(imgBtnRemoveItems);
            cvsBtnRemoveItems.Children.Add(tblBtnRemoveItems);
            btnRemoveItems.Content = cvsBtnRemoveItems;
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
            svLootTables.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            svLootTables.Background = new SolidColorBrush(Color.FromArgb(100, 50, 50, 50));

            //Add the loot table list scrollviewer to the grid
            Grid.SetColumn(svLootTables, 0);
            Grid.SetRow(svLootTables, 1);
            grdWorkspace.Children.Add(svLootTables);

            //Setup folder browser for datapack
            fbdDatapack.Description = "Select the datapack that you want to edit.";

            //Set version number in header
            tblHeader.Text = string.Format("Random Item Giver Updater\nVersion {0}", versionNumber);

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

            //Create textblock for sidebar when no datapack is loaded
            tblNoDatapackLoaded.FontSize = 18;
            tblNoDatapackLoaded.Text = "No datapack loaded";
            tblNoDatapackLoaded.Margin = new Thickness(45, 15, 0, 0);
            tblNoDatapackLoaded.Foreground = new SolidColorBrush(Colors.LightGray);
            stpLootTables.Children.Add(tblNoDatapackLoaded);
        }

        //-- Item Entry Event Handlers --//

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is itemEntry item)
            {
                if (item.IsDeleted() == false) //Set state to deleted
                {
                    item.changes.Add(itemEntry.changeType.Deleted);
                    button.Content = "Undo deletion";
                    item.SetIndicatorState(sender);
                }
                else if (item.IsDeleted() == true) //If the item has been deleted, set state to undeleted
                {
                    //Check if the item has been modified in some other way before setting the modified state to false
                    item.changes.Remove(itemEntry.changeType.Deleted);
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
                    item.changes.Add(itemEntry.changeType.NameChanged);
                    item.SetIndicatorState(sender);
                }
                else
                {
                    item.changes.Remove(itemEntry.changeType.NameChanged);
                    item.SetIndicatorState(sender);
                }
            }
        }

        private void btnEditNBT_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is itemEntry item)
            {
                wndNBTEditor = new wndNBTEditor(item);
                wndNBTEditor.ShowDialog();
            }
        }

        private void tblItemName_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock textblock)
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


        private void cvsItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Canvas canvas)
            {
                //Get neccessary controls
                Button button = canvas.FindName("btnDelete") as Button;
                Button button2 = canvas.FindName("btnEditNBT") as Button;
                TextBlock textblock = canvas.FindName("tblIndicator") as TextBlock;

                //Show the button and move the indicator accordingly
                button.Visibility = Visibility.Visible;
                button2.Visibility = Visibility.Visible;
                Canvas.SetRight(textblock, 400);
            }
        }

        private void cvsItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Canvas canvas)
            {
                //Get neccessary controls
                Button button = canvas.FindName("btnDelete") as Button;
                Button button2 = canvas.FindName("btnEditNBT") as Button;
                TextBlock textblock = canvas.FindName("tblIndicator") as TextBlock;

                //Hide the button and move the indicator accordingly
                button.Visibility = Visibility.Hidden;
                button2.Visibility = Visibility.Hidden;
                Canvas.SetRight(textblock, 90);
            }
        }
    }

}


