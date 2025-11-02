// ----------------------------------------------------------------------------
// <copyright company="death-scripts">
// Copyright (c) death-scripts. All rights reserved.
// </copyright>
//                   ██████╗ ███████╗ █████╗ ████████╗██╗  ██╗
//                   ██╔══██╗██╔════╝██╔══██╗╚══██╔══╝██║  ██║
//                   ██║  ██║█████╗  ███████║   ██║   ███████║
//                   ██║  ██║██╔══╝  ██╔══██║   ██║   ██╔══██║
//                   ██████╔╝███████╗██║  ██║   ██║   ██║  ██║
//                   ╚═════╝ ╚══════╝╚═╝  ╚═╝   ╚═╝   ╚═╝  ╚═╝
//
//              ███████╗ ██████╗██████╗ ██╗██████╗ ████████╗███████╗
//              ██╔════╝██╔════╝██╔══██╗██║██╔══██╗╚══██╔══╝██╔════╝
//              ███████╗██║     ██████╔╝██║██████╔╝   ██║   ███████╗
//              ╚════██║██║     ██╔══██╗██║██╔═══╝    ██║   ╚════██║
//              ███████║╚██████╗██║  ██║██║██║        ██║   ███████║
//              ╚══════╝ ╚═════╝╚═╝  ╚═╝╚═╝╚═╝        ╚═╝   ╚══════╝
// ----------------------------------------------------------------------------
using System.Data;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Input;
using Item_eyez.Controls;
using Item_eyez.Database;
using Microsoft.Win32;

namespace Item_eyez.Viewmodels
{
    /// <summary>
    /// The main view model.
    /// </summary>
    public class MainViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
        }

        /// <summary>
        /// Gets the import access command.
        /// </summary>
        /// <value>
        /// The import access command.
        /// </value>
        public ICommand ImportAccessCommand => new RelayCommand(this.ImportAccessDatabase);

        /// <summary>
        /// Gets the populate sample data command.
        /// </summary>
        /// <value>
        /// The populate sample data command.
        /// </value>
        public ICommand PopulateSampleDataCommand => new RelayCommand(this.PopulateSampleData);

        /// <summary>
        /// Gets the reset database command.
        /// </summary>
        /// <value>
        /// The reset database command.
        /// </value>
        public ICommand ResetDatabaseCommand => new RelayCommand(this.ResetDatabase);

        /// <summary>
        /// Determines whether the specified text contains keyword.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="keywords">The keywords.</param>
        /// <returns>
        ///   <c>true</c> if the specified text contains keyword; otherwise, <c>false</c>.
        /// </returns>
        private static bool ContainsKeyword(string text, string[] keywords)
        {
            foreach (string word in keywords)
            {
                if (text != null && text.ToLower().Contains(word))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Extracts the keyword.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="keywords">The keywords.</param>
        /// <returns>
        /// The nullable.
        /// </returns>
        private static string? ExtractKeyword(string text, string[] keywords)
        {
            foreach (string word in keywords)
            {
                if (text != null && text.ToLower().Contains(word))
                {
                    return word;
                }
            }

            return null;
        }

        /// <summary>
        /// Imports the access database.
        /// </summary>
        private void ImportAccessDatabase()
        {
            OpenFileDialog dialog = new()
            {
                Filter = "Access Database (*.accdb)|*.accdb",
                Title = "Select Access Database",
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            this.ImportFile(dialog.FileName);
        }

        /// <summary>
        /// Imports the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <exception cref="System.Exception">No tables found in database.</exception>
        private void ImportFile(string filePath)
        {
            try
            {
                string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};";
                OleDbConnection oleDbConnection = new(connectionString);
                using OleDbConnection connection = oleDbConnection;
                connection.Open();

                DataTable tables = connection.GetSchema("Tables");
                if (tables.Rows.Count == 0 || tables == null)
                {
                    throw new Exception("No tables found in database");
                }

                DataRow dataRow = tables.Rows[0];
                string tableName = dataRow["TABLE_NAME"].ToString()!;
                using OleDbDataAdapter adapter = new($"SELECT * FROM [{tableName}]", connection);
                DataTable data = new();
                _ = adapter.Fill(data);

                ItemEyezDatabase db = ItemEyezDatabase.Instance();
                Dictionary<string, Container> containers = db.GetContainersWithRelationships().ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
                Dictionary<string, Room> rooms = db.GetRoomsList().ToDictionary(r => r.Name, StringComparer.OrdinalIgnoreCase);

                ProgressWindow progress = new()
                {
                    Owner = Application.Current.MainWindow,
                };
                progress.Bar.Maximum = data.Rows.Count;
                progress.Show();

                int processed = 0;
                foreach (DataRow row in data.Rows)
                {
                    string itemName = row.Table.Columns.Contains("item") ? row["item"].ToString() ?? string.Empty : string.Empty;
                    if (string.IsNullOrWhiteSpace(itemName))
                    {
                        continue;
                    }

                    string description = row.Table.Columns.Contains("description") ? row["description"].ToString() ?? string.Empty : string.Empty;
                    string location = row.Table.Columns.Contains("location") ? row["location"].ToString() ?? string.Empty : string.Empty;
                    string categories = string.Empty;
                    if (row.Table.Columns.Contains("categories"))
                    {
                        categories = row["categories"].ToString() ?? string.Empty;
                    }
                    else if (row.Table.Columns.Contains("catagories"))
                    {
                        categories = row["catagories"].ToString() ?? string.Empty;
                    }

                    decimal value = 0m;
                    if (row.Table.Columns.Contains("cashvalue"))
                    {
                        _ = decimal.TryParse(row["cashvalue"].ToString(), out value);
                    }

                    bool isContainerLocation = ContainsKeyword(location, new[] { "lid", "box", "locker", "desk", "cabinet", "shelf", "drawer", "bin", "tub" });
                    bool isRoomLocation = ContainsKeyword(location, new[] { "room", "kitchen", "closet", "garage", "pantry", "shop" });

                    Guid itemId = db.AddItem(itemName, description, value, categories);

                    if (isContainerLocation)
                    {
                        if (!containers.TryGetValue(location, out Container? container))
                        {
                            Guid id = db.AddContainer(location, string.Empty);
                            container = new Container(id, location, string.Empty);
                            containers[location] = container;
                        }

                        db.AssociateItemWithContainer(itemId, container.Id);

                        if (isRoomLocation)
                        {
                            string? roomKey = ContainsKeyword(location, new[] { "room" })
                                ? location
                                : ExtractKeyword(location, new[] { "room", "kitchen", "closet", "garage", "pantry", "shop" });
                            if (roomKey != null)
                            {
                                if (!rooms.TryGetValue(roomKey, out Room? room))
                                {
                                    db.AddRoom(roomKey, string.Empty);
                                    room = db.GetRoomsList().First(r => r.Name.Equals(roomKey, StringComparison.OrdinalIgnoreCase));
                                    rooms[roomKey] = room;
                                }

                                db.SetItemsRoom(container.Id, room.Id);
                            }
                        }

                        string[] parts = location.Split(new[] { " in ", " on " }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 2)
                        {
                            string parentName = parts[1].Trim();
                            if (!string.IsNullOrEmpty(parentName) && ContainsKeyword(parentName, new[] { "lid", "box", "locker", "desk", "cabinet", "shelf", "drawer", "bin", "tub" }))
                            {
                                if (!containers.TryGetValue(parentName, out Container? parent))
                                {
                                    Guid pid = db.AddContainer(parentName, string.Empty);
                                    parent = new Container(pid, parentName, string.Empty);
                                    containers[parentName] = parent;
                                }

                                db.SetItemsContainer(container.Id, parent.Id);

                                if (isRoomLocation)
                                {
                                    string? roomKey = ContainsKeyword(parentName, new[] { "room" })
                                        ? parentName
                                        : ExtractKeyword(parentName, new[] { "room", "kitchen", "closet", "garage", "pantry", "shop" });
                                    if (roomKey != null)
                                    {
                                        if (!rooms.TryGetValue(roomKey, out Room? pr))
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
                        {
                            isRoomLocation = true; // default to room if uncertain
                        }

                        string roomKey = ContainsKeyword(location, new[] { "room" })
                            ? location
                            : ExtractKeyword(location, new[] { "room", "kitchen", "closet", "garage", "pantry", "shop" }) ?? location;
                        if (!rooms.TryGetValue(roomKey, out Room? room))
                        {
                            db.AddRoom(roomKey, string.Empty);
                            room = db.GetRoomsList().First(r => r.Name.Equals(roomKey, StringComparison.OrdinalIgnoreCase));
                            rooms[roomKey] = room;
                        }

                        db.AssociateItemWithRoom(itemId, room.Id);
                    }

                    processed++;
                    progress.Bar.Value = processed;
                    _ = progress.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => { }));
                }

                progress.Close();
                _ = MessageBox.Show("Import complete", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Failed to import: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Populates the sample data.
        /// </summary>
        private void PopulateSampleData()
        {
            ItemEyezDatabase db = ItemEyezDatabase.Instance();

            db.AddRoom("Kitchen", "Where food is prepared");
            db.AddRoom("Garage", "For tools and vehicles");

            System.Collections.ObjectModel.ObservableCollection<Room> rooms = db.GetRoomsList();
            Room kitchen = rooms.First(r => r.Name == "Kitchen");
            Room garage = rooms.First(r => r.Name == "Garage");

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

            _ = MessageBox.Show("Sample data populated.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Resets the database.
        /// </summary>
        private void ResetDatabase()
        {
            MessageBoxResult result = MessageBox.Show(
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
                    _ = MessageBox.Show("Database reset successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show($"An error occurred while resetting the database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}