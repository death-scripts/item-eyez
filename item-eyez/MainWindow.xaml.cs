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
using System.Windows.Markup;

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

        private void Theme1_Click(object sender, RoutedEventArgs e) => this.ApplyTheme("Themes/Theme1.xaml");

        private void Theme2_Click(object sender, RoutedEventArgs e) => this.ApplyTheme("Themes/Theme2.xaml");

        private void ApplyTheme(string source)
        {
            if (Application.Current == null)
            {
                return;
            }

            var dictionaries = Application.Current.Resources.MergedDictionaries;
            if (dictionaries == null)
            {
                return;
            }

            // Remove any existing theme dictionaries from the Themes folder.
            for (var i = dictionaries.Count - 1; i >= 0; i--)
            {
                var uri = dictionaries[i].Source;
                if (uri != null && uri.OriginalString.StartsWith("Themes/", System.StringComparison.OrdinalIgnoreCase))
                {
                    dictionaries.RemoveAt(i);
                }
            }

            // Add the requested theme.
            dictionaries.Add(new ResourceDictionary { Source = new System.Uri(source, System.UriKind.Relative) });
        }
    }
}
