using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Gui.Components;

namespace RandomItemGiverUpdater.Entries
{
    public class LootTableSelectionEntry : LootTable
    {
        public LootTableSelectionVisual visual;
        public bool isSelected = false;

        public LootTableSelectionEntry(string name, LootTableCategory category, string path) : base(name, category, path)
        {
            visual = new LootTableSelectionVisual() { DataContext = this };
        }

    }
}
