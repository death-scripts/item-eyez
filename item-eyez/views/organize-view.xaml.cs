using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace item_eyez
{
    public partial class organize_view : UserControl
    {
        private Point _startPoint;
        private TreeViewItem? _highlighted;
        private readonly ItemEyezDatabase _db = ItemEyezDatabase.Instance();
        private readonly HashSet<HierarchyNode> _selectedNodes = new();
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
                    var node = element.DataContext as HierarchyNode;
                    if (node == null) return;
                    if (!_selectedNodes.Contains(node))
                    {
                        ClearSelection();
                        node.IsSelected = true;
                        _selectedNodes.Add(node);
                    }
                    DragDrop.DoDragDrop(element, _selectedNodes.ToList(), DragDropEffects.Move);
                }
            }
        }

        private void Tree_Drop(object sender, DragEventArgs e)
        {
            var nodes = e.Data.GetData(typeof(List<HierarchyNode>)) as List<HierarchyNode>;
            if (nodes == null)
            {
                var single = e.Data.GetData(typeof(HierarchyNode)) as HierarchyNode;
                if (single == null) return;
                nodes = new List<HierarchyNode> { single };
            }

            var treeView = (TreeView)sender;
            var targetItem = GetContainerFromEvent(treeView, e.OriginalSource as DependencyObject);
            var vm = (OrganizeViewModel)DataContext;

            _db.BeginBatch();
            try
            {

            foreach (var node in nodes)
            {
                RemoveNode(vm.Roots, node);
                RemoveNode(vm.RightRoots, node);
            }

                if (targetItem == null)
                {
                    // dropping on empty space adds to root
                    var list = treeView == tree ? vm.Roots : vm.RightRoots;
                    foreach (var node in nodes)
                        list.Add(node);
                    e.Handled = true;
                }
                else
                {
                    var target = targetItem.DataContext as HierarchyNode;
                    if (target == null) return;

                    foreach (var node in nodes)
                    {
                        if (node == target) continue;

                        if (node.Entity is Room && target.Entity is Container)
                            continue; // containers cannot hold rooms

                        target.Children.Add(node);
                        target.IsExpanded = true;

                        if (node.Entity is Item item)
                        {
                            if (target.Entity is Container tc)
                                item.ContainedIn = tc;
                            else if (target.Entity is Room tr)
                                item.StoredIn = tr;
                        }
                        else if (node.Entity is Container sc)
                        {
                            if (target.Entity is Container tc)
                                sc.ContainedIn = tc;
                            else if (target.Entity is Room tr)
                                sc.StoredIn = tr;
                        }
                        else if (node.Entity is Room sr && target.Entity is Room tr2)
                        {
                            // rooms nested inside rooms are allowed; nothing to do in DB
                        }
                    }

                    vm.RemoveRightFromRoots();
                }
            }
            finally
            {
                _db.EndBatch();
            }

            ClearSelection();
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

        private void ClearSelection()
        {
            foreach (var n in _selectedNodes.ToList())
            {
                n.IsSelected = false;
            }
            _selectedNodes.Clear();
        }

        private void TreeViewItem_LeftClick(object sender, MouseButtonEventArgs e)
        {
            var item = (TreeViewItem)sender;
            var node = item.DataContext as HierarchyNode;
            if (node == null) return;

            if (!node.IsSelected)
            {
                ClearSelection();
                node.IsSelected = true;
                _selectedNodes.Add(node);
            }

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

        private void TreeViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = (TreeViewItem)sender;
            var node = item.DataContext as HierarchyNode;
            if (node == null) return;

            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (node.IsSelected)
                {
                    node.IsSelected = false;
                    _selectedNodes.Remove(node);
                }
                else
                {
                    node.IsSelected = true;
                    _selectedNodes.Add(node);
                }
            }
            else
            {
                ClearSelection();
                node.IsSelected = true;
                _selectedNodes.Add(node);
            }
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

        private void Tree_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeView = (TreeView)sender;
            if (GetContainerFromEvent(treeView, e.OriginalSource as DependencyObject) == null)
            {
                ClearSelection();
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
