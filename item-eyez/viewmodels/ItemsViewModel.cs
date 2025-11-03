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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows.Input;
using Item_eyez.Controls;
using Item_eyez.Database;

namespace Item_eyez.Viewmodels
{
    /// <summary>
    /// The items view model.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class ItemsViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The database helper.
        /// </summary>
        private readonly IItemEyezDatabase dbHelper;

        /// <summary>
        /// The catagories.
        /// </summary>
        private string catagories;

        /// <summary>
        /// The description.
        /// </summary>
        private string description;

        /// <summary>
        /// The name.
        /// </summary>
        private string name;

        /// <summary>
        /// The search filter.
        /// </summary>
        private string searchFilter;

        /// <summary>
        /// The selected container.
        /// </summary>
        private Container? selectedContainer;

        /// <summary>
        /// The selected item.
        /// </summary>
        private Item selectedItem;

        /// <summary>
        /// The selected room.
        /// </summary>
        private Room? selectedRoom;

        /// <summary>
        /// The value.
        /// </summary>
        private string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsViewModel"/> class.
        /// </summary>
        public ItemsViewModel()
            : this(ItemEyezDatabase.Instance())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsViewModel"/> class.
        /// </summary>
        /// <param name="database">The database.</param>
        public ItemsViewModel(IItemEyezDatabase database)
        {
            this.dbHelper = database;
            this.Load();
            this.dbHelper.DataChanged += this.DbHelper_DataChanged;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the add item command.
        /// </summary>
        /// <value>
        /// The add item command.
        /// </value>
        public ICommand AddItemCommand => new RelayCommand(this.Add);

        /// <summary>
        /// Gets or sets the catagories.
        /// </summary>
        /// <value>
        /// The catagories.
        /// </value>
        public string Catagories
        {
            get => this.catagories;
            set
            {
                this.catagories = value;
                this.OnPropertyChanged(nameof(this.Catagories));
            }
        }

        /// <summary>
        /// Gets the containers.
        /// </summary>
        /// <value>
        /// The containers.
        /// </value>
        public ObservableCollection<Container> Containers { get; private set; }

        /// <summary>
        /// Gets the containers dropped down command.
        /// </summary>
        /// <value>
        /// The containers dropped down command.
        /// </value>
        public ICommand ContainersDroppedDownCommand => new RelayCommand(this.ContainersDroppedDown);

        /// <summary>
        /// Gets the delete item command.
        /// </summary>
        /// <value>
        /// The delete item command.
        /// </value>
        public ICommand DeleteItemCommand => new RelayCommand(this.DeleteSelectedItem);

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description
        {
            get => this.description;
            set
            {
                this.description = value;
                this.OnPropertyChanged(nameof(this.Description));
            }
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public ObservableCollection<Item> Items { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get => this.name;
            set
            {
                this.name = value;
                this.OnPropertyChanged(nameof(this.Name));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [name focused].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [name focused]; otherwise, <c>false</c>.
        /// </value>
        public bool NameFocused { get; set; }

        /// <summary>
        /// Gets the rooms.
        /// </summary>
        /// <value>
        /// The rooms.
        /// </value>
        public List<Room> Rooms { get; private set; }

        /// <summary>
        /// Gets the rooms dropped down command.
        /// </summary>
        /// <value>
        /// The rooms dropped down command.
        /// </value>
        public ICommand RoomsDroppedDownCommand => new RelayCommand(this.RoomsDroppedDown);

        /// <summary>
        /// Gets or sets the search filter.
        /// </summary>
        /// <value>
        /// The search filter.
        /// </value>
        public string SearchFilter
        {
            get => this.searchFilter;
            set
            {
                this.searchFilter = value;
                this.OnPropertyChanged(nameof(this.SearchFilter));
                this.FilterRooms(this.searchFilter);
            }
        }

        /// <summary>
        /// Gets or sets the selected container.
        /// </summary>
        /// <value>
        /// The selected container.
        /// </value>
        public Container SelectedContainer
        {
            get => this.selectedContainer;
            set
            {
                this.selectedContainer = value;
                this.OnPropertyChanged(nameof(this.SelectedContainer));
                this.selectedRoom = null;
                this.OnPropertyChanged(nameof(this.SelectedRoom));
            }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>
        /// The selected item.
        /// </value>
        public Item SelectedItem
        {
            get => this.selectedItem;
            set
            {
                this.selectedItem = value;
                this.OnPropertyChanged(nameof(this.SelectedItem));
            }
        }

        /// <summary>
        /// Gets or sets the selected room.
        /// </summary>
        /// <value>
        /// The selected room.
        /// </value>
        public Room SelectedRoom
        {
            get => this.selectedRoom;
            set
            {
                this.selectedRoom = value;
                this.OnPropertyChanged(nameof(this.SelectedRoom));
                this.selectedContainer = null;
                this.OnPropertyChanged(nameof(this.SelectedContainer));
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value
        {
            get => this.value;
            set
            {
                this.value = value;
                this.OnPropertyChanged(nameof(this.Value));
            }
        }

        /// <summary>
        /// Adds this instance.
        /// </summary>
        public void Add()
        {
            if (!string.IsNullOrWhiteSpace(this.Name))
            {
                this.Description ??= string.Empty;
                this.Catagories ??= string.Empty;
                _ = decimal.TryParse(this.Value, out decimal value);

                Guid newId = this.dbHelper.AddItem(this.Name, this.Description, value, this.Catagories);

                if (this.SelectedContainer != null)
                {
                    this.dbHelper.AssociateItemWithContainer(newId, this.SelectedContainer.Id);
                }
                else if (this.SelectedRoom != null)
                {
                    this.dbHelper.AssociateItemWithRoom(newId, this.SelectedRoom.Id);
                }

                this.Name = string.Empty; // Clear input fields
                this.Description = string.Empty;
                this.Load();

                this.NameFocused = true;
                this.OnPropertyChanged(nameof(this.NameFocused));
            }
        }

        /// <summary>
        /// Filters the rooms.
        /// </summary>
        /// <param name="filterString">The filter string.</param>
        public void FilterRooms(string filterString)
        {
            if (string.IsNullOrWhiteSpace(filterString))
            {
                // Reset the collection to show all rooms
                this.Items = this.dbHelper.GetItemsWithRelationships();
            }
            else
            {
                // Filter the collection based on the search string
                ObservableCollection<Item> allRooms = this.dbHelper.GetItemsWithRelationships();
                ObservableCollection<Item> filteredRooms = new(
                    allRooms.Where(item =>
                        (!string.IsNullOrEmpty(item.Name) && item.Name.Contains(filterString, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(item.Description) && item.Description.Contains(filterString, StringComparison.OrdinalIgnoreCase))));
                this.Items = filteredRooms;
            }

            this.OnPropertyChanged(nameof(this.Items));
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public void Load()
        {
            this.Items = this.dbHelper.GetItemsWithRelationships();
            this.Items.CollectionChanged += this.Items_CollectionChanged;
            this.OnPropertyChanged(nameof(this.Items));
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged(string propertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Containerses the dropped down.
        /// </summary>
        private void ContainersDroppedDown()
        {
            this.Containers = this.dbHelper.GetContainersWithRelationships();
            this.OnPropertyChanged(nameof(this.Containers));
        }

        /// <summary>
        /// Handles the DataChanged event of the DbHelper control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void DbHelper_DataChanged(object sender, EventArgs e) => this.Load();

        /// <summary>
        /// Deletes the selected item.
        /// </summary>
        private void DeleteSelectedItem()
        {
            if (this.SelectedItem != null)
            {
                _ = this.Items.Remove(this.SelectedItem);
            }
        }

        /// <summary>
        /// Itemses the collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Item item in e.OldItems)
                {
                    this.dbHelper.DeleteItem(item.Id);
                }
            }
        }

        /// <summary>
        /// Roomses the dropped down.
        /// </summary>
        private void RoomsDroppedDown()
        {
            this.Rooms = [.. this.dbHelper.GetRoomsList()];
            this.OnPropertyChanged(nameof(this.Rooms));
        }
    }
}