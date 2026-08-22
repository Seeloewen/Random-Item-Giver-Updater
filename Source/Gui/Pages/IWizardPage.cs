using RandomItemGiverUpdater.Gui.Menus;

namespace RandomItemGiverUpdater.Gui.Pages
{
    public interface IWizardPage
    {
        public virtual void Execute() { }

        public abstract void SetWindow(Wizard wnd);
    }
}
