﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Windows.Navigation;

namespace RandomItemGiverUpdater.Core
{
    public class Item
    {
        public string name { get; set; }
        private JObject itemBody;

        private readonly JObject originalItemBody; //Used for identifying if the item body was modified

        public Item(string itemBody)
        {
            this.itemBody = JObject.Parse(itemBody);
            originalItemBody = JObject.Parse(itemBody);
            name = GetName();
        }

        public Item(string name, string prefix = "minecraft")
        {
            itemBody = new JObject()
            {
                "type", "minecraft:item",
                "name", $"{prefix}:{name}",
            };
        }

        public string GetItemBody() => itemBody.ToString(Formatting.Indented);

        public JObject GetItemBodyObject() => itemBody;
        public JArray GetFunctionsBody() => itemBody["functions"] as JArray;

        public bool HasNBT() => GetNBT() != null;
        public bool HasItemStackComponent() => GetItemStackComponent() != null;

        public bool HasFunctionsBody() => itemBody["functions"] != null;

        public string GetNBT()
        {
            if (HasFunctionsBody())
            {
                foreach (JToken function in GetFunctionsBody())
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
            //If no functions array is present, add one
            if (!HasFunctionsBody())
            {
                AddFunctionsBody();
            }

            //If no NBT component is present, add one
            if (!HasNBT())
            {
                AddNBTBody();
            }

            foreach (JToken function in GetFunctionsBody())
            {
                //Go through all functions to find and edit an existing nbt object
                JToken tag = function["tag"];
                if (tag != null)
                {
                    function["tag"] = newTag;
                    break;
                }
            }
        }

        public string GetItemStackComponent()
        {
            if (HasFunctionsBody())
            {
                foreach (JToken function in GetFunctionsBody())
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

        public void SetItemStackComponent(string newComponent)
        {
            //If no functions array is present, add one
            if (!HasFunctionsBody())
            {
                AddFunctionsBody();
            }

            //If no item stack component body is present, add one
            if (!HasItemStackComponent())
            {
                AddItemStackComponentBody();
            }

            foreach (JToken function in GetFunctionsBody())
            {
                //Go through all functions to find and edit an existing components object
                JToken components = function["components"];
                if (components != null)
                {
                    function["components"] = JToken.Parse(newComponent);
                    break;
                }
            }
        }

        public void AddFunctionsBody() => itemBody.Add("functions", new JArray());

        public void RemoveNbtOrComponentBody()
        {
            JArray functionsArray = GetFunctionsBody();

            if (HasFunctionsBody())
            {
                foreach (JToken function in functionsArray)
                {
                    //Try to remove the object that contains the tag
                    if (HasNBT())
                    {
                        functionsArray.Remove(function["tag"]);
                        break;
                    }

                    //Try to remove the object that contains the components
                    if (HasItemStackComponent())
                    {
                        functionsArray.Remove(function["components"]);
                        break;
                    }
                }


                foreach (JToken function in functionsArray)
                {
                    //If the set_count function exists, it should exit here and not remove the function body
                    JToken countArray = function["set_count"];
                    if (countArray != null)
                    {
                        return;
                    }
                }

                //If no count part is found, the function body can safely be deleted
                itemBody.Remove("functions");
            }
        }

        public void AddNBTBody()
        {
            if (HasFunctionsBody())
            {
                GetFunctionsBody().Add(new JObject()
                {
                    {"function", "set_nbt" },
                    {"tag", "" }
                });
            }
        }

        public void AddItemStackComponentBody()
        {
            if (HasFunctionsBody())
            {
                GetFunctionsBody().Add(new JObject
                        {
                            {"function", "minecraft:set_components" },
                            {"components", new JObject() }
                        });
            }
        }

        public void SetName(string newName)
        {
            //Replace the original name with the new one in both the construct and the variable
            itemBody["name"] = newName;
            name = GetName();
        }

        public string GetName() => itemBody["name"].ToString();

        public bool IsModified() => itemBody != originalItemBody;

        public string GetTag() //Warning: This should only be used if you don't care whether it's NBT or Item Stack Component
        {
            if (GetNBT() != null) return GetNBT();
            else if (GetItemStackComponent() != null) return GetItemStackComponent();
            else return null;
        }

        public bool HasTag() => GetNBT() != null || GetItemStackComponent != null;
    }
}
