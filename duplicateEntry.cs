using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Random_Item_Giver_Updater
{
    internal class duplicateEntry
    {
        //Controls
        public Canvas cvsItem = new Canvas();
        private TextBlock tblItemName = new TextBlock();
        private TextBlock tblAmount = new TextBlock();
        private TextBlock tblLootTables = new TextBlock();
        public Button btnViewAll = new Button();

        //Item attributes
        public string itemName;
        public string lootTables;
        public int amount;

        //Reference to main window
        MainWindow wndMain = (MainWindow)Application.Current.MainWindow;


        //-- Constructor --//

        public duplicateEntry(string name, string location, int index)
        {

            //Initialize variables
            itemName = name.TrimEnd('\r', '\n');
            lootTables = location;
            amount = 1;

            //Set backcolor
            if (index % 2 == 0)
            {
                cvsItem.Background = new SolidColorBrush(Color.FromArgb(100, 70, 70, 70));
            }
            else
            {
                cvsItem.Background = new SolidColorBrush(Color.FromArgb(100, 50, 50, 50));
            }
            cvsItem.Height = 40;

            //Create item name text
            tblItemName.Margin = new Thickness(22, 10, 0, 0);
            tblItemName.Text = itemName;
            tblItemName.Width = 220;
            tblItemName.FontSize = 16;
            tblItemName.Foreground = new SolidColorBrush(Colors.White);
            tblItemName.MouseDown += new MouseButtonEventHandler(tblItemName_MouseDown);
            cvsItem.Children.Add(tblItemName);

            //Create View All button button
            btnViewAll.Height = 23;
            btnViewAll.Width = 100;
            btnViewAll.Content = "View All";
            btnViewAll.FontSize = 15;
            btnViewAll.Margin = new Thickness(590, 10, 0, 0);
            btnViewAll.Click += new RoutedEventHandler(btnViewAll_Click);
            cvsItem.Children.Add(btnViewAll);

            //Create Amount text
            tblAmount.Margin = new Thickness(257, 10, 0, 0);
            tblAmount.Text = amount.ToString();
            tblAmount.FontSize = 16;
            tblAmount.Foreground = new SolidColorBrush(Colors.White);
            cvsItem.Children.Add(tblAmount);

            //Create Loot Tables Text
            tblLootTables.Margin = new Thickness(330, 10, 0, 0);
            tblLootTables.Text = lootTables;
            tblLootTables.FontSize = 16;
            tblLootTables.Height = 23;
            tblLootTables.Width = 240;
            tblLootTables.FontWeight = FontWeights.DemiBold;
            tblLootTables.Foreground = new SolidColorBrush(Colors.White);
            cvsItem.Children.Add(tblLootTables);
        }

        //-- Event Handlers --//
        private void btnViewAll_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format("The duplicate occurs in the following loot tables:\n{0}", lootTables.Replace(", ", "\n")), "View all Loot Tables", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void tblItemName_MouseDown(object sender, MouseEventArgs e)
        {
            //Show the controls for editing and hide the original name
            MessageBox.Show(string.Format("Full name of the item:\n{0}", tblItemName.Text), "Full item name", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //-- Custom Methods --//

        public void UpdateAmount()
        {
            //Splits the location list and set the amount of different loot tables. Note that items that occur more than twice in loot tables are handled as seperate duplicates
            string[] lootTableSplitted = lootTables.Split(',');
            amount = lootTableSplitted.Count();
            tblAmount.Text = amount.ToString();
        }

        public void UpdateLootTables(string newLootTable)
        {
            //Add a new loot table to the loot tables string and display it
            lootTables = string.Format("{0}, {1}", lootTables, newLootTable);
            tblLootTables.Text = lootTables;
        }
    }
}
