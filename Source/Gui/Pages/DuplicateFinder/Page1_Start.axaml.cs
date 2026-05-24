using Avalonia.Controls;
using Avalonia.Interactivity;
using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Gui.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RandomItemGiverUpdater.Gui.Pages.DuplicateFinder
{
    public partial class Page1_Start : UserControl, IWizardPage
    {
        private wndDuplicateFinder wndDuplicateFinder;

        public Page1_Start()
        {
            InitializeComponent();
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            //Check if either the datapack or loot table is valid, depending on what's selected
            if ((rbtnAll.IsChecked == true && RIGU.core.DatapackIsValid(RIGU.core.currentDatapack)) || (rbtnCurrent.IsChecked == true && RIGU.core.currentLootTable != null))
            {
                if (rbtnAll.IsChecked == true) RIGU.duplicateFinder.checkEntireDatapack = true;
                else if (rbtnCurrent.IsChecked == true) RIGU.duplicateFinder.checkEntireDatapack = false;
                wndDuplicateFinder.ShowNextPage();
            }
            else
            {
                //TODO: Avalonia Rework
                //MessageBox.Show("Cannot search for duplicates. Please make sure that a datapack or loot table is loaded.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) => wndDuplicateFinder.ShowPreviousPage();
    }
}
