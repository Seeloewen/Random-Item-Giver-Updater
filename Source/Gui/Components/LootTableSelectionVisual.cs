using RandomItemGiverUpdater.Core.Workspace.Entries;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace RandomItemGiverUpdater.Gui.Components
{
    public class LootTableSelectionVisual : Canvas
    {
        private LootTableSelectionEntry entry;
        public CheckBox cbAddToLootTable = new CheckBox();

        public LootTableSelectionVisual(LootTableSelectionEntry entry)
        {
            this.entry = entry;
            Height = 35;

            //Checkbox for ItemAdding
            cbAddToLootTable.Content = entry.lootTable.identifier;
            cbAddToLootTable.Foreground = new SolidColorBrush(Colors.White);
            cbAddToLootTable.Margin = new Thickness(20, 15, 0, 0);
            cbAddToLootTable.FontSize = 15;
            cbAddToLootTable.IsChecked = true;
            cbAddToLootTable.IsCheckedChanged += cbAddToLootTable_CheckedChanged;

            Children.Add(cbAddToLootTable);
        }

        public void cbAddToLootTable_CheckedChanged(object? sender, RoutedEventArgs e)
        {
            entry.isSelected = (bool)cbAddToLootTable.IsChecked;
        }
    }
}
