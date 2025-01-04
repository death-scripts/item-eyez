
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows.Input;

namespace item_eyez
{
    public class RoomViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _searchFilter;
        private string _description;
        private readonly ItemEyezDatabase _dbHelper;

        public RoomViewModel()
        {
            _dbHelper = ItemEyezDatabase.Instance();
            LoadRooms();
        }
        public ICommand AddRoomCommand => new RelayCommand(AddRoom);

        public ObservableCollection<DataRowView> Rooms { get; set; }

        public string SearchFilter
        {
            get => _searchFilter;
            set
            {
                _searchFilter = value;
                OnPropertyChanged(nameof(SearchFilter));
                FilterRooms(_searchFilter);
            }
        }
        public void FilterRooms(string filterString)
        {
            if (string.IsNullOrWhiteSpace(filterString))
            {
                // Reset the collection to show all rooms
                var dataTable = _dbHelper.GetRooms();
                var allRooms = new ObservableCollection<DataRowView>(dataTable.DefaultView.Cast<DataRowView>());
                Rooms = new ObservableCollection<DataRowView>(allRooms);
            }
            else
            {
                // Filter the collection
                var dataTable = _dbHelper.GetRooms();
                var filteredRooms = new ObservableCollection<DataRowView>(dataTable.DefaultView.Cast<DataRowView>())
                    .Where(row => row["name"].ToString().Contains(filterString, StringComparison.OrdinalIgnoreCase) ||
                                  row["description"].ToString().Contains(filterString, StringComparison.OrdinalIgnoreCase));
                Rooms = new ObservableCollection<DataRowView>(filteredRooms);
            }

            OnPropertyChanged(nameof(Rooms));
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public void LoadRooms()
        {
            var dataTable = _dbHelper.GetRooms();
            Rooms = new ObservableCollection<DataRowView>(dataTable.DefaultView.Cast<DataRowView>());
            Rooms.CollectionChanged -= this.Rooms_CollectionChanged;
            Rooms.CollectionChanged += this.Rooms_CollectionChanged;
            OnPropertyChanged(nameof(Rooms));
        }

        private void Rooms_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (DataRowView row in e.OldItems)
                {
                    _dbHelper.DeleteRoom((Guid)row[0]);
                }
            }
        }

        public void AddRoom()
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                Description = Description == null ? string.Empty : Description;
                _dbHelper.AddRoom(Name, Description);
                LoadRooms();
                Name = string.Empty; // Clear input fields
                Description = string.Empty;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
