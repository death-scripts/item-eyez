using System.Windows;

namespace item_eyez
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                string serverConnectionString = "Server=localhost\\SQLEXPRESS;Integrated Security=true;TrustServerCertificate=True;";
                string databaseName = "ITEMEYEZ";

                // Read the SQL script from a file
                string scriptPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "create.sql");
                string script = System.IO.File.ReadAllText(scriptPath);

                // Initialize the database
                var initializer = new DatabaseInitializer(serverConnectionString);
                initializer.InitializeDatabase(script, databaseName);

                // init instance
                ItemEyezDatabase.Instance("Server=localhost\\SQLEXPRESS;Database=ITEMEYEZ;Integrated Security=true;TrustServerCertificate=True;");

                Console.WriteLine("Database setup complete!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Application.Current.Shutdown();
            }
        }
    }
}
