using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows.Input;

namespace item_eyez
{
    internal class ItemsViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _searchFilter;
        private string _description;
        private string _catagories;
        private string _value;
        private readonly ItemEyezDatabase _dbHelper;

        public ItemsViewModel()
        {
            _dbHelper = ItemEyezDatabase.Instance();
            Load();
        }
        public ICommand AddItemCommand => new RelayCommand(Add);
        public ICommand ContainersDroppedDownCommand => new RelayCommand(ContainersDroppedDown);
        public ObservableCollection<Container> Containers { get; private set; }
        private void ContainersDroppedDown()
        {
            this.Containers = _dbHelper.GetContainersWithRelationships();
            OnPropertyChanged(nameof(Containers));
        }

        public ICommand RoomsDroppedDownCommand => new RelayCommand(RoomsDroppedDown);

        private void RoomsDroppedDown()
        {
            this.Rooms = _dbHelper.GetRoomsList().ToList();
            OnPropertyChanged(nameof(Rooms));
        }
        private Room _selectedRoom;
        public Room SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                _selectedRoom = value;
                OnPropertyChanged(nameof(SelectedRoom));
                this._selectedContainer = null;
                OnPropertyChanged(nameof(SelectedContainer));
            }
        }
        public List<Room> Rooms { get; private set; }
        public ObservableCollection<Item> Items { get; set; }
        private Container _selectedContainer;
        public Container SelectedContainer
        {
            get => _selectedContainer;
            set
            {
                _selectedContainer = value;
                OnPropertyChanged(nameof(SelectedContainer));
                this._selectedRoom = null;
                OnPropertyChanged(nameof(SelectedRoom));
            }
        }
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
                Items = _dbHelper.GetItemsWithRelationships();
            }
            else
            {
                // Filter the collection based on the search string
                var allRooms = _dbHelper.GetItemsWithRelationships();
                var filteredRooms = new ObservableCollection<Item>(
                    allRooms.Where(item =>
                        (!string.IsNullOrEmpty(item.Name) && item.Name.Contains(filterString, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(item.Description) && item.Description.Contains(filterString, StringComparison.OrdinalIgnoreCase))
                    )
                );
                Items = filteredRooms;
            }
            OnPropertyChanged(nameof(Items));
        }

        public string Catagories
        {
            get => _catagories;
            set
            {
                _catagories = value;
                OnPropertyChanged(nameof(Catagories));
            }
        }

        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
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

        public void Load()
        {
            Items = _dbHelper.GetItemsWithRelationships();
            Items.CollectionChanged += this.Items_CollectionChanged;
            OnPropertyChanged(nameof(Items));
        }

        private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Item item in e.OldItems)
                {
                    _dbHelper.DeleteItem(item.Id);
                }
            }
        }

        public void Add()
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                Description = Description == null ? string.Empty : Description;
                Catagories = Catagories == null ? string.Empty : Catagories;
                decimal value;
                decimal.TryParse(Value, out value);

                Guid newId = _dbHelper.AddItem(Name, Description, value, Catagories);

                if (this.SelectedContainer != null)
                {
                    _dbHelper.AssociateItemWithContainer(newId, this.SelectedContainer.Id);
                }
                else if (this.SelectedRoom != null)
                {
                    _dbHelper.AssociateItemWithRoom(newId, this.SelectedRoom.Id);
                }

                Name = string.Empty; // Clear input fields
                Description = string.Empty;
                Load();
            }


        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
