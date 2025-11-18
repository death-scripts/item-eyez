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
using System.Windows;
using Item_eyez.Themes;

namespace Item_eyez
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    /// <seealso cref="System.Windows.Window" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow() => this.InitializeComponent();

        private void Theme1_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.ApplyTheme(ThemeManager.Theme1Source);
            ThemeManager.SaveThemeSource(ThemeManager.Theme1Source);
        }

        private void Theme2_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.ApplyTheme(ThemeManager.Theme2Source);
            ThemeManager.SaveThemeSource(ThemeManager.Theme2Source);
        }

        private void Theme3_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.ApplyTheme(ThemeManager.Theme3Source);
            ThemeManager.SaveThemeSource(ThemeManager.Theme3Source);
        }

        private void Theme4_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.ApplyTheme(ThemeManager.Theme4Source);
            ThemeManager.SaveThemeSource(ThemeManager.Theme4Source);
        }
    }
}
