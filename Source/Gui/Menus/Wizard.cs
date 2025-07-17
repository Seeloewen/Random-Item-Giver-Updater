using RandomItemGiverUpdater.Gui.Pages;
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
        public IWizardPage[] pages = new IWizardPage[6];
        public Frame frame;

        public void ShowNextPage()
        {
            if (currentPage == 6) Close();

            frame.Content = pages[++currentPage];
        }

        public void ShowPreviousPage()
        {
            if (currentPage == 1) Close();

            frame.Content = pages[--currentPage];
        }

        //Basically a wrapper so I can use numbers from 1 to 6.
        //You need to specify the specific page class to use it properly
        public T GetPage<T>(int i) where T : IWizardPage => (T)pages[i + 1];
    }
}
