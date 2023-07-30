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
            //Clear Stackpanel before quitting
            stpLootTables.Children.Clear();

            //Close the window
            Close();
        }

        private void wndSelectLootTables1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Clear Stackpanel before quitting
            stpLootTables.Children.Clear();
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
