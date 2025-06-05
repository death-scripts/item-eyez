using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace item_eyez
{
    /// <summary>
    /// Interaction logic for containers_view.xaml
    /// </summary>
    public partial class containers_view : UserControl
    {
        public containers_view()
        {
            InitializeComponent();
        }

        private void DataGridRow_LeftClick(object sender, MouseButtonEventArgs e)
        {
            var row = (DataGridRow)sender;
            row.IsSelected = true;
            row.ContextMenu.DataContext = row.DataContext;
            row.ContextMenu.IsOpen = true;
        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ContainersViewModel vm && ((FrameworkElement)sender).DataContext is Container container)
            {
                vm.Containers.Remove(container);
            }
        }
    }
}
