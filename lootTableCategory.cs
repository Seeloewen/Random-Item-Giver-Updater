using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace Random_Item_Giver_Updater
{
    public class lootTableCategory
    {
        public string categoryName;
        public string categoryPath;
        public bool isCollapsed = true;
        public List<lootTable> lootTableList = new List<lootTable>();
        public StackPanel categoryStackPanel = new StackPanel();
        public Canvas categoryHeaderCanvas = new Canvas();
        public TextBlock categoryHeader = new TextBlock();

        public lootTableCategory(string name, string path)
        {
            //Map variables
            categoryName = name;
            categoryPath = path;

            //Create category canvas
            categoryStackPanel.Children.Add(categoryHeaderCanvas);

            //Create category header canvas
            categoryHeaderCanvas.Height = 35;
            categoryHeaderCanvas.MouseDown += new MouseButtonEventHandler(categoryHeaderCanvas_MouseDown);

            //categoryHeaderCanvas.HorizontalAlignment = HorizontalAlignment.Right;
            categoryHeaderCanvas.Background = new SolidColorBrush(Color.FromArgb(100, 16, 28, 28));
            categoryHeaderCanvas.Children.Add(categoryHeader);

            //Create category header
            categoryHeader.Text = categoryName;
            categoryHeader.FontSize = 15;
            categoryHeader.FontWeight = FontWeights.SemiBold;
            categoryHeader.Foreground = new SolidColorBrush(Colors.White);
            categoryHeader.Margin = new Thickness(10, 10, 0, 0);
        }

        private void categoryHeaderCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (isCollapsed == true)
            {
                //Show all the loot tables
                foreach (lootTable lootTable in lootTableList)
                {
                    lootTable.lootTableCanvas.Visibility = Visibility.Visible;
                    categoryStackPanel.Children.Add(lootTable.lootTableCanvas);
                }

                //Change the collapse state variable
                isCollapsed = false;
            }
            else if (isCollapsed == false)
            {
                //Hide the loot tables and collapse the categories
                categoryStackPanel.Children.Clear();

                //Readd the header
                categoryStackPanel.Children.Add(categoryHeaderCanvas);

                //Change the collapse state variable
                isCollapsed = true;
            }
        }
    }
}
