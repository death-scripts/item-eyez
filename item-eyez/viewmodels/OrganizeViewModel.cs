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
using Item_eyez.Database;

namespace Item_eyez.Viewmodels
{
    /// <summary>
    /// The organize view model.
    /// </summary>
    /// <seealso cref="Item_eyez.Viewmodels.ViewModelBase" />
    public class OrganizeViewModel : ViewModelBase
    {
        /// <summary>
        /// The database.
        /// </summary>
        private readonly IItemEyezDatabase db;

        /// <summary>
        /// The expansion state.
        /// </summary>
        private readonly Dictionary<Guid, bool> expansionState = [];

        /// <summary>
        /// The search text.
        /// </summary>
        private string searchText = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizeViewModel"/> class.
        /// </summary>
        public OrganizeViewModel()
            : this(ItemEyezDatabase.Instance())
        {
        }

        public OrganizeViewModel(IItemEyezDatabase database)
        {
            this.db = database;
            this.Load();
            this.RemoveRightFromRoots();
            this.db.DataChanged += (_, __) =>
            {
                this.Load();
                this.RemoveRightFromRoots();
            };
        }

        /// <summary>
        /// Gets the right roots.
        /// </summary>
        /// <value>
        /// The right roots.
        /// </value>
        public ObservableCollection<HierarchyNode> RightRoots { get; } = [];

        /// <summary>
        /// Gets the roots.
        /// </summary>
        /// <value>
        /// The roots.
        /// </value>
        public ObservableCollection<HierarchyNode> Roots { get; } = [];

        /// <summary>
        /// Gets or sets the search text.
        /// </summary>
        /// <value>
        /// The search text.
        /// </value>
        public string SearchText
        {
            get => this.searchText;
            set
            {
                if (this.searchText != value)
                {
                    this.searchText = value;
                    this.OnPropertyChanged(nameof(this.SearchText));
                    this.ApplySearch();
                }
            }
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public void Load()
        {
            this.expansionState.Clear();
            foreach (HierarchyNode root in this.Roots)
            {
                this.SaveExpansion(root);
            }

            this.Roots.Clear();
            ObservableCollection<Room> rooms = this.db.GetRoomsList();
            ObservableCollection<Container> containers = this.db.GetContainersWithRelationships();
            ObservableCollection<Item> items = this.db.GetItemsWithRelationships();

            Dictionary<Guid, HierarchyNode> roomNodes = rooms.ToDictionary(r => r.Id, r => new HierarchyNode(r));
            Dictionary<Guid, HierarchyNode> containerNodes = containers.ToDictionary(c => c.Id, c => new HierarchyNode(c));

            // link containers
            foreach (Container container in containers)
            {
                Guid? parentContainerId = this.db.GetContainerIdForEntity(container.Id);
                if (parentContainerId.HasValue && containerNodes.TryGetValue(parentContainerId.Value, out HierarchyNode? value1))
                {
                    value1.Children.Add(containerNodes[container.Id]);
                }
            }

            // assign containers to rooms or root
            foreach (Container container in containers)
            {
                HierarchyNode node = containerNodes[container.Id];
                Guid? parentContainerId = this.db.GetContainerIdForEntity(container.Id);
                if (parentContainerId.HasValue)
                {
                    continue;
                }

                Guid? roomId = this.db.GetRoomIdForEntity(container.Id);
                if (roomId.HasValue && roomNodes.TryGetValue(roomId.Value, out HierarchyNode? value))
                {
                    value.Children.Add(node);
                }
                else
                {
                    this.Roots.Add(node);
                }
            }

            // assign items
            foreach (Item item in items)
            {
                HierarchyNode itemNode = new(item);
                Guid? containerId = this.db.GetContainerIdForEntity(item.Id);
                if (containerId.HasValue && containerNodes.TryGetValue(containerId.Value, out HierarchyNode? value))
                {
                    value.Children.Add(itemNode);
                    continue;
                }

                Guid? roomId = this.db.GetRoomIdForEntity(item.Id);
                if (roomId.HasValue && roomNodes.TryGetValue(roomId.Value, out HierarchyNode? value1))
                {
                    value1.Children.Add(itemNode);
                }
                else
                {
                    this.Roots.Add(itemNode);
                }
            }

            foreach (HierarchyNode? rn in roomNodes.Values)
            {
                this.Roots.Add(rn);
            }

            foreach (HierarchyNode root in this.Roots)
            {
                this.RestoreExpansion(root);
            }

            this.OnPropertyChanged(nameof(this.Roots));
            this.ApplySearch();
        }

        /// <summary>
        /// Refreshes the search.
        /// </summary>
        public void RefreshSearch() => this.ApplySearch();

        /// <summary>
        /// Removes the right from roots.
        /// </summary>
        public void RemoveRightFromRoots()
        {
            List<Guid> ids = [.. this.RightRoots.Select(n => n.Id)];
            this.RightRoots.Clear();
            foreach (Guid id in ids)
            {
                HierarchyNode? node = this.FindNodeById(this.Roots, id);
                if (node != null)
                {
                    _ = this.RemoveNodeById(this.Roots, id);
                    this.RightRoots.Add(node);
                }
            }
        }

        /// <summary>
        /// Applies the search.
        /// </summary>
        private void ApplySearch()
        {
            foreach (HierarchyNode node in this.Roots)
            {
                _ = this.MarkMatches(node);
            }

            foreach (HierarchyNode node in this.RightRoots)
            {
                _ = this.MarkMatches(node);
            }

            foreach (HierarchyNode node in this.Roots)
            {
                _ = this.UpdateVisibility(node);
            }

            foreach (HierarchyNode node in this.RightRoots)
            {
                _ = this.UpdateVisibility(node);
            }
        }

        /// <summary>
        /// Finds the node by identifier.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// The nullable.
        /// </returns>
        private HierarchyNode? FindNodeById(ObservableCollection<HierarchyNode> list, Guid id)
        {
            foreach (HierarchyNode n in list)
            {
                if (n.Id == id)
                {
                    return n;
                }

                HierarchyNode? found = this.FindNodeById(n.Children, id);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        /// <summary>
        /// Marks the matches.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        /// The boolean.
        /// </returns>
        private bool MarkMatches(HierarchyNode node)
        {
            bool selfMatch = !string.IsNullOrWhiteSpace(this.SearchText) &&
                             node.Name.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase);
            bool childMatch = false;
            foreach (HierarchyNode child in node.Children)
            {
                childMatch |= this.MarkMatches(child);
            }

            node.IsMatch = selfMatch;
            if (!string.IsNullOrWhiteSpace(this.SearchText))
            {
                node.IsExpanded = node.IsExpanded || childMatch || (selfMatch && node.Children.Count > 0);
            }

            return selfMatch || childMatch;
        }

        /// <summary>
        /// Removes the node by identifier.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// The boolean.
        /// </returns>
        private bool RemoveNodeById(ObservableCollection<HierarchyNode> list, Guid id)
        {
            HierarchyNode? existing = list.FirstOrDefault(n => n.Id == id);
            if (existing != null)
            {
                _ = list.Remove(existing);
                return true;
            }

            foreach (HierarchyNode child in list)
            {
                if (this.RemoveNodeById(child.Children, id))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Restores the expansion.
        /// </summary>
        /// <param name="node">The node.</param>
        private void RestoreExpansion(HierarchyNode node)
        {
            if (this.expansionState.TryGetValue(node.Id, out bool expanded))
            {
                node.IsExpanded = expanded;
            }

            foreach (HierarchyNode child in node.Children)
            {
                this.RestoreExpansion(child);
            }
        }

        /// <summary>
        /// Saves the expansion.
        /// </summary>
        /// <param name="node">The node.</param>
        private void SaveExpansion(HierarchyNode node)
        {
            this.expansionState[node.Id] = node.IsExpanded;
            foreach (HierarchyNode child in node.Children)
            {
                this.SaveExpansion(child);
            }
        }

        /// <summary>
        /// Updates the visibility.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        /// The boolean.
        /// </returns>
        private bool UpdateVisibility(HierarchyNode node)
        {
            bool childVisible = false;
            foreach (HierarchyNode child in node.Children)
            {
                childVisible |= this.UpdateVisibility(child);
            }

            bool visible = string.IsNullOrWhiteSpace(this.SearchText) || node.IsMatch || childVisible;
            node.IsVisible = visible;
            return node.IsMatch || childVisible;
        }
    }
}