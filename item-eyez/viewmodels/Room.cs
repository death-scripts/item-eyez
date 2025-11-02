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
    /// The room.
    /// </summary>
    public class Room
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
        /// Initializes a new instance of the <see cref="Room"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        public Room(Guid id, string name, string description)
        {
            this.Id = id;
            this.name = name;
            this.description = description;
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
                ItemEyezDatabase.Instance().UpdateRoom(
                            this.Id,
                            this.Name,
                            this.Description);
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
                ItemEyezDatabase.Instance().UpdateRoom(
                            this.Id,
                            this.Name,
                            this.Description);
            }
        }
    }
}