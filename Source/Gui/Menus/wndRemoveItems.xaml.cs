using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Workspace;
using RandomItemGiverUpdater.Gui.Pages.ItemRemover;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RandomItemGiverUpdater.Gui.Menus
{
    public partial class wndRemoveItems : Wizard
    {
        private ItemRemover itemRemover;

        public wndRemoveItems()
        {
            InitializeComponent();

            itemRemover = RIGU.itemRemover;
            DataContext = itemRemover;

            Init(frWizard, 5);
            InitUI();

            ShowPage(1);
        }

        private void InitUI()
        {
            //Setup pages
            pages[0] = new Page1_Start(this);
            pages[1] = new Page2_ItemList(this);
            pages[2] = new Page3_RemoveEntries(this);
            pages[3] = new Page4_Removing(this);
            pages[4] = new Page5_Finished(this);

            GetPage<Page3_RemoveEntries>(3).DataContext = DataContext;
        }

        public void SkipWithInput(List<string> input)
        {
            ShowPage(2);
            Page2_ItemList page2 = GetPage<Page2_ItemList>(2);

            //Selects custom prefixes and pastes the predefined input in the textbox
            page2.cbIncludesCustomPrefixes.IsChecked = true;
            foreach (string item in input)
            {
                page2.tbItems.AppendText($"{item}\n");
            }
        }

        public void UpdateProgress(double pbValue, int percentage, int totalItems, int finishedLootTables)
        {
            //Relay the progress to the page
            GetPage<Page4_Removing>(4).UpdateProgress(pbValue, percentage, totalItems, finishedLootTables);
        }
    }
}

