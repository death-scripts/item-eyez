using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace item_eyez
{
    public partial class organize_view : UserControl
    {
        private Point _startPoint;
        private TreeViewItem? _highlighted;
        private readonly ItemEyezDatabase _db = ItemEyezDatabase.Instance();
        public organize_view()
        {
            InitializeComponent();
        }

        private void DragHandle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        private void DragHandle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(null);
                if (Math.Abs(pos.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(pos.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    var element = (FrameworkElement)sender;
                    DragDrop.DoDragDrop(element, element.DataContext, DragDropEffects.Move);
                }
            }
        }

        private void Tree_Drop(object sender, DragEventArgs e)
        {
            var source = e.Data.GetData(typeof(HierarchyNode)) as HierarchyNode;
            if (source == null) return;

            var treeView = (TreeView)sender;
            var targetItem = GetContainerFromEvent(treeView, e.OriginalSource as DependencyObject);
            var vm = (OrganizeViewModel)DataContext;

            // remove from whichever tree currently holds the node
            RemoveNode(vm.Roots, source);
            RemoveNode(vm.RightRoots, source);

            if (targetItem == null)
            {
                // dropping on empty space adds to root
                var list = treeView == tree ? vm.Roots : vm.RightRoots;
                list.Add(source);
                e.Handled = true;
            }
            else
            {
                var target = targetItem.DataContext as HierarchyNode;
                if (target == null || target == source) return;

                target.Children.Add(source);
                target.IsExpanded = true;

                if (source.Entity is Item item)
                {
                    if (target.Entity is Container tc)
                        item.ContainedIn = tc;
                    else if (target.Entity is Room tr)
                        item.StoredIn = tr;
                }
                else if (source.Entity is Container sc)
                {
                    if (target.Entity is Container tc)
                        sc.ContainedIn = tc;
                    else if (target.Entity is Room tr)
                        sc.StoredIn = tr;
                }

                vm.RemoveRightFromRoots();
            }

            vm.RefreshSearch();

            if (_highlighted != null)
            {
                _highlighted.Background = Brushes.Transparent;
                _highlighted = null;
            }
        }

        private void Tree_DragOver(object sender, DragEventArgs e)
        {
            var treeView = (TreeView)sender;
            var item = GetContainerFromEvent(treeView, e.OriginalSource as DependencyObject);
            if (_highlighted != item)
            {
                if (_highlighted != null)
                    _highlighted.Background = Brushes.Transparent;
                _highlighted = item;
                if (_highlighted != null)
                    _highlighted.Background = Brushes.AliceBlue;
            }
        }

        private TreeViewItem? GetContainerFromEvent(ItemsControl container, DependencyObject? source)
        {
            while (source != null && source != container)
            {
                if (source is TreeViewItem item)
                    return item;
                source = VisualTreeHelper.GetParent(source);
            }
            return null;
        }

        private bool RemoveNode(ObservableCollection<HierarchyNode> list, HierarchyNode node)
        {
            if (list.Remove(node))
                return true;
            foreach (var child in list)
            {
                if (RemoveNode(child.Children, node))
                    return true;
            }
            return false;
        }

        private void TreeViewItem_LeftClick(object sender, MouseButtonEventArgs e)
        {
            var item = (TreeViewItem)sender;
            item.IsSelected = true;
            var node = item.DataContext as HierarchyNode;
            if (node == null) return;

            ContextMenu menu = new ContextMenu();
            if (node.Entity is Container cont)
            {
                var addItem = new MenuItem { Header = "Add Item", DataContext = cont };
                addItem.Click += AddItem_OnClick;
                menu.Items.Add(addItem);

                var addContainer = new MenuItem { Header = "Add Container", DataContext = cont };
                addContainer.Click += AddContainer_OnClick;
                menu.Items.Add(addContainer);
            }
            menu.Items.Add(new MenuItem { Header = "Delete", DataContext = node, Command = null });
            ((MenuItem)menu.Items[^1]).Click += DeleteNode_OnClick;
            menu.IsOpen = true;
        }

        private void Tree_LeftClick(object sender, MouseButtonEventArgs e)
        {
            var treeView = (TreeView)sender;
            if (GetContainerFromEvent(treeView, e.OriginalSource as DependencyObject) == null)
            {
                ContextMenu menu = new ContextMenu();
                menu.Items.Add(new MenuItem { Header = "Add Item" });
                ((MenuItem)menu.Items[0]).Click += (s, _) => AddItem_OnClick(s, _);
                menu.Items.Add(new MenuItem { Header = "Add Container" });
                ((MenuItem)menu.Items[1]).Click += (s, _) => AddContainer_OnClick(s, _);
                menu.Items.Add(new MenuItem { Header = "Add Room" });
                ((MenuItem)menu.Items[2]).Click += (s, _) => AddRoomRoot_Click(s, _);
                menu.IsOpen = true;
            }
        }

        private void AddItem_OnClick(object sender, RoutedEventArgs e)
        {
            Container? parent = (sender as MenuItem)?.DataContext as Container;
            var id = _db.AddItem("New Item", string.Empty, 0m, string.Empty);
            if (parent != null)
                _db.AssociateItemWithContainer(id, parent.Id);
        }

        private void AddItemRoot_Click(object sender, RoutedEventArgs e) => AddItem_OnClick(sender, e);

        private void AddContainer_OnClick(object sender, RoutedEventArgs e)
        {
            Container? parent = (sender as MenuItem)?.DataContext as Container;
            var id = _db.AddContainer("New Container", string.Empty);
            if (parent != null)
                _db.AssociateItemWithContainer(id, parent.Id);
        }

        private void AddContainerRoot_Click(object sender, RoutedEventArgs e) => AddContainer_OnClick(sender, e);

        private void AddRoomRoot_Click(object sender, RoutedEventArgs e)
        {
            _db.AddRoom("New Room", string.Empty);
        }

        private void DeleteNode_OnClick(object sender, RoutedEventArgs e)
        {
            if ((sender as MenuItem)?.DataContext is HierarchyNode node)
            {
                switch (node.Entity)
                {
                    case Item item:
                        _db.DeleteItem(item.Id);
                        break;
                    case Container cont:
                        _db.DeleteContainer(cont.Id);
                        break;
                    case Room room:
                        _db.DeleteRoom(room.Id);
                        break;
                }
            }
        }
    }
}
