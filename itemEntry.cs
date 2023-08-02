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
    public class itemEntry
    {
        //Controls
        public Border bdrItem = new Border();
        private Canvas cvsItem = new Canvas();
        private TextBlock tblItemName = new TextBlock();
        private TextBlock tblItemNBT = new TextBlock();
        public Button btnDelete = new Button();
        private TextBox tbItemName = new TextBox();
        private Button btnSaveItemName = new Button();
        private TextBox tbItemNBT = new TextBox();
        private Button btnSaveItemNBT = new Button();
        public TextBlock tblEntryIndex = new TextBlock();
        private TextBlock tblIndicator = new TextBlock();

        //Item attributes
        public string itemName;     
        public string itemNBT;
        public string newName;
        public string newNBT;
        private int itemIndex;
        public bool isModified = false;
        public bool isDeleted = false;

        //Reference to main window
        MainWindow wndMain = (MainWindow)Application.Current.MainWindow;

        //-- Constructor --//

        public itemEntry(string name, string nbt, int index)
        {

            //Initialize variables
            itemName = name.TrimEnd('\r', '\n');
            itemNBT = nbt.TrimEnd('\r', '\n'); ;
            itemIndex = index;
            newName = itemName;
            newNBT = itemNBT;

            //Set backcolor
            if (index % 2 == 0)
            {
                cvsItem.Background = new SolidColorBrush(Color.FromArgb(100, 70, 70, 70));
            }
            else
            {
                cvsItem.Background = new SolidColorBrush(Color.FromArgb(100, 90, 90, 90));
            }
            cvsItem.Height = 50;

            //Create itemborder
            bdrItem.Margin = new Thickness(0, 0, 0, 0);
            bdrItem.Child = cvsItem;
            bdrItem.HorizontalAlignment = HorizontalAlignment.Stretch;
            bdrItem.VerticalAlignment = VerticalAlignment.Top;

            //Create item name text
            tblItemName.Margin = new Thickness(10, 10, 0, 0);
            tblItemName.Text = itemName;
            tblItemName.FontSize = 20;
            tblItemName.Foreground = new SolidColorBrush(Colors.White);
            cvsItem.Children.Add(tblItemName);
            tblItemName.MouseDown += new MouseButtonEventHandler(tblItemName_MouseDown);

            //Create delete button
            btnDelete.Height = 25;
            btnDelete.Width = 100;
            btnDelete.Content = "Delete";
            btnDelete.Margin = new Thickness(wndMain.ActualWidth - 460, 10, 0, 0);
            btnDelete.Click += new RoutedEventHandler(btnDelete_Click);
            cvsItem.Children.Add(btnDelete);

            //Create item name save button
            btnSaveItemName.Height = 25;
            btnSaveItemName.Width = 100;
            btnSaveItemName.Content = "Confirm";
            btnSaveItemName.Margin = new Thickness(370, 10, 0, 0);
            btnSaveItemName.Click += new RoutedEventHandler(btnSaveItemName_Click);
            cvsItem.Children.Add(btnSaveItemName);
            btnSaveItemName.Visibility = Visibility.Hidden;

            //Create item name textbox
            tbItemName.Width = 350;
            tbItemName.Height = 25;
            tbItemName.FontSize = 18;
            tbItemName.Margin = new Thickness(10, 10, 0, 0);
            tbItemName.Visibility = Visibility.Hidden;
            cvsItem.Children.Add(tbItemName);

            //Create item NBT save button
            btnSaveItemNBT.Height = 25;
            btnSaveItemNBT.Width = 100;
            btnSaveItemNBT.Content = "Confirm";
            btnSaveItemNBT.Margin = new Thickness(859, 10, 0, 0);
            btnSaveItemNBT.Click += new RoutedEventHandler(btnSaveItemNBT_Click);
            cvsItem.Children.Add(btnSaveItemNBT);
            btnSaveItemNBT.Visibility = Visibility.Hidden;

            //Create item NBT textbox
            tbItemNBT.Width = 350;
            tbItemNBT.Height = 25;
            tbItemNBT.FontSize = 18;
            tbItemNBT.Margin = new Thickness(500, 10, 0, 0);
            tbItemNBT.Visibility = Visibility.Hidden;
            cvsItem.Children.Add(tbItemNBT);

            //Create item NBT text
            if (itemNBT != "none")
            {
                tblItemNBT.Margin = new Thickness(500, 10, 0, 0);
                tblItemNBT.Text = string.Format("NBT: {0}", itemNBT);
                tblItemNBT.FontSize = 20;
                tblItemNBT.Foreground = new SolidColorBrush(Colors.White);
                cvsItem.Children.Add(tblItemNBT);
                tblItemNBT.MouseDown += new MouseButtonEventHandler(tblItemNBT_MouseDown);
            }

            //Create entry index
            tblEntryIndex.Margin = new Thickness(wndMain.ActualWidth - 345, 10, 0, 0);
            tblEntryIndex.Text = (itemIndex + 1).ToString();
            tblEntryIndex.TextAlignment = TextAlignment.Center;
            tblEntryIndex.FontSize = 20;
            tblEntryIndex.FontWeight = FontWeights.DemiBold;
            tblEntryIndex.Foreground = new SolidColorBrush(Colors.White);
            cvsItem.Children.Add(tblEntryIndex);

            //Create Indicator
            tblIndicator.Margin = new Thickness(wndMain.ActualWidth - 490, 5, 0, 0);
            tblIndicator.FontWeight = FontWeights.Bold;
            tblIndicator.FontSize = 24;
            cvsItem.Children.Add(tblIndicator);
        }

        //-- Event Handlers --//

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (isDeleted == false) //Set state to deleted
            {
                isModified = true;
                isDeleted = true;
                btnDelete.Content = "Undo deletion";
                SetIndicatorState();
            }
            else if (isDeleted == true) //If the item has been deleted, set state to undeleted
            {
                //Check if the item has been modified in some other way before setting the modified state to false
                if (itemName == newName)
                {
                    isModified = false;
                }
                isDeleted = false;
                btnDelete.Content = "Delete";
                SetIndicatorState();
            }

        }

        private void btnSaveItemName_Click(object sender, RoutedEventArgs e)
        {
            //Hide the controls for editing and show the item name
            tbItemName.Visibility = Visibility.Hidden;
            btnSaveItemName.Visibility = Visibility.Hidden;
            tblItemName.Visibility = Visibility.Visible;
            newName = tbItemName.Text;
            tblItemName.Text = newName;

            //Check if the item name has been changed and change modified state
            if (newName != itemName)
            {
                isModified = true;
                SetIndicatorState();
            }
            else
            {
                if (isDeleted == false && itemNBT == newNBT)
                {
                    isModified = false;
                }
                SetIndicatorState();
            }
        }

        private void btnSaveItemNBT_Click(object sender, RoutedEventArgs e)
        {
            //Hide the controls for editing and show the item NBT
            tbItemNBT.Visibility = Visibility.Hidden;
            btnSaveItemNBT.Visibility = Visibility.Hidden;
            tblItemNBT.Visibility = Visibility.Visible;
            newNBT = tbItemNBT.Text;
            tblItemNBT.Text = string.Format("NBT: {0}", newNBT);

            //Check if the item NBT has been changed and change modified state
            if (newNBT != itemNBT)
            {
                isModified = true;
                SetIndicatorState();
            }
            else
            {
                if (isDeleted == false && itemName == newName)
                {
                    isModified = false;
                }
                SetIndicatorState();
            }
        }

        private void tblItemName_MouseDown(object sender, MouseEventArgs e)
        {
            //Show the controls for editing and hide the original name
            tbItemName.Visibility = Visibility.Visible;
            btnSaveItemName.Visibility = Visibility.Visible;
            tblItemName.Visibility = Visibility.Hidden;
            tbItemName.Text = tblItemName.Text;
        }

        private void tblItemNBT_MouseDown(object sender, MouseEventArgs e)
        {
            //Show the controls for editing and hide the original NBT
            tbItemNBT.Visibility = Visibility.Visible;
            btnSaveItemNBT.Visibility = Visibility.Visible;
            tblItemNBT.Visibility = Visibility.Hidden;
            tbItemNBT.Text = tblItemNBT.Text.Replace("NBT: ", "");
        }

        //-- Custom Methods --//

        private void SetIndicatorState() { 
        
            if(isModified == true && isDeleted == true)
            {
                //Item deleted, show indicator
                tblIndicator.Visibility = Visibility.Visible;
                tblIndicator.Text = "X";
                tblIndicator.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (isModified == true && isDeleted == false)
            {
                //Item modified, show indicator
                tblIndicator.Visibility = Visibility.Visible;
                tblIndicator.Text = "#";
                tblIndicator.Foreground = new SolidColorBrush(Colors.LightBlue);
            }
            else if(isModified == false && isDeleted == false)
            {
                //No changes, hide indicator
                tblIndicator.Visibility = Visibility.Hidden;
            }
            else
            {
                //Invalid state provided, hide indicator
                tblIndicator.Visibility = Visibility.Hidden;
            }    
        }
    }
}
