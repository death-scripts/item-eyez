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

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Item_eyez.Database;
using Item_eyez.Logging;
using Item_eyez.Themes;

namespace Item_eyez
{
    /// <summary>
    /// The application.
    /// </summary>
    /// <seealso cref="System.Windows.Application" />
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            this.DispatcherUnhandledException += this.OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                ThemeManager.ApplyThemeFromConfig();

                string serverConnectionString = "Server=localhost\\SQLEXPRESS;Integrated Security=true;TrustServerCertificate=True;";
                string databaseName = "ITEMEYEZ";

                DatabaseInitializer.InitializeDatabase(serverConnectionString, databaseName);

                _ = ItemEyezDatabase.Instance("Server=localhost\\SQLEXPRESS;Database=ITEMEYEZ;Integrated Security=true;TrustServerCertificate=True;");
            }
            catch (Exception ex)
            {
                Logger.LogException("Startup failure", ex);
                _ = MessageBox.Show(
                    ex.ToString(),
                    "Item-eyez - Startup Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.LogException("Unhandled UI exception", e.Exception);
            _ = MessageBox.Show(
                e.Exception.ToString(),
                "Item-eyez - Unhandled UI Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            e.Handled = true;
            Current.Shutdown();
        }

        private static void CurrentDomainOnUnhandledException(object? sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception ?? new Exception("Non-exception unhandled error.");
            Logger.LogException("Unhandled domain exception", exception);
            _ = MessageBox.Show(
                exception.ToString(),
                "Item-eyez - Unhandled Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        private static void TaskSchedulerOnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            Logger.LogException("Unobserved task exception", e.Exception);
            _ = MessageBox.Show(
                e.Exception.ToString(),
                "Item-eyez - Background Task Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            e.SetObserved();
        }
    }
}
