using Microsoft.Data.SqlClient;
using System.IO;

namespace item_eyez
{
    public class DatabaseHelper
    {
        private readonly string _serverConnectionString;
        private readonly string _databaseName;

        public static DatabaseHelper Instance { get; internal set; }

        public DatabaseHelper(string serverConnectionString, string databaseName)
        {
            _serverConnectionString = serverConnectionString;
            _databaseName = databaseName;
        }

        public void DeleteDatabase()
        {
            using (var connection = new SqlConnection(_serverConnectionString))
            {
                connection.Open();
                var query = $@"IF DB_ID('{_databaseName}') IS NOT NULL
                           BEGIN
                               ALTER DATABASE [{_databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                               DROP DATABASE [{_databaseName}];
                           END";
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void CreateDatabase()
        {
            using (var connection = new SqlConnection(_serverConnectionString))
            {
                connection.Open();

                // Create the database
                var createDbQuery = $@"CREATE DATABASE [{_databaseName}];";
                using (var command = new SqlCommand(createDbQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Switch to the new database
                connection.ChangeDatabase(_databaseName);

                // Run the database schema creation script
                var schemaScript = File.ReadAllText("create.sql"); // Ensure `create.sql` is in the executing directory
                foreach (var batch in schemaScript.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    using (var command = new SqlCommand(batch, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }

}
