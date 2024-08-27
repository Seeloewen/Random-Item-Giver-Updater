using System.Windows;

namespace Random_Item_Giver_Updater
{
    public partial class wndNBTEditor : Window
    {
        public EditorResult result;
        public string newNbt = "";
        private string oldNbt;

        public wndNBTEditor()
        {
            InitializeComponent();
        }

        public (EditorResult, string) GetFromDialog(string itemName, string currentNbt)
        {
            //Show the dialog and wait for the result and new nbt
            tblHeader.Text = $"Editing NBT of item {itemName}";
            tbNBT.Text = currentNbt;
            oldNbt = currentNbt;
            ShowDialog();

            return (result, newNbt);
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            newNbt = tbNBT.Text;

            if (newNbt == oldNbt)
            {
                //If the nbt is unchanged
                result = EditorResult.Unchanged;
                MessageBox.Show("The changes were saved successfully", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (newNbt == "")
            {
                //If the user entered no nbt tag but did not delete it, ask if they want to delete it since having an empty tag is pretty much useless
                MessageBoxResult msgResult = MessageBox.Show("You did not enter any NBT tag. Do you want to delete it?", "Empty NBT tag", MessageBoxButton.YesNo, MessageBoxImage.Question);
                switch (msgResult)
                {
                    case MessageBoxResult.Yes:
                        result = EditorResult.Deleted;
                        MessageBox.Show("The NBT tag was successfully deleted!", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                        newNbt = "";
                        break;
                    case MessageBoxResult.No:
                        result = EditorResult.Edited;
                        break;
                }
            }
            else
            {
                result = EditorResult.Edited;
                MessageBox.Show("The changes were saved successfully", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            Close();
        }

        private void btnDeleteNBTTag_Click(object sender, RoutedEventArgs e)
        {
            //Ask the user whether they really want to delete the tag
            MessageBoxResult msgResult = MessageBox.Show("Are you sure that you want to delete the NBT Tag?", "Delete NBT Tag", MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (msgResult)
            {
                case MessageBoxResult.Yes:
                    result = EditorResult.Deleted;
                    MessageBox.Show("The NBT tag was successfully deleted!", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                    newNbt = "";
                    Close();
                    break;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //Close without any changes
            result = EditorResult.Unchanged;
            newNbt = oldNbt;
            Close();
        }
    }
}
