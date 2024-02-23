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
using SeeloewenLib;
using System.Runtime.Remoting.Messaging;
using System.Collections.ObjectModel;

namespace Random_Item_Giver_Updater
{
    public partial class wndDuplicateFinder : Window
    {
        //Controls
        Wizard wzdDuplicateFinder;
        TextBlock tblNoDuplicatesFound = new TextBlock();
        System.Windows.Forms.SaveFileDialog sfdDuplicateList = new System.Windows.Forms.SaveFileDialog();

        //General attributes
        public ObservableCollection<duplicateEntry> duplicateEntries { get; set; } = new ObservableCollection<duplicateEntry>();
        int duplicateIndex = 0;

        //Reference to other windows
        MainWindow wndMain = (MainWindow)Application.Current.MainWindow;
        static wndRemoveItems wndRemoveItems;

        //Seeloewen Lib
        SeeloewenLibTools SeeloewenLibTools = new SeeloewenLibTools();

        //-- Constructor --//

        public wndDuplicateFinder()
        {
            InitializeComponent();
            CreateWizard();
            SetupControls();
            DataContext = this;
        }

        //-- Event Handlers --//

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //Add the duplicates to a string list
            List<string> duplicates = new List<string>();
            foreach(duplicateEntry duplicate in duplicateEntries)
            {
               duplicates.Add(duplicate.itemName);
            }

            //Open remove item window
            wndRemoveItems = new wndRemoveItems(true, duplicates) { Owner = this };
            wndRemoveItems.Owner = Application.Current.MainWindow;
            if (wndRemoveItems.isOpen == false)
            {
                //Check if the datapack path exists before opening the window
                if (Directory.Exists(wndMain.currentDatapack))
                {
                    wndRemoveItems.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Error: Could not detect datapack. Please make sure the currently selected datapack exists and is valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            Close();
        }

        //-- Custom Methods --//

        private void CheckForDuplicates()
        {
            //If the rabiobutton for only the current loot table is checked
            if (rbtnCurrent.IsChecked == true)
            {
                //Check the current loot table
                duplicateEntries.Clear();
                CheckLootTable(wndMain.currentLootTable, wndMain.currentLootTable.Replace(wndMain.currentDatapack, "").Replace("/data/randomitemgiver/loot_tables/", ""));

                //Show a message with the amount of duplicates and display them in a list
                MessageBox.Show(string.Format("Successfully searched for duplicates. Found {0} results.", duplicateEntries.Count()), "Search completed", MessageBoxButton.OK, MessageBoxImage.Information);

                //If no duplicates are found, show information text
                if (duplicateEntries.Count == 0)
                {
                    tblNoDuplicatesFound.Visibility = Visibility.Visible;
                    tblNoDuplicatesFound.Margin = new Thickness(225, 275, 0, 0);
                }
            }

            //if the radiobutton for all loot tables in the current datapack is checked
            else if (rbtnAll.IsChecked == true)
            {
                //Check all loot tables in the current datapack
                duplicateEntries.Clear();
                duplicateIndex = 0;
                foreach (lootTable lootTable in wndMain.lootTableList)
                {
                    CheckLootTable(lootTable.fullLootTablePath, lootTable.fullLootTablePath.Replace(wndMain.currentDatapack, "").Replace("/data/randomitemgiver/loot_tables/", ""));
                }

                //Show a message with the amount of duplicates
                MessageBox.Show(string.Format("Successfully searched for duplicates. Found {0} results.", duplicateEntries.Count()), "Search completed", MessageBoxButton.OK, MessageBoxImage.Information);

                //If no duplicates are found, show information text and disable buttons regarding duplicates
                if (duplicateEntries.Count == 0)
                {
                    tblNoDuplicatesFound.Visibility = Visibility.Visible;
                    tblNoDuplicatesFound.Margin = new Thickness(225, 275, 0, 0);
                    btnExportList.IsEnabled = false;
                    btnDelete.IsEnabled = false;
                }
                else
                {
                    btnExportList.IsEnabled = true;
                    btnDelete.IsEnabled = true;
                }
            }
        }

        private void CheckLootTable(string path, string lootTable)
        {
            //Load the loot table so it can go through each item
            //Get list of content in file, remove all non-item lines so only items remain
            string[] loadedItems = File.ReadAllLines(path);
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
                    items.Add(itemFiltered + "{isNBT}"); //Add extra NBT marker to let software know it's NBT
                }
                else if (!item.Contains("\"tag\"") && !item.Contains("{") && !item.Contains("}") && !item.Contains("[") && !item.Contains("]") && !item.Contains("\"rolls\"") && !item.Contains("\"type\"") && !item.Contains("\"function\"") && item.Contains("\"") && !item.Contains("\"weight\"") && !item.Contains("\"count\"") && !item.Contains("\"min\": 1") && !item.Contains("\"max\": 64")&& !item.Contains("\"RandomItemGiver\"") && !item.Contains("\"out\"") && !item.Contains("\"score\""))
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
                if (!items[i].Contains("{isNBT}"))
                {
                    if (i < items.Count - 1)
                    {

                        if (items[i + 1].Contains("{"))
                        {
                            finalItemList.Add(string.Format("{0} (NBT: {1})", items[i], items[i + 1].Replace("{isNBT}", "")));
                        }

                        if (!items[i + 1].Contains("{"))
                        {
                            finalItemList.Add(string.Format("{0}", items[i]));
                        }
                    }
                    else
                    {
                        {
                            finalItemList.Add(string.Format("{0}", items[i]));
                        }
                    }
                }
            }

            //Filter the duplicates from the full item list
            var duplicates = finalItemList
                .GroupBy(i => i)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            //Go through each item in the duplicate list
            foreach (string item in duplicates)
            {
                bool wasAdded = false;

                //Go through every item in the duplicate list
                foreach (duplicateEntry duplicate in duplicateEntries)
                {
                    //If the item already exists in the duplicate list, update it and stop searching
                    if (duplicate.itemName == item)
                    {
                        duplicate.UpdateLootTables(lootTable);
                        duplicate.UpdateAmount();
                        wasAdded = true;
                        break;
                    }
                }

                //Add the item as a new entry if it hasn't been added yet
                if (wasAdded == false)
                {
                    duplicateIndex++;
                    duplicateEntries.Add(new duplicateEntry(item, lootTable, duplicateIndex));
                }
            }
        }

        private void CreateWizard()
        {
            //Create the wizard
            wzdDuplicateFinder = new Wizard(2, 580, 742, btnContinue, btnBack, Close, Close, new Thickness(0, 0, 0, 0));
            grdDuplicateFinder.Children.Add(wzdDuplicateFinder.gbWizard);
            grdDuplicateFinder.Children.Remove(cvsStep1);
            grdDuplicateFinder.Children.Remove(cvsStep2);
            wzdDuplicateFinder.gbWizard.Foreground = new SolidColorBrush(Colors.White);
            wzdDuplicateFinder.gbWizard.FontSize = 16;

            //Setup the pages
            wzdDuplicateFinder.pages[0].grdContent.Children.Add(cvsStep1);
            wzdDuplicateFinder.pages[1].grdContent.Children.Add(cvsStep2);
            wzdDuplicateFinder.pages[1].code = CheckForDuplicates;
            wzdDuplicateFinder.pages[1].requirements = pageTwoRequirements;
            wzdDuplicateFinder.pages[1].requirementsNotFulfilledMsg = "Cannot search for duplicates. Please make sure that a datapack or loot table is loaded.";
            wzdDuplicateFinder.pages[1].canGoBack = false;
            cvsStep1.Margin = new Thickness(10, 10, 0, 0);
            cvsStep2.Margin = new Thickness(10, 10, 0, 0);
            cvsStep1.Visibility = Visibility.Visible;
            cvsStep2.Visibility = Visibility.Visible;
        }

        private void SetupControls()
        {
            //Create information text when no duplicates are found
            tblNoDuplicatesFound.Text = "No duplicates were found.";
            tblNoDuplicatesFound.Foreground = new SolidColorBrush(Colors.White);
            tblNoDuplicatesFound.FontSize = 24;
            tblNoDuplicatesFound.Visibility = Visibility.Hidden;
            cvsStep2.Children.Add(tblNoDuplicatesFound);
        }

        private bool pageTwoRequirements()
        {
            //Check which radiobutton is checked
            if (rbtnAll.IsChecked == true)
            {
                //If radiobutton for all loot tables is checked, no additional check is needed since the software already made sure that a datapack is loaded before opening duplicate finder
                return true;
            }
            else if (rbtnCurrent.IsChecked == true)
            {
                if (wndMain.currentLootTable != "none")
                {
                    //If the radiobutton for the current loot table is checked and a loot table is selected, return true
                    return true;
                }
                else
                {
                    //If no loot table is selected, don't continue and show error (return false)
                    return false;
                }
            }
            else
            {
                //If no radiobutton is checked (which shouldn't be possible), return false
                return false;
            }
        }

        private void btnExportList_Click(object sender, RoutedEventArgs e)
        {
            //Create a list of strings and add each entry in the duplicate list as a new line
            List<string> duplicatesList = new List<string>();
            duplicatesList.Add("Item Name - Amount - Loot Table(s)");
            foreach (duplicateEntry duplicate in duplicateEntries)
            {
                duplicatesList.Add(string.Format("{0} - {1} - {2}", duplicate.itemName, duplicate.amount, duplicate.lootTables));
            }

            //Show save file dialog
            sfdDuplicateList.Filter = "Text (*.txt)|*.txt|All (*.*)|*.*";
            sfdDuplicateList.FileName = "Random_Item_Giver_Updater_Duplicate_List.txt";
            sfdDuplicateList.Title = "Export duplicate list...";
            if (sfdDuplicateList.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //Save the duplicate list as file
                File.WriteAllText(sfdDuplicateList.FileName, SeeloewenLibTools.ConvertListToString(duplicatesList));

                //Show confirmation message
                MessageBox.Show(string.Format("Successfully saved the duplicate list to {0}", sfdDuplicateList.FileName), "Saved duplicate list", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //-- Duplicate Entry Event Handlers --//

        private void btnViewAll_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                //Get the canvas which the button is in
                Canvas canvas = SeeloewenLibTools.FindVisualParent<Canvas>(button);

                //Show a message in which loot tables the duplicate occurs
                if (canvas.DataContext is duplicateEntry duplicateEntry)
                {
                    MessageBox.Show(string.Format("The duplicate occurs in the following loot tables:\n{0}", duplicateEntry.lootTables.Replace(", ", "\n")), "View all Loot Tables", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void tblItemName_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                //Show the controls for editing and hide the original name
                MessageBox.Show(string.Format("Full name of the item:\n{0}", textBlock.Text), "Full item name", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
