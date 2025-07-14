using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RandomItemGiverUpdater.Source.Core;
using RandomItemGiverUpdater.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using RandomItemGiverUpdater.Gui.Components;

namespace RandomItemGiverUpdater.Gui.Menus
{
    public partial class wndMain : Window
    {
        private Main core;

        public static wndAddItems wndAddItem;
        public static wndRemoveItems wndRemoveItems;
        public static wndAbout wndAbout;
        public static wndDuplicateFinder wndDuplicateFinder;
        public static wndSettings wndSettings;
        public wndNBTEditor wndNBTEditor;

        private ScrollViewer svLootTables = new ScrollViewer();
        private StackPanel stpLootTables = new StackPanel();
        private OpenFolderDialog fbdDatapack = new OpenFolderDialog();
        private TextBlock tblLoadingItems = new TextBlock();
        private Canvas cvsLootTableStats = new Canvas();
        private TextBlock tblLootTableStats = new TextBlock();
        private ProgressBar pbSavingItems = new ProgressBar();
        private TextBlock tblNoDatapackLoaded = new TextBlock();

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

        public wndMain(Main core)
        {
            InitializeComponent();
            SetupControls();
            SetupButtons();

            this.core = core;
        }

        public void Reload() //Should be called when a new datapack is loaded
        {
            ReloadWorkspace();
            ReloadSidebar();
        }

        public void ReloadWorkspace()
        {
            //Toggle workplace backgroundimage
            imgWorkplace.Visibility = RIGU.core.currentDatapack == null ? Visibility.Visible : Visibility.Hidden;

           //TODO
        }

        public void ReloadSidebar()
        {
            if (RIGU.core.currentDatapack == null) return;

            //Create the sidebar entries by going through each loot table and category
            foreach (LootTableCategory category in RIGU.core.currentDatapack.lootTableCategories)
            {
                LootTableCategoryVisual catVis = new LootTableCategoryVisual(category);

                //Add all the loot table displays to the category display
                foreach (LootTable lootTable in category.lootTables)
                {
                    LootTableSidebarVisual lootVis = new LootTableSidebarVisual(lootTable.name) { DataContext = lootTable };
                    catVis.Children.Add(lootVis);
                }

                stpSidebar.Children.Add(catVis);
            }
        }

        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            wndAddItem = new wndAddItems();

            //Check if the datapack path exists before opening the window
            if (core.DatapackIsValid(core.currentDatapack))
            {
                wndAddItem.ShowDialog();
            }
            else
            {
                MessageBox.Show("Error: Could not detect datapack. Please make sure the currently selected datapack exists and is valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRemoveItems_Click(object sender, RoutedEventArgs e)
        {
            wndRemoveItems = new wndRemoveItems(false, null);

            //Check if the datapack path exists before opening the window
            if (core.DatapackIsValid(core.currentDatapack))
            {
                wndRemoveItems.ShowDialog();
            }
            else
            {
                MessageBox.Show("Error: Could not detect datapack. Please make sure the currently selected datapack exists and is valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void btnBrowseDatapack_Click(object sender, RoutedEventArgs e)
        {
            //Open folder browser dialog to get datapack path
            fbdDatapack.ShowDialog();
            tbDatapack.Text = fbdDatapack.FolderName;
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            tbDatapack.Text = "C:/Users/Louis/OneDrive/Desktop/Random Item Giver 1.21 Dev 2.0";

            core.LoadDatapack(tbDatapack.Text);
        }

        private void btnSaveLootTable_Click(object sender, RoutedEventArgs e)
        {
            //Check if a loot table is loaded and save it
            if (core.DatapackIsValid(core.currentDatapack))
            {
                SaveCurrentLootTable();
            }
            else
            {
                MessageBox.Show("Please load a loot table before saving!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            wndSettings = new wndSettings();
            wndSettings.ShowDialog();
        }

        private void wndMain_Closing(object sender, CancelEventArgs e)
        {
            //Try exiting and cancel if the core denies it
            e.Cancel = core.Exit();
        }

        public void UpdateEditProgress(double progress)
        {
            pbSavingItems.Value = progress;
        }

        public void SetSaveButtonState(bool saving)
        {
            //Set the state of the button depending on whether the software is saving or not
            btnSave.IsEnabled = !saving;
            tblBtnSave.Text = saving ? "Saving..." : "Save Loot Table";
        }


        private void btnDuplicateFinder_Click(object sender, RoutedEventArgs e)
        {
            wndDuplicateFinder = new wndDuplicateFinder();

            //Show duplicate finder window if a datapack is loaded
            if (core.DatapackIsValid(core.currentDatapack))
            {
                wndDuplicateFinder.ShowDialog();
            }
            else
            {
                MessageBox.Show("Error: Could not detect datapack. Please make sure the currently selected datapack exists and is valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            wndAbout = new wndAbout();
            wndAbout.ShowDialog();
        }

        //-- Custom Methods --//



        public string GetItemBody(string fileContent, int itemIndex)
        {
            JObject jsonObject = JObject.Parse(fileContent);

            var items = jsonObject.SelectToken("pools[0].entries") as JArray;

            return items[itemIndex].ToString();
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
                foreach (Item item in itemList)
                {
                    /*if (item.IsModified() == true)
                    {
                        //If any item is modified, stop and return
                        isModified = true;
                        break;
                    }*/ //TODO: FIX
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
            if (sender is Button button && button.DataContext is MainEntry entry)
            {
                if (!entry.isDeleted) //Set state to deleted
                {
                    entry.isDeleted = true;
                    button.Content = "Undo deletion";
                }
                else if (entry.isDeleted) //If the item has been deleted, set state to undeleted
                {
                    entry.isDeleted = false;
                    button.Content = "Delete";
                }

                entry.SetIndicatorState(button, entry.GetModificationState());
            }
        }

        private void btnSaveItemName_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is MainEntry entry)
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

                entry.SetName(textbox.Text);
                textblock.Text = textbox.Text;

                entry.SetIndicatorState(button, entry.GetModificationState());
            }
        }

        private void btnEditNBTComponent_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is MainEntry entry)
            {
                //If the datapack still uses legacy nbt, open the nbt editor and set the tag
                if (datapackUsesLegacyNBT)
                {
                    wndNBTEditor editor = new wndNBTEditor();
                    (ModificationState result, string nbt) = editor.GetFromDialog(entry.name, entry.GetNBT());

                    switch (result)
                    {
                        case ModificationState.Edited:
                            entry.SetNBT(nbt);
                            break;
                        case ModificationState.Deleted:
                            entry.RemoveNbtOrComponentBody();
                            break;
                    }
                }
                else
                {
                    //If it uses the item stack component, open the editor and set the component
                    wndComponentEditor editor = new wndComponentEditor();
                    (ModificationState result, string component) = editor.GetFromDialog(entry.name, entry.GetItemStackComponent());

                    switch (result)
                    {
                        case ModificationState.Edited:
                            entry.SetItemStackComponent(component);
                            break;
                        case ModificationState.Deleted:
                            entry.RemoveNbtOrComponentBody();
                            break;
                    }
                }

                entry.SetIndicatorState(button, entry.GetModificationState());
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
                Button button2 = canvas.FindName("btnEditNBTComponent") as Button;
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
                Button button2 = canvas.FindName("btnEditNBTComponent") as Button;
                TextBlock textblock = canvas.FindName("tblIndicator") as TextBlock;

                //Hide the button and move the indicator accordingly
                button.Visibility = Visibility.Hidden;
                button2.Visibility = Visibility.Hidden;
                Canvas.SetRight(textblock, 90);
            }
        }
    }

}


