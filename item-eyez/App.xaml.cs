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

                DatabaseInitializer.InitializeDatabase(serverConnectionString, databaseName);

                //// init instance
                ItemEyezDatabase.Instance("Server=localhost\\SQLEXPRESS;Database=ITEMEYEZ;Integrated Security=true;TrustServerCertificate=True;");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Application.Current.Shutdown();
            }
        }
    }
}
