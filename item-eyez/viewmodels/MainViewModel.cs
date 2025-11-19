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
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
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
        /// The separator.
        /// </summary>
        private static readonly string[] Separator = [" in ", " on "];

        /// <summary>
        /// The string array.
        /// </summary>
        private static readonly string[] StringArray = ["lid", "box", "locker", "desk", "cabinet", "shelf", "drawer", "bin", "tub"];

        /// <summary>
        /// The string array0.
        /// </summary>
        private static readonly string[] StringArray0 = ["room", "kitchen", "closet", "garage", "pantry", "shop"];

        /// <summary>
        /// The string array0 value.
        /// </summary>
        private static readonly string[] StringArray0Value = ["room"];

        /// <summary>
        /// The string array1.
        /// </summary>
        private static readonly string[] StringArray1 = ["room"];

        /// <summary>
        /// The string array value.
        /// </summary>
        private static readonly string[] StringArrayValue = ["room", "kitchen", "closet", "garage", "pantry", "shop"];

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
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
        /// Gets the install latest release command.
        /// </summary>
        public ICommand InstallLatestReleaseCommand => new RelayCommand(this.InstallLatestRelease);

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
        /// Gets the export database data command.
        /// </summary>
        public ICommand ExportDataCommand => new RelayCommand(this.ExportDatabaseData);

        /// <summary>
        /// Gets the import database data command.
        /// </summary>
        public ICommand ImportDataCommand => new RelayCommand(this.ImportDatabaseData);

        /// <summary>
        /// Determines whether the specified text contains keyword.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="keywords">The keywords.</param>
        /// <returns>
        ///   <c>true</c> if the specified text contains keyword; otherwise, <c>false</c>.
        /// </returns>
        internal static bool ContainsKeyword(string text, string[] keywords)
        {
            foreach (string word in keywords)
            {
                if (text != null && text.Contains(word, StringComparison.CurrentCultureIgnoreCase))
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
        internal static string? ExtractKeyword(string text, string[] keywords)
        {
            foreach (string word in keywords)
            {
                if (text != null && text.Contains(word, StringComparison.CurrentCultureIgnoreCase))
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
        /// <exception cref="Exception">No tables found in database.</exception>
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

                    bool isContainerLocation = ContainsKeyword(location, StringArray);
                    bool isRoomLocation = ContainsKeyword(location, StringArray0);

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
                            string? roomKey = ContainsKeyword(location, StringArray1)
                                ? location
                                : ExtractKeyword(location, StringArray0);
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

                        string[] parts = location.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 2)
                        {
                            string parentName = parts[1].Trim();
                            if (!string.IsNullOrEmpty(parentName) && ContainsKeyword(parentName, StringArray))
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
                                    string? roomKey = ContainsKeyword(parentName, StringArray1)
                                        ? parentName
                                        : ExtractKeyword(parentName, StringArrayValue);
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

                        string roomKey = ContainsKeyword(location, StringArray0Value)
                            ? location
                            : ExtractKeyword(location, StringArrayValue) ?? location;
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
        /// Downloads and installs the latest MSI release from GitHub, then attempts to launch the installed app.
        /// </summary>
        private void InstallLatestRelease()
        {
            _ = this.InstallLatestReleaseAsync();
        }

        /// <summary>
        /// Asynchronously downloads and installs the latest MSI release from GitHub.
        /// </summary>
        /// <returns>The task.</returns>
        private async Task InstallLatestReleaseAsync()
        {
            MessageBoxResult confirmation = MessageBox.Show(
                "This will download and run the latest Item-eyez installer (MSI) from GitHub. " +
                "You may be prompted by Windows to confirm the installation. Continue?",
                "Install Latest Release",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                using HttpClient client = new();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("item-eyez-updater/1.0");

                // Get metadata for the latest release.
                using HttpResponseMessage releaseResponse = await client.GetAsync("https://api.github.com/repos/death-scripts/item-eyez/releases/latest").ConfigureAwait(true);
                releaseResponse.EnsureSuccessStatusCode();
                string releaseJson = await releaseResponse.Content.ReadAsStringAsync().ConfigureAwait(true);

                using JsonDocument document = JsonDocument.Parse(releaseJson);
                JsonElement root = document.RootElement;
                if (!root.TryGetProperty("assets", out JsonElement assetsElement) ||
                    assetsElement.ValueKind != JsonValueKind.Array)
                {
                    throw new InvalidOperationException("Latest release does not contain any assets.");
                }

                JsonElement msiAsset = default;
                foreach (JsonElement asset in assetsElement.EnumerateArray())
                {
                    if (asset.TryGetProperty("name", out JsonElement nameElement))
                    {
                        string? name = nameElement.GetString();
                        if (!string.IsNullOrWhiteSpace(name) &&
                            name.EndsWith(".msi", StringComparison.OrdinalIgnoreCase))
                        {
                            msiAsset = asset;
                            break;
                        }
                    }
                }

                if (msiAsset.ValueKind == JsonValueKind.Undefined)
                {
                    throw new InvalidOperationException("No MSI asset was found in the latest release.");
                }

                string? downloadUrl = msiAsset.GetProperty("browser_download_url").GetString();
                string? assetName = msiAsset.GetProperty("name").GetString();
                if (string.IsNullOrWhiteSpace(downloadUrl) || string.IsNullOrWhiteSpace(assetName))
                {
                    throw new InvalidOperationException("The MSI asset in the latest release is missing download information.");
                }

                // Read checksum directly from the asset's digest field (e.g. "sha256:...").
                string? digest = msiAsset.TryGetProperty("digest", out JsonElement digestElement)
                    ? digestElement.GetString()
                    : null;

                if (string.IsNullOrWhiteSpace(digest) ||
                    !digest.StartsWith("sha256:", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("The MSI asset in the latest release does not expose a SHA-256 digest.");
                }

                string expectedChecksum = digest.Substring("sha256:".Length)
                    .Trim()
                    .ToLowerInvariant();

                string tempPath = Path.Combine(Path.GetTempPath(), assetName);

                // Download the MSI securely over HTTPS.
                using HttpResponseMessage msiResponse = await client.GetAsync(downloadUrl).ConfigureAwait(true);
                msiResponse.EnsureSuccessStatusCode();

                await using (FileStream fileStream = new(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await msiResponse.Content.CopyToAsync(fileStream).ConfigureAwait(true);
                }

                // Verify the downloaded MSI against the expected SHA-256 checksum.
                using (FileStream fs = new(tempPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashBytes = sha256.ComputeHash(fs);
                    string actualChecksum = BitConverter.ToString(hashBytes).Replace("-", string.Empty, StringComparison.Ordinal).ToLowerInvariant();

                    if (!string.Equals(actualChecksum, expectedChecksum, StringComparison.OrdinalIgnoreCase))
                    {
                        fs.Close();
                        File.Delete(tempPath);
                        throw new InvalidOperationException("Downloaded installer failed checksum verification and has been deleted.");
                    }
                }

                MessageBoxResult runInstaller = MessageBox.Show(
                    "The latest installer has been downloaded. Windows will now run the installer. " +
                    "Follow the prompts to complete the installation.",
                    "Run Installer",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Information);

                if (runInstaller != MessageBoxResult.OK)
                {
                    return;
                }

                // Use the shell so Windows can prompt for elevation and run MSI with the default handler.
                ProcessStartInfo startInfo = new()
                {
                    FileName = tempPath,
                    UseShellExecute = true,
                    Verb = "open",
                };

                _ = Process.Start(startInfo);

                // Close this instance after starting the installer so only the
                // installed application instance remains for normal use.
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(
                    $"An error occurred while installing the latest release: {ex.Message}",
                    "Install Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Attempts to launch the installed Item-eyez application after installation.
        /// </summary>
        private void TryLaunchInstalledItemEyez()
        {
            try
            {
                string? programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                string? programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

                string[] candidatePaths =
                [
                    Path.Combine(programFiles, "item-eyez", "Item-eyez.exe"),
                    Path.Combine(programFiles, "Item-eyez", "Item-eyez.exe"),
                    Path.Combine(programFilesX86, "item-eyez", "Item-eyez.exe"),
                    Path.Combine(programFilesX86, "Item-eyez", "Item-eyez.exe"),
                ];

                foreach (string candidate in candidatePaths)
                {
                    if (!string.IsNullOrWhiteSpace(candidate) && File.Exists(candidate))
                    {
                        ProcessStartInfo startInfo = new()
                        {
                            FileName = candidate,
                            UseShellExecute = true,
                        };

                        _ = Process.Start(startInfo);
                        return;
                    }
                }

                _ = MessageBox.Show(
                    "The installer has finished. If Item-eyez did not start automatically, " +
                    "please launch it from the Start menu or the installation folder.",
                    "Installation Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(
                    $"The installer completed, but Item-eyez could not be started automatically: {ex.Message}",
                    "Launch Warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Exports the current SQL database data to a JSON backup file.
        /// </summary>
        private void ExportDatabaseData()
        {
            SaveFileDialog dialog = new()
            {
                Filter = "Item-eyez backup (*.json)|*.json",
                Title = "Export database data",
                FileName = "item-eyez-backup.json",
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                ItemEyezDatabase.Instance().ExportData(dialog.FileName);
                _ = MessageBox.Show("Database export completed successfully.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Failed to export: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Imports database data from a JSON backup file.
        /// </summary>
        private void ImportDatabaseData()
        {
            OpenFileDialog dialog = new()
            {
                Filter = "Item-eyez backup (*.json)|*.json|All files (*.*)|*.*",
                Title = "Import database data",
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Importing data will replace the current database contents with the contents of the selected file. This cannot be undone. Do you want to continue?",
                "Confirm Import",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                ItemEyezDatabase.Instance().ImportData(dialog.FileName, resetExisting: true);
                _ = MessageBox.Show("Database import completed successfully.", "Import", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Failed to import backup: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
