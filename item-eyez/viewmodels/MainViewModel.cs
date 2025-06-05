using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Data;
using System.Data.OleDb;
using Microsoft.Win32;

namespace item_eyez
{
    public class MainViewModel
    {
        public ICommand ResetDatabaseCommand => new RelayCommand(ResetDatabase);
        public ICommand PopulateSampleDataCommand => new RelayCommand(PopulateSampleData);
        public ICommand ImportAccessCommand => new RelayCommand(ImportAccessDatabase);

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

        private void ImportAccessDatabase()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Access Database (*.accdb)|*.accdb",
                Title = "Select Access Database"
            };

            if (dialog.ShowDialog() != true)
                return;

            ImportFile(dialog.FileName);
        }

        private void ImportFile(string filePath)
        {
            try
            {
                string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};";
                using var connection = new OleDbConnection(connectionString);
                connection.Open();

                DataTable tables = connection.GetSchema("Tables");
                if (tables.Rows.Count == 0)
                    throw new Exception("No tables found in database");

                string tableName = tables.Rows[0]["TABLE_NAME"].ToString();
                using var adapter = new OleDbDataAdapter($"SELECT * FROM [{tableName}]", connection);
                var data = new DataTable();
                adapter.Fill(data);

                var db = ItemEyezDatabase.Instance();
                var containers = db.GetContainersWithRelationships().ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
                var rooms = db.GetRoomsList().ToDictionary(r => r.Name, StringComparer.OrdinalIgnoreCase);

                var progress = new ProgressWindow
                {
                    Owner = Application.Current.MainWindow
                };
                progress.Bar.Maximum = data.Rows.Count;
                progress.Show();

                int processed = 0;
                foreach (DataRow row in data.Rows)
                {
                    string itemName = row.Table.Columns.Contains("item") ? row["item"].ToString() ?? string.Empty : string.Empty;
                    if (string.IsNullOrWhiteSpace(itemName))
                        continue;

                    string description = row.Table.Columns.Contains("description") ? row["description"].ToString() ?? string.Empty : string.Empty;
                    string location = row.Table.Columns.Contains("location") ? row["location"].ToString() ?? string.Empty : string.Empty;
                    string categories = string.Empty;
                    if (row.Table.Columns.Contains("categories"))
                        categories = row["categories"].ToString() ?? string.Empty;
                    else if (row.Table.Columns.Contains("catagories"))
                        categories = row["catagories"].ToString() ?? string.Empty;

                    decimal value = 0m;
                    if (row.Table.Columns.Contains("cashvalue"))
                        decimal.TryParse(row["cashvalue"].ToString(), out value);

                    bool isContainerLocation = ContainsKeyword(location, new[] { "lid", "box", "locker", "desk", "cabinet", "shelf", "drawer", "bin", "tub" });
                    bool isRoomLocation = ContainsKeyword(location, new[] { "room", "kitchen", "closet", "garage", "pantry" });

                    Guid itemId = db.AddItem(itemName, description, value, categories);

                    if (isContainerLocation)
                    {
                        if (!containers.TryGetValue(location, out var container))
                        {
                            var id = db.AddContainer(location, string.Empty);
                            container = new Container(id, location, string.Empty);
                            containers[location] = container;
                        }

                        db.AssociateItemWithContainer(itemId, container.Id);

                        if (isRoomLocation)
                        {
                            string roomKey = ExtractKeyword(location, new[] { "room", "kitchen", "closet", "garage", "pantry" });
                            if (roomKey != null)
                            {
                                if (!rooms.TryGetValue(roomKey, out var room))
                                {
                                    db.AddRoom(roomKey, string.Empty);
                                    room = db.GetRoomsList().First(r => r.Name.Equals(roomKey, StringComparison.OrdinalIgnoreCase));
                                    rooms[roomKey] = room;
                                }
                                db.SetItemsRoom(container.Id, room.Id);
                            }
                        }

                        var parts = location.Split(new[] { " in ", " on " }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 2)
                        {
                            var parentName = parts[1].Trim();
                            if (!string.IsNullOrEmpty(parentName) && ContainsKeyword(parentName, new[] { "lid", "box", "locker", "desk", "cabinet", "shelf", "drawer", "bin", "tub" }))
                            {
                                if (!containers.TryGetValue(parentName, out var parent))
                                {
                                    var pid = db.AddContainer(parentName, string.Empty);
                                    parent = new Container(pid, parentName, string.Empty);
                                    containers[parentName] = parent;
                                }
                                db.SetItemsContainer(container.Id, parent.Id);

                                if (isRoomLocation)
                                {
                                    string roomKey = ExtractKeyword(parentName, new[] { "room", "kitchen", "closet", "garage", "pantry" });
                                    if (roomKey != null)
                                    {
                                        if (!rooms.TryGetValue(roomKey, out var pr))
                                        {
                                            db.AddRoom(roomKey, string.Empty);
                                            pr = db.GetRoomsList().First(r => r.Name.Equals(roomKey, StringComparison.OrdinalIgnoreCase));
                                            rooms[roomKey] = pr;
                                        }
                                        db.SetItemsRoom(parent.Id, pr.Id);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!isRoomLocation)
                            isRoomLocation = true; // default to room if uncertain

                        var roomKey = ExtractKeyword(location, new[] { "room", "kitchen", "closet", "garage", "pantry" }) ?? location;
                        if (!rooms.TryGetValue(roomKey, out var room))
                        {
                            db.AddRoom(roomKey, string.Empty);
                            room = db.GetRoomsList().First(r => r.Name.Equals(roomKey, StringComparison.OrdinalIgnoreCase));
                            rooms[roomKey] = room;
                        }
                        db.AssociateItemWithRoom(itemId, room.Id);
                    }

                    processed++;
                    progress.Bar.Value = processed;
                    progress.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => { }));
                }

                progress.Close();
                MessageBox.Show("Import complete", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to import: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static bool ContainsKeyword(string text, string[] keywords)
        {
            foreach (var word in keywords)
            {
                if (text != null && text.ToLower().Contains(word))
                    return true;
            }
            return false;
        }

        private static string? ExtractKeyword(string text, string[] keywords)
        {
            foreach (var word in keywords)
            {
                if (text != null && text.ToLower().Contains(word))
                    return word;
            }
            return null;
        }
    }
}
