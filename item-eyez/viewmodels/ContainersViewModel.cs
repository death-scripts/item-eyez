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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Windows.Input;
using Item_eyez.Controls;
using Item_eyez.Database;

namespace Item_eyez.Viewmodels
{
    /// <summary>
    /// The containers view model.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class ContainersViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The database helper.
        /// </summary>
        private readonly IItemEyezDatabase dbHelper;

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
        /// The selected container row.
        /// </summary>
        private Container selectedContainerRow;

        /// <summary>
        /// The selected room.
        /// </summary>
        private Room? selectedRoom;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainersViewModel"/> class.
        /// </summary>
        public ContainersViewModel()
            : this(ItemEyezDatabase.Instance())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainersViewModel"/> class.
        /// </summary>
        /// <param name="database">The database.</param>
        public ContainersViewModel(IItemEyezDatabase database)
        {
            this.dbHelper = database;
            this.dbHelper.DataChanged += this.DbHelper_DataChanged;
            this.Load();
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the add container command.
        /// </summary>
        /// <value>
        /// The add container command.
        /// </value>
        public ICommand AddContainerCommand => new RelayCommand(this.Add);

        /// <summary>
        /// Gets or sets the containers.
        /// </summary>
        /// <value>
        /// The containers.
        /// </value>
        public ObservableCollection<Container> Containers { get; set; }

        /// <summary>
        /// Gets the containers dropped down command.
        /// </summary>
        /// <value>
        /// The containers dropped down command.
        /// </value>
        public ICommand ContainersDroppedDownCommand => new RelayCommand(this.ContainersDroppedDown);

        /// <summary>
        /// Gets the delete container command.
        /// </summary>
        /// <value>
        /// The delete container command.
        /// </value>
        public ICommand DeleteContainerCommand => new RelayCommand(this.DeleteSelectedContainer);

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
        public ObservableCollection<DataRowView> Items { get; set; }

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
        /// Gets or sets a value indicating whether [preserve description].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [preserve description]; otherwise, <c>false</c>.
        /// </value>
        public bool PreserveDescription { get; set; }

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
        /// Gets or sets the selected container row.
        /// </summary>
        /// <value>
        /// The selected container row.
        /// </value>
        public Container SelectedContainerRow
        {
            get => this.selectedContainerRow;
            set
            {
                this.selectedContainerRow = value;
                this.OnPropertyChanged(nameof(this.SelectedContainerRow));
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
        /// Adds this instance.
        /// </summary>
        public void Add()
        {
            if (!string.IsNullOrWhiteSpace(this.Name))
            {
                this.Description ??= string.Empty;
                Guid newId = this.dbHelper.AddContainer(this.Name, this.Description);

                if (this.SelectedContainer != null)
                {
                    this.dbHelper.AssociateItemWithContainer(newId, this.SelectedContainer.Id);
                }
                else if (this.SelectedRoom != null)
                {
                    this.dbHelper.AssociateItemWithRoom(newId, this.SelectedRoom.Id);
                }

                this.Load();
                this.Name = string.Empty; // Clear input fields

                if (!this.PreserveDescription)
                {
                    this.Description = string.Empty;
                }

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
                this.Containers = this.dbHelper.GetContainersWithRelationships();
            }
            else
            {
                // Filter the collection based on the search string
                ObservableCollection<Container> allRooms = this.dbHelper.GetContainersWithRelationships();
                ObservableCollection<Container> filteredRooms = new(
                    allRooms.Where(container =>
                        (!string.IsNullOrEmpty(container.Name) && container.Name.Contains(filterString, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(container.Description) && container.Description.Contains(filterString, StringComparison.OrdinalIgnoreCase))));
                this.Containers = filteredRooms;
            }

            this.OnPropertyChanged(nameof(this.Containers));
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public void Load()
        {
            this.Containers = this.dbHelper.GetContainersWithRelationships();
            this.Containers.CollectionChanged += this.Rooms_CollectionChanged;
            this.OnPropertyChanged(nameof(this.Containers));
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
        /// Deletes the selected container.
        /// </summary>
        private void DeleteSelectedContainer()
        {
            if (this.SelectedContainerRow != null)
            {
                _ = this.Containers.Remove(this.SelectedContainerRow);
            }
        }

        /// <summary>
        /// Roomses the collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void Rooms_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Container container in e.OldItems)
                {
                    this.dbHelper.DeleteContainer(container.Id);
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