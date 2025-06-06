using System.Collections.ObjectModel;

namespace item_eyez
{
    public class HierarchyNode : ViewModelBase
    {
        public object Entity { get; }
        public ObservableCollection<HierarchyNode> Children { get; } = new();

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set { _isExpanded = value; OnPropertyChanged(nameof(IsExpanded)); }
        }

        private bool _isMatch;
        public bool IsMatch
        {
            get => _isMatch;
            set { _isMatch = value; OnPropertyChanged(nameof(IsMatch)); }
        }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            set { _isVisible = value; OnPropertyChanged(nameof(IsVisible)); }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); }
        }

        public HierarchyNode(object entity)
        {
            Entity = entity;
        }

        public Guid Id
        {
            get
            {
                return Entity switch
                {
                    Item i => i.Id,
                    Container c => c.Id,
                    Room r => r.Id,
                    _ => Guid.Empty
                };
            }
        }

        public string Name
        {
            get
            {
                return Entity switch
                {
                    Item i => i.Name,
                    Container c => c.Name,
                    Room r => r.Name,
                    _ => string.Empty
                };
            }
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

        public override string ToString() => Name;
    }
}
