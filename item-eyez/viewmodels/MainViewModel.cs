using System.Windows;
using System.Windows.Input;

namespace item_eyez
{
    public class MainViewModel
    {
        public ICommand ResetDatabaseCommand => new RelayCommand(ResetDatabase);

        public MainViewModel()
        {
        }

        private void ResetDatabase()
        {
            var result = MessageBox.Show(
                "Are you sure you want to delete and recreate the database? This will erase all data.",
                "Confirm Reset",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DatabaseHelper.Instance.DeleteDatabase();
                    DatabaseHelper.Instance.CreateDatabase();
                    ItemEyezDatabase.Instance().OnDataChanged();
                    MessageBox.Show("Database reset successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while resetting the database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
