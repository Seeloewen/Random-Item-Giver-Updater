using RandomItemGiverUpdater.Entries;
using RandomItemGiverUpdater.Core;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RandomItemGiverUpdater.Gui.Menus
{
    public partial class wndSelectLootTables : Window
    {
        public List<LootTableSelectionEntry> lootTables = new List<LootTableSelectionEntry>();

        public wndSelectLootTables(List<LootTable> lootTables, string header)
        {
            InitializeComponent();
            tblHeader.Text = header;
            cbxScheme.Items.Add("None");
            cbxScheme.SelectedIndex = 0;

            foreach (LootTable lootTable in lootTables)
            {
                LootTableSelectionEntry entry = new LootTableSelectionEntry(lootTable.name, lootTable.type, lootTable.prePath);
                this.lootTables.Add(entry); //Set up loot table list
                stpLootTables.Children.Add(entry.visual); //Display the checkboxes for the entries

                //Construct scheme selection
                if (!cbxScheme.Items.Contains(lootTable.name))
                {
                    cbxScheme.Items.Add(lootTable.name);
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e) => Close();

        private void wndSelectLootTables1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool lootTableSelected = false;

            //Check if a loot table is selected before quitting 
            foreach (LootTableSelectionEntry lootTable in lootTables)
            {
                if (lootTable.visual.cbAddToLootTable.IsChecked == true)
                {
                    lootTableSelected = true;
                    break;
                }
            }

            if (!lootTableSelected)
            {
                //Stop quitting and show error if no loot table is selected
                e.Cancel = true;
                MessageBox.Show("Please select at least one loot table!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCheckAll_Click(object sender, RoutedEventArgs e)
        {
            //Check all checkboxes
            foreach (LootTableSelectionEntry lootTable in lootTables)
            {
                lootTable.visual.cbAddToLootTable.IsChecked = true;
            }
        }

        private void btnUncheckAll_Click(object sender, RoutedEventArgs e)
        {
            //Uncheck all checkboxes
            foreach (LootTableSelectionEntry lootTable in lootTables)
            {
                lootTable.visual.cbAddToLootTable.IsChecked = false;
            }
        }

        private void btnSelectScheme_Click(object sender, RoutedEventArgs e)
        {
            if (cbxScheme.Text != "None")
            {
                //Check each checkbox if it matches the scheme and change check state properly
                foreach (LootTableSelectionEntry lootTable in lootTables)
                {
                    if (lootTable.name == cbxScheme.Text)
                    {
                        lootTable.visual.cbAddToLootTable.IsChecked = true;
                    }
                    else
                    {
                        lootTable.visual.cbAddToLootTable.IsChecked = false;
                    }
                }
            }
        }
    }
}
