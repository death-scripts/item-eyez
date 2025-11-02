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
using Item_eyez.Database;

namespace Item_eyez.Viewmodels
{
    /// <summary>
    /// The container.
    /// </summary>
    /// <seealso cref="Item_eyez.Viewmodels.ViewModelBase" />
    public class Container : ViewModelBase
    {
        /// <summary>
        /// The description.
        /// </summary>
        private string description;

        /// <summary>
        /// The name.
        /// </summary>
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Container"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        public Container(Guid id, string name = "", string description = "")
        {
            this.Id = id;
            this.name = name;
            this.description = description;
        }

        /// <summary>
        /// Gets or sets the contained in.
        /// </summary>
        /// <value>
        /// The contained in.
        /// </value>
        public Container ContainedIn
        {
            get => ItemEyezDatabase.Instance().GetItemsContainer(this.Id);
            set
            {
                Guid? id = value?.Id;
                ItemEyezDatabase.Instance().SetItemsContainer(this.Id, id);

                if (value != null)
                {
                    Guid? roomId = ItemEyezDatabase.Instance().GetRoomIdForEntity(value.Id);
                    ItemEyezDatabase.Instance().SetItemsRoom(this.Id, roomId);
                }

                this.OnPropertyChanged(nameof(this.ContainedIn));
                this.OnPropertyChanged(nameof(this.StoredIn));
            }
        }

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
                ItemEyezDatabase.Instance().UpdateContainer(this.Id, this.Name, this.Description);
                this.OnPropertyChanged(nameof(this.Description));
            }
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }

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
                ItemEyezDatabase.Instance().UpdateContainer(this.Id, this.Name, this.Description);
                this.OnPropertyChanged(nameof(this.Name));
            }
        }

        /// <summary>
        /// Gets or sets the stored in.
        /// </summary>
        /// <value>
        /// The stored in.
        /// </value>
        public Room StoredIn
        {
            get => ItemEyezDatabase.Instance().GetItemsRoom(this.Id);
            set
            {
                Guid? id = value?.Id;
                if (this.ContainedIn != null)
                {
                    ItemEyezDatabase.Instance().UnassociateItemFromContainer(this.Id);
                }

                ItemEyezDatabase.Instance().SetItemsRoom(this.Id, id);
                this.OnPropertyChanged(nameof(this.StoredIn));
            }
        }
    }
}