using Microsoft.Win32;
using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Gui.Components;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RandomItemGiverUpdater.Gui.Menus
{
    public partial class wndMain : Window
    {
        private Main core;

        public wndAddItems wndAddItem;
        public wndRemoveItems wndRemoveItems;
        public wndAbout wndAbout;
        public wndDuplicateFinder wndDuplicateFinder;
        public wndSettings wndSettings;
        public wndNBTEditor wndNBTEditor;

        private ScrollViewer svLootTables = new ScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Background = new SolidColorBrush(Color.FromArgb(100, 50, 50, 50)) };
        private StackPanel stpLootTables = new StackPanel() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        private OpenFolderDialog fbdDatapack = new OpenFolderDialog() { Title = "Select the datapack that you want to edit." };
        private TextBlock tblLoadingItems = new TextBlock() { Text = "Loading items, please wait...\nThis may take a few seconds!", Foreground = new SolidColorBrush(Colors.White), FontSize = 24 };
        private Canvas cvsLootTableStats = new Canvas() { Height = 50, Background = new SolidColorBrush(Color.FromArgb(100, 20, 20, 20)) };
        private TextBlock tblLootTableStats = new TextBlock() { Foreground = new SolidColorBrush(Colors.White), FontSize = 20, FontWeight = FontWeights.DemiBold, Margin = new Thickness(10, 10, 0, 0) };
        private ProgressBar pbSavingItems = new ProgressBar() { Height = 15, Width = 300 };
        private TextBlock tblNoDatapackLoaded = new TextBlock() { FontSize = 18, Text = "No datapack loaded", Margin = new Thickness(45, 15, 0, 0), Foreground = new SolidColorBrush(Colors.LightGray) };

        private TextBlock tblBtnSave;

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
        }

        public void ReloadSidebar()
        {
            if (core.currentDatapack == null) return;

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
            //wndRemoveItems = new wndRemoveItems(false, null);

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
            tbDatapack.Text = "C:/Users/Louis/OneDrive/Desktop/Random Item Giver 1.21 Dev 2.0"; //Debug

            core.LoadDatapack(tbDatapack.Text);
        }

        private void btnSaveLootTable_Click(object sender, RoutedEventArgs e)
        {
            //Check if a loot table is loaded and save it
            if (core.DatapackIsValid(core.currentDatapack))
            {
                core.currentLootTable.Save();
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

        private void SetupButtons() //Adds Canvas with image and textblock to button
        {
            //btnAbout
            Image imgBtnAbout = new Image() { Width = 20, Height = 20, Stretch = Stretch.UniformToFill, Margin = new Thickness(5, -10, 0, 0) };
            imgBtnAbout.Source = new BitmapImage(new Uri(@"/Random Item Giver Updater;component/Resources/imgAbout.png", UriKind.Relative));
            TextBlock tblBtnAbout = new TextBlock() { Text = "About", FontSize = 17, Margin = new Thickness(35, -12, 0, 0) };

            Canvas cvsBtnAbout = new Canvas();
            cvsBtnAbout.Children.Add(imgBtnAbout);
            cvsBtnAbout.Children.Add(tblBtnAbout);
            btnAbout.Content = cvsBtnAbout;

            //btnSave
            Image imgBtnSave = new Image() { Width = 20, Height = 20, Stretch = Stretch.UniformToFill, Margin = new Thickness(5, -10, 0, 0) };
            imgBtnSave.Source = new BitmapImage(new Uri(@"/Random Item Giver Updater;component/Resources/imgSave.png", UriKind.Relative));
            tblBtnSave = new TextBlock() { Text = "Save Loot Table", FontSize = 17, Margin = new Thickness(35, -12, 0, 0) };

            Canvas cvsBtnSave = new Canvas();
            cvsBtnSave.Children.Add(imgBtnSave);
            cvsBtnSave.Children.Add(tblBtnSave);
            btnSave.Content = cvsBtnSave;

            //btnDuplicateFinder
            Image imgBtnDuplicateFinder = new Image() { Width = 20, Height = 20, Stretch = Stretch.UniformToFill, Margin = new Thickness(5, -10, 0, 0) };
            imgBtnDuplicateFinder.Source = new BitmapImage(new Uri(@"/Random Item Giver Updater;component/Resources/imgDuplicateFinder.png", UriKind.Relative));
            TextBlock tblBtnDuplicateFinder = new TextBlock() { Text = "Duplicate Finder", FontSize = 17, Margin = new Thickness(35, -12, 0, 0) };

            Canvas cvsBtnDuplicateFinder = new Canvas();
            cvsBtnDuplicateFinder.Children.Add(imgBtnDuplicateFinder);
            cvsBtnDuplicateFinder.Children.Add(tblBtnDuplicateFinder);
            btnDuplicateFinder.Content = cvsBtnDuplicateFinder;

            //btnAddItems
            Image imgBtnAddItems = new Image() { Width = 20, Height = 20, Stretch = Stretch.UniformToFill, Margin = new Thickness(5, -10, 0, 0) };
            imgBtnAddItems.Source = new BitmapImage(new Uri(@"/Random Item Giver Updater;component/Resources/imgAddItems.png", UriKind.Relative));
            TextBlock tblBtnAddItems = new TextBlock() { Text = "Add Items", FontSize = 17, Margin = new Thickness(35, -12, 0, 0) };

            Canvas cvsBtnAddItems = new Canvas();
            cvsBtnAddItems.Children.Add(imgBtnAddItems);
            cvsBtnAddItems.Children.Add(tblBtnAddItems);
            btnAddItems.Content = cvsBtnAddItems;

            //btnRemove
            Image imgBtnRemoveItems = new Image() { Width = 20, Height = 20, Stretch = Stretch.UniformToFill, Margin = new Thickness(5, -10, 0, 0) };
            imgBtnRemoveItems.Source = new BitmapImage(new Uri(@"/Random Item Giver Updater;component/Resources/imgRemoveItems.png", UriKind.Relative));
            TextBlock tblBtnRemoveItems = new TextBlock() { Text = "Remove Items", FontSize = 17, Margin = new Thickness(35, -12, 0, 0) };

            Canvas cvsBtnRemoveItems = new Canvas();
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

            //Add the loot table list scrollviewer to the grid
            Grid.SetColumn(svLootTables, 0);
            Grid.SetRow(svLootTables, 1);
            grdWorkspace.Children.Add(svLootTables);
            svLootTables.Content = stpLootTables;

            //Set version number in header
            tblHeader.Text = $"Random Item Giver Updater\nVersion {RIGU.VERSION_NUM}";

            //Create loot table stats canvas and textblock
            cvsLootTableStats.Children.Add(tblLootTableStats);

            //Create textblock for sidebar when no datapack is loaded
            stpLootTables.Children.Add(tblNoDatapackLoaded);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            MainEntry entry = (MainEntry)btn.DataContext;

            //Update delete button of entry
            entry.isDeleted = !entry.isDeleted;
            btn.Content = entry.isDeleted ? "Delete" : "Undo deletion";

            entry.SetIndicatorState(btn, entry.GetModificationState());
        }

        private void btnSaveItemName_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            MainEntry entry = (MainEntry)btn.DataContext;
            Canvas cvs = SeeloewenLib.Tools.FindVisualParent<Canvas>(btn);
            TextBlock tbl = (TextBlock)cvs.FindName("tblItemName");
            TextBox tb = (TextBox)cvs.FindName("tbItemName");

            //Hide the controls for editing and show/update the item name
            tb.Visibility = Visibility.Hidden;
            btn.Visibility = Visibility.Hidden;
            tbl.Visibility = Visibility.Visible;

            entry.SetName(tb.Text);
            tbl.Text = tb.Text;

            entry.SetIndicatorState(btn, entry.GetModificationState());
        }

        private void btnEditNBTComponent_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            MainEntry entry = (MainEntry)btn.DataContext;

            if (core.currentDatapack.usesLegacyNBT)
            {
                //If the datapack still uses legacy nbt, open the nbt editor and set the tag
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

            entry.SetIndicatorState(btn, entry.GetModificationState());
        }

        private void tblItemName_MouseDown(object sender, MouseEventArgs e)
        {
            TextBlock tbl = (TextBlock)sender;
            Canvas cvs = SeeloewenLib.Tools.FindVisualParent<Canvas>(tbl);
            TextBox tb = (TextBox)cvs.FindName("tbItemName");
            Button btn = (Button)cvs.FindName("btnSaveItemName");

            //Show the textbox for editing the item name
            tb.Visibility = Visibility.Visible;
            btn.Visibility = Visibility.Visible;
            tbl.Visibility = Visibility.Hidden;
            tb.Text = tbl.Text;
            tb.Focus();
        }


        private void cvsItem_MouseEnter(object sender, MouseEventArgs e)
        {
            Canvas cvs = (Canvas)sender;
            Button btn = (Button)cvs.FindName("btnDelete");
            Button btn2 = (Button)cvs.FindName("btnEditNBTComponent");
            TextBlock tbl = (TextBlock)cvs.FindName("tblIndicator");

            //Show the button and move the indicator accordingly
            btn.Visibility = Visibility.Visible;
            btn2.Visibility = Visibility.Visible;
            Canvas.SetRight(tbl, 400);
        }

        private void cvsItem_MouseLeave(object sender, MouseEventArgs e)
        {
            Canvas cvs = (Canvas)sender;
            Button btn = (Button)cvs.FindName("btnDelete");
            Button btn2 = (Button)cvs.FindName("btnEditNBTComponent");
            TextBlock tbl = (TextBlock)cvs.FindName("tblIndicator");

            //Hide the button and move the indicator accordingly
            btn.Visibility = Visibility.Hidden;
            btn2.Visibility = Visibility.Hidden;
            Canvas.SetRight(tbl, 90);
        }
    }
}