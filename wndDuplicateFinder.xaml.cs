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

namespace Random_Item_Giver_Updater
{
    public partial class wndDuplicateFinder : Window
    {
        //Controls
        Wizard wzdDuplicateFinder;
        TextBlock tblLoadingDuplicates = new TextBlock();
        TextBlock tblNoDuplicatesFound = new TextBlock();
        System.Windows.Forms.SaveFileDialog sfdDuplicateList = new System.Windows.Forms.SaveFileDialog();

        //General attributes
        List<duplicateEntry> duplicateEntries = new List<duplicateEntry>();
        int duplicateIndex = 0;

        //Reference to main window
        MainWindow wndMain = (MainWindow)Application.Current.MainWindow;

        //Seeloewen Lib
        SeeloewenLibTools SeeloewenLibTools = new SeeloewenLibTools();

        //-- Constructor --//

        public wndDuplicateFinder()
        {
            InitializeComponent();
            CreateWizard();
            SetupControls();
        }

        //-- Event Handlers --//

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This feature is not available yet.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        //-- Custom Methods --//

        private async void CheckForDuplicates()
        {
            //If the rabiobutton for only the current loot table is checked
            if (rbtnCurrent.IsChecked == true)
            {
                //Show loading message
                stpDuplicates.Children.Add(tblLoadingDuplicates);
                tblLoadingDuplicates.Margin = new Thickness(175, 150, 0, 0);
                await Task.Delay(5);

                //Check the current loot table
                duplicateEntries.Clear();
                CheckLootTable(wndMain.currentLootTable, wndMain.currentLootTable.Replace(wndMain.currentDatapack, "").Replace("/data/randomitemgiver/loot_tables/", ""));

                //Show a message with the amount of duplicates and display them in a list
                stpDuplicates.Children.Clear();
                foreach (duplicateEntry duplicateEntry in duplicateEntries)
                {
                    stpDuplicates.Children.Add(duplicateEntry.cvsItem);
                }
                MessageBox.Show(string.Format("Successfully searched for duplicates. Found {0} results.", duplicateEntries.Count()), "Search completed", MessageBoxButton.OK, MessageBoxImage.Information);

                //If no duplicates are found, show information text
                if (duplicateEntries.Count == 0)
                {
                    stpDuplicates.Children.Add(tblNoDuplicatesFound);
                    tblNoDuplicatesFound.Margin = new Thickness(220, 175, 0, 0);
                }
            }

            //if the radiobutton for all loot tables in the current datapack is checked
            else if (rbtnAll.IsChecked == true)
            {
                //Show loading message
                stpDuplicates.Children.Add(tblLoadingDuplicates);
                tblLoadingDuplicates.Margin = new Thickness(175, 150, 0, 0);
                await Task.Delay(5);

                //Check all loot tables in the current datapack
                duplicateEntries.Clear();
                duplicateIndex = 0;
                foreach (lootTable lootTable in wndMain.lootTableList)
                {
                    CheckLootTable(lootTable.fullLootTablePath, lootTable.fullLootTablePath.Replace(wndMain.currentDatapack, "").Replace("/data/randomitemgiver/loot_tables/", ""));
                }

                //Show a message with the amount of duplicates and display them in a list
                stpDuplicates.Children.Clear();
                foreach (duplicateEntry duplicateEntry in duplicateEntries)
                {
                    stpDuplicates.Children.Add(duplicateEntry.cvsItem);
                }
                MessageBox.Show(string.Format("Successfully searched for duplicates. Found {0} results.", duplicateEntries.Count()), "Search completed", MessageBoxButton.OK, MessageBoxImage.Information);

                //If no duplicates are found, show information text
                if (duplicateEntries.Count == 0)
                {
                    stpDuplicates.Children.Add(tblNoDuplicatesFound);
                    tblNoDuplicatesFound.Margin = new Thickness(220, 175, 0, 0);
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
                            finalItemList.Add(string.Format("{0} (NBT: {1})", items[i], items[i + 1]));
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
                foreach(duplicateEntry duplicate in duplicateEntries)
                {
                    //If the item already exists in the duplicate list, update it and stop searching
                    if(duplicate.itemName == item)
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
            //Create 'Loading' text for searching duplicates
            tblLoadingDuplicates.Text = "Searching for duplicates, please wait...\nThis may take a few seconds!";
            tblLoadingDuplicates.Foreground = new SolidColorBrush(Colors.White);
            tblLoadingDuplicates.FontSize = 24;

            //Create information text when no duplicates are found
            tblNoDuplicatesFound.Text = "No duplicates were found.";
            tblNoDuplicatesFound.Foreground = new SolidColorBrush(Colors.White);
            tblNoDuplicatesFound.FontSize = 24;
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
            foreach(duplicateEntry duplicate in duplicateEntries)
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
    }
}
