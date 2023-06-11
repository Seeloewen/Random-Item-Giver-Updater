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
    internal class addItemEntry
    {
        //Controls
        public Border bdrItem = new Border();
        public Canvas cvsItem = new Canvas();
        public TextBox tbItemName = new TextBox();
        public TextBox tbItemNBT = new TextBox();
        public TextBox tbItemPrefix = new TextBox();

        //-- Constructor --//

        public addItemEntry(string name, int index)
        {

            //Set backcolor
            if (index % 2 == 0)
            {
                cvsItem.Background = new SolidColorBrush(Color.FromArgb(100, 70, 70, 70));
            }
            else
            {
                cvsItem.Background = new SolidColorBrush(Color.FromArgb(100, 90, 90, 90));
            }
            cvsItem.Height = 40;

            //Create itemborder
            bdrItem.Margin = new Thickness(0, 0, 0, 0);
            bdrItem.Child = cvsItem;
            bdrItem.HorizontalAlignment = HorizontalAlignment.Stretch;
            bdrItem.VerticalAlignment = VerticalAlignment.Top;

            //Create item name textbox
            tbItemName.Width = 220;
            tbItemName.Height = 20;
            tbItemName.FontSize = 12;
            tbItemName.Margin = new Thickness(160, 5, 0, 0);
            tbItemName.Text = name.TrimEnd('\r', '\n');
            cvsItem.Children.Add(tbItemName);

            //Create item NBT textbox
            tbItemNBT.Width = 100;
            tbItemNBT.Height = 20;
            tbItemNBT.FontSize = 12;
            tbItemNBT.Margin = new Thickness(400, 5, 0, 0);
            cvsItem.Children.Add(tbItemNBT);

            //Create item Prefix textbox
            tbItemPrefix.Width = 130;
            tbItemPrefix.Height = 20;
            tbItemPrefix.FontSize = 12;
            tbItemPrefix.Margin = new Thickness(5, 5, 0, 0);
            tbItemPrefix.Text = "minecraft:";
            cvsItem.Children.Add(tbItemPrefix);
        }
    }
}
