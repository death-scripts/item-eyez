using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace item_eyez
{
    /// <summary>
    /// Interaction logic for items_view.xaml
    /// </summary>
    public partial class items_view : UserControl
    {
        public items_view()
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
            if (DataContext is ItemsViewModel vm && ((FrameworkElement)sender).DataContext is Item item)
            {
                vm.Items.Remove(item);
            }
        }
    }
}
