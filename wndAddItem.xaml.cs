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

namespace Random_Item_Giver_Updater
{
    public partial class wndAddItem : Window
    {
        //General attributes
        public bool isOpen;
        List<addItemEntry> itemEntries = new List<addItemEntry>();

        //Reference to main window
        MainWindow wndMain = (MainWindow)Application.Current.MainWindow;

        //-- Constructor --//

        public wndAddItem()
        {
            InitializeComponent();
        }

        //-- Event Handlers --//

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Change open state to true
            isOpen = true;
            svItems.Content = stpItems;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            //Change open state to false
            isOpen = false;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //Go through each loot table and add the items
            foreach (lootTable lootTable in MainWindow.lootTableList)
            {
                AddItems(string.Format("{0}/{1}", lootTable.lootTablePath, lootTable.lootTableName));                       
            }

            //WIP -REMOVE
            MessageBox.Show("Successfully added the items!");
        }

        //-- Custom Methods --//

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
                        lootTableFile[lootTableFile.Length - index - 1] = lootTableFile[lootTableFile.Length - index - 1].Replace(GetItemName(lootTableFile[lootTableFile.Length - index - 1]), "!REPLACE_NAME!");
                    }
                    //If it contains the item NBT, replace
                    else if (lootTableFile[lootTableFile.Length - index - 1].Contains("\"tag\""))
                    {
                        hasNbtEntry = true;
                        lootTableFile[lootTableFile.Length - index - 1] = lootTableFile[lootTableFile.Length - index - 1].Replace(GetItemNBT(lootTableFile[lootTableFile.Length - index - 1]), "!REPLACE_NBT!");
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
                            lootTableFile[lootTableFile.Length - index - 1] = lootTableFile[lootTableFile.Length - index - 1].Replace(GetItemName(lootTableFile[lootTableFile.Length - index - 1]), "!REPLACE_NAME!");
                        }
                        //If it contains the item NBT, replace
                        else if (lootTableFile[lootTableFile.Length - index - 1].Contains("\"tag\""))
                        {
                            hasNbtEntry = true;
                            lootTableFile[lootTableFile.Length - index - 1] = lootTableFile[lootTableFile.Length - index - 1].Replace(GetItemNBT(lootTableFile[lootTableFile.Length - index - 1]), "!REPLACE_NBT!");
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
                List<string> temporaryConstruct = new List<string>();
                foreach (string line in itemConstruct)
                {
                    //Replace the name placeholder with the actual prefix and name and add to a temporary construct
                    if (line.Contains("!REPLACE_NAME!"))
                    {
                        temporaryConstruct.Add(line.Replace("!REPLACE_NAME!", string.Format("{0}{1}", item.tbItemPrefix.Text, item.tbItemName.Text)));
                    }
                    //Just add to the temporary construct
                    else
                    {
                        temporaryConstruct.Add(line);
                    }
                }

                //If an NBT tag is given
                List<string> temporaryFinalConstruct = new List<string>();
                if (!string.IsNullOrEmpty(item.tbItemNBT.Text))
                {
                    //Add nbt entry to construct if a NBT tag is given
                    if (hasNbtEntry == false)
                    {
                        temporaryConstruct = AddNbtEntry(temporaryConstruct);
                    }

                    foreach(string line in temporaryConstruct)
                    {
                        //Replace the NBT placeholder with the actual NBT tag and add to a temporary construct
                        if (line.Contains("!REPLACE_NBT!"))
                        {
                            temporaryFinalConstruct.Add(line.Replace("!REPLACE_NBT!", item.tbItemNBT.Text));
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

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            //Get all items into an array
            string[] items = tbItemName.Text.Split('\n');

            //Create an entry for every item
            int index = 0;
            foreach (string item in items)
            {
                itemEntries.Add(new addItemEntry(item, index));
                index++;
            }

            //Display all items
            foreach (addItemEntry entry in itemEntries)
            {
                stpItems.Children.Add(entry.bdrItem);
            }
        }
    }
}
