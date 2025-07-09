using System.Windows;

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

        public (ModificationState, string) GetFromDialog(string itemName, string currentComponent)
        {
            //Show the dialog and wait for the result and new nbt
            tblHeader.Text = $"Editing Component of item {itemName}";
            tbComponent.Text = currentComponent;
            oldComponent = currentComponent;
            ShowDialog();

            return (result, newComponent);
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            newComponent = tbComponent.Text;

            if (newComponent == oldComponent)
            {
                //If the nbt is unchanged
                result = ModificationState.Unchanged;
                MessageBox.Show("The changes were saved successfully", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (newComponent == "")
            {
                //If the user entered no nbt tag but did not delete it, ask if they want to delete it since having an empty tag is pretty much useless
                MessageBoxResult msgResult = MessageBox.Show("You did not enter any Item Stack Component. Do you want to delete it?", "Empty Component", MessageBoxButton.YesNo, MessageBoxImage.Question);
                switch (msgResult)
                {
                    case MessageBoxResult.Yes:
                        result = ModificationState.Deleted;
                        MessageBox.Show("The Item Stack Component was successfully deleted!", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                        newComponent = "";
                        break;
                    case MessageBoxResult.No:
                        result = ModificationState.Edited;
                        break;
                }
            }
            else
            {
                result = ModificationState.Edited;
                MessageBox.Show("The changes were saved successfully", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void btnDeleteComponent_Click(object sender, RoutedEventArgs e)
        {
            //Ask the user whether they really want to delete the tag
            MessageBoxResult msgResult = MessageBox.Show("Are you sure that you want to delete the Item Stack Component?", "Delete Component", MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (msgResult)
            {
                case MessageBoxResult.Yes:
                    result = ModificationState.Deleted;
                    MessageBox.Show("The Item Stack Component was successfully deleted!", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                    newComponent = "";
                    Close();
                    break;
            }
        }
    }
}
