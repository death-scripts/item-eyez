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
            LoadRooms();
        }
        public ICommand AddItemCommand => new RelayCommand(Add);

        public ObservableCollection<DataRowView> Items { get; set; }

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
                var dataTable = _dbHelper.GetItems();
                var allItems = new ObservableCollection<DataRowView>(dataTable.DefaultView.Cast<DataRowView>());
                Items = new ObservableCollection<DataRowView>(allItems);
            }
            else
            {
                // Filter the collection
                var dataTable = _dbHelper.GetItems();
                var filteredItems = new ObservableCollection<DataRowView>(dataTable.DefaultView.Cast<DataRowView>())
                    .Where(row => row["name"].ToString().Contains(filterString, StringComparison.OrdinalIgnoreCase) ||
                                  row["description"].ToString().Contains(filterString, StringComparison.OrdinalIgnoreCase) ||
                                  row["catagories"].ToString().Contains(filterString, StringComparison.OrdinalIgnoreCase));
                Items = new ObservableCollection<DataRowView>(filteredItems);
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

        public void LoadRooms()
        {
            var dataTable = _dbHelper.GetItems();
            Items = new ObservableCollection<DataRowView>(dataTable.DefaultView.Cast<DataRowView>());
            Items.CollectionChanged += this.Items_CollectionChanged;
            OnPropertyChanged(nameof(Items));
        }

        private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (DataRowView row in e.OldItems)
                {
                    _dbHelper.DeleteRoom((Guid)row[0]);
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

                _dbHelper.AddItem(Name, Description, value, Catagories);
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
