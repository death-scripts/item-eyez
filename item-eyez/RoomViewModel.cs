
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

        public ObservableCollection<Room> Rooms { get; set; }

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
        public bool NameFocused { get; set; }
        public void FilterRooms(string filterString)
        {
            if (string.IsNullOrWhiteSpace(filterString))
            {
                // Reset the collection to show all rooms
                Rooms = _dbHelper.GetRoomsList();
            }
            else
            {
                // Filter the collection based on the search string
                var allRooms = _dbHelper.GetRoomsList();
                var filteredRooms = new ObservableCollection<Room>(
                    allRooms.Where(room =>
                        (!string.IsNullOrEmpty(room.Name) && room.Name.Contains(filterString, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(room.Description) && room.Description.Contains(filterString, StringComparison.OrdinalIgnoreCase))
                    )
                );
                Rooms = filteredRooms;
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
            Rooms = _dbHelper.GetRoomsList();
            Rooms.CollectionChanged += this.Rooms_CollectionChanged;
            OnPropertyChanged(nameof(Rooms));
        }

        private void Rooms_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Room room in e.OldItems)
                {
                    _dbHelper.DeleteRoom(room.Id);
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

                this.NameFocused = true;
                this.OnPropertyChanged(nameof(NameFocused));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
