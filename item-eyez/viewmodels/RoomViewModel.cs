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
    /// The room view model.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class RoomViewModel : INotifyPropertyChanged
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
        /// The selected room row.
        /// </summary>
        private Room selectedRoomRow;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomViewModel" /> class.
        /// </summary>
        public RoomViewModel()
            : this(ItemEyezDatabase.Instance())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomViewModel"/> class.
        /// </summary>
        /// <param name="database">The database.</param>
        public RoomViewModel(IItemEyezDatabase database)
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
        /// Gets the add room command.
        /// </summary>
        /// <value>
        /// The add room command.
        /// </value>
        public ICommand AddRoomCommand => new RelayCommand(this.AddRoom);

        /// <summary>
        /// Gets the delete room command.
        /// </summary>
        /// <value>
        /// The delete room command.
        /// </value>
        public ICommand DeleteRoomCommand => new RelayCommand(this.DeleteSelectedRoom);

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
        /// Gets or sets the rooms.
        /// </summary>
        /// <value>
        /// The rooms.
        /// </value>
        public ObservableCollection<Room> Rooms { get; set; }

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
        /// Gets or sets the selected room row.
        /// </summary>
        /// <value>
        /// The selected room row.
        /// </value>
        public Room SelectedRoomRow
        {
            get => this.selectedRoomRow;
            set
            {
                this.selectedRoomRow = value;
                this.OnPropertyChanged(nameof(this.SelectedRoomRow));
            }
        }

        /// <summary>
        /// Adds the room.
        /// </summary>
        public void AddRoom()
        {
            if (!string.IsNullOrWhiteSpace(this.Name))
            {
                this.Description ??= string.Empty;
                this.dbHelper.AddRoom(this.Name, this.Description);
                this.Load();
                this.Name = string.Empty; // Clear input fields
                this.Description = string.Empty;

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
                this.Rooms = this.dbHelper.GetRoomsList();
            }
            else
            {
                // Filter the collection based on the search string
                ObservableCollection<Room> allRooms = this.dbHelper.GetRoomsList();
                ObservableCollection<Room> filteredRooms = new(
                    allRooms.Where(room =>
                        (!string.IsNullOrEmpty(room.Name) && room.Name.Contains(filterString, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(room.Description) && room.Description.Contains(filterString, StringComparison.OrdinalIgnoreCase))));
                this.Rooms = filteredRooms;
            }

            this.OnPropertyChanged(nameof(this.Rooms));
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public void Load()
        {
            this.Rooms = this.dbHelper.GetRoomsList();
            this.Rooms.CollectionChanged += this.Rooms_CollectionChanged;
            this.OnPropertyChanged(nameof(this.Rooms));
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged(string propertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Handles the DataChanged event of the DbHelper control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void DbHelper_DataChanged(object sender, EventArgs e) => this.Load();

        /// <summary>
        /// Deletes the selected room.
        /// </summary>
        private void DeleteSelectedRoom()
        {
            if (this.SelectedRoomRow != null)
            {
                _ = this.Rooms.Remove(this.SelectedRoomRow);
            }
        }

        /// <summary>
        /// Roomses the collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        private void Rooms_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Room room in e.OldItems)
                {
                    this.dbHelper.DeleteRoom(room.Id);
                }
            }
        }
    }
}