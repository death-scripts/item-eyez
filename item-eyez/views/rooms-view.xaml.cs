using System.Windows.Controls;
using System.Windows.Input;

namespace item_eyez
{
    /// <summary>
    /// Interaction logic for rooms_view.xaml
    /// </summary>
    public partial class rooms_view : UserControl
    {
        public rooms_view()
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
            if (DataContext is RoomViewModel vm && ((FrameworkElement)sender).DataContext is Room room)
            {
                vm.Rooms.Remove(room);
            }
        }
    }
}
