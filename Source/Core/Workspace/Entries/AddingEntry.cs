using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Data;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Xml.Linq;

namespace RandomItemGiverUpdater.Core.Workspace.Entries
{
    public class AddingEntry : Item
    {
        private string id;
        private string prefix;


        //Id and prefix are stored seperately to make up the name
        //This is a property because of WPF databinding
        public string Prefix
        {
            get => prefix;
            set
            {
                prefix = value;
                SetName($"{prefix}:{id}");
            }
        }
        public string Id
        {
            get => id;
            set
            {
                id = value;
                SetName($"{prefix}:{id}");
            }
        }
        public int index { get; set; }

        public List<LootTable> lootTableWhiteList = new List<LootTable>();

        public bool defaultLootTables = true;
        public SolidColorBrush canvasBackColor { get; set; }

        public AddingEntry(string prefix, string id, int index) : base(id.TrimEnd('\r', '\n'), prefix)
        {
            this.prefix = prefix;
            this.id = id;

            //Set the backcolor
            this.index = index;
            canvasBackColor = SetBackColor();
        }

        public SolidColorBrush SetBackColor()
        {
            //Set the backcolor depending on the item index
            return new SolidColorBrush(Convert.ToInt32(index) % 2 == 0 ? Color.FromArgb(100, 70, 70, 70) : Color.FromArgb(100, 90, 90, 90));
        }
    }
}
