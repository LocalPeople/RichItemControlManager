using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserUI.Util;

namespace UserUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LeftTreeViewControl : Window, INotifyPropertyChanged
    {
        List<Data.Section> sectionGroup;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<Data.Section> SectionGroup
        {
            get { return sectionGroup; }
            set
            {
                sectionGroup = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SectionGroup"));
            }
        }

        public LeftTreeViewControl()
        {
            InitializeComponent();
            Data.Section root = DataReader.Read(@"C:\Users\lenovo\Desktop\节点图", 0);
            InitTreeView(treeView, root);
            SectionGroup = Data.Section.EmptyList;
        }

        private void InitTreeView(ItemsControl treeView, Data.Section root)
        {
            foreach (Data.Section child in root.Group)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = child.Name;
                treeView.Items.Add(item);
                if (child.Group.Count > 0 && !child.Group[0].IsTreeViewShow)
                {
                    item.Tag = child.Group;
                }
                else
                {
                    InitTreeView(item, child);
                }
            }
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem item = treeView.SelectedItem as TreeViewItem;
            if (item.Tag != null)
            {
                SectionGroup = item.Tag as List<Data.Section>;
            }
            else
            {
                SectionGroup = Data.Section.EmptyList;
            }
        }
    }
}
