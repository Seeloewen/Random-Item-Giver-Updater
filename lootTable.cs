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
                if (MainWindow.lootTableModified() == true)
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
}
