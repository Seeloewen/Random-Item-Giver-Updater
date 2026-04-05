using RandomItemGiverUpdater.Core;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RandomItemGiverUpdater.Core.Workspace.Entries;
using RandomItemGiverUpdater.Core.Data;

namespace RandomItemGiverUpdater.Gui.Menus
{
    public partial class wndSelectLootTables : Window
    {
        public List<LootTableSelectionEntry> lootTables = new List<LootTableSelectionEntry>();

        private wndSelectLootTables()
        {
            InitializeComponent();
        }

        public static List<LootTableSelectionEntry> Display(List<LootTable> lootTables, string header)
        {
            wndSelectLootTables wnd = new wndSelectLootTables();

            wnd.tblHeader.Text = header;
            wnd.cbxScheme.Items.Add("None");
            wnd.cbxScheme.SelectedIndex = 0;

            foreach (LootTable lootTable in lootTables)
            {
                LootTableSelectionEntry entry = new LootTableSelectionEntry(lootTable);
                wnd.lootTables.Add(entry); //Set up loot table list
                wnd.stpLootTables.Children.Add(entry.visual); //Display the checkboxes for the entries

                //Construct scheme selection
                if (!wnd.cbxScheme.Items.Contains(lootTable.name))
                {
                    wnd.cbxScheme.Items.Add(lootTable.name);
                }
            }

            wnd.ShowDialog();

            return wnd.lootTables;
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
                foreach (LootTableSelectionEntry entry in lootTables)
                {
                    if (entry.lootTable.name == cbxScheme.Text)
                    {
                        entry.visual.cbAddToLootTable.IsChecked = true;
                    }
                    else
                    {
                        entry.visual.cbAddToLootTable.IsChecked = false;
                    }
                }
            }
        }
    }
}
