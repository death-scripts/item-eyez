using Microsoft.Data.SqlClient;
using System.Data;

namespace item_eyez
{
    public class ItemEyezDatabase
    {
        private static ItemEyezDatabase _instance;
        private static readonly object _lock = new object();
        private static string _connectionString;
        public ItemEyezDatabase(string connectionString)
        {
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
        }
        public void AddContainer(string name, string description)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("AddContainer", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@containerName", name);
                command.Parameters.AddWithValue("@containerDescription", description);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }


        public void AddItem(string itemName, string itemDescription, decimal itemValue, string itemCategories)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("AddItem", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@itemName", itemName);
                command.Parameters.AddWithValue("@itemDescription", itemDescription);
                command.Parameters.AddWithValue("@itemValue", itemValue);
                command.Parameters.AddWithValue("@itemCategories", itemCategories);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void AssociateItemWithContainer(Guid itemId, int containerId)
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
        }

        public void AssociateItemWithRoom(Guid itemId, int roomId)
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
    }
}
