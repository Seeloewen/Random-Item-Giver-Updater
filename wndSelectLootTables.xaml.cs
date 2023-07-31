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
    /// <summary>
    /// Interaktionslogik für wndSelectLootTables.xaml
    /// </summary>
    public partial class wndSelectLootTables : Window
    {
        //Attributes
        public static List<CheckBox> checkBoxList = new List<CheckBox>();
        public static List<lootTable> lootTableList = new List<lootTable>();
        public bool lootTableSelected = false;

        //-- Constructor --//
        public wndSelectLootTables(List<lootTable> lootTableListArg)
        {
            InitializeComponent();

            //Set loot table list
            lootTableList = lootTableListArg;

            //Display all checkboxes
            foreach (lootTable lootTable in lootTableList)
            {
                stpLootTables.Children.Add(lootTable.cbAddToLootTable);
            }
        }

        //-- Event Handlers --//

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //Close the window
            Close();
        }

        private void wndSelectLootTables1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Assume no loot table is selected
            lootTableSelected = false;

            //Check if a loot table is selected and change the attribute
            foreach (CheckBox checkBox in stpLootTables.Children.OfType<CheckBox>())
            {
                if (checkBox.IsChecked == true)
                {
                    lootTableSelected = true;
                    break;
                }
            }

            if (lootTableSelected)
            {
                //Clear Stackpanel before quitting
                stpLootTables.Children.Clear();
            }
            else
            {
                //Stop quitting and show error
                e.Cancel = true;
                MessageBox.Show("Please select at least one loot table!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCheckAll_Click(object sender, RoutedEventArgs e)
        {
            //Check all checkboxes
            foreach (lootTable lootTable in lootTableList)
            {
                lootTable.cbAddToLootTable.IsChecked = true;
            }
        }

        private void btnUncheckAll_Click(object sender, RoutedEventArgs e)
        {
            //Uncheck all checkboxes
            foreach (lootTable lootTable in lootTableList)
            {
                lootTable.cbAddToLootTable.IsChecked = false;
            }
        }
    }
}
