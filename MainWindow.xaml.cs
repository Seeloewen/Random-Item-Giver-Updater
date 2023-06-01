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

            //Add header to loot table list
            lootTableList.Add(new lootTable("Loot Tables", "header", "none"));
        }

        public static void LoadLootTable(string path)
        {
            {
                //Get list of content in file, remove all non-item lines so only items remain
                var loadedItems = File.ReadLines(path);
                items.Clear();
                foreach (var item in loadedItems)
                {
                    if (!item.Contains("{") && !item.Contains("}") && !item.Contains("[") && !item.Contains("]") && !item.Contains("rolls") && !item.Contains("\"minecraft:item\"") && item.Contains("\""))
                    {
                        string itemFiltered;
                        itemFiltered = item.Replace("\"", "");
                        itemFiltered = itemFiltered.Replace("name:", "");
                        itemFiltered = itemFiltered.Replace(" ", "");
                        items.Add(itemFiltered);
                    }

                }

                itemList.Clear();
                //Add an entry for all items
                for (int i = 0; i < items.Count; i++)
                {
                    itemList.Add(new itemEntry(i));
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
                MessageBox.Show(categories[i]);
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
    }

    public class itemEntry
    {
        //Attributes of an item slot
        public Border itemBorder = new Border();
        public Canvas itemCanvas = new Canvas();
        public TextBlock itemTextblock = new TextBlock();

        public itemEntry(int slotNumber)
        {
            //Set backcolor
            if (slotNumber % 2 == 0)
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

            //Create text
            itemTextblock.Margin = new Thickness(10, 10, 0, 0);
            itemTextblock.Text = MainWindow.items[slotNumber];
            itemTextblock.FontSize = 20;
            itemTextblock.Foreground = new SolidColorBrush(Colors.White);
            itemCanvas.Children.Add(itemTextblock);

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
            //Load the loot table
            MainWindow.LoadLootTable(lootTablePath + "/" + lootTableName);
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

