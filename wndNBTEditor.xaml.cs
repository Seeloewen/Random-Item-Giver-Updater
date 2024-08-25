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

namespace Random_Item_Giver_Updater
{

    public partial class wndNBTEditor : Window
    {
        ItemEntry item;

        public wndNBTEditor(ItemEntry item)
        {
            InitializeComponent();
            this.item = item;
            tblHeader.Text = $"Editing NBT of item {item.name}";
            if (this.item.GetNBT() == null)
            {
                if (this.item.hasFunctionBody)
                {
                    //Ask the user if they want to create an NBT body
                    MessageBoxResult dialogResult = MessageBox.Show("The item that you want to edit does not have an NBT body yet. This is required to have an NBT tag. Do you want to add the NBT body?", "NBT body missing", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (dialogResult == MessageBoxResult.Yes)
                    {
                        this.item.changes.Add(ItemEntry.changeType.NBTBodyAdded);

                        //Add the NBT body
                        item.AddNBTBody();

                    }
                    else if (dialogResult == MessageBoxResult.No)
                    {
                        Close();
                    }
                }
                else if (!item.hasFunctionBody)
                {
                    //Ask the user if they want to create a function body and a NBT body
                    MessageBoxResult dialogResult = MessageBox.Show("The item that you want to edit does not have a function body or an NBT body yet. Both are required to have an NBT tag. Do you want to add them?", "NBT and function body missing", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (dialogResult == MessageBoxResult.Yes)
                    {
                        //Add the function body and the NBT body                        
                        this.item.changes.Add(ItemEntry.changeType.FunctionBodyAdded);
                        this.item.changes.Add(ItemEntry.changeType.NBTBodyAdded);
                        this.item.hasFunctionBody = true;
                        item.AddFunctionBody();
                        item.AddNBTBody();
                    }
                    else if (dialogResult == MessageBoxResult.No)
                    {
                        Close();
                    }
                }
            }
            else
            {
                tbNBT.Text = this.item.GetNBT();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(tbNBT.Text != item.GetNBT())
            {
                item.changes.Add(ItemEntry.changeType.NBTChanged);
            }
        }

        private void btnDeleteNBTTag_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
