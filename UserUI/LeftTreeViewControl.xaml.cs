using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using UserUI.Control;
using UserUI.Util;
using UserUI.XML;

namespace UserUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LeftTreeViewControl : Window, INotifyPropertyChanged
    {
        List<Data.Section> sectionGroup;
        DataXmlManager dataXmlMgr;

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
            string package = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(LeftTreeViewControl).Assembly.Location), "节点图");
            Data.Section root = DataReader.Read(package, 0);
            if (DataXmlManager.Exist(package))
            {
                dataXmlMgr = new DataXmlManager(package);
            }
            else
            {
                dataXmlMgr = new DataXmlManager(root, package);
            }
            InitTreeView(treeView, root);
            SectionGroup = Data.Section.EmptyGroup;
        }

        private void InitTreeView(ItemsControl treeView, Data.Section root)
        {
            foreach (Data.Section child in root.Group)
            {
                ExpandTreeViewItem item = new ExpandTreeViewItem();
                treeView.Items.Add(item);
                if (child.Group.Count > 0 && child.Group[0].IsLeap)
                {
                    item.Tag = child.Group;
                    item.Header = child.Name + string.Format("  (当前配置：{0})", dataXmlMgr.GetLeapName(child));
                }
                else
                {
                    item.Header = child.Name;
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
                Match match = Regex.Match(item.Header as string, "(\\S+)\\s{2}\\(当前配置：(\\S+)\\)");
                sectionShowControl.SetCaption(match.Groups[1].Value);
                sectionShowControl.SetComboBoxSelectedItem(sectionGroup.First(sec => sec.Name == match.Groups[2].Value));
            }
        }

        private void sectionShowControl_SectionChanged(object sender, Event.SectionChangedEventArgs e)
        {
            if (e.OldSection != null)
            {
                TreeViewItem item = treeView.SelectedItem as TreeViewItem;
                item.Header = ((string)item.Header).Replace(e.OldSection.Name, e.NewSection.Name);
                dataXmlMgr.SetLeapName(e.NewSection.Parent, e.NewSection.Name);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            treeView.Focus();// 保证RichItemsControl的数据得到更新
            sectionShowControl.SaveLastSection();
        }

        private void treeView_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            treeView.Focus();
        }
    }
}
