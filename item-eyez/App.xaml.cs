using System.Windows;

namespace item_eyez
{
    /// <summary>
    /// The application.
    /// </summary>
    /// <seealso cref="System.Windows.Application" />
    public partial class App : Application
    {
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
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
