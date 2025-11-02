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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Item_eyez.Database;
using Item_eyez.Viewmodels;

namespace Item_eyez.Views
{
    /// <summary>
    /// The organize view.
    /// </summary>
    /// <seealso cref="System.Windows.Controls.UserControl" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    /// <seealso cref="System.Windows.Markup.IStyleConnector" />
    public partial class Organize_view : UserControl
    {
        /// <summary>
        /// The database.
        /// </summary>
        private readonly ItemEyezDatabase db = ItemEyezDatabase.Instance();

        /// <summary>
        /// The selected nodes.
        /// </summary>
        private readonly HashSet<HierarchyNode> selectedNodes = [];

        /// <summary>
        /// The highlighted.
        /// </summary>
        private TreeViewItem? highlighted;

        /// <summary>
        /// The start point.
        /// </summary>
        private Point startPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="Organize_view"/> class.
        /// </summary>
        public Organize_view() => this.InitializeComponent();

        /// <summary>
        /// Gets the container from event.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="source">The source.</param>
        /// <returns>
        /// The nullable.
        /// </returns>
        private static TreeViewItem? GetContainerFromEvent(ItemsControl container, DependencyObject? source)
        {
            while (source != null && source != container)
            {
                if (source is TreeViewItem item)
                {
                    return item;
                }

                source = VisualTreeHelper.GetParent(source);
            }

            return null;
        }

        /// <summary>
        /// Removes the node.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="node">The node.</param>
        /// <returns>
        /// The boolean.
        /// </returns>
        private static bool RemoveNode(ObservableCollection<HierarchyNode> list, HierarchyNode node)
        {
            if (list.Remove(node))
            {
                return true;
            }

            foreach (HierarchyNode child in list)
            {
                if (RemoveNode(child.Children, node))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Handles the OnClick event of the AddContainer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AddContainer_OnClick(object sender, RoutedEventArgs e)
        {
            Guid id = this.db.AddContainer("New Container", string.Empty);
            if ((sender as MenuItem)?.DataContext is Container parent)
            {
                this.db.AssociateItemWithContainer(id, parent.Id);
            }
        }

        /// <summary>
        /// Handles the Click event of the AddContainerRoot control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AddContainerRoot_Click(object sender, RoutedEventArgs e) => this.AddContainer_OnClick(sender, e);

        /// <summary>
        /// Handles the OnClick event of the AddItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AddItem_OnClick(object sender, RoutedEventArgs e)
        {
            Guid id = this.db.AddItem("New Item", string.Empty, 0m, string.Empty);
            if ((sender as MenuItem)?.DataContext is Container parent)
            {
                this.db.AssociateItemWithContainer(id, parent.Id);
            }
        }

        /// <summary>
        /// Handles the Click event of the AddItemRoot control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AddItemRoot_Click(object sender, RoutedEventArgs e) => this.AddItem_OnClick(sender, e);

        /// <summary>
        /// Handles the Click event of the AddRoomRoot control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AddRoomRoot_Click(object sender, RoutedEventArgs e) => this.db.AddRoom("New Room", string.Empty);

        /// <summary>
        /// Clears the selection.
        /// </summary>
        private void ClearSelection()
        {
            foreach (HierarchyNode? n in this.selectedNodes.ToList())
            {
                n.IsSelected = false;
            }

            this.selectedNodes.Clear();
        }

        /// <summary>
        /// Handles the OnClick event of the DeleteNode control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.Exception">Unexpected Case.</exception>
        private void DeleteNode_OnClick(object sender, RoutedEventArgs e)
        {
            if ((sender as MenuItem)?.DataContext is HierarchyNode node)
            {
                switch (node.Entity)
                {
                    case Item item:
                        this.db.DeleteItem(item.Id);
                        break;

                    case Container cont:
                        this.db.DeleteContainer(cont.Id);
                        break;

                    case Room room:
                        this.db.DeleteRoom(room.Id);
                        break;

                    default:
                        throw new Exception("Unexpected Case");
                }
            }
        }

        /// <summary>
        /// Handles the MouseMove event of the DragHandle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void DragHandle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point pos = e.GetPosition(null);
                if (Math.Abs(pos.X - this.startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(pos.Y - this.startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    FrameworkElement element = (FrameworkElement)sender;
                    if (element.DataContext is not HierarchyNode node)
                    {
                        return;
                    }

                    if (!this.selectedNodes.Contains(node))
                    {
                        this.ClearSelection();
                        node.IsSelected = true;
                        _ = this.selectedNodes.Add(node);
                    }

                    _ = DragDrop.DoDragDrop(element, this.selectedNodes.ToList(), DragDropEffects.Move);
                }
            }
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the DragHandle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void DragHandle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.startPoint = e.GetPosition(null);

        /// <summary>
        /// Handles the DragOver event of the Tree control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DragEventArgs"/> instance containing the event data.</param>
        private void Tree_DragOver(object sender, DragEventArgs e)
        {
            TreeView treeView = (TreeView)sender;
            TreeViewItem? item = GetContainerFromEvent(treeView, e.OriginalSource as DependencyObject);
            if (this.highlighted != item)
            {
                if (this.highlighted != null)
                {
                    this.highlighted.Background = Brushes.Transparent;
                }

                this.highlighted = item;
                if (this.highlighted != null)
                {
                    this.highlighted.Background = Brushes.AliceBlue;
                }
            }
        }

        /// <summary>
        /// Handles the Drop event of the Tree control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DragEventArgs"/> instance containing the event data.</param>
        private void Tree_Drop(object sender, DragEventArgs e)
        {
            List<HierarchyNode>? nodes = e.Data.GetData(typeof(List<HierarchyNode>)) as List<HierarchyNode>;
            if (nodes == null)
            {
                if (e.Data.GetData(typeof(HierarchyNode)) is not HierarchyNode single)
                {
                    return;
                }

                nodes = [single];
            }

            TreeView treeView = (TreeView)sender;
            TreeViewItem? targetItem = GetContainerFromEvent(treeView, e.OriginalSource as DependencyObject);
            OrganizeViewModel vm = (OrganizeViewModel)this.DataContext;

            this.db.BeginBatch();
            try
            {
                foreach (HierarchyNode node in nodes)
                {
                    _ = RemoveNode(vm.Roots, node);
                    _ = RemoveNode(vm.RightRoots, node);
                }

                if (targetItem == null)
                {
                    // dropping on empty space adds to root
                    ObservableCollection<HierarchyNode> list = treeView == this.tree ? vm.Roots : vm.RightRoots;
                    foreach (HierarchyNode node in nodes)
                    {
                        list.Add(node);
                    }

                    e.Handled = true;
                }
                else
                {
                    if (targetItem.DataContext is not HierarchyNode target)
                    {
                        return;
                    }

                    foreach (HierarchyNode node in nodes)
                    {
                        if (node == target)
                        {
                            continue;
                        }

                        if (node.Entity is Room && target.Entity is Container)
                        {
                            continue; // containers cannot hold rooms
                        }

                        target.Children.Add(node);
                        target.IsExpanded = true;
                        if (node.Entity is Item item)
                        {
                            if (target.Entity is Container tc)
                            {
                                item.ContainedIn = tc;
                            }
                            else if (target.Entity is Room tr)
                            {
                                item.StoredIn = tr;
                            }
                        }
                        else if (node.Entity is Container sc)
                        {
                            if (target.Entity is Container tc)
                            {
                                sc.ContainedIn = tc;
                            }
                            else if (target.Entity is Room tr)
                            {
                                sc.StoredIn = tr;
                            }
                        }
                        else if (node.Entity is Room && target.Entity is Room)
                        {
                            // rooms nested inside rooms are allowed; nothing to do in DB
                        }
                    }

                    vm.RemoveRightFromRoots();
                }
            }
            finally
            {
                this.db.EndBatch();
            }

            this.ClearSelection();
            vm.RefreshSearch();

            if (this.highlighted != null)
            {
                this.highlighted.Background = Brushes.Transparent;
                this.highlighted = null;
            }
        }

        /// <summary>
        /// Handles the LeftClick event of the Tree control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Tree_LeftClick(object sender, MouseButtonEventArgs e)
        {
            TreeView treeView = (TreeView)sender;
            if (GetContainerFromEvent(treeView, e.OriginalSource as DependencyObject) == null)
            {
                ContextMenu menu = new();
                _ = menu.Items.Add(new MenuItem { Header = "Add Item" });
                ((MenuItem)menu.Items[0]).Click += this.AddItem_OnClick;
                _ = menu.Items.Add(new MenuItem { Header = "Add Container" });
                ((MenuItem)menu.Items[1]).Click += this.AddContainer_OnClick;
                _ = menu.Items.Add(new MenuItem { Header = "Add Room" });
                ((MenuItem)menu.Items[2]).Click += this.AddRoomRoot_Click;
                menu.IsOpen = true;
            }
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the Tree control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Tree_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeView treeView = (TreeView)sender;
            if (GetContainerFromEvent(treeView, e.OriginalSource as DependencyObject) == null)
            {
                this.ClearSelection();
            }
        }

        /// <summary>
        /// Handles the LeftClick event of the TreeViewItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void TreeViewItem_LeftClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.DataContext is not HierarchyNode node)
            {
                return;
            }

            if (!node.IsSelected)
            {
                this.ClearSelection();
                node.IsSelected = true;
                _ = this.selectedNodes.Add(node);
            }

            ContextMenu menu = new();
            if (node.Entity is Container cont)
            {
                MenuItem addItem = new() { Header = "Add Item", DataContext = cont };
                addItem.Click += this.AddItem_OnClick;
                _ = menu.Items.Add(addItem);

                MenuItem addContainer = new() { Header = "Add Container", DataContext = cont };
                addContainer.Click += this.AddContainer_OnClick;
                _ = menu.Items.Add(addContainer);
            }

            _ = menu.Items.Add(new MenuItem { Header = "Delete", DataContext = node, Command = null });
            ((MenuItem)menu.Items[^1]).Click += this.DeleteNode_OnClick;
            menu.IsOpen = true;
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the TreeViewItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void TreeViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.DataContext is not HierarchyNode node)
            {
                return;
            }

            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (node.IsSelected)
                {
                    node.IsSelected = false;
                    _ = this.selectedNodes.Remove(node);
                }
                else
                {
                    node.IsSelected = true;
                    _ = this.selectedNodes.Add(node);
                }
            }
            else
            {
                this.ClearSelection();
                node.IsSelected = true;
                _ = this.selectedNodes.Add(node);
            }
        }
    }
}