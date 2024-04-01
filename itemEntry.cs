using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Reflection;
using SeeloewenLib;

namespace Random_Item_Giver_Updater
{
    public class itemEntry
    {
        //Item attributes
        public List<string> itemStackComponent;
        public List<string> itemBody;
        public string itemName { get; set; }
        public string itemNBT { get; set; }
        public string itemIndex { get; set; }
        public string newName;

        public enum changeType
        {
            NameChanged,
            FunctionBodyAdded,
            ItemStackComponentBodyAdded,
            ItemStackComponentBodyRemoved,
            NBTBodyAdded,
            NBTBodyRemoved,
            NBTChanged,
            Deleted
        }
        public List<changeType> changes = new List<changeType>();
        public bool hasLegacyNBT = false;
        public bool hasItemStackComponent = false;
        public bool hasFunctionBody = false;


        //Canvas attributes
        public SolidColorBrush canvasBackColor { get; set; }

        //Important references
        SeeloewenLibTools SeeloewenLibTools = new SeeloewenLibTools();

        //-- Constructor --//

        public itemEntry(string name, string nbt, int index, bool hasFunctionBody, List<string> itemBody)
        {
            //Initialize variables
            this.hasFunctionBody = hasFunctionBody;
            this.itemBody = itemBody;
            itemName = name.TrimEnd('\r', '\n');
            itemNBT = nbt.TrimEnd('\r', '\n'); ;
            itemIndex = (index + 1).ToString();
            newName = itemName;
            hasLegacyNBT = true;

            //Set the backcolor
            canvasBackColor = SetBackColor();
        }

        public itemEntry(string name, List<string> itemStackComponent, int index, bool hasFunctionBody, List<string> itemBody)
        {
            //Initialize variables
            this.itemStackComponent = itemStackComponent;
            this.hasFunctionBody = hasFunctionBody;
            this.itemBody = itemBody;
            itemName = name.TrimEnd('\r', '\n');
            itemIndex = (index + 1).ToString();
            newName = itemName;
            hasItemStackComponent = true;


            //Set the backcolor
            canvasBackColor = SetBackColor();
        }

        //-- Custom Methods --//

        public void SetIndicatorState(object sender)
        {
            if (sender is Button button)
            {
                //Get the parent canvas
                Canvas canvas = SeeloewenLibTools.FindVisualParent<Canvas>(button);

                //Get the necessary controls
                TextBlock textblock = canvas.FindName("tblIndicator") as TextBlock;
                if (IsModified() == true && IsDeleted() == true)
                {
                    //Item deleted, show indicator
                    textblock.Visibility = Visibility.Visible;
                    textblock.Text = "X";
                    textblock.Foreground = new SolidColorBrush(Colors.Red);
                }
                else if (IsModified() == true && IsDeleted() == false)
                {
                    //Item modified, show indicator
                    textblock.Visibility = Visibility.Visible;
                    textblock.Text = "#";
                    textblock.Foreground = new SolidColorBrush(Colors.LightBlue);
                }
                else if (IsModified() == false && IsDeleted() == false)
                {
                    //No changes, hide indicator
                    textblock.Visibility = Visibility.Hidden;
                }
                else
                {
                    //Invalid state provided, hide indicator
                    textblock.Visibility = Visibility.Hidden;
                }
            }

        }

        public SolidColorBrush SetBackColor()
        {
            //Set the backcolor depending on the item index
            if (Convert.ToInt32(itemIndex) % 2 == 0)
            {
                return new SolidColorBrush(Color.FromArgb(100, 70, 70, 70));
            }
            else
            {
                return new SolidColorBrush(Color.FromArgb(100, 90, 90, 90));
            }
        }

        public bool IsModified()
        {
            //Check if the item is modified in any way
            if (changes.Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool IsDeleted()
        {
            //Check if the item was deleted
            if(changes.Contains(changeType.Deleted))
            {
                return true;
            }
            else
            { 
                return false;
            }
        }
    }
}
