using RandomItemGiverUpdater.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RandomItemGiverUpdater.Gui.Components
{
    public class LootTableSidebarVisual : Canvas
    {
        public TextBlock tblLootTable = new TextBlock();

        public LootTableSidebarVisual(string name)
        {
            //Canvas
            Height = 35;
            Background = new SolidColorBrush(Color.FromArgb(100, 65, 65, 65));

            //Textblock			
            tblLootTable.Text = name;
            tblLootTable.Foreground = new SolidColorBrush(Colors.White);
            tblLootTable.Margin = new Thickness(25, 10, 0, 0);
            tblLootTable.FontSize = 15;
            Children.Add(tblLootTable);

            //Add mouse down event to load the loot table
            MouseDown += new MouseButtonEventHandler(cvsLootTable_MouseDown);
        }

        private void cvsLootTable_MouseDown(object sender, MouseEventArgs e)
        {
            RIGU.core.SetLootTable((LootTable)DataContext);
        }
    }
}
