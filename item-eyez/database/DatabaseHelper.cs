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
        public void CreateDatabase()
        {
            using SqlConnection connection = new(this.serverConnectionString);
            connection.Open();

            // Create the database
            string createDbQuery = $@"CREATE DATABASE [{this.databaseName}];";
            using (SqlCommand command = new(createDbQuery, connection))
            {
                _ = command.ExecuteNonQuery();
            }

            // Switch to the new database
            connection.ChangeDatabase(this.databaseName);

            // Run the database schema creation script
            string schemaScript = File.ReadAllText("create.sql"); // Ensure `create.sql` is in the executing directory
            foreach (string batch in schemaScript.Split(Separator, StringSplitOptions.RemoveEmptyEntries))
            {
                using SqlCommand command = new(batch, connection);
                _ = command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes the database.
        /// </summary>
        public void DeleteDatabase()
        {
            using SqlConnection connection = new(this.serverConnectionString);
            connection.Open();
            string query = $@"IF DB_ID('{this.databaseName}') IS NOT NULL
                           BEGIN
                               ALTER DATABASE [{this.databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                               DROP DATABASE [{this.databaseName}];
                           END";
            using SqlCommand command = new(query, connection);
            _ = command.ExecuteNonQuery();
        }
    }
}