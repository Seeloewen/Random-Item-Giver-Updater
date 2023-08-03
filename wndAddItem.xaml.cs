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
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Reflection;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace Random_Item_Giver_Updater
{
    public partial class wndAddItem : Window
    {
        //General attributes
        public bool isOpen;
        public int currentPage = 1;
        public static List<addItemEntry> itemEntries = new List<addItemEntry>();
        double addItemsWorkerProgress;
        int addItemsWorkerAddedItems;
        int addItemsWorkerAddedItemsLootTables;
        DateTime startTime;

        //Reference to main window
        public MainWindow wndMain = (MainWindow)Application.Current.MainWindow;

        //Controls
        private BackgroundWorker bgwAddItems = new BackgroundWorker();
        private TextBlock tblLoadingItems = new TextBlock();

        //-- Constructor --//

        public wndAddItem()
        {
            InitializeComponent();
            SetupControls();
        }

        //-- Event Handlers --//

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Change open state to true
            isOpen = true;
            svItems.Content = stpItems;

            //Show the first page
            currentPage = 1;
            gbStep1.Visibility = Visibility.Visible;
            gbStep2.Visibility = Visibility.Hidden;
            gbStep3.Visibility = Visibility.Hidden;
            gbStep4.Visibility = Visibility.Hidden;
            gbStep5.Visibility = Visibility.Hidden;
            gbStep6.Visibility = Visibility.Hidden;
            btnBack.Content = "Close";
            tblCurrentlySelectedDatapack.Text = string.Format("Currently selected Datapack: {0}\n{1}", wndMain.currentDatapack, wndMain.GetDatapackVersionInfo(wndMain.currentDatapack));

            //Setup backgroundworker
            bgwAddItems.WorkerReportsProgress = true;
            bgwAddItems.DoWork += delegate (object s, DoWorkEventArgs args)
            {
                //Reset previous progress
                addItemsWorkerProgress = 0;
                addItemsWorkerAddedItems = 0;
                addItemsWorkerAddedItemsLootTables = 0;
                //Go through each loot table and add the items
                foreach (lootTable lootTable in MainWindow.lootTableList)
                {
                    AddItems(string.Format("{0}/{1}", lootTable.lootTablePath, lootTable.lootTableName));
                    addItemsWorkerAddedItemsLootTables++;
                    addItemsWorkerAddedItems = 0;
                }

            };

            bgwAddItems.RunWorkerCompleted += delegate (object s, RunWorkerCompletedEventArgs args)
            {
                //Go to the next page
                currentPage++;
                gbStep1.Visibility = Visibility.Hidden;
                gbStep2.Visibility = Visibility.Hidden;
                gbStep3.Visibility = Visibility.Hidden;
                gbStep4.Visibility = Visibility.Hidden;
                gbStep5.Visibility = Visibility.Hidden;
                gbStep6.Visibility = Visibility.Visible;
                btnContinue.Content = "Finish";
                btnContinue.IsEnabled = true;

                foreach (addItemEntry item in itemEntries)
                {
                    tbAddedItemsList.AppendText(string.Format("{0}:{1}\n", item.itemPrefix, item.itemName));
                }

                //Show the elapsed time
                tblElapsedTime.Text = string.Format("Elapsed time: {0}", (DateTime.Now - startTime).ToString(@"hh\:mm\:ss"));
            };

            bgwAddItems.ProgressChanged += delegate (object s, ProgressChangedEventArgs progress)
            {
                //Report worker progress to progress bar
                pbAddingItems.Value = Convert.ToDouble(progress.UserState);

                //Report added items
                tblAddingItemsProgress.Text = string.Format("Adding items... (Item {0}/{1} - Loot Table {2}/{3})", progress.ProgressPercentage, itemEntries.Count, addItemsWorkerAddedItemsLootTables, MainWindow.lootTableList.Count);
            };
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            //Change open state to false
            isOpen = false;
        }

        private async void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage == 1)
            {

                //Show the corresponding next page
                gbStep1.Visibility = Visibility.Hidden;
                gbStep2.Visibility = Visibility.Visible;
                gbStep3.Visibility = Visibility.Hidden;
                gbStep4.Visibility = Visibility.Hidden;
                gbStep5.Visibility = Visibility.Hidden;
                gbStep6.Visibility = Visibility.Hidden;
                btnBack.Content = "Back";

            }

            else if (currentPage == 2)
            {
                if (!string.IsNullOrEmpty(tbItemName.Text))
                {


                    //Show the corresponding next page
                    gbStep1.Visibility = Visibility.Hidden;
                    gbStep2.Visibility = Visibility.Hidden;
                    gbStep3.Visibility = Visibility.Visible;
                    gbStep4.Visibility = Visibility.Hidden;
                    gbStep5.Visibility = Visibility.Hidden;
                    gbStep6.Visibility = Visibility.Hidden;
                    await Task.Delay(5);

                    //Get all items into an array
                    string[] items = tbItemName.Text.Split('\n');

                    //Create an entry for every item
                    itemEntries.Clear();
                    int index = 0;
                    if (cbIncludesPrefixes.IsChecked == true)
                    {
                        //Split the item into prefix and item name if checkbox for prefixes is checked
                        foreach (string item in items)
                        {
                            string[] line = item.Split(':');
                            if (line.Length != 2)
                            {
                                //If the line actually doesn't contain a prefix, add the item normally with default prefix
                                itemEntries.Add(new addItemEntry("minecraft", line[0], index));
                                index++;
                            }
                            else
                            {
                                //Add the item with prefix and name
                                itemEntries.Add(new addItemEntry(line[0], line[1], index));
                                index++;
                            }
                        }
                    }
                    else
                    {
                        //Add the line as an item name if checkbox for prefixes is not checked
                        foreach (string item in items)
                        {
                            itemEntries.Add(new addItemEntry("minecraft", item, index));
                            index++;
                        }
                    }

                    //Show all items
                    UpdateItemDisplay();
                }
                else
                {
                    //Don't go to the next page and throw error
                    currentPage--;
                    MessageBox.Show("Please enter some items that you want to add!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else if (currentPage == 3)
            {
                //Show the corresponding next page
                gbStep1.Visibility = Visibility.Hidden;
                gbStep2.Visibility = Visibility.Hidden;
                gbStep3.Visibility = Visibility.Hidden;
                gbStep4.Visibility = Visibility.Visible;
                gbStep5.Visibility = Visibility.Hidden;
                gbStep6.Visibility = Visibility.Hidden;
                await Task.Delay(5);

                //Show loading message
                stpLootTableItems.Children.Add(tblLoadingItems);
                tblLoadingItems.Margin = new Thickness(220, 180, 0, 0);
                await Task.Delay(5);

                //Update the items based on the settings from the last page
                foreach (addItemEntry entry in itemEntries)
                {
                    entry.itemName = entry.tbItemName.Text;
                    entry.itemPrefix = entry.tbItemPrefix.Text;
                    entry.itemNBT = entry.tbItemNBT.Text;
                }

                //Add a loot table entry to every item
                foreach (addItemEntry entry in itemEntries)
                {
                    entry.lootTableEntry = new addToLootTableEntry(string.Format("{0}:{1}", entry.tbItemPrefix.Text, entry.tbItemName.Text), entry.itemIndex);
                }


                //Display all loot tables
                stpLootTableItems.Children.Clear();
                foreach (addItemEntry entry in itemEntries)
                {
                    stpLootTableItems.Children.Add(entry.lootTableEntry.bdrItem);
                }
            }
            else if (currentPage == 4)
            {
                //Show the corresponding next page
                gbStep1.Visibility = Visibility.Hidden;
                gbStep2.Visibility = Visibility.Hidden;
                gbStep3.Visibility = Visibility.Hidden;
                gbStep4.Visibility = Visibility.Hidden;
                gbStep5.Visibility = Visibility.Visible;
                gbStep6.Visibility = Visibility.Hidden;
                btnBack.IsEnabled = false;
                btnContinue.IsEnabled = false;

                //Set start time for time calculation afterwards
                startTime = DateTime.Now;

                //Add the items
                bgwAddItems.RunWorkerAsync();
            }
            else if (currentPage == 6)
            {
                //Exit the window
                Close();
            }

            //Increase the page number
            currentPage++;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage == 1)
            {
                //Exit the window
                Close();

            }
            if (currentPage == 2)
            {
                //Show the corresponding last page
                gbStep1.Visibility = Visibility.Visible;
                gbStep2.Visibility = Visibility.Hidden;
                gbStep3.Visibility = Visibility.Hidden;
                gbStep4.Visibility = Visibility.Hidden;
                gbStep5.Visibility = Visibility.Hidden;
                gbStep6.Visibility = Visibility.Hidden;
                btnBack.Content = "Close";
            }
            else if (currentPage == 3)
            {
                //Show warning if there are items in the list as they could be modified
                MessageBoxResult result = MessageBox.Show("Warning: You will loose any modifications to your items. Do you really want to go back?", "Go back", MessageBoxButton.YesNo, MessageBoxImage.Question);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        //Show the corresponding last page
                        gbStep1.Visibility = Visibility.Hidden;
                        gbStep2.Visibility = Visibility.Visible;
                        gbStep3.Visibility = Visibility.Hidden;
                        gbStep4.Visibility = Visibility.Hidden;
                        gbStep5.Visibility = Visibility.Hidden;
                        gbStep6.Visibility = Visibility.Hidden;

                        //Clear content of current page
                        stpItems.Children.Clear();

                        break;
                    case MessageBoxResult.No:
                        //Increase the page number by one as it will be decreased later anyway
                        currentPage++;
                        break;
                }

            }
            if (currentPage == 4)
            {
                //Show warning if there are items in the list as they could be modified
                MessageBoxResult result = MessageBox.Show("Warning: You will loose any loot table selections. Do you really want to go back?", "Go back", MessageBoxButton.YesNo, MessageBoxImage.Question);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        //Show the corresponding last page
                        gbStep1.Visibility = Visibility.Hidden;
                        gbStep2.Visibility = Visibility.Hidden;
                        gbStep3.Visibility = Visibility.Visible;
                        gbStep4.Visibility = Visibility.Hidden;
                        gbStep5.Visibility = Visibility.Hidden;
                        gbStep6.Visibility = Visibility.Hidden;

                        //Clear content of current page
                        stpLootTableItems.Children.Clear();

                        break;
                    case MessageBoxResult.No:
                        //Increase the page number by one as it will be decreased later anyway
                        currentPage++;
                        break;
                }
            }

            //Decrease the page number
            currentPage--;
        }

        private void btnAddAdditionalItem_Click(object sender, RoutedEventArgs e)
        {
            //Add a new empty item
            itemEntries.Add(new addItemEntry("minecraft", "", itemEntries.Count));

            //Display all items
            UpdateItemDisplay();
        }

        //-- Custom Methods --//

        public async void UpdateItemDisplay()
        {
            //Show loading message
            stpItems.Children.Add(tblLoadingItems);
            tblLoadingItems.Margin = new Thickness(220, 180, 0, 0);
            await Task.Delay(5);

            //Display all items
            stpItems.Children.Clear();
            foreach (addItemEntry entry in itemEntries)
            {
                stpItems.Children.Add(entry.bdrItem);
            }
        }

        public void AddItems(string lootTable)
        {
            //Load the file
            string[] lootTableFile = File.ReadAllLines(lootTable);
            List<string> fileEnd = new List<string>();
            bool hasNbtEntry = false;

            //Remove the last 4 lines to allow adding new items by copying the array into a new array that is 5 lines shorter
            string[] fileWithoutEnd = new string[lootTableFile.Length - 4];
            Array.Copy(lootTableFile, fileWithoutEnd, lootTableFile.Length - 4);

            //Build the file end construct to insert later
            for (int i = 0; i < 4; i++)
            {
                fileEnd.Insert(0, lootTableFile[lootTableFile.Length - i - 1]);
            }

            //Add a comma at the last bracket
            fileWithoutEnd[fileWithoutEnd.Length - 1] = fileWithoutEnd[fileWithoutEnd.Length - 1].Replace("}", "},");

            //Write to file
            File.WriteAllLines(lootTable, fileWithoutEnd);

            //Copy the last part item construct and paste it at the bottom
            List<string> itemConstruct = new List<string>();
            lootTableFile = File.ReadAllLines(lootTable);

            //Remove the comma at the end to avoid corruption
            lootTableFile[lootTableFile.Length - 1] = lootTableFile[lootTableFile.Length - 1].Replace("},", "}");

            //Start at the end and go through each line until it hits another item
            bool doLoop = true;
            int index = 0;
            while (doLoop == true)
            {
                if (!(lootTableFile[lootTableFile.Length - index - 1].Contains("},")))
                {
                    //If it contains the item name, replace
                    if (lootTableFile[lootTableFile.Length - index - 1].Contains("\"name\"") && lootTableFile[lootTableFile.Length - index - 2].Contains("item\""))
                    {
                        lootTableFile[lootTableFile.Length - index - 1] = lootTableFile[lootTableFile.Length - index - 1].Replace(GetItemName(lootTableFile[lootTableFile.Length - index - 1]), "{!REPLACE_NAME!}");
                    }
                    //If it contains the item NBT, replace
                    else if (lootTableFile[lootTableFile.Length - index - 1].Contains("\"tag\""))
                    {
                        hasNbtEntry = true;
                        lootTableFile[lootTableFile.Length - index - 1] = lootTableFile[lootTableFile.Length - index - 1].Replace(GetItemNBT(lootTableFile[lootTableFile.Length - index - 1]), "{!REPLACE_NBT!}");
                    }

                    //Add line to item construct and continue
                    itemConstruct.Insert(0, lootTableFile[lootTableFile.Length - index - 1]);
                    index++;
                }
                else
                {
                    if (lootTableFile[lootTableFile.Length - index - 2].Contains("\"name\": \"out\"") || lootTableFile[lootTableFile.Length - index - 2].Contains("\"count\":") || lootTableFile[lootTableFile.Length - index - 2].Contains("\"tag\":") || lootTableFile[lootTableFile.Length - index - 2].Contains("}"))
                    {
                        //If it contains the item name, replace
                        if (lootTableFile[lootTableFile.Length - index - 1].Contains("\"name\"") && lootTableFile[lootTableFile.Length - index - 2].Contains("item\""))
                        {
                            lootTableFile[lootTableFile.Length - index - 1] = lootTableFile[lootTableFile.Length - index - 1].Replace(GetItemName(lootTableFile[lootTableFile.Length - index - 1]), "{!REPLACE_NAME!}");
                        }
                        //If it contains the item NBT, replace
                        else if (lootTableFile[lootTableFile.Length - index - 1].Contains("\"tag\""))
                        {
                            hasNbtEntry = true;
                            lootTableFile[lootTableFile.Length - index - 1] = lootTableFile[lootTableFile.Length - index - 1].Replace(GetItemNBT(lootTableFile[lootTableFile.Length - index - 1]), "{!REPLACE_NBT!}");
                        }

                        //If it has reached a bracket with comma, but not one that indicates another item, add line continue
                        itemConstruct.Insert(0, lootTableFile[lootTableFile.Length - index - 1]);
                        index++;
                    }
                    else
                    {
                        //Stop if it has reached another item
                        doLoop = false;
                    }
                }
            }
            //Add the comma back
            itemConstruct[itemConstruct.Count - 1] = itemConstruct[itemConstruct.Count - 1] + ",";

            //Add all the items to the loot table
            foreach (addItemEntry item in itemEntries)
            {

                //Only add the item to the loot table if selected
                if (item.lootTableEntry.allLootTablesChecked == true)
                {
                    if (lootTable.Contains("main") || lootTable.Contains("special"))
                    {
                        List<string> temporaryConstruct = new List<string>();
                        foreach (string line in itemConstruct)
                        {
                            //Replace the name placeholder with the actual prefix and name and add to a temporary construct
                            if (line.Contains("{!REPLACE_NAME!}"))
                            {
                                temporaryConstruct.Add(line.Replace("{!REPLACE_NAME!}", string.Format("{0}:{1}", item.itemPrefix, item.itemName)));
                            }
                            //Just add to the temporary construct
                            else
                            {
                                temporaryConstruct.Add(line);
                            }
                        }

                        //If an NBT tag is given
                        List<string> temporaryFinalConstruct = new List<string>();
                        if (!string.IsNullOrEmpty(item.itemNBT))
                        {
                            //Add nbt entry to construct if a NBT tag is given
                            if (hasNbtEntry == false)
                            {
                                temporaryConstruct = AddNbtEntry(temporaryConstruct);
                            }

                            foreach (string line in temporaryConstruct)
                            {
                                //Replace the NBT placeholder with the actual NBT tag and add to a temporary construct
                                if (line.Contains("{!REPLACE_NBT!}"))
                                {
                                    temporaryFinalConstruct.Add(line.Replace("{!REPLACE_NBT!}", item.itemNBT));
                                }
                                else
                                {
                                    temporaryFinalConstruct.Add(line);
                                }
                            }
                        }
                        else
                        {
                            //Continue without NBT
                            temporaryFinalConstruct = temporaryConstruct;
                        }
                        //Write the construct with the item prefix, name and nbt to the file
                        File.AppendAllText(lootTable, string.Join(Environment.NewLine, temporaryFinalConstruct) + "\n");


                    }
                }
                else if (item.lootTableEntry.allLootTablesChecked == false && item.lootTableEntry.lootTableWhiteList.Contains(lootTable))
                {
                    List<string> temporaryConstruct = new List<string>();
                    foreach (string line in itemConstruct)
                    {
                        //Replace the name placeholder with the actual prefix and name and add to a temporary construct
                        if (line.Contains("{!REPLACE_NAME!}"))
                        {
                            temporaryConstruct.Add(line.Replace("{!REPLACE_NAME!}", string.Format("{0}:{1}", item.itemPrefix, item.itemName)));
                        }
                        //Just add to the temporary construct
                        else
                        {
                            temporaryConstruct.Add(line);
                        }
                    }

                    //If an NBT tag is given
                    List<string> temporaryFinalConstruct = new List<string>();
                    if (!string.IsNullOrEmpty(item.itemNBT))
                    {
                        //Add nbt entry to construct if a NBT tag is given
                        if (hasNbtEntry == false)
                        {
                            temporaryConstruct = AddNbtEntry(temporaryConstruct);
                        }

                        foreach (string line in temporaryConstruct)
                        {
                            //Replace the NBT placeholder with the actual NBT tag and add to a temporary construct
                            if (line.Contains("{!REPLACE_NBT!}"))
                            {
                                temporaryFinalConstruct.Add(line.Replace("{!REPLACE_NBT!}", item.itemNBT));
                            }
                            else
                            {
                                temporaryFinalConstruct.Add(line);
                            }
                        }
                    }
                    else
                    {
                        //Continue without NBT
                        temporaryFinalConstruct = temporaryConstruct;
                    }

                    //Write the construct with the item prefix, name and nbt to the file
                    File.AppendAllText(lootTable, string.Join(Environment.NewLine, temporaryFinalConstruct) + "\n");

                }

                //Report worker progress
                addItemsWorkerProgress = addItemsWorkerProgress + (100 / (Convert.ToDouble(itemEntries.Count * MainWindow.lootTableList.Count)));
                addItemsWorkerAddedItems++;
                bgwAddItems.ReportProgress(addItemsWorkerAddedItems, addItemsWorkerProgress);

            }
            //Remove the comma at the end to avoid corruption
            lootTableFile = File.ReadAllLines(lootTable);
            lootTableFile[lootTableFile.Length - 1] = lootTableFile[lootTableFile.Length - 1].Replace("},", "}");
            File.WriteAllLines(lootTable, lootTableFile);

            //Write the end construct to the file
            File.AppendAllText(lootTable, string.Join(Environment.NewLine, fileEnd));
        }

        public string GetItemName(string line)
        {
            //Remove strings to only return item name
            line = line.Replace("\"name\": ", "");
            line = line.Replace(" ", "");
            line = line.Replace("\"", "");
            line = line.Replace(",", "");
            return line;
        }

        public string GetItemNBT(string line)
        {
            //Remove strings to only return item NBT
            line = line.Replace("\"tag\": ", "");
            line = line.Replace(" ", "");
            line = line.Substring(1, line.Length - 2);
            return line;
        }

        public List<string> AddNbtEntry(List<string> construct)
        {
            //Check if "functions" entry exists
            bool functionsExists = false;
            foreach (string line in construct)
            {
                if (line.Contains("\"functions\": ["))
                {
                    functionsExists = true;
                }
            }

            //If "functions" entry already exists
            if (functionsExists == true)
            {
                construct[construct.Count - 3] = construct[construct.Count - 3].Replace("}", "},");
                construct.Insert(construct.Count - 2, "                        {");
                construct.Insert(construct.Count - 2, "                            \"function\": \"set_nbt\",");
                construct.Insert(construct.Count - 2, "                            \"tag\": \"!REPLACE_NBT!\"");
                construct.Insert(construct.Count - 2, "                        }");
                return construct;
            }
            else //If it doesn't exist
            {
                construct[construct.Count - 2] = construct[construct.Count - 2] + ",";
                construct.Insert(construct.Count - 1, "                    \"functions\": [");
                construct.Insert(construct.Count - 1, "                        {");
                construct.Insert(construct.Count - 1, "                            \"function\": \"set_nbt\",");
                construct.Insert(construct.Count - 1, "                            \"tag\": \"!REPLACE_NBT!\"");
                construct.Insert(construct.Count - 1, "                        }");
                construct.Insert(construct.Count - 1, "                    ]");
                return construct;
            }
        }

        private void SetupControls()
        {
            //Create 'Loading' text for loading items
            tblLoadingItems.Text = "Processing items, please wait...\nThis may take a few seconds!";
            tblLoadingItems.Foreground = new SolidColorBrush(Colors.White);
            tblLoadingItems.FontSize = 24;
        }
    }
}
