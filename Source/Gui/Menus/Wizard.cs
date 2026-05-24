using Avalonia.Controls;
using RandomItemGiverUpdater.Gui.Pages;

namespace RandomItemGiverUpdater.Gui.Menus
{
    public class Wizard : Window
    {
        public int currentPage = 1;
        public int maxPages;
        public IWizardPage[] pages = new IWizardPage[6];
        public ContentControl control;

        public void Init(ContentControl control, int maxPages)
        {
            this.maxPages = maxPages;
            this.control = control;
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
            control.Content = pages[page - 1];
        }


        //Basically a wrapper so I can use numbers from 1 to 6.
        //You need to specify the specific page class to use it properly
        public T GetPage<T>(int i) where T : IWizardPage => (T)pages[i - 1];
    }
}