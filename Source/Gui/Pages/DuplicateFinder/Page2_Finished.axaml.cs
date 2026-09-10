using System;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;
using MsBox.Avalonia.Enums;
using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Util;
using RandomItemGiverUpdater.Core.Workspace.Entries;
using RandomItemGiverUpdater.Gui.Menus;

namespace RandomItemGiverUpdater.Gui.Pages.DuplicateFinder
{
    public partial class Page2_Finished : UserControl, IWizardPage
    {
        private wndDuplicateFinder wndDuplicateFinder;
        private TextBlock tblNoDuplicatesFound = new TextBlock() { Text = "No duplicates were found", Foreground = new SolidColorBrush(Colors.White), FontSize = 24, IsVisible = false};

        public Page2_Finished()
        {
            InitializeComponent();
            cvsContent.Children.Add(tblNoDuplicatesFound);
        }

        public void SetWindow(Wizard wnd)
        {
            wndDuplicateFinder = (wndDuplicateFinder)wnd;
        }

        public void Execute()
        {
            RIGU.duplicateFinder.Run();
        }

        public void UpdateDuplicateDisplay(int amount)
        {
            //Toggle controls based on whether duplicates were found or not
            if (amount == 0)
            {
                tblNoDuplicatesFound.IsVisible = true;
                tblNoDuplicatesFound.Margin = new Thickness(225, 275, 0, 0);
                btnExportList.IsEnabled = false;
                btnDelete.IsEnabled = false;
            }
            else
            {
                tblNoDuplicatesFound.IsVisible = false;
                tblNoDuplicatesFound.Margin = new Thickness(225, 275, 0, 0);
                btnExportList.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e) => wndDuplicateFinder.ShowNextPage();

        private void btnBack_Click(object sender, RoutedEventArgs e) => wndDuplicateFinder.ShowPreviousPage();

        private void btnExportList_Click(object sender, RoutedEventArgs e) => RIGU.duplicateFinder.Export();

        private async void btnViewAll_Click(object sender, RoutedEventArgs e)
        {
            Canvas cvs = ((Button)sender).FindAncestorOfType<Canvas>();
            DuplicateEntry entry = (DuplicateEntry)cvs.DataContext;

            //Show a message in which loot tables the duplicate occurs
            await Crossplatform.Dialog(wndDuplicateFinder, $"The duplicate occurs in the following loot tables:\n{entry.lootTables.Replace(", ", "\n")}", "View all Loot Tables", ButtonEnum.Ok, Icon.Info);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e) => RIGU.duplicateFinder.DisplayItemRemover();

        private async void tblItemName_MouseDown(object sender, PointerPressedEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;

            //Show the controls for editing and hide the original name
            await Crossplatform.Dialog(wndDuplicateFinder, $"Full name of the item:\n{tb.Text}", "Full item name", ButtonEnum.Ok, Icon.Info);
        }
    }
}
