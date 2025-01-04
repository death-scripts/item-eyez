
using System.Collections.ObjectModel;
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
            Load();
        }
        public ICommand AddContainerCommand => new RelayCommand(Add);

        public ObservableCollection<DataRowView> Containers { get; set; }
        internal List<Container> AvailableContainers { get; set; }
        internal List<Room> Rooms { get; set; }
        internal List<object> RoomContainerList { get; set; }

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
                var dataTable = _dbHelper.GetContainers();
                var allContainers = new ObservableCollection<DataRowView>(dataTable.DefaultView.Cast<DataRowView>());
                Containers = new ObservableCollection<DataRowView>(allContainers);
            }
            else
            {
                // Filter the collection
                var dataTable = _dbHelper.GetContainers();
                var filterContainers = new ObservableCollection<DataRowView>(dataTable.DefaultView.Cast<DataRowView>())
                    .Where(row => row["name"].ToString().Contains(filterString, StringComparison.OrdinalIgnoreCase) ||
                                  row["description"].ToString().Contains(filterString, StringComparison.OrdinalIgnoreCase));
                Containers = new ObservableCollection<DataRowView>(filterContainers);
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
            var dataTable = _dbHelper.GetContainers();
            Containers = new ObservableCollection<DataRowView>(dataTable.DefaultView.Cast<DataRowView>());
            Containers.CollectionChanged += this.Rooms_CollectionChanged;
            OnPropertyChanged(nameof(Containers));
            RoomContainerList =
            [
                new Container
                {
                    Id = 1,
                    Name = "TEST"
                },
            ];
            OnPropertyChanged(nameof(RoomContainerList));
        }

        private void Rooms_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (DataRowView row in e.OldItems)
                {
                    _dbHelper.DeleteContainer((Guid)row[0]);
                }
            }
        }

        public void Add()
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                Description = Description == null ? string.Empty : Description;
                _dbHelper.AddContainer(Name, Description);
                Load();
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
