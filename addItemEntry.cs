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
    public class addItemEntry
    {
        //Controls
        public Border bdrItem = new Border();
        public Canvas cvsItem = new Canvas();
        public TextBox tbItemName = new TextBox();
        public TextBox tbItemNBT = new TextBox();
        public TextBox tbItemPrefix = new TextBox();
        public Button btnRemove = new Button();

        //Attributes
        public string itemName;
        public int itemIndex;
        public string itemNBT;
        public string itemPrefix;
        public addToLootTableEntry lootTableEntry;

        //-- Constructor --//

        public addItemEntry(string prefix, string name, int index)
        {
            //Set variables
            itemName = name.TrimEnd('\r', '\n');
            itemIndex = index;
            itemPrefix = prefix;

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

            //Create item name textbox
            tbItemName.Width = 255;
            tbItemName.Height = 24;
            tbItemName.FontSize = 15;
            tbItemName.Background = new SolidColorBrush(Color.FromArgb(100, 164, 164, 164));
            tbItemName.Foreground = new SolidColorBrush(Colors.White);
            tbItemName.Margin = new Thickness(205, 6, 0, 0);
            tbItemName.Text = itemName;
            cvsItem.Children.Add(tbItemName);

            //Create item NBT textbox
            tbItemNBT.Width = 200;
            tbItemNBT.Height = 24;
            tbItemNBT.FontSize = 15;
            tbItemNBT.Background = new SolidColorBrush(Color.FromArgb(100, 164, 164, 164));
            tbItemNBT.Foreground = new SolidColorBrush(Colors.White);
            tbItemNBT.Margin = new Thickness(475, 6, 0, 0);
            cvsItem.Children.Add(tbItemNBT);

            //Create item Prefix textbox
            tbItemPrefix.Width = 175;
            tbItemPrefix.Height = 24;
            tbItemPrefix.FontSize = 15;
            tbItemPrefix.Background = new SolidColorBrush(Color.FromArgb(100, 164, 164, 164));
            tbItemPrefix.Foreground = new SolidColorBrush(Colors.White);
            tbItemPrefix.Margin = new Thickness(15, 6, 0, 0);
            tbItemPrefix.Text = prefix;
            cvsItem.Children.Add(tbItemPrefix);

            //Create Remove button
            btnRemove.Height = 24;
            btnRemove.Width = 20;
            btnRemove.Content = "X";
            btnRemove.FontSize = 15;
            btnRemove.Margin = new Thickness(685, 6, 0, 0);
            btnRemove.Click += new RoutedEventHandler(btnRemove_Click);
            cvsItem.Children.Add(btnRemove);
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            //Remove the current item from the list
            wndAddItem.itemEntries.Remove(this);

            //Display all items
            MainWindow.wndAddItem.UpdateItemDisplay();
        }
    }
}
