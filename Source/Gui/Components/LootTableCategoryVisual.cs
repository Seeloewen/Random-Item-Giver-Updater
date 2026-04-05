using System.Collections.Generic;
using System.Text;
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
        public List<UIElement> entries = new List<UIElement>();

        private bool isCollapsed = true;
        private string category;

        public LootTableCategoryVisual(string category, int depth)
        {
            this.category = category.Replace("\\", "");

            //Category header canvas
            cvsCategoryHeader.Height = 35;
            cvsCategoryHeader.MouseDown += new MouseButtonEventHandler(cvsCategoryHeader_MouseDown);
            cvsCategoryHeader.Background = new SolidColorBrush(Color.FromArgb(100, 16, 28, 28));
            cvsCategoryHeader.Children.Add(tblCategoryHeader);

            //Category header
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < depth; i++)
            {
                sb.Append("   ");
            }
            sb.Append($"{COLLAPSED_SYMBOL} {this.category}");

            tblCategoryHeader.Text = sb.ToString();
            tblCategoryHeader.FontSize = 15;
            tblCategoryHeader.FontWeight = FontWeights.SemiBold;
            tblCategoryHeader.Foreground = new SolidColorBrush(Colors.White);
            tblCategoryHeader.Margin = new Thickness(10, 10, 0, 0);

            Children.Add(cvsCategoryHeader);
        }

        private void cvsCategoryHeader_MouseDown(object sender, MouseEventArgs e)
        {
            if (isCollapsed)
            {
                //Show the different loot tables
                foreach (UIElement lootTable in entries)
                {
                    Children.Add(lootTable);
                }

                isCollapsed = false;
                tblCategoryHeader.Text = tblCategoryHeader.Text.Replace(COLLAPSED_SYMBOL, EXPANDED_SYMBOL);
            }
            else
            {
                //Collapse the category by clearing the children
                Children.Clear();
                Children.Add(cvsCategoryHeader);
                isCollapsed = true;
                tblCategoryHeader.Text = tblCategoryHeader.Text.Replace(EXPANDED_SYMBOL, COLLAPSED_SYMBOL);
            }
        }
    }
}
