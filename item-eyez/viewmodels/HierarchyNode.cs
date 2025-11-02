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
        public HierarchyNode(object entity) => Entity = entity;

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
        public Guid Id => Entity switch
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
            get => isExpanded;
            set
            {
                isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
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
            get => isMatch;
            set
            {
                isMatch = value;
                OnPropertyChanged(nameof(IsMatch));
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
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
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
            get => isVisible;
            set
            {
                isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
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
            get => Entity switch
            {
                Item i => i.Name,
                Container c => c.Name,
                Room r => r.Name,
                _ => string.Empty,
            };
            set
            {
                switch (Entity)
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

                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString() => Name;
    }
}