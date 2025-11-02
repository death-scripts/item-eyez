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
using System.Collections.ObjectModel;
using System.Data;
using Item_eyez.Viewmodels;
using Microsoft.Data.SqlClient;

namespace Item_eyez.Database
{
    /// <summary>
    /// The item eyez database.
    /// </summary>
    public class ItemEyezDatabase
    {
        /// <summary>
        /// The lock.
        /// </summary>
        private static readonly object Lock = new();

        /// <summary>
        /// The connection string.
        /// </summary>
        private static string? connectionString;

        /// <summary>
        /// The instance.
        /// </summary>
        private static ItemEyezDatabase? instance;

        /// <summary>
        /// The suppress notifications.
        /// </summary>
        private bool suppressNotifications;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemEyezDatabase"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public ItemEyezDatabase(string connectionString)
        {
        }

        /// <summary>
        /// The data changed event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public delegate void DataChangedEventHandler(object sender, EventArgs e);

        /// <summary>
        /// Occurs when [data changed].
        /// </summary>
        public event DataChangedEventHandler? DataChanged;

        /// <summary>
        /// Gets the containers.
        /// </summary>
        /// <value>
        /// The containers.
        /// </value>
        public static ObservableCollection<Container> Containers => Instance().GetContainersWithRelationships();

        /// <summary>
        /// Gets the rooms.
        /// </summary>
        /// <value>
        /// The rooms.
        /// </value>
        public static ObservableCollection<Room> Rooms => Instance().GetRoomsList();

        /// <summary>
        /// Instances the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>
        /// The item eyez database.
        /// </returns>
        public static ItemEyezDatabase Instance(string connectionString = "Server=localhost\\SQLEXPRESS;Database=ITEMEYEZ;Integrated Security=true;TrustServerCertificate=True;")
        {
            ItemEyezDatabase.connectionString = connectionString;
            if (instance == null)
            {
                lock (Lock)
                {
                    if (instance == null && connectionString != null)
                    {
                        instance = new ItemEyezDatabase(connectionString);
                    }
                }
            }

            return instance ?? throw new Exception("database is null");
        }

        /// <summary>
        /// Adds the container.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <returns>
        /// The unique identifier.
        /// </returns>
        public Guid AddContainer(string name, string description)
        {
            using SqlConnection connection = new(connectionString);
            using SqlCommand command = new("AddContainer", connection);
            command.CommandType = CommandType.StoredProcedure;

            // Input parameters
            _ = command.Parameters.AddWithValue("@containerName", name);
            _ = command.Parameters.AddWithValue("@containerDescription", description);

            // Output parameter for the ID
            SqlParameter idParam = new("@containerId", SqlDbType.UniqueIdentifier)
            {
                Direction = ParameterDirection.Output,
            };
            _ = command.Parameters.Add(idParam);

            connection.Open();
            _ = command.ExecuteNonQuery();

            this.OnDataChanged();

            // Return the generated ID
            return (Guid)idParam.Value;
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="value">The value.</param>
        /// <param name="categories">The categories.</param>
        /// <returns>
        /// The unique identifier.
        /// </returns>
        public Guid AddItem(string name, string description, decimal value, string categories)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();
            using SqlCommand command = new("AddItem", connection);
            command.CommandType = CommandType.StoredProcedure;

            // Input parameters
            _ = command.Parameters.AddWithValue("@itemName", name);
            _ = command.Parameters.AddWithValue("@itemDescription", description);
            _ = command.Parameters.AddWithValue("@itemValue", value);
            _ = command.Parameters.AddWithValue("@itemCategories", categories);

            // Output parameter for the ID
            SqlParameter idParam = new("@itemId", SqlDbType.UniqueIdentifier)
            {
                Direction = ParameterDirection.Output,
            };

            _ = command.Parameters.Add(idParam);

            _ = command.ExecuteNonQuery();

            this.OnDataChanged();

            // Return the generated ID
            return (Guid)idParam.Value;
        }

        /// <summary>
        /// Adds the room.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        public void AddRoom(string name, string description)
        {
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand command = new("AddRoom", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                _ = command.Parameters.AddWithValue("@roomName", name);
                _ = command.Parameters.AddWithValue("@roomDescription", description);

                connection.Open();
                _ = command.ExecuteNonQuery();
            }

            this.OnDataChanged();
        }

        /// <summary>
        /// Associates the item with container.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="containerId">The container identifier.</param>
        public void AssociateItemWithContainer(Guid itemId, Guid containerId)
        {
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand command = new("AssociateItemWithContainer", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                _ = command.Parameters.AddWithValue("@itemId", itemId);
                _ = command.Parameters.AddWithValue("@containerId", containerId);

                connection.Open();
                _ = command.ExecuteNonQuery();
            }

            this.OnDataChanged();
        }

        /// <summary>
        /// Associates the item with room.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="roomId">The room identifier.</param>
        public void AssociateItemWithRoom(Guid itemId, Guid roomId)
        {
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand command = new("AssociateItemWithRoom", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                _ = command.Parameters.AddWithValue("@itemId", itemId);
                _ = command.Parameters.AddWithValue("@roomId", roomId);

                connection.Open();
                _ = command.ExecuteNonQuery();
            }

            this.OnDataChanged();
        }

        /// <summary>
        /// Begins the batch.
        /// </summary>
        public void BeginBatch() => this.suppressNotifications = true;

        /// <summary>
        /// Deletes the container.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        public void DeleteContainer(Guid roomId)
        {
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand command = new("DeleteContainer", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                _ = command.Parameters.AddWithValue("@containerId", roomId);

                connection.Open();
                _ = command.ExecuteNonQuery();
            }

            this.OnDataChanged();
        }

        /// <summary>
        /// Deletes the item.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        public void DeleteItem(Guid itemId)
        {
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand command = new("DeleteItem", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                _ = command.Parameters.AddWithValue("@itemId", itemId);

                connection.Open();
                _ = command.ExecuteNonQuery();
            }

            this.OnDataChanged();
        }

        /// <summary>
        /// Deletes the room.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        public void DeleteRoom(Guid roomId)
        {
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand command = new("DeleteRoom", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                _ = command.Parameters.AddWithValue("@roomId", roomId);

                connection.Open();
                _ = command.ExecuteNonQuery();
            }

            this.OnDataChanged();
        }

        /// <summary>
        /// Ends the batch.
        /// </summary>
        public void EndBatch()
        {
            this.suppressNotifications = false;
            this.OnDataChanged();
        }

        /// <summary>
        /// Gets the container identifier for entity.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <returns>
        /// The nullable.
        /// </returns>
        public Guid? GetContainerIdForEntity(Guid entityId)
        {
            using SqlConnection connection = new(connectionString);
            using SqlCommand command = new("GetContainerIdForItem", connection);
            command.CommandType = CommandType.StoredProcedure;

            _ = command.Parameters.AddWithValue("@itemId", entityId);

            SqlParameter containerIdParam = new("@containerId", SqlDbType.UniqueIdentifier)
            {
                Direction = ParameterDirection.Output,
            };
            _ = command.Parameters.Add(containerIdParam);

            connection.Open();
            _ = command.ExecuteNonQuery();

            return containerIdParam.Value != DBNull.Value ? (Guid?)containerIdParam.Value : null;
        }

        /// <summary>
        /// Gets the containers.
        /// </summary>
        /// <returns>
        /// The data table.
        /// </returns>
        public DataTable GetContainers()
        {
            using SqlConnection connection = new(connectionString);
            string query = "SELECT * FROM container";
            SqlDataAdapter adapter = new(query, connection);
            DataTable table = new();
            _ = adapter.Fill(table);
            return table;
        }

        /// <summary>
        /// Gets the containers with relationships.
        /// </summary>
        /// <returns>
        /// The observable collection.
        /// </returns>
        public ObservableCollection<Container> GetContainersWithRelationships()
        {
            ObservableCollection<Container> containers = [];
            DataTable containersTable = this.GetContainers();

            foreach (DataRow row in containersTable.Rows)
            {
                Container container = new(
                    row.Field<Guid>("id"),
                    row.Field<string>("name")!,
                    row.Field<string>("description")!);

                // Use stored procedure to determine the room the container is stored in
                _ = container.Id;

                containers.Add(container);
            }

            return containers;
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <returns>
        /// The data table.
        /// </returns>
        public DataTable GetItems()
        {
            using SqlConnection connection = new(connectionString);
            string query = "SELECT * FROM item";
            SqlDataAdapter adapter = new(query, connection);
            DataTable table = new();
            _ = adapter.Fill(table);
            return table;
        }

        /// <summary>
        /// Gets the items container.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <returns>
        /// The container.
        /// </returns>
        public Container GetItemsContainer(Guid itemId)
        {
            Guid? containerId = Instance().GetContainerIdForEntity(itemId);

            if (containerId.HasValue)
            {
                // Fetch the container details from the database
                DataRow? containerRow = Instance().GetContainers()
                    .AsEnumerable()
                    .FirstOrDefault(row => row.Field<Guid>("id") == containerId.Value);

                if (containerRow != null)
                {
                    Container container = new(
                        containerRow.Field<Guid>("id"),
                        containerRow.Field<string>("name")!,
                        containerRow.Field<string>("description")!);

                    return container;
                }
            }

            // Return null if no container is found
            return null;
        }

        /// <summary>
        /// Gets the items room.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <returns>
        /// The room.
        /// </returns>
        public Room GetItemsRoom(Guid itemId)
        {
            this.ResolveContainerAndRoomForItem(itemId, out _, out Room? room);

            return room;
        }

        /// <summary>
        /// Gets the items with relationships.
        /// </summary>
        /// <returns>
        /// The observable collection.
        /// </returns>
        public ObservableCollection<Item> GetItemsWithRelationships()
        {
            ObservableCollection<Item> items = [];
            DataTable itemsTable = this.GetItems();

            foreach (DataRow row in itemsTable.Rows)
            {
                Item item = new(
                    row.Field<Guid>("id"),
                    row.Field<string>("name")!,
                    row.Field<string>("description")!,
                    row.Field<decimal>("value")!,
                    row.Field<string>("categories")!);

                // Use stored procedures to determine relationships
                _ = item.Id;
                items.Add(item);
            }

            return items;
        }

        /// <summary>
        /// Gets the room identifier for entity.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <returns>
        /// The nullable.
        /// </returns>
        public Guid? GetRoomIdForEntity(Guid entityId)
        {
            using SqlConnection connection = new(connectionString);
            using SqlCommand command = new("GetRoomIdForItem", connection);
            command.CommandType = CommandType.StoredProcedure;

            _ = command.Parameters.AddWithValue("@itemId", entityId);

            SqlParameter roomIdParam = new("@roomId", SqlDbType.UniqueIdentifier)
            {
                Direction = ParameterDirection.Output,
            };
            _ = command.Parameters.Add(roomIdParam);

            connection.Open();
            _ = command.ExecuteNonQuery();

            return roomIdParam.Value != DBNull.Value ? (Guid?)roomIdParam.Value : null;
        }

        /// <summary>
        /// Gets the rooms.
        /// </summary>
        /// <returns>
        /// The data table.
        /// </returns>
        public DataTable GetRooms()
        {
            using SqlConnection connection = new(connectionString);
            string query = "SELECT * FROM room";
            SqlDataAdapter adapter = new(query, connection);
            DataTable table = new();
            _ = adapter.Fill(table);
            return table;
        }

        /// <summary>
        /// Gets the rooms list.
        /// </summary>
        /// <returns>
        /// The observable collection.
        /// </returns>
        public ObservableCollection<Room> GetRoomsList()
        {
            ObservableCollection<Room> rooms = [];
            DataTable roomsTable = this.GetRooms();

            foreach (DataRow row in roomsTable.Rows)
            {
                rooms.Add(new Room(
                    row.Field<Guid>("id"),
                    row.Field<string>("name")!,
                    row.Field<string>("description")!));
            }

            return rooms;
        }

        /// <summary>
        /// Called when [data changed].
        /// </summary>
        public virtual void OnDataChanged()
        {
            if (!this.suppressNotifications)
            {
                this.DataChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sets the items container.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="containerId">The container identifier.</param>
        public void SetItemsContainer(Guid itemId, Guid? containerId)
        {
            Instance().UnassociateItemFromContainer(itemId);

            // Update the database to reflect the new container association
            if (containerId != null)
            {
                Instance().AssociateItemWithContainer(itemId, (Guid)containerId);
            }

            this.OnDataChanged();
        }

        /// <summary>
        /// Sets the items room.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="roomId">The room identifier.</param>
        public void SetItemsRoom(Guid itemId, Guid? roomId)
        {
            Instance().UnassociateItemFromRoom(itemId);
            if (roomId != null)
            {
                Instance().AssociateItemWithRoom(itemId, (Guid)roomId);
            }

            this.OnDataChanged();
        }

        /// <summary>
        /// Unassociates the item from container.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        public void UnassociateItemFromContainer(Guid itemId)
        {
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand command = new("UnassociateItemFromContainer", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Add input parameters
                _ = command.Parameters.AddWithValue("@itemId", itemId);

                connection.Open();
                _ = command.ExecuteNonQuery();
            }

            this.OnDataChanged();
        }

        /// <summary>
        /// Unassociates the item from room.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        public void UnassociateItemFromRoom(Guid itemId)
        {
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand command = new("UnassociateItemFromRoom", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Add input parameters
                _ = command.Parameters.AddWithValue("@itemId", itemId);

                connection.Open();
                _ = command.ExecuteNonQuery();
            }

            this.OnDataChanged();
        }

        /// <summary>
        /// Updates the container.
        /// </summary>
        /// <param name="containerId">The container identifier.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="newDescription">The new description.</param>
        public void UpdateContainer(Guid containerId, string newName, string newDescription)
        {
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand command = new("UpdateContainer", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                _ = command.Parameters.AddWithValue("@containerId", containerId);
                _ = command.Parameters.AddWithValue("@newName", newName);
                _ = command.Parameters.AddWithValue("@newDescription", newDescription);

                connection.Open();
                _ = command.ExecuteNonQuery();
            }

            this.OnDataChanged();
        }

        /// <summary>
        /// Updates the item.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="newDescription">The new description.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="newCategories">The new categories.</param>
        public void UpdateItem(Guid itemId, string newName, string newDescription, decimal newValue, string newCategories)
        {
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand command = new("UpdateItem", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                _ = command.Parameters.AddWithValue("@itemId", itemId);
                _ = command.Parameters.AddWithValue("@newName", newName);
                _ = command.Parameters.AddWithValue("@newDescription", newDescription);
                _ = command.Parameters.AddWithValue("@newValue", newValue);
                _ = command.Parameters.AddWithValue("@newCategories", newCategories);

                connection.Open();
                _ = command.ExecuteNonQuery();
            }

            this.OnDataChanged();
        }

        /// <summary>
        /// Updates the room.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="newDescription">The new description.</param>
        public void UpdateRoom(Guid roomId, string newName, string newDescription)
        {
            using (SqlConnection connection = new(connectionString))
            using (SqlCommand command = new("UpdateRoom", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                _ = command.Parameters.AddWithValue("@roomId", roomId);
                _ = command.Parameters.AddWithValue("@newName", newName);
                _ = command.Parameters.AddWithValue("@newDescription", newDescription);

                connection.Open();
                _ = command.ExecuteNonQuery();
            }

            this.OnDataChanged();
        }

        /// <summary>
        /// Resolves the container and room for item.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="topContainer">The top container.</param>
        /// <param name="room">The room.</param>
        private void ResolveContainerAndRoomForItem(Guid itemId, out Container? topContainer, out Room? room)
        {
            topContainer = null;
            room = null;

            // Check if the item is directly in a room
            Guid? roomId = this.GetRoomIdForEntity(itemId);
            if (roomId.HasValue)
            {
                DataRow? roomRow = this.GetRooms().AsEnumerable()
                    .FirstOrDefault(r => r.Field<Guid>("id") == roomId.Value);

                if (roomRow != null)
                {
                    room = new Room(
                        roomRow.Field<Guid>("id"),
                        roomRow.Field<string>("name")!,
                        roomRow.Field<string>("description")!);
                }

                // If the item is directly in a room, no need to resolve containers
                return;
            }

            // Check if the item is in a container
            Guid? containerId = this.GetContainerIdForEntity(itemId);
            if (!containerId.HasValue)
            {
                // Item is neither in a container nor a room
                return;
            }

            // Resolve the container chain
            Guid currentContainerId = containerId.Value;
            while (true)
            {
                // Get the container information
                DataRow? containerRow = this.GetContainers().AsEnumerable()
                    .FirstOrDefault(r => r.Field<Guid>("id") == currentContainerId);

                if (containerRow == null)
                {
                    break; // Stop if the container does not exist
                }

                // Create the container object
                Container currentContainer = new(
                    containerRow.Field<Guid>("id"),
                    containerRow.Field<string>("name")!,
                    containerRow.Field<string>("description")!);

                // Check if the container is contained in another container
                Guid? parentContainerId = this.GetContainerIdForEntity(currentContainerId);

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
                    Guid? topContainerRoomId = this.GetRoomIdForEntity(currentContainerId);
                    if (topContainerRoomId.HasValue)
                    {
                        DataRow? topContainerRoomRow = this.GetRooms().AsEnumerable()
                            .FirstOrDefault(r => r.Field<Guid>("id") == topContainerRoomId.Value);

                        if (topContainerRoomRow != null)
                        {
                            room = new Room(
                                topContainerRoomRow.Field<Guid>("id"),
                                topContainerRoomRow.Field<string>("name")!,
                                topContainerRoomRow.Field<string>("description")!);
                        }
                    }

                    break; // Stop recursion
                }
            }
        }
    }
}