using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UserUI.Control
{
    class ExpandTreeViewItem : TreeViewItem
    {
        public ExpandTreeViewItem()
        {
            Selected += ExpandTreeViewItem_Selected;
            Expanded += TreeViewItem_Expanded;
        }

        private void ExpandTreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            if (Items.Count > 0)
            {
                IsExpanded = true;
            }
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            var query = (Parent as ItemsControl).Items.OfType<ExpandTreeViewItem>().Where(item => item != this);
            foreach (var item in query)
            {
                item.IsExpanded = false;
            }
        }
    }
}
