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
namespace Item_eyez.Database
{
    using Microsoft.Data.SqlClient;

    /// <summary>
    /// The database initializer.
    /// </summary>
    public class DatabaseInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseInitializer"/> class.
        /// </summary>
        public DatabaseInitializer()
        {
        }

        /// <summary>
        /// Initializes the database.
        /// </summary>
        /// <param name="serverConnectionString">The server connection string.</param>
        /// <param name="databaseName">Name of the database.</param>
        public static void InitializeDatabase(string serverConnectionString, string databaseName)
        {
            using SqlConnection connection = new(serverConnectionString);
            connection.Open();

            DatabaseHelper.Instance = new DatabaseHelper(serverConnectionString, databaseName);

            // Check if the database exists
            SqlCommand checkDbCommand = new($"IF DB_ID('{databaseName}') IS NOT NULL SELECT 1 ELSE SELECT 0;", connection);
            bool databaseExists = (int)checkDbCommand.ExecuteScalar() == 1;

            if (databaseExists)
            {
                Console.WriteLine($"Database '{databaseName}' already exists. Skipping initialization.");
                return;
            }

            DatabaseHelper.Instance.CreateDatabase();
        }
    }
}