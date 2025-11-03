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

namespace Item_eyez.Viewmodels
{
    /// <summary>
    /// The hierarchy node.
    /// </summary>
    /// <seealso cref="Item_eyez.Viewmodels.ViewModelBase" />
    public class HierarchyNode : ViewModelBase
    {
        /// <summary>
        /// The is expanded.
        /// </summary>
        private bool isExpanded;

        /// <summary>
        /// The is match.
        /// </summary>
        private bool isMatch;

        /// <summary>
        /// The is selected.
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// The is visible.
        /// </summary>
        private bool isVisible = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyNode"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public HierarchyNode(object entity) => this.Entity = entity;

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public ObservableCollection<HierarchyNode> Children { get; } = [];

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <value>
        /// The entity.
        /// </value>
        public object Entity { get; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id => this.Entity switch
        {
            Item i => i.Id,
            Container c => c.Id,
            Room r => r.Id,
            _ => Guid.Empty,
        };

        /// <summary>
        /// Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpanded
        {
            get => this.isExpanded;
            set
            {
                this.isExpanded = value;
                this.OnPropertyChanged(nameof(this.IsExpanded));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is match.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is match; otherwise, <c>false</c>.
        /// </value>
        public bool IsMatch
        {
            get => this.isMatch;
            set
            {
                this.isMatch = value;
                this.OnPropertyChanged(nameof(this.IsMatch));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get => this.isSelected;
            set
            {
                this.isSelected = value;
                this.OnPropertyChanged(nameof(this.IsSelected));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisible
        {
            get => this.isVisible;
            set
            {
                this.isVisible = value;
                this.OnPropertyChanged(nameof(this.IsVisible));
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
            get => this.Entity switch
            {
                Item i => i.Name,
                Container c => c.Name,
                Room r => r.Name,
                _ => string.Empty,
            };
            set
            {
                switch (this.Entity)
                {
                    case Item i:
                        i.Name = value;
                        break;

                    case Container c:
                        c.Name = value;
                        break;

                    case Room r:
                        r.Name = value;
                        break;
                }

                this.OnPropertyChanged(nameof(this.Name));
            }
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString() => this.Name;
    }
}