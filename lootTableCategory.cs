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

        //Controls
        public StackPanel stpCategory = new StackPanel();
        public Canvas cvsCategoryHeader = new Canvas();
        public TextBlock tblCategoryHeader = new TextBlock();

        //Attributes
        public string categoryName;
        public string categoryPath;
        public bool isCollapsed = true;
        public List<lootTable> lootTableList = new List<lootTable>();

        //-- Constructor --//

        public lootTableCategory(string name, string path)
        {
            //Map variables
            categoryName = name;
            categoryPath = path;

            //Create category canvas
            stpCategory.Children.Add(cvsCategoryHeader);

            //Create category header canvas
            cvsCategoryHeader.Height = 35;
            cvsCategoryHeader.MouseDown += new MouseButtonEventHandler(cvsCategoryHeader_MouseDown);
            cvsCategoryHeader.Background = new SolidColorBrush(Color.FromArgb(100, 16, 28, 28));
            cvsCategoryHeader.Children.Add(tblCategoryHeader);

            //Create category header
            tblCategoryHeader.Text = categoryName;
            tblCategoryHeader.FontSize = 15;
            tblCategoryHeader.FontWeight = FontWeights.SemiBold;
            tblCategoryHeader.Foreground = new SolidColorBrush(Colors.White);
            tblCategoryHeader.Margin = new Thickness(10, 10, 0, 0);
        }

        //-- Event Handlers --//

        private void cvsCategoryHeader_MouseDown(object sender, MouseEventArgs e)
        {
            if (isCollapsed == true)
            {
                //Show all the loot tables
                foreach (lootTable lootTable in lootTableList)
                {
                    lootTable.cvsLootTable.Visibility = Visibility.Visible;
                    stpCategory.Children.Add(lootTable.cvsLootTable);
                }

                //Change the collapse state variable
                isCollapsed = false;
            }
            else if (isCollapsed == false)
            {
                //Hide the loot tables and collapse the categories
                stpCategory.Children.Clear();

                //Readd the header
                stpCategory.Children.Add(cvsCategoryHeader);

                //Change the collapse state variable
                isCollapsed = true;
            }
        }
    }
}
