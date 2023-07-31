using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace Random_Item_Giver_Updater
{
    public class lootTable
    {
        //Controls
        public Canvas cvsLootTable = new Canvas();
        public TextBlock tblLootTable = new TextBlock();
        public CheckBox cbAddToLootTable = new CheckBox();

        //Attributes
        public string lootTableName;
        public string lootTableType;
        public string lootTablePath;
        public string fullLootTablePath;
        public bool isSelectedForAdding = true;

        //Windows
        MainWindow wndMain = (MainWindow)Application.Current.MainWindow;


        //-- Constructor --//

        public lootTable(string name, string type, string path)
        {
            //Set attributes
            lootTableName = name.Replace("\\", "");
            lootTableType = type;
            lootTablePath = path;
            fullLootTablePath = string.Format("{0}/{1}", lootTablePath, lootTableName);

            //Create canvas
            cvsLootTable.Height = 35;
            cvsLootTable.Background = new SolidColorBrush(Color.FromArgb(100, 65, 65, 65));

            //Create text			
            tblLootTable.Text = name.Replace("\\", "");
            tblLootTable.FontSize = 15;
            tblLootTable.Foreground = new SolidColorBrush(Colors.White);
            tblLootTable.FontSize = 15;
            tblLootTable.Margin = new Thickness(25, 10, 0, 0);
            cvsLootTable.Children.Add(tblLootTable);

            //Create checkbox for adding items window
            cbAddToLootTable.Content = fullLootTablePath.Replace(string.Format("{0}/data/randomitemgiver/loot_tables/", MainWindow.currentDatapack), "").Replace(".json", "");
            cbAddToLootTable.Foreground = new SolidColorBrush(Colors.White);
            cbAddToLootTable.Margin = new Thickness(20, 15, 0, 0);
            cbAddToLootTable.FontSize = 15;
            cbAddToLootTable.IsChecked = true;

            //Add mouse down event to load the loot table
            cvsLootTable.MouseDown += new MouseButtonEventHandler(cvsLootTable_MouseDown);
        }


        //-- Event Handlers --//

        private void cvsLootTable_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainWindow.currentLootTable != "none")
            {
                if (MainWindow.lootTableModified() == true)
                {
                    //Show warning if there are unsaved changes to the loot table
                    MessageBoxResult result = MessageBox.Show("You still have unsaved modifications in the current loot table.\nDo you want to save the changes before continuing?", "Save changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            //Save the current loot table
                            wndMain.SaveCurrentLootTable();

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
}
