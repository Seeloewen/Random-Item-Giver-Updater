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
using System.Windows.Media.Animation;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace Random_Item_Giver_Updater
{
    public class ItemEntry
    {
        //Item attributes
        public string itemBody;
        public string name { get; set; }
        public string index { get; set; }
        public string originalName;

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

        public ItemEntry(string itemBody, int index)
        {
            this.itemBody = itemBody;
            this.index = index.ToString();
            name = GetName();
            canvasBackColor = SetBackColor();
        }

        public string GetNBT()
        {
            JObject item = JObject.Parse(itemBody);
            JArray functions = item["functions"] as JArray;

            if (functions != null)
            {
                foreach (JToken function in functions)
                {
                    JToken tag = function["tag"];
                    if (tag != null)
                    {
                        return tag.ToString();
                    }
                }
            }

            return null;
        }

        public void SetNBT(string newTag)
        {
            //Get the functions array
            JObject itemObject = JObject.Parse(itemBody);
            JArray functionsArray = itemObject["functions"] as JArray;

            if (functionsArray != null)
            {
                bool changedNbt = false;
                foreach (JToken function in functionsArray)
                {
                    //Go through all functions to find and edit an existing nbt object
                    JToken tag = function["tag"];
                    if (tag != null)
                    {
                        function["tag"] = JToken.Parse(newTag);
                        changedNbt = true;
                        break;
                    }
                }

                if (!changedNbt)
                {
                    //If no nbt object is present, add it
                    functionsArray.Add(new JObject
                        {
                            {"function", "minecraft:set_nbt" },
                            {"tag", newTag }
                        });
                }
            }
            else
            {
                //If no functions array exists, add it and restart the nbt adding
                AddFunctionBody();
                SetNBT(newTag);
                return;
            }

            itemBody = itemObject.ToString(Formatting.Indented);
        }

        public string GetItemStackComponents()
        {
            JObject itemObject = JObject.Parse(itemBody);
            JArray functionsArray = itemObject["functions"] as JArray;

            if (functionsArray != null)
            {
                foreach (JToken function in functionsArray)
                {
                    JToken components = function["components"];
                    if (components != null)
                    {
                        return components.ToString();
                    }
                }
            }

            return null;
        }

        public void SetItemStackComponent(string newComponents)
        {
            JObject itemObject = JObject.Parse(itemBody);
            JArray functionsArray = itemObject["functions"] as JArray;

            if (functionsArray != null)
            {
                foreach (JToken function in functionsArray)
                {
                    //Go through all functions to find and edit an existing components object
                    bool changedComponent = false;
                    JToken components = function["components"];
                    if (components != null)
                    {
                        function["components"] = JToken.Parse(newComponents);
                        changedComponent = true;
                        break;
                    }

                    if(!changedComponent)
                    {
                        //If no component object is present, add it
                        functionsArray.Add(new JObject
                        {
                            {"function", "minecraft:set_nbt" },
                            {"tag", JToken.Parse(newComponents) }
                        });
                    }
                }
            }
            else
            {
                //If no functions array exists, add it and restart the nbt adding
                AddFunctionBody();
                SetItemStackComponent(newComponents);
                return;
            }

            itemBody = itemObject.ToString(Formatting.Indented);
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

        public void AddFunctionBody()
        {
            JObject itemObject = JObject.Parse(itemBody);
            JArray functionArray = new JArray();

            itemObject.Add("functions", functionArray);

            itemBody = itemObject.ToString(Formatting.Indented);
        }

        public void RemoveNbtOrComponentBody()
        {
            JObject itemObject = JObject.Parse(itemBody);
            JArray functionsArray = itemObject["functions"] as JArray;

            if (functionsArray != null)
            {
                foreach (JToken function in functionsArray)
                {
                    //Try to remove the object that contains the tag
                    JToken tag = function["tag"];
                    if (tag != null)
                    {
                        functionsArray.Remove(function);
                        break;
                    }

                    //Try to remove the object that contains the components
                    JToken components = function["components"];
                    if (components != null)
                    {
                        functionsArray.Remove(function);
                        break;
                    }
                }


                //TODO: Potentially rewrite to check if any more functions are there, not only nbt
                bool hasCount = false;
                foreach (JToken function in functionsArray)
                {
                    //If the set_count function  doesn't exist, it should mark the function body as deletable
                    JToken Count = function["set_count"];
                    if (Count != null)
                    {
                        break;
                    }
                }

                //If no count part is found, the function body can safely be deleted
                if (!hasCount)
                {
                    itemObject.Remove("functions");
                }
            }


            itemBody = itemObject.ToString(Formatting.Indented);
        }

        public void AddNBTBody()
        {
            JObject itemObject = JObject.Parse(itemBody);
            JArray functionsArray = itemObject["functions"] as JArray;

            if (functionsArray != null)
            {
                functionsArray.Add(new JObject()
                {
                    {"function", "set_nbt" },
                    {"tag", "" }
                });
            }

            itemBody = itemObject.ToString(Formatting.Indented);
        }

        public SolidColorBrush SetBackColor()
        {
            //Set the backcolor depending on the item index
            if (Convert.ToInt32(index) % 2 == 0)
            {
                return new SolidColorBrush(Color.FromArgb(100, 70, 70, 70));
            }
            else
            {
                return new SolidColorBrush(Color.FromArgb(100, 90, 90, 90));
            }
        }

        public void SetName(string newName, object sender)
        {
            //Replace the original name with the new one in both the construct and the variable
            JObject itemObject = JObject.Parse(itemBody);
            itemObject["name"] = newName;


            itemBody = itemObject.ToString(Formatting.Indented);
            name = GetName();

            //Check if the item name has been changed and change modified state
            if (name != originalName)
            {
                changes.Add(changeType.NameChanged);
                SetIndicatorState(sender);
            }
            else
            {
                changes.Remove(changeType.NameChanged);
                SetIndicatorState(sender);
            }
        }

        public string GetName()
        {
            //Replace the original name with the new one in both the construct and the variable
            JObject itemObject = JObject.Parse(itemBody);
            JToken name = itemObject["name"];

            return name.ToString();
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
            if (changes.Contains(changeType.Deleted))
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
