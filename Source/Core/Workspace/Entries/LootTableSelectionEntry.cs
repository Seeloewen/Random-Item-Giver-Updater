using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Data;
using RandomItemGiverUpdater.Gui.Components;

namespace RandomItemGiverUpdater.Core.Workspace.Entries
{
    public class LootTableSelectionEntry
    {
        public LootTable lootTable;
        public LootTableSelectionVisual visual;
        public bool isSelected = true;

        public LootTableSelectionEntry(LootTable lootTable)
        {
            this.lootTable = lootTable;
            visual = new LootTableSelectionVisual(this);
        }
    }
}
