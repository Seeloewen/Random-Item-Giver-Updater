using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RandomItemGiverUpdater.Gui.Components
{
    public class LootTableSidebarVisual : Canvas
    {
        public LootTable lootTable;
        public TextBlock tblLootTable = new TextBlock();

        public LootTableSidebarVisual(LootTable lootTable, int depth)
        {
            this.lootTable = lootTable;

            //Canvas
            Height = 35;
            Background = new SolidColorBrush(Color.FromArgb(100, 65, 65, 65));

            //Textblock			
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < depth; i++)
            {
                sb.Append("   ");
            }
            sb.Append(lootTable.name);
            tblLootTable.Text = sb.ToString();
            tblLootTable.Foreground = new SolidColorBrush(Colors.White);
            tblLootTable.Margin = new Thickness(10, 10, 0, 0);
            tblLootTable.FontSize = 15;
            Children.Add(tblLootTable);

            //Add mouse down event to load the loot table
            MouseDown += new MouseButtonEventHandler(cvsLootTable_MouseDown);
        }

        private void cvsLootTable_MouseDown(object sender, MouseEventArgs e)
        {
            RIGU.core.SetLootTable(lootTable);
        }
    }
}
