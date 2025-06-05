using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace item_eyez
{
    public partial class organize_view : UserControl
    {
        private Point _startPoint;
        private TreeViewItem? _highlighted;
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
            if (targetItem == null) return;
            var target = targetItem.DataContext as HierarchyNode;
            if (target == null || target == source) return;

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

            ((OrganizeViewModel)DataContext).Load();

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
                    _highlighted.Background = Brushes.LightSkyBlue;
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
    }
}
