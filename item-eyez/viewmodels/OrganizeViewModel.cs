using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace item_eyez
{
    public class OrganizeViewModel : ViewModelBase
    {
        private readonly ItemEyezDatabase _db = ItemEyezDatabase.Instance();

        public ObservableCollection<HierarchyNode> Roots { get; } = new();
        public ObservableCollection<HierarchyNode> RightRoots { get; } = new();

        private readonly Dictionary<Guid, bool> _expansionState = new();

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    ApplySearch();
                }
            }
        }

        public OrganizeViewModel()
        {
            Load();
            RemoveRightFromRoots();
            _db.DataChanged += (_, __) =>
            {
                Load();
                RemoveRightFromRoots();
            };
        }

        private void ApplySearch()
        {
            foreach (var node in Roots)
                MarkMatches(node);
        }

        private bool MarkMatches(HierarchyNode node)
        {
            bool selfMatch = !string.IsNullOrWhiteSpace(SearchText) &&
                             node.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
            bool childMatch = false;
            foreach (var child in node.Children)
                childMatch |= MarkMatches(child);
            node.IsMatch = selfMatch;
            if (!string.IsNullOrWhiteSpace(SearchText))
                node.IsExpanded = node.IsExpanded || childMatch || (selfMatch && node.Children.Count > 0);
            return selfMatch || childMatch;
        }

        private void SaveExpansion(HierarchyNode node)
        {
            _expansionState[node.Id] = node.IsExpanded;
            foreach (var child in node.Children)
                SaveExpansion(child);
        }

        private void RestoreExpansion(HierarchyNode node)
        {
            if (_expansionState.TryGetValue(node.Id, out bool expanded))
                node.IsExpanded = expanded;
            foreach (var child in node.Children)
                RestoreExpansion(child);
        }

        public void Load()
        {
            _expansionState.Clear();
            foreach (var root in Roots)
                SaveExpansion(root);

            Roots.Clear();
            var rooms = _db.GetRoomsList();
            var containers = _db.GetContainersWithRelationships();
            var items = _db.GetItemsWithRelationships();

            var roomNodes = rooms.ToDictionary(r => r.Id, r => new HierarchyNode(r));
            var containerNodes = containers.ToDictionary(c => c.Id, c => new HierarchyNode(c));

            // link containers
            foreach (var container in containers)
            {
                var parentContainerId = _db.GetContainerIdForEntity(container.Id);
                if (parentContainerId.HasValue && containerNodes.ContainsKey(parentContainerId.Value))
                {
                    containerNodes[parentContainerId.Value].Children.Add(containerNodes[container.Id]);
                }
            }

            // assign containers to rooms or root
            foreach (var container in containers)
            {
                var node = containerNodes[container.Id];
                var parentContainerId = _db.GetContainerIdForEntity(container.Id);
                if (parentContainerId.HasValue)
                    continue;
                var roomId = _db.GetRoomIdForEntity(container.Id);
                if (roomId.HasValue && roomNodes.ContainsKey(roomId.Value))
                    roomNodes[roomId.Value].Children.Add(node);
                else
                    Roots.Add(node);
            }

            // assign items
            foreach (var item in items)
            {
                var itemNode = new HierarchyNode(item);
                var containerId = _db.GetContainerIdForEntity(item.Id);
                if (containerId.HasValue && containerNodes.ContainsKey(containerId.Value))
                {
                    containerNodes[containerId.Value].Children.Add(itemNode);
                    continue;
                }
                var roomId = _db.GetRoomIdForEntity(item.Id);
                if (roomId.HasValue && roomNodes.ContainsKey(roomId.Value))
                    roomNodes[roomId.Value].Children.Add(itemNode);
                else
                    Roots.Add(itemNode);
            }

            foreach (var rn in roomNodes.Values)
            {
                Roots.Add(rn);
            }

            foreach (var root in Roots)
                RestoreExpansion(root);

            OnPropertyChanged(nameof(Roots));
            ApplySearch();
        }

        public void RemoveRightFromRoots()
        {
            foreach (var node in RightRoots.ToList())
            {
                RemoveNodeById(Roots, node.Id);
            }
        }

        private bool RemoveNodeById(ObservableCollection<HierarchyNode> list, Guid id)
        {
            var existing = list.FirstOrDefault(n => n.Id == id);
            if (existing != null)
            {
                list.Remove(existing);
                return true;
            }
            foreach (var child in list)
            {
                if (RemoveNodeById(child.Children, id))
                    return true;
            }
            return false;
        }
    }
}
