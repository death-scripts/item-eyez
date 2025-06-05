using System.Collections.ObjectModel;
using System.Linq;

namespace item_eyez
{
    public class OrganizeViewModel : ViewModelBase
    {
        private readonly ItemEyezDatabase _db = ItemEyezDatabase.Instance();

        public ObservableCollection<HierarchyNode> Roots { get; } = new();

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
            _db.DataChanged += (_, __) => Load();
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
            node.IsExpanded = childMatch || selfMatch && node.Children.Count > 0;
            return selfMatch || childMatch;
        }

        public void Load()
        {
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

            OnPropertyChanged(nameof(Roots));
            ApplySearch();
        }
    }
}
