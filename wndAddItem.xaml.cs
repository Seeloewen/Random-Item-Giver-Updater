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
using System.Collections.ObjectModel;
using SeeloewenLib;
using System.Diagnostics;

namespace Random_Item_Giver_Updater
{
    public partial class wndAddItem : Window
    {
        //General attributes
        public bool isOpen;
        public int currentPage = 1;
        public ObservableCollection<addItemEntry> itemEntries { get; set; } = new ObservableCollection<addItemEntry>();
        public ObservableCollection<addToLootTableEntry> lootTableEntries { get; set; } = new ObservableCollection<addToLootTableEntry>();
        double addItemsWorkerProgress;
        int addItemsWorkerAddedItems;
        int addItemsWorkerAddedItemsLootTables;
        DateTime startTime;

        //Important references
        public MainWindow wndMain = (MainWindow)Application.Current.MainWindow;
        private wndSelectLootTables wndSelectLootTables;
        public SeeloewenLibTools SeeloewenLibTools = new SeeloewenLibTools();

        //Controls
        private BackgroundWorker bgwAddItems = new BackgroundWorker();
        private TextBlock tblLoadingItems = new TextBlock();
        public Wizard wzdAddItems;

        //-- Constructor --//

        public wndAddItem()
        {
            InitializeComponent();
            SetupControls();
            CreateWizard();
            DataContext = this;
        }

        //-- Event Handlers --//

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Change open state to true
            isOpen = true;

            //Setup the first page
            tblCurrentlySelectedDatapack.Text = string.Format("Currently selected Datapack: {0}\n{1}", wndMain.currentDatapack, wndMain.GetDatapackVersionInfo(wndMain.currentDatapack));

            //Setup backgroundworker
            bgwAddItems.WorkerReportsProgress = true;
            bgwAddItems.DoWork += bgwAddItems_DoWork;
            bgwAddItems.RunWorkerCompleted += bgwAddItems_RunWorkerCompleted;
            bgwAddItems.ProgressChanged += bgwAddItems_ProgressChanged;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            //Change open state to false
            isOpen = false;
        }

        private void btnAddAdditionalItem_Click(object sender, RoutedEventArgs e)
        {
            //Add a new empty item
            itemEntries.Add(new addItemEntry("minecraft", "", itemEntries.Count));
        }

        private void bgwAddItems_DoWork(object s, DoWorkEventArgs args)
        {
            //Reset previous progress
            addItemsWorkerProgress = 0;
            addItemsWorkerAddedItems = 0;
            addItemsWorkerAddedItemsLootTables = 0;

            //Go through each loot table and add the items
            foreach (lootTable lootTable in wndMain.lootTableList)
            {
                AddItems(string.Format("{0}/{1}", lootTable.lootTablePath, lootTable.lootTableName));
                addItemsWorkerAddedItemsLootTables++;
                addItemsWorkerAddedItems = 0;
            }
        }

        private void bgwAddItems_RunWorkerCompleted(object s, RunWorkerCompletedEventArgs args)
        {
            //Go to the next page
            wzdAddItems.ShowNextPage();
        }

        private void bgwAddItems_ProgressChanged(object s, ProgressChangedEventArgs progress)
        {
            //Report worker progress to progress bar
            pbAddingItems.Value = Convert.ToDouble(progress.UserState);

            //Report added items
            tblAddingItemsProgress.Text = string.Format("Adding items... (Item {0}/{1} - Loot Table {2}/{3})", progress.ProgressPercentage, itemEntries.Count, addItemsWorkerAddedItemsLootTables, wndMain.lootTableList.Count);
        }

        //-- Custom Methods --//


        private void CreateWizard()
        {
            //Create the wizard
            wzdAddItems = new Wizard(6, 580, 742, btnContinue, btnBack, Close, codeFinish, new Thickness(0, 0, 0, 0));
            grdAddItems.Children.Add(wzdAddItems.gbWizard);
            gbStep1.Content = null;
            gbStep2.Content = null;
            gbStep3.Content = null;
            gbStep4.Content = null;
            gbStep5.Content = null;
            gbStep6.Content = null;
            wzdAddItems.gbWizard.Foreground = new SolidColorBrush(Colors.White);
            wzdAddItems.gbWizard.FontSize = 16;

            //Setup the pages
            wzdAddItems.pages[0].grdContent.Children.Add(cvsStep1);
            wzdAddItems.pages[1].grdContent.Children.Add(cvsStep2);
            wzdAddItems.pages[2].grdContent.Children.Add(cvsStep3);
            wzdAddItems.pages[3].grdContent.Children.Add(cvsStep4);
            wzdAddItems.pages[4].grdContent.Children.Add(cvsStep5);
            wzdAddItems.pages[5].grdContent.Children.Add(cvsStep6);

            wzdAddItems.pages[2].code = codePage3;
            wzdAddItems.pages[2].requirements = requirementsPage3;
            wzdAddItems.pages[2].requirementsNotFulfilledMsg = "Please enter items you want to add to the datapack to continue!";
            wzdAddItems.pages[3].code = codePage4;
            wzdAddItems.pages[4].code = codePage5;
            wzdAddItems.pages[4].canGoBack = false;
            wzdAddItems.pages[4].canContinue = false;
            wzdAddItems.pages[5].code = codePage6;
            wzdAddItems.pages[5].canGoBack = false;
        }
        private bool requirementsPage3()
        {
            //Check if the item name textbox is empty
            if (!string.IsNullOrEmpty(tbItemName.Text))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void codePage3()
        {
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
        }

        private void codePage4()
        {
            //Update the items based on the settings from the last page
            foreach (addItemEntry item in lbItems.Items)
            {
                ListBoxItem listBoxItem = (ListBoxItem)lbItems.ItemContainerGenerator.ContainerFromItem(item);
                if (listBoxItem != null)
                {
                    ContentPresenter contentPresenter = FindVisualChild<ContentPresenter>(listBoxItem);
                    if (contentPresenter != null)
                    {
                        DataTemplate dataTemplate = contentPresenter.ContentTemplate;
                        if (dataTemplate != null)
                        {
                            Canvas canvas = dataTemplate.FindName("cvsItem", contentPresenter) as Canvas;
                            if (canvas != null)
                            {
                                TextBlock textblock = canvas.FindName("tbItemName") as TextBlock;
                                TextBlock textblock2 = canvas.FindName("tbItemPrefix") as TextBlock;
                                TextBlock textblock3 = canvas.FindName("tbItemNBT") as TextBlock;

                                if (textblock != null && textblock2 != null && textblock3 != null)
                                {
                                    // Aktualisiere die Daten im addItemEntry-Objekt
                                    item.itemName = textblock.Text;
                                    item.itemPrefix = textblock2.Text;
                                    item.itemNBT = textblock3.Text;
                                }
                            }
                        }
                    }
                }
            }


            //Add a loot table entry to every item
            lootTableEntries.Clear();
            foreach (addItemEntry entry in itemEntries)
            {
                entry.lootTableEntry = new addToLootTableEntry(string.Format("{0}:{1}", entry.itemPrefix, entry.itemName), entry.itemIndex);
                lootTableEntries.Add(entry.lootTableEntry);
            }
        }

        private void codePage5()
        {
            //Set start time for time calculation afterwards
            startTime = DateTime.Now;

            //Add the items
            bgwAddItems.RunWorkerAsync();
        }

        private void codePage6()
        {
            //Add all items to the added items list
            foreach (addItemEntry item in itemEntries)
            {
                tbAddedItemsList.AppendText(string.Format("{0}:{1}\n", item.itemPrefix, item.itemName));
            }

            //Show the elapsed time
            tblElapsedTime.Text = string.Format("Elapsed time: {0}", (DateTime.Now - startTime).ToString(@"hh\:mm\:ss"));
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
                    if (lootTable.Contains("main") || lootTable.Contains("special") || lootTable.Contains("normal"))
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

                            //Remove nbt entry construct if there is one even though the item has no NBT
                            if (hasNbtEntry == true)
                            {
                                temporaryConstruct = RemoveNbtEntry(temporaryConstruct);
                            }

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
                        //Remove nbt entry construct if there is one even though the item has no NBT
                        if (hasNbtEntry == true)
                        {
                            temporaryConstruct = RemoveNbtEntry(temporaryConstruct);
                        }

                        //Continue without NBT
                        temporaryFinalConstruct = temporaryConstruct;
                    }

                    //Write the construct with the item prefix, name and nbt to the file
                    File.AppendAllText(lootTable, string.Join(Environment.NewLine, temporaryFinalConstruct) + "\n");

                }

                //Report worker progress
                addItemsWorkerProgress = addItemsWorkerProgress + (100 / (Convert.ToDouble(itemEntries.Count * wndMain.lootTableList.Count)));
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

        public List<string> RemoveNbtEntry(List<string> construct)
        {
            //Create neccessary temporary variables
            List<string> tempConstruct = new List<string>();
            List<string> endConstruct = new List<string>();
            List<string> endConstruct2 = new List<string>();
            bool hasCount = false;
            bool hasCountSpecial = false;
            tempConstruct = construct;

            for (int i = construct.Count - 1; i >= 0; i--)
            {
                //Check if the loot table has a count, and if the count is special
                if (construct[i].Contains("\"target\":"))
                {
                    hasCountSpecial = true;
                }
                else if (construct[i].Contains("\"count\":"))
                {
                    hasCount = true;
                    //If it's not special, add the "count" section already to the end construct
                    if (hasCountSpecial == false)
                    {
                        endConstruct.Add(construct[i - 2]);
                        endConstruct.Add(construct[i - 1]);
                        endConstruct.Add(construct[i]);
                    }
                    break;
                }
            }

            if (hasCount == true && hasCountSpecial == false)
            {
                //If it has count but isn't special add all brackets (end part) to the a temporary end construct 2 until it hits a count or tag part
                for (int i = construct.Count - 1; i >= 0; i--)
                {
                    if (construct[i].Contains("\"count\":") || construct[i].Contains("\"tag\":"))
                    {
                        break;
                    }
                    else if (construct[i].Contains("}") || construct[i].Contains("]"))
                    {
                        endConstruct2.Add(construct[i]);
                    }
                }

                //Reverse the code and add it to the actual end construct
                endConstruct2.Reverse();
                foreach (string entry in endConstruct2)
                {
                    endConstruct.Add(entry);
                }

            }
            else if (hasCount == true && hasCountSpecial == true)
            {
                //If the loot table has special count add all the lines at the end to the temporary end construct 2 until it hits the count part
                for (int i = construct.Count - 1; i >= 0; i--)
                {
                    //If it hits the count section add that and stop
                    if (construct[i].Contains("\"count\":"))
                    {
                        endConstruct2.Add(construct[i]);
                        endConstruct2.Add(construct[i - 1]);
                        endConstruct2.Add(construct[i - 2]);
                        break;
                    }
                    else
                    {
                        endConstruct2.Add(construct[i]);
                        //If it hits a NBT section skip that
                        if (construct[i - 1].Contains("\"tag\":"))
                        {
                            i = i - 4;
                        }
                    }
                }

                //Reverse the code and add it to the actual end construct
                endConstruct2.Reverse();
                foreach (string entry in endConstruct2)
                {
                    endConstruct.Add(entry);
                }
            }
            else if (hasCount == false)
            {
                //If it hasn't got any count, remove the comma in the name part as it is no longer needed and would cause errors
                for (int i = construct.Count - 1; i >= 0; i--)
                {
                    if (construct[i].Contains("\"name\":"))
                    {
                        tempConstruct[i] = tempConstruct[i].Replace(",", "");
                    }
                }

                //Simply add the last character (most likely a comma) to the end construct
                endConstruct.Add(construct.Last());
            }

            for (int i = construct.Count - 1; i >= 0; i--)
            {
                //If it hasn't got a count, remove everything up until the name part
                if (hasCount == false)
                {
                    if (!tempConstruct[i].Contains("\"name\""))
                    {
                        tempConstruct.Remove(tempConstruct[i]);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    //If it has got a count, remove everything up until the beginning of the function part
                    if (!tempConstruct[i].Contains("\"functions\""))
                    {
                        tempConstruct.Remove(tempConstruct[i]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            //Add the end construct to the full construct and return that
            foreach (string entry in endConstruct)
            {
                tempConstruct.Add(entry);
            }
            return tempConstruct;
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                    return typedChild;

                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }

        private void SetupControls()
        {
            //Create 'Loading' text for loading items
            tblLoadingItems.Text = "Processing items, please wait...\nThis may take a few seconds!";
            tblLoadingItems.Foreground = new SolidColorBrush(Colors.White);
            tblLoadingItems.FontSize = 24;
        }

        private void codeFinish()
        {
            //Reload the currently loaded loot table and close this window
            if (wndMain.currentLootTable != "none")
            {
                wndMain.LoadLootTable(wndMain.currentLootTable);          
            }
            Close();
        }

        //-- Add Item Entry Event Handlers --//

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                //Get the canvas which the button is in
                Canvas canvas = SeeloewenLibTools.FindVisualParent<Canvas>(button);

                if (canvas.DataContext is addItemEntry item)
                {
                    //Remove the current item from the list
                    itemEntries.Remove(item);
                }
            }
        }

        //-- Add To Loot Table Entry Event Handlers --//

        private void btnEditCertainLootTables_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                //Get the canvas which the button is in
                Canvas canvas = SeeloewenLibTools.FindVisualParent<Canvas>(button);

                if (canvas.DataContext is addToLootTableEntry item)
                {
                    //Open loot table selection window
                    wndSelectLootTables = new wndSelectLootTables(item.lootTableCheckList, "Select the Loot Tables, that you want to add the item to.") { Owner = Application.Current.MainWindow };
                    wndSelectLootTables.Owner = Application.Current.MainWindow;
                    wndSelectLootTables.ShowDialog();

                    //Get loot tables string from loot table selection window
                    foreach (lootTable lootTable in wndSelectLootTables.lootTableList)
                    {
                        if (lootTable.cbAddToLootTable.IsChecked == true)
                        {
                            item.lootTableWhiteList = string.Format("{0}{1}", item.lootTableWhiteList, lootTable.fullLootTablePath);
                        }
                    }
                }
            }
        }

        private void rbtnAllLootTables_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radiobutton)
            {
                //Get the canvas which the radiobutton is in
                Canvas canvas = SeeloewenLibTools.FindVisualParent<Canvas>(radiobutton);

                if (canvas.DataContext is addToLootTableEntry item)
                {
                    //Disable edit button
                    Button button = canvas.FindName("btnEditCertainLootTables") as Button;
                    button.IsEnabled = false;

                    //Set checkstate on variable that gets accessed by the item adding thread
                    item.allLootTablesChecked = true;
                }
            }
        }

        private void rbtnCertainLootTables_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radiobutton)
            {
                //Get the canvas which the radiobutton is in
                Canvas canvas = SeeloewenLibTools.FindVisualParent<Canvas>(radiobutton);

                if (canvas.DataContext is addToLootTableEntry item)
                {
                    //Enable edit button
                    Button button = canvas.FindName("btnEditCertainLootTables") as Button;
                    button.IsEnabled = true;

                    //Set checkstate on variable that gets accessed by the item adding thread
                    item.allLootTablesChecked = false;
                }
            }
        }
    }
}
