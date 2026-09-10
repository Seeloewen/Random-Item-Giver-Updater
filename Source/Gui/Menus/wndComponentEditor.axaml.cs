using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia.Enums;
using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Util;
using System.Threading.Tasks;

namespace RandomItemGiverUpdater.Gui.Menus
{
    public partial class wndComponentEditor : Window
    {
        public ModificationState result;
        public string newComponent = "";
        private string oldComponent;

        public wndComponentEditor()
        {
            InitializeComponent();
        }

        public async Task<(ModificationState, string)> GetFromDialog(string itemName, string currentComponent)
        {
            //Show the dialog and wait for the result and new nbt
            tblHeader.Text = $"Editing Component of item {itemName}";
            tbComponent.Text = currentComponent;
            oldComponent = currentComponent;
            await ShowDialog(RIGU.core.wndMain);

            return (result, newComponent);
        }


        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            newComponent = tbComponent.Text;

            if (newComponent == oldComponent)
            {
                //If the nbt is unchanged
                result = ModificationState.Unchanged;

                await this.MsgBox("The changes were saved successfully", "Saved", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Info);
            }
            else if (newComponent == "")
            {
                //If the user entered no nbt tag but did not delete it, ask if they want to delete it since having an empty tag is pretty much useless
                ButtonResult msgResult = await this.MsgBox("You did not enter any Item Stack Component. Do you want to delete it?", "Empty Component", ButtonEnum.YesNo, MsBox.Avalonia.Enums.Icon.Question);
                switch (msgResult)
                {
                    case ButtonResult.Yes:
                        result = ModificationState.Deleted;
                        await this.MsgBox("The Item Stack Component was successfully deleted!", "Deleted", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Info);
                        newComponent = "";
                        break;
                    case ButtonResult.No:
                        result = ModificationState.Modified;
                        break;
                }
            }
            else
            {
                result = ModificationState.Modified;
                await this.MsgBox("The changes were saved successfully", "Saved", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Info);
            }

            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //Close without any changes
            result = ModificationState.Unchanged;
            newComponent = oldComponent;
            Close();
        }

        private async void btnDeleteComponent_Click(object sender, RoutedEventArgs e)
        {
            //Ask the user whether they really want to delete the tag
            ButtonResult msgResult = await this.MsgBox("Are you sure that you want to delete the Item Stack Component?", "Delete Component", ButtonEnum.YesNo, MsBox.Avalonia.Enums.Icon.Question);
            switch (msgResult)
            {
                case ButtonResult.Yes:
                    result = ModificationState.Deleted;
                    await this.MsgBox("The Item Stack Component was successfully deleted!", "Deleted", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Info);
                    newComponent = "";
                    Close();
                    break;
            }
        }
    }
}
