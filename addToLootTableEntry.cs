using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Random_Item_Giver_Updater
{
    public class addToLootTableEntry
    {
        //Attributes
        public bool allLootTablesChecked = true;

        //Controls
        public Border bdrItem = new Border();
        public Canvas cvsItem = new Canvas();
        public TextBlock tblItemName = new TextBlock();
        public RadioButton rbtnAllLootTables = new RadioButton();
        public RadioButton rbtnCertainLootTables = new RadioButton();
        public Button btnEditCertainLootTables = new Button();
        public List<lootTable> lootTableCheckList = new List<lootTable>();
        public string lootTableWhiteList = "";

        //Windows
        wndSelectLootTables wndSelectLootTables;

        //Reference to main window
        public MainWindow wndMain = (MainWindow)Application.Current.MainWindow;

        //-- Constructor --//

        public addToLootTableEntry(string name, int index)
        {
            //Set loot table checklist
            foreach(lootTable lootTable in wndMain.lootTableList)
            {
                lootTableCheckList.Add(new lootTable(lootTable.lootTableName, lootTable.lootTableType, lootTable.lootTablePath));
            }

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

            //Create itemborder
            bdrItem.Margin = new Thickness(0, 0, 0, 0);
            bdrItem.Child = cvsItem;
            bdrItem.HorizontalAlignment = HorizontalAlignment.Stretch;
            bdrItem.VerticalAlignment = VerticalAlignment.Top;

            //Create item name textblock
            tblItemName.Width = 140;
            tblItemName.Height = 24;
            tblItemName.FontSize = 15;
            tblItemName.Foreground = new SolidColorBrush(Colors.White);
            tblItemName.Margin = new Thickness(20, 6, 0, 0);
            tblItemName.Text = name.TrimEnd('\r', '\n');
            cvsItem.Children.Add(tblItemName);

            //Create all loot tables radiobutton
            rbtnAllLootTables.Width = 255;
            rbtnAllLootTables.Height = 24;
            rbtnAllLootTables.FontSize = 15;
            rbtnAllLootTables.Background = new SolidColorBrush(Color.FromArgb(100, 164, 164, 164));
            rbtnAllLootTables.Foreground = new SolidColorBrush(Colors.White);
            rbtnAllLootTables.Margin = new Thickness(175, 10, 0, 0);
            rbtnAllLootTables.Content = "All normal loot tables";
            rbtnAllLootTables.Padding = new Thickness(5,-3.5,0,0);
            rbtnAllLootTables.IsChecked = true;
            rbtnAllLootTables.Checked += new RoutedEventHandler(rbtnAllLootTables_Checked);
            cvsItem.Children.Add(rbtnAllLootTables);

            //Create certain loot tables radiobutton
            rbtnCertainLootTables.Width = 255;
            rbtnCertainLootTables.Height = 24;
            rbtnCertainLootTables.FontSize = 15;
            rbtnCertainLootTables.Background = new SolidColorBrush(Color.FromArgb(100, 164, 164, 164));
            rbtnCertainLootTables.Foreground = new SolidColorBrush(Colors.White);
            rbtnCertainLootTables.Margin = new Thickness(350, 10, 0, 0);
            rbtnCertainLootTables.Content = "Only certain loot tables:";
            rbtnCertainLootTables.Padding = new Thickness(5, -3.5, 0, 0);
            rbtnCertainLootTables.Checked += new RoutedEventHandler(rbtnCertainLootTables_Checked);
            cvsItem.Children.Add(rbtnCertainLootTables);

            //Create edit button
            btnEditCertainLootTables.Height = 25;
            btnEditCertainLootTables.Width = 150;
            btnEditCertainLootTables.Content = "Select loot tables";
            btnEditCertainLootTables.Margin = new Thickness(550, 6, 0, 0);
            btnEditCertainLootTables.Click += new RoutedEventHandler(btnEditCertainLootTables_Click);
            btnEditCertainLootTables.IsEnabled = false;
            cvsItem.Children.Add(btnEditCertainLootTables);
        }

        private void btnEditCertainLootTables_Click(object sender, RoutedEventArgs e)
        {
            //Open loot table selection window
            wndSelectLootTables = new wndSelectLootTables(lootTableCheckList) { Owner = Application.Current.MainWindow };
            wndSelectLootTables.Owner = Application.Current.MainWindow;
            wndSelectLootTables.ShowDialog();

            //Get loot tables string from loot table selection window
            foreach(lootTable lootTable in wndSelectLootTables.lootTableList)
            {
                if(lootTable.cbAddToLootTable.IsChecked == true)
                {
                    lootTableWhiteList = string.Format("{0}{1}", lootTableWhiteList, lootTable.fullLootTablePath);
                }
            }

        }

        private void rbtnAllLootTables_Checked(object sender, RoutedEventArgs e)
        {
            //Disable edit button
            btnEditCertainLootTables.IsEnabled = false;

            //Set checkstate on variable that gets accessed by the item adding thread
            allLootTablesChecked = true;
           
        }

        private void rbtnCertainLootTables_Checked(object sender, RoutedEventArgs e)
        {
            //Enable edit button
            btnEditCertainLootTables.IsEnabled = true;

            //Set checkstate on variable that gets accessed by the item adding thread
            allLootTablesChecked = false;
        }
    }
}
