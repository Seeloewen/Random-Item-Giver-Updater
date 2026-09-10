using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia.Enums;
using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Util;
using System.Threading.Tasks;

namespace RandomItemGiverUpdater.Gui.Menus
{
    public partial class wndNBTEditor : Window
    {
        public ModificationState result;
        public string newNbt = "";
        private string oldNbt;

        public wndNBTEditor()
        {
            InitializeComponent();
        }

        public async Task<(ModificationState, string)> GetFromDialog(string itemName, string currentNbt)
        {
            //Show the dialog and wait for the result and new nbt
            tblHeader.Text = $"Editing NBT of item {itemName}";
            tbNBT.Text = currentNbt;
            oldNbt = currentNbt;
            await ShowDialog(this);

            return (result, newNbt);
        }


        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            newNbt = tbNBT.Text;

            if (newNbt == oldNbt)
            {
                //If the nbt is unchanged
                result = ModificationState.Unchanged;
                await this.MsgBox("The changes were saved successfully", "Saved", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Info);
            }
            else if (newNbt == "")
            {
                //If the user entered no nbt tag but did not delete it, ask if they want to delete it since having an empty tag is pretty much useless
                ButtonResult msgResult = await this.MsgBox("You did not enter any NBT tag. Do you want to delete it?", "Empty NBT tag", ButtonEnum.YesNo, MsBox.Avalonia.Enums.Icon.Question);
                switch (msgResult)
                {
                    case ButtonResult.Yes:
                        result = ModificationState.Deleted;
                        await this.MsgBox("The NBT tag was successfully deleted!", "Deleted", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Info);
                        newNbt = "";
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

        private async void btnDeleteNBTTag_Click(object sender, RoutedEventArgs e)
        {
            //Ask the user whether they really want to delete the tag
            ButtonResult msgResult = await this.MsgBox("Are you sure that you want to delete the NBT Tag?", "Delete NBT Tag", ButtonEnum.YesNo, MsBox.Avalonia.Enums.Icon.Question);
            switch (msgResult)
            {
                case ButtonResult.Yes:
                    result = ModificationState.Deleted;
                    await this.MsgBox("The NBT tag was successfully deleted!", "Deleted", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Info);
                    newNbt = "";
                    Close();
                    break;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //Close without any changes
            result = ModificationState.Unchanged;
            newNbt = oldNbt;
            Close();
        }
    }
}