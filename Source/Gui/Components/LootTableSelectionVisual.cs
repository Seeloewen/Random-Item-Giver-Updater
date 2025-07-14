using RandomItemGiverUpdater.Entries;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RandomItemGiverUpdater.Gui.Components
{
    public class LootTableSelectionVisual : Canvas
    {
        public CheckBox cbAddToLootTable = new CheckBox();

        public LootTableSelectionVisual()
        {
            //Checkbox for ItemAdding
            cbAddToLootTable.Content = $"{GetEntryData().category.name}\\{GetEntryData().name}";
            cbAddToLootTable.Foreground = new SolidColorBrush(Colors.White);
            cbAddToLootTable.Margin = new Thickness(20, 15, 0, 0);
            cbAddToLootTable.FontSize = 15;
            cbAddToLootTable.IsChecked = true;
        }

        private LootTableSelectionEntry GetEntryData()
        {
            return (LootTableSelectionEntry)DataContext; //Assumes that the datacontext is set to LootTableSelectionEntry, which should be the case
        }
    }
}
