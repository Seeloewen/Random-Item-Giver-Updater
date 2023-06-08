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

namespace Random_Item_Giver_Updater
{

    public partial class MainWindow : Window
    {
        public static string versionNumber = string.Format("Dev{0}", ((Convert.ToString(DateTime.Now).Replace(" ", "")).Replace(":", ""))).Replace(".", "");
        public ScrollViewer svWorkspace = new ScrollViewer();
        public static StackPanel stpWorkspace = new StackPanel();
        public ScrollViewer svLootTables = new ScrollViewer();
        public static StackPanel stpLootTables = new StackPanel();
        public static List<itemEntry> itemList = new List<itemEntry>();
        public static List<string> items = new List<string>();
        public static List<string> lootTables = new List<string>();
        public static List<lootTableCategory> lootTableCategoryList = new List<lootTableCategory>();
        public List<lootTable> lootTableList = new List<lootTable>();
        System.Windows.Forms.FolderBrowserDialog fbdDatapack = new System.Windows.Forms.FolderBrowserDialog();

        //I really gotta sort these variables
        public static string currentLootTable = "none";


        public MainWindow()
        {
            InitializeComponent();

            //Create workspace stack panel
            stpWorkspace.HorizontalAlignment = HorizontalAlignment.Stretch;
            stpWorkspace.VerticalAlignment = VerticalAlignment.Stretch;
            stpWorkspace.Children.Clear();

            //Create workspace scrollviewer
            svWorkspace.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            svWorkspace.HorizontalAlignment = HorizontalAlignment.Stretch;
            svWorkspace.VerticalAlignment = VerticalAlignment.Stretch;
            svWorkspace.Content = stpWorkspace;
            svWorkspace.Background = new SolidColorBrush(Color.FromArgb(100, 140, 140, 140));

            //Add the workspace scrollviewer to the grid
            Grid.SetColumn(svWorkspace, 1);
            Grid.SetRow(svWorkspace, 1);
            grdWorkspace.Children.Add(svWorkspace);

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
            tblHeader.Text = String.Format("Random Item Giver Updater {0}", versionNumber);
        }

        public static void LoadLootTable(string path)
        {
            {
                //Get list of content in file, remove all non-item lines so only items remain
                string[] loadedItems = File.ReadAllLines(currentLootTable);

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

                //Add all item entrys to workspace
                stpWorkspace.Children.Clear();
                foreach (itemEntry entry in itemList)
                {
                    stpWorkspace.Children.Add(entry.itemBorder);
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
            //WIP
            tbDatapack.Text = "C:/Users/Louis/OneDrive/Desktop/Random Item Giver 1.20 Dev";
            GetLootTables(tbDatapack.Text);
        }

        private void GetLootTables(string path)
        {
            //Get all categories
            string[] categories = Directory.GetDirectories(String.Format("{0}/data/randomitemgiver/loot_tables/", path));
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
                foreach (lootTable lootTable in category.lootTableList)
                {
                    //lootTable.lootTableCanvas.Visibility = Visibility.Hidden;
                }
                stpLootTables.Children.Add(category.categoryStackPanel);
            }
        }

        private int GetDatapackVersionNumber(string path)
        {
            //Read line with pack version from pack.mcmeta file, replace unnecessary characters and return the raw version
            string[] loadedItems = File.ReadAllLines(String.Format("{0}/pack.mcmeta", path));
            string versionString = loadedItems[2];
            versionString = versionString.Replace("    \"pack_format\":", "");
            versionString = versionString.Replace(",", "");
            int version = int.Parse(versionString);
            return version;
        }

        public static void SaveCurrentLootTable()
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
                            if (loadedItems[i].Contains(item.itemName))
                            {
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
            foreach (string line in loadedItems)
            {
                using (var writer = new StreamWriter(currentLootTable, true))
                {
                    writer.WriteLine(line);
                }
            }

            //Reload loot table
            LoadLootTable(currentLootTable);

            //WIP - Remove
            MessageBox.Show("Saved the loot table!", "Saved");

        }

        private void btnSaveLootTable_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentLootTable();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Move delete button when window is resized
            foreach (itemEntry item in itemList)
            {
                item.deleteButton.Margin = new Thickness(ActualWidth - 420, 10, 0, 0);
            }
        }

        public static bool lootTableModified()
        {
            bool isModified = false;

            //Check every item in the loot table
            foreach(itemEntry item in itemList)
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

        private void wndMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MainWindow.currentLootTable != "none")
            {
                if (MainWindow.lootTableModified() == true)
                {
                    //Show warning if there are unsaved changes to the loot table
                    MessageBoxResult result = MessageBox.Show("You still have unsaved modifications in the current loot table.\nDo you want to save the changes before quitting?", "Save changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            //Save the current loot table
                            MainWindow.SaveCurrentLootTable();
                            break;
                        case MessageBoxResult.Cancel:
                            //Stop the quitting
                            e.Cancel = true;
                            break;
                    }
                }
            }
        }
    }

    public class itemEntry
    {
        //Objects of an item slot
        public Border itemBorder = new Border();
        public Canvas itemCanvas = new Canvas();
        public TextBlock itemTextblockName = new TextBlock();
        public TextBlock itemTextblockNBT = new TextBlock();
        public Button deleteButton = new Button();
        public TextBox itemNameTextBox = new TextBox();
        public Button itemNameSaveButton = new Button();
        public TextBox itemNBTTextBox = new TextBox();
        public Button itemNBTSaveButton = new Button();

        //Item attributes
        public string itemName;
        public string itemNBT;
        public int itemIndex;

        //Item editing
        public bool isModified = false;
        public bool isDeleted = false;
        public string newName;
        public string newNBT;

        //Reference to main window
        MainWindow wndMain = (MainWindow)Application.Current.MainWindow;

        public itemEntry(string name, string nbt, int index)
        {

            //Initialize variables
            itemName = name.TrimEnd('\r', '\n');
            itemNBT = nbt.TrimEnd('\r', '\n'); ;
            itemIndex = index;
            newName = itemName;
            newNBT = itemNBT;

            //Set backcolor
            if (index % 2 == 0)
            {
                itemCanvas.Background = new SolidColorBrush(Color.FromArgb(100, 70, 70, 70));
            }
            else
            {
                itemCanvas.Background = new SolidColorBrush(Color.FromArgb(100, 90, 90, 90));
            }
            itemCanvas.Height = 50;

            //Create itemborder
            itemBorder.Margin = new Thickness(0, 0, 0, 0);
            itemBorder.Child = itemCanvas;
            itemBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
            itemBorder.VerticalAlignment = VerticalAlignment.Top;

            //Create item name text
            itemTextblockName.Margin = new Thickness(10, 10, 0, 0);
            itemTextblockName.Text = itemName;
            itemTextblockName.FontSize = 20;
            itemTextblockName.Foreground = new SolidColorBrush(Colors.White);
            itemCanvas.Children.Add(itemTextblockName);
            itemTextblockName.MouseDown += new MouseButtonEventHandler(itemTextBlockName_MouseDown);

            //Create delete button
            deleteButton.Height = 25;
            deleteButton.Width = 100;
            deleteButton.Content = "Delete";
            deleteButton.Margin = new Thickness(wndMain.ActualWidth - 420, 10, 0, 0);
            deleteButton.Click += new RoutedEventHandler(btnDelete_Click);
            itemCanvas.Children.Add(deleteButton);

            //Create item name save button
            itemNameSaveButton.Height = 25;
            itemNameSaveButton.Width = 100;
            itemNameSaveButton.Content = "Confirm";
            itemNameSaveButton.Margin = new Thickness(370, 10, 0, 0);
            itemNameSaveButton.Click += new RoutedEventHandler(btnSaveItemName_Click);
            itemCanvas.Children.Add(itemNameSaveButton);
            itemNameSaveButton.Visibility = Visibility.Hidden;

            //Create item name textbox
            itemNameTextBox.Width = 350;
            itemNameTextBox.Height = 25;
            itemNameTextBox.FontSize = 18;
            itemNameTextBox.Margin = new Thickness(10, 10, 0, 0);
            itemNameTextBox.Visibility = Visibility.Hidden;
            itemCanvas.Children.Add(itemNameTextBox);

            //Create item NBT save button
            itemNBTSaveButton.Height = 25;
            itemNBTSaveButton.Width = 100;
            itemNBTSaveButton.Content = "Confirm";
            itemNBTSaveButton.Margin = new Thickness(859, 10, 0, 0);
            itemNBTSaveButton.Click += new RoutedEventHandler(btnSaveItemNBT_Click);
            itemCanvas.Children.Add(itemNBTSaveButton);
            itemNBTSaveButton.Visibility = Visibility.Hidden;

            //Create item NBT textbox
            itemNBTTextBox.Width = 350;
            itemNBTTextBox.Height = 25;
            itemNBTTextBox.FontSize = 18;
            itemNBTTextBox.Margin = new Thickness(500, 10, 0, 0);
            itemNBTTextBox.Visibility = Visibility.Hidden;
            itemCanvas.Children.Add(itemNBTTextBox);

            //Create item NBT text
            if (itemNBT != "none")
            {
                itemTextblockNBT.Margin = new Thickness(500, 10, 0, 0);
                itemTextblockNBT.Text = string.Format("NBT: {0}", itemNBT);
                itemTextblockNBT.FontSize = 20;
                itemTextblockNBT.Foreground = new SolidColorBrush(Colors.White);
                itemCanvas.Children.Add(itemTextblockNBT);
                itemTextblockNBT.MouseDown += new MouseButtonEventHandler(itemTextblockNBT_MouseDown);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (isDeleted == false) //Set state to deleted
            {
                isModified = true;
                isDeleted = true;
                deleteButton.Content = "Undo deletion";
            }
            else if (isDeleted == true) //If the item has been deleted, set state to undeleted
            {
                //Check if the item has been modified in some other way before setting the modified state to false
                if (itemName == newName && itemNBT == newNBT)
                {
                    isModified = false;
                }
                isDeleted = false;
                deleteButton.Content = "Delete";
            }

        }

        private void btnSaveItemName_Click(object sender, RoutedEventArgs e)
        {
            //Hide the controls for editing and show the item name
            itemNameTextBox.Visibility = Visibility.Hidden;
            itemNameSaveButton.Visibility = Visibility.Hidden;
            itemTextblockName.Visibility = Visibility.Visible;
            newName = itemNameTextBox.Text;
            itemTextblockName.Text = newName;

            //Check if the item name has been changed and change modified state
            if (newName != itemName)
            {
                isModified = true;
            }
        }

        private void btnSaveItemNBT_Click(object sender, RoutedEventArgs e)
        {
            //Hide the controls for editing and show the item NBT
            itemNBTTextBox.Visibility = Visibility.Hidden;
            itemNBTSaveButton.Visibility = Visibility.Hidden;
            itemTextblockNBT.Visibility = Visibility.Visible;
            newNBT = itemNBTTextBox.Text;
            itemTextblockNBT.Text = string.Format("NBT: {0}", newNBT);

            //Check if the item name has been changed and change modified state
            if (newNBT != itemNBT)
            {
                isModified = true;
            }
        }

        private void itemTextBlockName_MouseDown(object sender, MouseEventArgs e)
        {
            //Show the controls for editing and hide the original name
            itemNameTextBox.Visibility = Visibility.Visible;
            itemNameSaveButton.Visibility = Visibility.Visible;
            itemTextblockName.Visibility = Visibility.Hidden;
            itemNameTextBox.Text = itemTextblockName.Text;
        }

        private void itemTextblockNBT_MouseDown(object sender, MouseEventArgs e)
        {
            //Show the controls for editing and hide the original NBT
            itemNBTTextBox.Visibility = Visibility.Visible;
            itemNBTSaveButton.Visibility = Visibility.Visible;
            itemTextblockNBT.Visibility = Visibility.Hidden;
            itemNBTTextBox.Text = itemTextblockNBT.Text.Replace("NBT: ","");
        }

    }

    public class lootTable
    {
        public Canvas lootTableCanvas = new Canvas();
        public TextBlock lootTableTextBlock = new TextBlock();
        public string lootTableName;
        public string lootTableType;
        public string lootTablePath;

        public lootTable(string name, string type, string path)
        {
            //Create canvas
            lootTableCanvas.Height = 35;
            lootTableCanvas.Background = new SolidColorBrush(Color.FromArgb(100, 65, 65, 65));

            //Create text			
            lootTableTextBlock.Text = name.Replace("\\", "");
            lootTableTextBlock.FontSize = 15;
            lootTableTextBlock.Foreground = new SolidColorBrush(Colors.White);
            lootTableTextBlock.FontSize = 15;
            lootTableTextBlock.Margin = new Thickness(25, 10, 0, 0);
            lootTableCanvas.Children.Add(lootTableTextBlock);

            //Set some final attributes
            lootTableName = name.Replace("\\", "");
            lootTableType = type;
            lootTablePath = path;

            //Add mouse down event to load the loot table
            lootTableCanvas.MouseDown += new MouseButtonEventHandler(lootTableCanvas_MouseDown);
        }

        private void lootTableCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainWindow.currentLootTable != "none")
            {
                if(MainWindow.lootTableModified() == true)
                {
                    //Show warning if there are unsaved changes to the loot table
                    MessageBoxResult result = MessageBox.Show("You still have unsaved modifications in the current loot table.\nDo you want to save the changes before continuing?", "Save changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            //Save the current loot table
                            MainWindow.SaveCurrentLootTable();

                            //Load the loot table
                            MainWindow.currentLootTable = string.Format("{0}/{1}", lootTablePath, lootTableName);
                            MainWindow.LoadLootTable(MainWindow.currentLootTable);
                            break;
                        case MessageBoxResult.No:
                            //Just load the loot table without saving
                            MainWindow.currentLootTable = string.Format("{0}/{1}", lootTablePath, lootTableName);
                            MainWindow.LoadLootTable(MainWindow.currentLootTable);
                            break;
                        case MessageBoxResult.Cancel:
                            break;
                    }
                }
                else
                {
                    //Load the loot table
                    MainWindow.currentLootTable = string.Format("{0}/{1}", lootTablePath, lootTableName);
                    MainWindow.LoadLootTable(MainWindow.currentLootTable);
                }
            }
            else
            {
                //Load the loot table
                MainWindow.currentLootTable = string.Format("{0}/{1}", lootTablePath, lootTableName);
                MainWindow.LoadLootTable(MainWindow.currentLootTable);
            }
        }
    }

    public class lootTableCategory
    {
        public string categoryName;
        public string categoryPath;
        public bool isCollapsed = true;
        public List<lootTable> lootTableList = new List<lootTable>();
        public StackPanel categoryStackPanel = new StackPanel();
        public Canvas categoryHeaderCanvas = new Canvas();
        public TextBlock categoryHeader = new TextBlock();

        public lootTableCategory(string name, string path)
        {
            //Map variables
            categoryName = name;
            categoryPath = path;

            //Create category canvas
            categoryStackPanel.Children.Add(categoryHeaderCanvas);

            //Create category header canvas
            categoryHeaderCanvas.Height = 35;
            categoryHeaderCanvas.MouseDown += new MouseButtonEventHandler(categoryHeaderCanvas_MouseDown);

            //categoryHeaderCanvas.HorizontalAlignment = HorizontalAlignment.Right;
            categoryHeaderCanvas.Background = new SolidColorBrush(Color.FromArgb(100, 16, 28, 28));
            categoryHeaderCanvas.Children.Add(categoryHeader);

            //Create category header
            categoryHeader.Text = categoryName;
            categoryHeader.FontSize = 15;
            categoryHeader.FontWeight = FontWeights.SemiBold;
            categoryHeader.Foreground = new SolidColorBrush(Colors.White);
            categoryHeader.Margin = new Thickness(10, 10, 0, 0);
        }

        private void categoryHeaderCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (isCollapsed == true)
            {
                //Show all the loot tables
                foreach (lootTable lootTable in lootTableList)
                {
                    lootTable.lootTableCanvas.Visibility = Visibility.Visible;
                    categoryStackPanel.Children.Add(lootTable.lootTableCanvas);
                }

                //Change the collapse state variable
                isCollapsed = false;
            }
            else if (isCollapsed == false)
            {
                //Hide the loot tables and collapse the categories
                categoryStackPanel.Children.Clear();

                //Readd the header
                categoryStackPanel.Children.Add(categoryHeaderCanvas);

                //Change the collapse state variable
                isCollapsed = true;
            }
        }
    }
}

