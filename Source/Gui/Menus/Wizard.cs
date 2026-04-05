using RandomItemGiverUpdater.Gui.Pages;
using SeeloewenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RandomItemGiverUpdater.Gui.Menus
{
    public class Wizard : Window
    {
        public int currentPage = 1;
        public int maxPages;
        public IWizardPage[] pages = new IWizardPage[6];
        public Frame frame;

        public void Init(Frame frame, int maxPages)
        {
            this.maxPages = maxPages;
            this.frame = frame;
        }

        public void ShowNextPage()
        {
            if (currentPage == maxPages)
            {
                Close();
                return;
            }

            ShowPage(++currentPage);
        }

        public void ShowPreviousPage()
        {
            if (currentPage == 1)
            {
                Close();
                return;
            }

            ShowPage(--currentPage);
        }

        public void ShowPage(int page)
        {
            currentPage = page;
            pages[page - 1].Execute();
            frame.Content = pages[page - 1];
        }


        //Basically a wrapper so I can use numbers from 1 to 6.
        //You need to specify the specific page class to use it properly
        public T GetPage<T>(int i) where T : IWizardPage => (T)pages[i - 1];
    }
}
