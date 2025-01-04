using Microsoft.Data.SqlClient;

public class DatabaseInitializer
{
    private readonly string _serverConnectionString;

    public DatabaseInitializer(string serverConnectionString)
    {
        _serverConnectionString = serverConnectionString;
    }

    public void InitializeDatabase(string script, string databaseName)
    {
        using (var connection = new SqlConnection(_serverConnectionString))
        {
            connection.Open();

            // Check if the database exists
            var checkDbCommand = new SqlCommand($"IF DB_ID('{databaseName}') IS NOT NULL SELECT 1 ELSE SELECT 0;", connection);
            bool databaseExists = (int)checkDbCommand.ExecuteScalar() == 1;

            if (databaseExists)
            {
                Console.WriteLine($"Database '{databaseName}' already exists. Skipping initialization.");
                return;
            }

            // Create the database if it does not exist
            var createDbCommand = new SqlCommand($"CREATE DATABASE {databaseName};", connection);
            createDbCommand.ExecuteNonQuery();

            // Switch to the new database
            connection.ChangeDatabase(databaseName);

            // Split the script into batches and execute each batch
            foreach (var batch in script.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries))
            {
                using (var command = new SqlCommand(batch, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            Console.WriteLine($"Database '{databaseName}' initialized successfully.");
        }
    }

}
