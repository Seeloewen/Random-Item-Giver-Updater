using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Data;
using RandomItemGiverUpdater.Core.Workspace;
using RandomItemGiverUpdater.Core.Workspace.Entries;
using RandomItemGiverUpdater.Gui.Components;
using System;
using System.ComponentModel;
using System.IO;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.VisualTree;
using RandomItemGiverUpdater.Core.Util;

namespace RandomItemGiverUpdater.Gui.Menus
{
    public partial class wndMain : Window
    {
        public Main core;

        public wndAbout wndAbout;

        private ScrollViewer svLootTables = new ScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Background = new SolidColorBrush(Color.FromArgb(100, 50, 50, 50)) };
        private StackPanel stpLootTables = new StackPanel() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        private TextBlock tblLoadingItems = new TextBlock() { Text = "Loading items, please wait...\nThis may take a few seconds!", Foreground = new SolidColorBrush(Colors.White), FontSize = 24 };
        private Canvas cvsLootTableStats = new Canvas() { Height = 50, Background = new SolidColorBrush(Color.FromArgb(100, 20, 20, 20)) };
        private TextBlock tblLootTableStats = new TextBlock() { Foreground = new SolidColorBrush(Colors.White), FontSize = 20, FontWeight = FontWeight.DemiBold, Margin = new Thickness(10, 10, 0, 0) };
        private ProgressBar pbSavingItems = new ProgressBar() { Height = 15, Width = 300 };
        private TextBlock tblNoDatapackLoaded = new TextBlock() { FontSize = 18, Text = "No datapack loaded", Margin = new Thickness(45, 15, 0, 0), Foreground = new SolidColorBrush(Colors.LightGray) };

        private TextBlock tblBtnSave;

        public wndMain()
        {
            InitializeComponent();
            SetupControls();
            SetupButtons();
        }

        public void Reload() //Should be called when a new datapack is loaded
        {
            ReloadWorkspace();
            ReloadSidebar();
        }

        public void ReloadWorkspace()
        {
            //Toggle workplace backgroundimage
            imgWorkplace.IsVisible = Datapack.Get() == null ? true : false;
        }

        public void ReloadSidebar()
        {
            stpLootTables.Children.Clear();

            if (core.currentDatapack == null)
            {
                stpLootTables.Children.Add(tblNoDatapackLoaded);
                return;
            }

            CreateSidebarEntries($"{Datapack.Get().lootTablesDirectory}", 0, stpLootTables);
        }

        public void CreateSidebarEntries(string path, int depth, StackPanel e) //Recursively goes through the directories and creates entries
        {
            string[] directories = Directory.GetDirectories(path);
            foreach (string dir in directories) //Create a new category for each directory
            {
                LootTableCategoryVisual vis = new LootTableCategoryVisual(dir.Replace(path, ""), depth);

                if (e.GetType() == typeof(LootTableCategoryVisual))
                {
                    ((LootTableCategoryVisual)e).entries.Add(vis);
                }
                else
                {
                    e.Children.Add(vis);
                }
                CreateSidebarEntries(dir, depth + 1, vis);
            }

            string[] files = Directory.GetFiles(path);
            foreach (string file in files) //Create a new loot table entry for each file
            {
                if (!file.Contains(".json")) continue;

                if (e.GetType() == typeof(LootTableCategoryVisual))
                {
                    ((LootTableCategoryVisual)e).entries.Add(new LootTableSidebarVisual(Datapack.Get().GetLootTable(file), depth));
                }
                else
                {
                    e.Children.Add(new LootTableSidebarVisual(Datapack.Get().GetLootTable(file), depth));
                }
            }
        }

        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            //Check if the datapack path exists before opening the window
            if (core.DatapackIsValid(core.currentDatapack))
            {
                RIGU.itemAdding.BeginSession();
            }
            else
            {
                //TODO: Avalonia Rework
                //MessageBox.Show("Error: Could not detect datapack. Please make sure the currently selected datapack exists and is valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRemoveItems_Click(object sender, RoutedEventArgs e)
        {
            //Check if the datapack path exists before opening the window
            if (core.DatapackIsValid(core.currentDatapack))
            {
                RIGU.itemRemover.BeginSession();
            }
            else
            {
                //TODO: Avalonia Rework
                //MessageBox.Show("Error: Could not detect datapack. Please make sure the currently selected datapack exists and is valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void btnBrowseDatapack_Click(object sender, RoutedEventArgs e)
        {
            //Open folder browser dialog to get datapack path
            tbDatapack.Text = await StorageProvider.OpenFolderAsync(this);
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            //tbDatapack.Text = "C:/Users/Louis/OneDrive/Desktop/Random Item Giver 1.21 Dev 2.0"; //Debug
            core.LoadDatapack(tbDatapack.Text);
            Reload();
        }

        private async void btnSaveLootTable_Click(object sender, RoutedEventArgs e)
        {
            //Check if a loot table is loaded and save it
            if (core.DatapackIsValid(Datapack.Get()) && LootTable.Get() != null)
            {
                RIGU.core.wndMain.SetSaveButtonState(true);
                await core.currentLootTable.Save();
                core.ReloadLootTable();
                RIGU.core.wndMain.SetSaveButtonState(false);
                RIGU.core.wndMain.ReloadWorkspace();
                //TODO: Avalonia Rework
                //MessageBox.Show("Successfully saved the loot table!", "Save Loot Table", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                //TODO: Avalonia Rework
                //MessageBox.Show("Please load a loot table before saving!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void wndMain_Closing(object sender, WindowClosingEventArgs e)
        {
            //Try exiting and cancel if the core denies it
            e.Cancel = !core.Exit();
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
            //Show duplicate finder window if a datapack is loaded
            if (core.DatapackIsValid(core.currentDatapack))
            {
                RIGU.duplicateFinder.BeginSession();
            }
            else
            {
                //TODO: Avalonia Rework
                //MessageBox.Show("Error: Could not detect datapack. Please make sure the currently selected datapack exists and is valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            wndAbout = new wndAbout();
            wndAbout.ShowDialog(this);
        }

        private void SetupButtons() //Adds Canvas with image and textblock to button
        {
            //btnAbout
            Image imgBtnAbout = new Image() { Width = 20, Height = 20, Stretch = Stretch.UniformToFill, Margin = new Thickness(5, -10, 0, 0) };
            imgBtnAbout.Source = new Bitmap(AssetLoader.Open(new Uri("avares://Random Item Giver Updater/Resources/imgAbout.png")));
            TextBlock tblBtnAbout = new TextBlock() { Text = "About", FontSize = 17, Margin = new Thickness(35, -12, 0, 0) };

            Canvas cvsBtnAbout = new Canvas();
            cvsBtnAbout.Children.Add(imgBtnAbout);
            cvsBtnAbout.Children.Add(tblBtnAbout);
            btnAbout.Content = cvsBtnAbout;

            //btnSave
            Image imgBtnSave = new Image() { Width = 20, Height = 20, Stretch = Stretch.UniformToFill, Margin = new Thickness(5, -10, 0, 0) };
            imgBtnSave.Source = new Bitmap(AssetLoader.Open(new Uri("avares://Random Item Giver Updater/Resources/imgSave.png")));
            tblBtnSave = new TextBlock() { Text = "Save Loot Table", FontSize = 17, Margin = new Thickness(35, -12, 0, 0) };

            Canvas cvsBtnSave = new Canvas();
            cvsBtnSave.Children.Add(imgBtnSave);
            cvsBtnSave.Children.Add(tblBtnSave);
            btnSave.Content = cvsBtnSave;

            //btnDuplicateFinder
            Image imgBtnDuplicateFinder = new Image() { Width = 20, Height = 20, Stretch = Stretch.UniformToFill, Margin = new Thickness(5, -10, 0, 0) };
            imgBtnDuplicateFinder.Source = new Bitmap(AssetLoader.Open(new Uri("avares://Random Item Giver Updater/Resources/imgDuplicateFinder.png")));
            TextBlock tblBtnDuplicateFinder = new TextBlock() { Text = "Duplicate Finder", FontSize = 17, Margin = new Thickness(35, -12, 0, 0) };

            Canvas cvsBtnDuplicateFinder = new Canvas();
            cvsBtnDuplicateFinder.Children.Add(imgBtnDuplicateFinder);
            cvsBtnDuplicateFinder.Children.Add(tblBtnDuplicateFinder);
            btnDuplicateFinder.Content = cvsBtnDuplicateFinder;

            //btnAddItems
            Image imgBtnAddItems = new Image() { Width = 20, Height = 20, Stretch = Stretch.UniformToFill, Margin = new Thickness(5, -10, 0, 0) };
            imgBtnAddItems.Source = new Bitmap(AssetLoader.Open(new Uri("avares://Random Item Giver Updater/Resources/imgAddItems.png")));
            TextBlock tblBtnAddItems = new TextBlock() { Text = "Add Items", FontSize = 17, Margin = new Thickness(35, -12, 0, 0) };

            Canvas cvsBtnAddItems = new Canvas();
            cvsBtnAddItems.Children.Add(imgBtnAddItems);
            cvsBtnAddItems.Children.Add(tblBtnAddItems);
            btnAddItems.Content = cvsBtnAddItems;

            //btnRemove
            Image imgBtnRemoveItems = new Image() { Width = 20, Height = 20, Stretch = Stretch.UniformToFill, Margin = new Thickness(5, -10, 0, 0) };
            imgBtnRemoveItems.Source = new Bitmap(AssetLoader.Open(new Uri("avares://Random Item Giver Updater/Resources/imgRemoveItems.png")));
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
            entry.item.isDeleted = !entry.item.isDeleted;
            btn.Content = entry.item.isDeleted ? "Delete" : "Undo deletion";

            entry.UpdateIndicator();
        }

        private void btnSaveItemName_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            MainEntry entry = (MainEntry)btn.DataContext;
            Canvas cvs = btn.FindAncestorOfType<Canvas>();
            TextBlock tbl = cvs.Find<TextBlock>("tblItemName");
            TextBox tb = cvs.Find<TextBox>("tbItemName");

            //Hide the controls for editing and show/update the item name
            tb.IsVisible = false;
            btn.IsVisible = false;
            tbl.IsVisible = true;

            entry.item.SetName(tb.Text);
            tbl.Text = tb.Text;

            entry.UpdateIndicator();
        }

        private void btnEditNBTComponent_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            MainEntry entry = (MainEntry)btn.DataContext;

            if (core.currentDatapack.usesLegacyNBT)
            {
                //If the datapack still uses legacy nbt, open the nbt editor and set the tag
                wndNBTEditor editor = new wndNBTEditor();
                (ModificationState result, string nbt) = editor.GetFromDialog(entry.item.name, entry.item.GetNBT());

                switch (result)
                {
                    case ModificationState.Modified:
                        entry.item.SetNBT(nbt);
                        break;
                    case ModificationState.Deleted:
                        entry.item.RemoveNbtOrComponentBody();
                        break;
                }
            }
            else
            {
                //If it uses the item stack component, open the editor and set the component
                wndComponentEditor editor = new wndComponentEditor();
                (ModificationState result, string component) = editor.GetFromDialog(entry.item.name, entry.item.GetItemStackComponent());

                switch (result)
                {
                    case ModificationState.Modified:
                        entry.item.SetItemStackComponent(component);
                        break;
                    case ModificationState.Deleted:
                        entry.item.RemoveNbtOrComponentBody();
                        break;
                }
            }

            entry.UpdateIndicator();
        }

        private void tblItemName_MouseDown(object sender, PointerPressedEventArgs e)
        {
            TextBlock tbl = (TextBlock)sender;
            Canvas cvs = tbl.FindAncestorOfType<Canvas>();
            TextBox tb = cvs.Find<TextBox>("tbItemName");
            Button btn = cvs.Find<Button>("btnSaveItemName");

            //Show the textbox for editing the item name
            tb.IsVisible = true;
            btn.IsVisible = true;
            tbl.IsVisible = false;
            tb.Text = tbl.Text;
            tb.Focus();
        }


        private void cvsItem_MouseEnter(object sender, PointerEventArgs e)
        {
            Canvas cvs = (Canvas)sender;
            Button btn = cvs.Find<Button>("btnDelete");
            Button btn2 = cvs.Find<Button>("btnEditNBTComponent");
            TextBlock tbl = cvs.Find<TextBlock>("tblIndicator");

            //Show the button and move the indicator accordingly
            btn.IsVisible = true;
            btn2.IsVisible = true;
            Canvas.SetRight(tbl, 400);
        }

        private void cvsItem_MouseLeave(object sender, PointerEventArgs e)
        {
            Canvas cvs = (Canvas)sender;
            Button btn = cvs.Find<Button>("btnDelete");
            Button btn2 = cvs.Find<Button>("btnEditNBTComponent");
            TextBlock tbl = cvs.Find<TextBlock>("tblIndicator");

            //Hide the button and move the indicator accordingly
            btn.IsVisible = false;
            btn2.IsVisible = false;
            Canvas.SetRight(tbl, 90);
        }
    }
}