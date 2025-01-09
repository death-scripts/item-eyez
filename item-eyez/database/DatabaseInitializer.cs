using item_eyez;
using Microsoft.Data.SqlClient;

public class DatabaseInitializer
{
    private readonly string _serverConnectionString;

    public DatabaseInitializer(string serverConnectionString)
    {
        _serverConnectionString = serverConnectionString;
    }

    public static void InitializeDatabase(string serverConnectionString, string databaseName)
    {
        using (var connection = new SqlConnection(serverConnectionString))
        {
            connection.Open();

            DatabaseHelper.Instance = new DatabaseHelper(serverConnectionString, databaseName);

            // Check if the database exists
            var checkDbCommand = new SqlCommand($"IF DB_ID('{databaseName}') IS NOT NULL SELECT 1 ELSE SELECT 0;", connection);
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
