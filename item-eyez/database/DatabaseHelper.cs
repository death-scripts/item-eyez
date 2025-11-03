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
using System.IO;
using Microsoft.Data.SqlClient;

namespace Item_eyez.Database
{
    /// <summary>
    /// The database helper.
    /// </summary>
    public class DatabaseHelper
    {
        /// <summary>
        /// The separator.
        /// </summary>
        private static readonly string[] Separator = ["GO"];

        /// <summary>
        /// The database name.
        /// </summary>
        private readonly string databaseName;

        /// <summary>
        /// The server connection string.
        /// </summary>
        private readonly string serverConnectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseHelper" /> class.
        /// </summary>
        /// <param name="serverConnectionString">The server connection string.</param>
        /// <param name="databaseName">Name of the database.</param>
        public DatabaseHelper(string serverConnectionString, string databaseName)
        {
            this.serverConnectionString = serverConnectionString;
            this.databaseName = databaseName;
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static DatabaseHelper Instance { get; internal set; }

        /// <summary>
        /// Creates the database.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public static void CreateDatabase(string connectionString)
        {
            SqlConnectionStringBuilder builder = new(connectionString);
            string databaseName = builder.InitialCatalog;
            string serverConnectionString = $"Server={builder.DataSource};Integrated Security=true;TrustServerCertificate=True;";

            // First, try to delete the database if it exists
            try
            {
                DeleteDatabase(connectionString);
                System.Threading.Thread.Sleep(1000); // Add a small delay
            }
            catch (SqlException ex)
            {
                // Log the exception or handle it if necessary, but don't rethrow if it's just that the DB didn't exist
                // For now, we'll just ignore it if the database didn't exist
                if (!ex.Message.Contains("Cannot drop the database") && !ex.Message.Contains("does not exist"))
                {
                    throw;
                }
            }

            using SqlConnection connection = new(serverConnectionString);
            connection.Open();

            // Create the database
            string createDbQuery = $@"CREATE DATABASE [{databaseName}];";
            using (SqlCommand command = new(createDbQuery, connection))
            {
                _ = command.ExecuteNonQuery();
            }

            // Switch to the new database
            connection.ChangeDatabase(databaseName);

            // Run the database schema creation script
            string schemaScript = File.ReadAllText("create.sql"); // Ensure `create.sql` is in the executing directory
            foreach (string batch in schemaScript.Split(Separator, System.StringSplitOptions.RemoveEmptyEntries))
            {
                using SqlCommand command = new(batch, connection);
                _ = command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes the database.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public static void DeleteDatabase(string connectionString)
        {
            SqlConnectionStringBuilder builder = new(connectionString);
            string databaseName = builder.InitialCatalog;
            string serverConnectionString = $"Server={builder.DataSource};Integrated Security=true;TrustServerCertificate=True;";

            // Retry up to 5 times
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    using SqlConnection connection = new(serverConnectionString);
                    connection.Open();

                    // Kill all connections to the database
                    string killConnectionsQuery = $@"
                        USE master;
                        ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                        ALTER DATABASE [{databaseName}] SET MULTI_USER;
                        ALTER DATABASE [{databaseName}] SET OFFLINE WITH ROLLBACK IMMEDIATE;
                        ALTER DATABASE [{databaseName}] SET ONLINE;
                        DROP DATABASE [{databaseName}];";

                    using (SqlCommand command = new(killConnectionsQuery, connection))
                    {
                        _ = command.ExecuteNonQuery();
                    }

                    System.Threading.Thread.Sleep(1000); // Add a small delay
                    return; // Success, exit loop
                }
                catch (SqlException ex)
                {
                    // If the database doesn't exist, that's fine.
                    if (ex.Message.Contains("Cannot drop the database") && ex.Message.Contains("does not exist"))
                    {
                        return;
                    }

                    // Log the exception for debugging
                    System.Diagnostics.Debug.WriteLine($"Attempt {i + 1} to delete database failed: {ex.Message}");
                    System.Threading.Thread.Sleep(2000); // Wait longer before retrying
                }
            }

            throw new Exception($"Failed to delete database {databaseName} after multiple attempts.");
        }

        /// <summary>
        /// Creates the database.
        /// </summary>
        public void CreateDatabase() => CreateDatabase($"Server={this.serverConnectionString};Database={this.databaseName};Integrated Security=true;TrustServerCertificate=True;");

        /// <summary>
        /// Deletes the database.
        /// </summary>
        public void DeleteDatabase() => DeleteDatabase($"Server={this.serverConnectionString};Database={this.databaseName};Integrated Security=true;TrustServerCertificate=True;");
    }
}