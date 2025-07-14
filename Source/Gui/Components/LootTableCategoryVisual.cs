using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Gui.Components;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RandomItemGiverUpdater
{
    public class LootTableCategoryVisual : StackPanel
    {
        private const char COLLAPSED_SYMBOL = '▼';
        private const char EXPANDED_SYMBOL = '▲';

        private Canvas cvsCategoryHeader = new Canvas();
        private TextBlock tblCategoryHeader = new TextBlock();
        public List<LootTableSidebarVisual> lootTableEntries = new List<LootTableSidebarVisual>();

        private bool isCollapsed = true;
        private LootTableCategory category;

        public LootTableCategoryVisual(LootTableCategory category)
        {
            this.category = category;

            //Category header canvas
            cvsCategoryHeader.Height = 35;
            cvsCategoryHeader.MouseDown += new MouseButtonEventHandler(cvsCategoryHeader_MouseDown);
            cvsCategoryHeader.Background = new SolidColorBrush(Color.FromArgb(100, 16, 28, 28));
            cvsCategoryHeader.Children.Add(tblCategoryHeader);

            //Category header
            tblCategoryHeader.Text = $"{COLLAPSED_SYMBOL} {category.name}";
            tblCategoryHeader.FontSize = 15;
            tblCategoryHeader.FontWeight = FontWeights.SemiBold;
            tblCategoryHeader.Foreground = new SolidColorBrush(Colors.White);
            tblCategoryHeader.Margin = new Thickness(10, 10, 0, 0);
        }

        private void cvsCategoryHeader_MouseDown(object sender, MouseEventArgs e)
        {
            if (isCollapsed)
            {
                //Show the different loot tables
                foreach (LootTableSidebarVisual lootTable in lootTableEntries)
                {
                    Children.Add(lootTable);
                }

                isCollapsed = false;
                tblCategoryHeader.Text = $"{EXPANDED_SYMBOL} {category.name}";
            }
            else
            {
                //Collapse the category by clearing the children
                Children.Clear();
                Children.Add(cvsCategoryHeader);
                isCollapsed = true;
                tblCategoryHeader.Text = $"{COLLAPSED_SYMBOL} {category.name}";
            }
        }
    }
}
