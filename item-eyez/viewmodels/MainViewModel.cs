using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace item_eyez
{
    public class MainViewModel
    {
        public ICommand ResetDatabaseCommand => new RelayCommand(ResetDatabase);
        public ICommand PopulateSampleDataCommand => new RelayCommand(PopulateSampleData);

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

        private void PopulateSampleData()
        {
            var db = ItemEyezDatabase.Instance();

            db.AddRoom("Kitchen", "Where food is prepared");
            db.AddRoom("Garage", "For tools and vehicles");

            var rooms = db.GetRoomsList();
            var kitchen = rooms.First(r => r.Name == "Kitchen");
            var garage = rooms.First(r => r.Name == "Garage");

            Guid shelf = db.AddContainer("Shelf", "Wall shelf");
            db.SetItemsRoom(shelf, garage.Id);

            Guid box = db.AddContainer("Box", "Cardboard box");
            db.SetItemsRoom(box, kitchen.Id);

            Guid hammer = db.AddItem("Hammer", "Steel hammer", 10m, "Tools");
            db.AssociateItemWithContainer(hammer, shelf);

            Guid plates = db.AddItem("Plates", "Stack of plates", 15m, "Kitchen");
            db.AssociateItemWithContainer(plates, box);

            Guid chair = db.AddItem("Chair", "Wooden chair", 25m, "Furniture");
            db.AssociateItemWithRoom(chair, kitchen.Id);

            MessageBox.Show("Sample data populated.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
