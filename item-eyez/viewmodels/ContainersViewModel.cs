
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Windows.Input;

namespace item_eyez
{
    public class ContainersViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _searchFilter;
        private string _description;
        private readonly ItemEyezDatabase _dbHelper;

        public ContainersViewModel()
        {
            _dbHelper = ItemEyezDatabase.Instance();
            _dbHelper.DataChanged += _dbHelper_DataChanged;
            Load();
        }

        private void _dbHelper_DataChanged(object sender, EventArgs e)
        {
            this.Load();
        }

        public ICommand AddContainerCommand => new RelayCommand(Add);

        public ICommand ContainersDroppedDownCommand => new RelayCommand(ContainersDroppedDown);
        public ObservableCollection<Container> Containers { get; set; }

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
        public ObservableCollection<DataRowView> Items { get; set; }
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

        private Container _selectedContainerRow;
        public Container SelectedContainerRow
        {
            get => _selectedContainerRow;
            set { _selectedContainerRow = value; OnPropertyChanged(nameof(SelectedContainerRow)); }
        }

        public ICommand DeleteContainerCommand => new RelayCommand(DeleteSelectedContainer);
        public bool PreserveDescription { get; set; }
        public bool NameFocused { get; set; }
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
                Containers = _dbHelper.GetContainersWithRelationships();
            }
            else
            {
                // Filter the collection based on the search string
                var allRooms = _dbHelper.GetContainersWithRelationships();
                var filteredRooms = new ObservableCollection<Container>(
                    allRooms.Where(container =>
                        (!string.IsNullOrEmpty(container.Name) && container.Name.Contains(filterString, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(container.Description) && container.Description.Contains(filterString, StringComparison.OrdinalIgnoreCase))
                    )
                );
                Containers = filteredRooms;
            }
            OnPropertyChanged(nameof(Containers));
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
            Containers = _dbHelper.GetContainersWithRelationships();
            Containers.CollectionChanged += this.Rooms_CollectionChanged;
            OnPropertyChanged(nameof(Containers));
        }

        private void Rooms_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Container container in e.OldItems)
                {
                    _dbHelper.DeleteContainer(container.Id);
                }
            }
        }

        public void Add()
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                Description = Description == null ? string.Empty : Description;
                Guid newId = _dbHelper.AddContainer(Name, Description);

                if (this.SelectedContainer != null)
                {
                    _dbHelper.AssociateItemWithContainer(newId, this.SelectedContainer.Id);
                }
                else if (this.SelectedRoom != null)
                {
                    _dbHelper.AssociateItemWithRoom(newId, this.SelectedRoom.Id);
                }

                Load();
                Name = string.Empty; // Clear input fields

                if (!this.PreserveDescription)
                {
                    Description = string.Empty;
                }

                this.NameFocused = true;
                this.OnPropertyChanged(nameof(NameFocused));
            }
        }

        private void DeleteSelectedContainer()
        {
            if (SelectedContainerRow != null)
            {
                Containers.Remove(SelectedContainerRow);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
