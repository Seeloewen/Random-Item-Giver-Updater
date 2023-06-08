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
        public Border itemBorder = new Border();
        public Canvas itemCanvas = new Canvas();
        public TextBlock itemTextblockName = new TextBlock();
        public TextBlock itemTextblockNBT = new TextBlock();
        public Button deleteButton = new Button();
        public TextBox itemNameTextBox = new TextBox();
        public Button itemNameSaveButton = new Button();
        public TextBox itemNBTTextBox = new TextBox();
        public Button itemNBTSaveButton = new Button();

        //Item attributes
        public string itemName;
        public string itemNBT;
        public string newName;
        public string newNBT;
        public int itemIndex;
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
                itemCanvas.Background = new SolidColorBrush(Color.FromArgb(100, 70, 70, 70));
            }
            else
            {
                itemCanvas.Background = new SolidColorBrush(Color.FromArgb(100, 90, 90, 90));
            }
            itemCanvas.Height = 50;

            //Create itemborder
            itemBorder.Margin = new Thickness(0, 0, 0, 0);
            itemBorder.Child = itemCanvas;
            itemBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
            itemBorder.VerticalAlignment = VerticalAlignment.Top;

            //Create item name text
            itemTextblockName.Margin = new Thickness(10, 10, 0, 0);
            itemTextblockName.Text = itemName;
            itemTextblockName.FontSize = 20;
            itemTextblockName.Foreground = new SolidColorBrush(Colors.White);
            itemCanvas.Children.Add(itemTextblockName);
            itemTextblockName.MouseDown += new MouseButtonEventHandler(itemTextBlockName_MouseDown);

            //Create delete button
            deleteButton.Height = 25;
            deleteButton.Width = 100;
            deleteButton.Content = "Delete";
            deleteButton.Margin = new Thickness(wndMain.ActualWidth - 420, 10, 0, 0);
            deleteButton.Click += new RoutedEventHandler(btnDelete_Click);
            itemCanvas.Children.Add(deleteButton);

            //Create item name save button
            itemNameSaveButton.Height = 25;
            itemNameSaveButton.Width = 100;
            itemNameSaveButton.Content = "Confirm";
            itemNameSaveButton.Margin = new Thickness(370, 10, 0, 0);
            itemNameSaveButton.Click += new RoutedEventHandler(btnSaveItemName_Click);
            itemCanvas.Children.Add(itemNameSaveButton);
            itemNameSaveButton.Visibility = Visibility.Hidden;

            //Create item name textbox
            itemNameTextBox.Width = 350;
            itemNameTextBox.Height = 25;
            itemNameTextBox.FontSize = 18;
            itemNameTextBox.Margin = new Thickness(10, 10, 0, 0);
            itemNameTextBox.Visibility = Visibility.Hidden;
            itemCanvas.Children.Add(itemNameTextBox);

            //Create item NBT save button
            itemNBTSaveButton.Height = 25;
            itemNBTSaveButton.Width = 100;
            itemNBTSaveButton.Content = "Confirm";
            itemNBTSaveButton.Margin = new Thickness(859, 10, 0, 0);
            itemNBTSaveButton.Click += new RoutedEventHandler(btnSaveItemNBT_Click);
            itemCanvas.Children.Add(itemNBTSaveButton);
            itemNBTSaveButton.Visibility = Visibility.Hidden;

            //Create item NBT textbox
            itemNBTTextBox.Width = 350;
            itemNBTTextBox.Height = 25;
            itemNBTTextBox.FontSize = 18;
            itemNBTTextBox.Margin = new Thickness(500, 10, 0, 0);
            itemNBTTextBox.Visibility = Visibility.Hidden;
            itemCanvas.Children.Add(itemNBTTextBox);

            //Create item NBT text
            if (itemNBT != "none")
            {
                itemTextblockNBT.Margin = new Thickness(500, 10, 0, 0);
                itemTextblockNBT.Text = string.Format("NBT: {0}", itemNBT);
                itemTextblockNBT.FontSize = 20;
                itemTextblockNBT.Foreground = new SolidColorBrush(Colors.White);
                itemCanvas.Children.Add(itemTextblockNBT);
                itemTextblockNBT.MouseDown += new MouseButtonEventHandler(itemTextblockNBT_MouseDown);
            }
        }

        //-- Event Handlers --//

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (isDeleted == false) //Set state to deleted
            {
                isModified = true;
                isDeleted = true;
                deleteButton.Content = "Undo deletion";
            }
            else if (isDeleted == true) //If the item has been deleted, set state to undeleted
            {
                //Check if the item has been modified in some other way before setting the modified state to false
                if (itemName == newName && itemNBT == newNBT)
                {
                    isModified = false;
                }
                isDeleted = false;
                deleteButton.Content = "Delete";
            }

        }

        private void btnSaveItemName_Click(object sender, RoutedEventArgs e)
        {
            //Hide the controls for editing and show the item name
            itemNameTextBox.Visibility = Visibility.Hidden;
            itemNameSaveButton.Visibility = Visibility.Hidden;
            itemTextblockName.Visibility = Visibility.Visible;
            newName = itemNameTextBox.Text;
            itemTextblockName.Text = newName;

            //Check if the item name has been changed and change modified state
            if (newName != itemName)
            {
                isModified = true;
            }
        }

        private void btnSaveItemNBT_Click(object sender, RoutedEventArgs e)
        {
            //Hide the controls for editing and show the item NBT
            itemNBTTextBox.Visibility = Visibility.Hidden;
            itemNBTSaveButton.Visibility = Visibility.Hidden;
            itemTextblockNBT.Visibility = Visibility.Visible;
            newNBT = itemNBTTextBox.Text;
            itemTextblockNBT.Text = string.Format("NBT: {0}", newNBT);

            //Check if the item name has been changed and change modified state
            if (newNBT != itemNBT)
            {
                isModified = true;
            }
        }

        private void itemTextBlockName_MouseDown(object sender, MouseEventArgs e)
        {
            //Show the controls for editing and hide the original name
            itemNameTextBox.Visibility = Visibility.Visible;
            itemNameSaveButton.Visibility = Visibility.Visible;
            itemTextblockName.Visibility = Visibility.Hidden;
            itemNameTextBox.Text = itemTextblockName.Text;
        }

        private void itemTextblockNBT_MouseDown(object sender, MouseEventArgs e)
        {
            //Show the controls for editing and hide the original NBT
            itemNBTTextBox.Visibility = Visibility.Visible;
            itemNBTSaveButton.Visibility = Visibility.Visible;
            itemTextblockNBT.Visibility = Visibility.Hidden;
            itemNBTTextBox.Text = itemTextblockNBT.Text.Replace("NBT: ", "");
        }
    }
}
