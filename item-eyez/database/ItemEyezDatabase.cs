using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data;

namespace item_eyez
{
    public class ItemEyezDatabase
    {
        public delegate void DataChangedEventHandler(object sender, EventArgs e);
        public event DataChangedEventHandler? DataChanged;

        private bool _suppressNotifications;

        public virtual void OnDataChanged()
        {
            if (!_suppressNotifications)
                DataChanged?.Invoke(this, EventArgs.Empty);
        }

        public void BeginBatch() => _suppressNotifications = true;
        public void EndBatch()
        {
            _suppressNotifications = false;
            OnDataChanged();
        }
        private static ItemEyezDatabase _instance;
        private static readonly object _lock = new object();
        private static string _connectionString;
        public ItemEyezDatabase(string connectionString)
        {
        }
        public static ObservableCollection<Room> Rooms
        {
            get
            {
                return Instance().GetRoomsList();
            }
        }

        public static ObservableCollection<Container> Containers
        {
            get
            {
                return Instance().GetContainersWithRelationships();
            }
        }
        public static ItemEyezDatabase Instance(string connectionString = "Server=localhost\\SQLEXPRESS;Database=ITEMEYEZ;Integrated Security=true;TrustServerCertificate=True;")
        {
            _connectionString = connectionString;
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null && connectionString != null)
                    {
                        _instance = new ItemEyezDatabase(connectionString);
                    }
                }
            }

            return _instance;
        }

        public void AddRoom(string name, string description)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("AddRoom", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@roomName", name);
                command.Parameters.AddWithValue("@roomDescription", description);

                connection.Open();
                command.ExecuteNonQuery();
            }
            OnDataChanged();
        }

        public Guid AddContainer(string name, string description)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("AddContainer", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Input parameters
                command.Parameters.AddWithValue("@containerName", name);
                command.Parameters.AddWithValue("@containerDescription", description);

                // Output parameter for the ID
                var idParam = new SqlParameter("@containerId", SqlDbType.UniqueIdentifier)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(idParam);

                connection.Open();
                command.ExecuteNonQuery();

                OnDataChanged();
                // Return the generated ID
                return (Guid)idParam.Value;
            }
        }


        public Guid AddItem(string name, string description, decimal value, string categories)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("AddItem", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    command.Parameters.AddWithValue("@itemName", name);
                    command.Parameters.AddWithValue("@itemDescription", description);
                    command.Parameters.AddWithValue("@itemValue", value);
                    command.Parameters.AddWithValue("@itemCategories", categories);

                    // Output parameter for the ID
                    var idParam = new SqlParameter("@itemId", SqlDbType.UniqueIdentifier)
                    {
                        Direction = ParameterDirection.Output
                    };

                    command.Parameters.Add(idParam);

                    command.ExecuteNonQuery();

                    OnDataChanged();
                    // Return the generated ID
                    return (Guid)idParam.Value;
                }
            }
        }

        public void UpdateContainer(Guid containerId, string newName, string newDescription)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("UpdateContainer", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@containerId", containerId);
                command.Parameters.AddWithValue("@newName", newName);
                command.Parameters.AddWithValue("@newDescription", newDescription);

                connection.Open();
                command.ExecuteNonQuery();
            }
            OnDataChanged();
        }

        public void UpdateRoom(Guid roomId, string newName, string newDescription)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("UpdateRoom", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@roomId", roomId);
                command.Parameters.AddWithValue("@newName", newName);
                command.Parameters.AddWithValue("@newDescription", newDescription);

                connection.Open();
                command.ExecuteNonQuery();
            }
            OnDataChanged();
        }

        public void UpdateItem(Guid itemId, string newName, string newDescription, decimal newValue, string newCategories)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("UpdateItem", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@itemId", itemId);
                command.Parameters.AddWithValue("@newName", newName);
                command.Parameters.AddWithValue("@newDescription", newDescription);
                command.Parameters.AddWithValue("@newValue", newValue);
                command.Parameters.AddWithValue("@newCategories", newCategories);

                connection.Open();
                command.ExecuteNonQuery();
            }
            OnDataChanged();
        }

        public void DeleteContainer(Guid roomId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("DeleteContainer", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@containerId", roomId);

                connection.Open();
                command.ExecuteNonQuery();
            }
            OnDataChanged();
        }

        public void DeleteRoom(Guid roomId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("DeleteRoom", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@roomId", roomId);

                connection.Open();
                command.ExecuteNonQuery();
            }
            OnDataChanged();
        }

        public void DeleteItem(Guid itemId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("DeleteItem", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@itemId", itemId);

                connection.Open();
                command.ExecuteNonQuery();
            }
            OnDataChanged();
        }

        public DataTable GetRooms()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM room";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
            OnDataChanged();
        }

        public DataTable GetContainers()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM container";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
            OnDataChanged();
        }

        public DataTable GetItems()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM item";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        public void AssociateItemWithContainer(Guid itemId, Guid containerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("AssociateItemWithContainer", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@itemId", itemId);
                command.Parameters.AddWithValue("@containerId", containerId);

                connection.Open();
                command.ExecuteNonQuery();
            }
            OnDataChanged();
        }

        public void AssociateItemWithRoom(Guid itemId, Guid roomId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("AssociateItemWithRoom", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@itemId", itemId);
                command.Parameters.AddWithValue("@roomId", roomId);

                connection.Open();
                command.ExecuteNonQuery();
            }
            OnDataChanged();
        }

        public Room GetItemsRoom(Guid itemId)
        {
            Container topContainer;
            Room room;

            ResolveContainerAndRoomForItem(itemId, out topContainer, out room);

            return room;
        }

        public Container GetItemsContainer(Guid itemId)
        {
            var containerId = Instance().GetContainerIdForEntity(itemId);

            if (containerId.HasValue)
            {
                // Fetch the container details from the database
                var containerRow = ItemEyezDatabase.Instance().GetContainers()
                    .AsEnumerable()
                    .FirstOrDefault(row => row.Field<Guid>("id") == containerId.Value);

                if (containerRow != null)
                {
                    var container = new Container
                    (
                        containerRow.Field<Guid>("id"),
                        containerRow.Field<string>("name"),
                        containerRow.Field<string>("description")
                    );

                    return container;
                }
            }

            // Return null if no container is found
            return null;
        }
        public void SetItemsRoom(Guid itemId, Guid? roomId)
        {
            ItemEyezDatabase.Instance().UnassociateItemFromRoom(itemId);
            if (roomId != null)
            {
                ItemEyezDatabase.Instance().AssociateItemWithRoom(itemId, (Guid)roomId);
            }
            OnDataChanged();
        }
        public void SetItemsContainer(Guid itemId, Guid? containerId)
        {
            ItemEyezDatabase.Instance().UnassociateItemFromContainer(itemId);
            // Update the database to reflect the new container association
            if (containerId != null)
            {
                ItemEyezDatabase.Instance().AssociateItemWithContainer(itemId, (Guid)containerId);
            }
            OnDataChanged();
        }

        public ObservableCollection<Item> GetItemsWithRelationships()
        {
            var items = new ObservableCollection<Item>();
            var itemsTable = GetItems();

            foreach (DataRow row in itemsTable.Rows)
            {
                var item = new Item
                (
                    row.Field<Guid>("id"),
                    row.Field<string>("name"),
                    row.Field<string>("description"),
                    row.Field<decimal>("value"),
                    row.Field<string>("categories")
                );

                // Use stored procedures to determine relationships
                Guid itemId = item.Id;
                items.Add(item);
            }

            return items;
        }

        public ObservableCollection<Container> GetContainersWithRelationships()
        {
            var containers = new ObservableCollection<Container>();
            var containersTable = GetContainers();

            foreach (DataRow row in containersTable.Rows)
            {
                var container = new Container
                (
                    row.Field<Guid>("id"),
                    row.Field<string>("name"),
                    row.Field<string>("description")
                );

                // Use stored procedure to determine the room the container is stored in
                Guid containerId = container.Id;

                containers.Add(container);
            }

            return containers;
        }
        private void ResolveContainerAndRoomForItem(Guid itemId, out Container topContainer, out Room room)
        {
            topContainer = null;
            room = null;

            // Check if the item is directly in a room
            var roomId = GetRoomIdForEntity(itemId);
            if (roomId.HasValue)
            {
                var roomRow = GetRooms().AsEnumerable()
                    .FirstOrDefault(r => r.Field<Guid>("id") == roomId.Value);

                if (roomRow != null)
                {
                    room = new Room
                    (
                        roomRow.Field<Guid>("id"),
                        roomRow.Field<string>("name"),
                        roomRow.Field<string>("description")
                    );
                }

                // If the item is directly in a room, no need to resolve containers
                return;
            }

            // Check if the item is in a container
            var containerId = GetContainerIdForEntity(itemId);
            if (!containerId.HasValue)
            {
                // Item is neither in a container nor a room
                return;
            }

            // Resolve the container chain
            var currentContainerId = containerId.Value;
            while (true)
            {
                // Get the container information
                var containerRow = GetContainers().AsEnumerable()
                    .FirstOrDefault(r => r.Field<Guid>("id") == currentContainerId);

                if (containerRow == null)
                {
                    break; // Stop if the container does not exist
                }

                // Create the container object
                var currentContainer = new Container
                (
                    containerRow.Field<Guid>("id"),
                    containerRow.Field<string>("name"),
                    containerRow.Field<string>("description")
                );

                // Check if the container is contained in another container
                var parentContainerId = GetContainerIdForEntity(currentContainerId);

                if (parentContainerId.HasValue)
                {
                    // Update the current container and continue to check the parent
                    currentContainerId = parentContainerId.Value;
                }
                else
                {
                    // No parent container, so this is the top-level container
                    topContainer = currentContainer;

                    // Check which room this top-level container is stored in
                    var topContainerRoomId = GetRoomIdForEntity(currentContainerId);
                    if (topContainerRoomId.HasValue)
                    {
                        var topContainerRoomRow = GetRooms().AsEnumerable()
                            .FirstOrDefault(r => r.Field<Guid>("id") == topContainerRoomId.Value);

                        if (topContainerRoomRow != null)
                        {
                            room = new Room
                            (
                                topContainerRoomRow.Field<Guid>("id"),
                                topContainerRoomRow.Field<string>("name"),
                                topContainerRoomRow.Field<string>("description")
                            );
                        }
                    }

                    break; // Stop recursion
                }
            }
        }
        public void UnassociateItemFromRoom(Guid itemId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("UnassociateItemFromRoom", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Add input parameters
                command.Parameters.AddWithValue("@itemId", itemId);

                connection.Open();
                command.ExecuteNonQuery();
            }
            OnDataChanged();
        }

        public void UnassociateItemFromContainer(Guid itemId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("UnassociateItemFromContainer", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Add input parameters
                command.Parameters.AddWithValue("@itemId", itemId);

                connection.Open();
                command.ExecuteNonQuery();
            }
            OnDataChanged();
        }

        public ObservableCollection<Room> GetRoomsList()
        {
            var rooms = new ObservableCollection<Room>();
            var roomsTable = GetRooms();

            foreach (DataRow row in roomsTable.Rows)
            {
                rooms.Add(new Room
                (
                    row.Field<Guid>("id"),
                    row.Field<string>("name"),
                    row.Field<string>("description")
                ));
            }

            return rooms;
        }
        public Guid? GetRoomIdForEntity(Guid entityId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("GetRoomIdForItem", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@itemId", entityId);

                var roomIdParam = new SqlParameter("@roomId", SqlDbType.UniqueIdentifier)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(roomIdParam);

                connection.Open();
                command.ExecuteNonQuery();

                return roomIdParam.Value != DBNull.Value ? (Guid?)roomIdParam.Value : null;
            }
        }
        public Guid? GetContainerIdForEntity(Guid entityId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("GetContainerIdForItem", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@itemId", entityId);

                var containerIdParam = new SqlParameter("@containerId", SqlDbType.UniqueIdentifier)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(containerIdParam);

                connection.Open();
                command.ExecuteNonQuery();

                return containerIdParam.Value != DBNull.Value ? (Guid?)containerIdParam.Value : null;
            }
        }
    }
}
