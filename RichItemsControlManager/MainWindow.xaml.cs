using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Wpf = System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using XcWpfControlLib.DataXml;
using XcWpfControlLib.Control;
using WPF.JoshSmith.ServiceProviders.UI;
using System.Globalization;
using System.Collections.ObjectModel;
using WinForm = System.Windows.Forms;
using System.IO;
using System.Collections.Specialized;
using WPF.JoshSmith.Controls.Utilities;

namespace RichItemsControlManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ListViewDragDropManager<RichItemViewModel> dragMgr;
        /// <summary>
        /// 图片存储根目录
        /// </summary>
        //string imgDir;
        /// <summary>
        /// 配置文件路径
        /// </summary>
        //string configPath;
        ObservableCollection<RichItemViewModel> itemsSource;
        ObservableCollection<RichItemViewModel> itemsSourceToDisplay;
        public static string ImgsInput;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dragMgr = new ListViewDragDropManager<RichItemViewModel>(listView);
            mainMenu.AddHandler(System.Windows.Controls.MenuItem.ClickEvent, new RoutedEventHandler(MenuItem_Click));
        }

        private void ItemsSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    itemsSourceToDisplay.Add(e.NewItems[0] as RichItemViewModel);
                    break;
                case NotifyCollectionChangedAction.Move:
                    itemsSourceToDisplay.Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    itemsSourceToDisplay.Remove(e.OldItems[0] as RichItemViewModel);
                    break;
            }
        }

        private void listView_SelectionChanged(object sender, Wpf.SelectionChangedEventArgs e)
        {
            dataPanel.DataContext = listView.SelectedItem;
            ImgsInput = imgsTextBox.Value;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Wpf.HeaderedItemsControl menuItem = e.OriginalSource as Wpf.HeaderedItemsControl;
            if (menuItem != null)
            {
                switch (menuItem.Header.ToString().TrimStart(' '))
                {
                    case "文本":
                        Add(new TextBoxItemViewModel("请输入类别", "请输入名称", ""));
                        break;
                    case "整数":
                        Add(new TextBoxItemViewModel("请输入类别", "请输入名称", "0"));
                        break;
                    case "小数":
                        Add(new TextBoxItemViewModel("请输入类别", "请输入名称", "0.0"));
                        break;
                    case "文本下拉单选框":
                        Add(new StringComboBoxItemViewModel("请输入类别", "请输入名称", "示例 1", new string[] { "示例 1", "示例 2" }, StringComboBoxType.Single));
                        break;
                    case "文本下拉多选框":
                        Add(new StringComboBoxItemViewModel("请输入类别", "请输入名称", "示例 1;示例 2", new string[] { "示例 1", "示例 2" }, StringComboBoxType.Multiple));
                        break;
                    case "图片下拉单选框":
                        Add(new ImageComboBoxItemViewModel("请输入类别", "请输入名称", 1, new ImageComboBoxItemViewModel.ImageAttribute[] { new ImageComboBoxItemViewModel.ImageAttribute(1, "名称 1", "描述 1", "image1.jpg"), new ImageComboBoxItemViewModel.ImageAttribute(2, "名称 2", "描述 2", "image2.jpg") }));
                        break;
                    case "新建":
                        New();
                        break;
                    case "打开":
                        Open();
                        break;
                    case "保存":
                        Save();
                        break;
                }
            }
        }

        private void Save()
        {
            appDirectory.Update();
            string[] directorys = appDirectory.Value.TrimEnd(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
            string configPath = Path.Combine(appDirectory.Value, directorys[directorys.Length - 1] + ".xml");
            string imgDir = Path.Combine(appDirectory.Value, "Image");
            Directory.CreateDirectory(imgDir);
            richItemControl.ImageDir = imgDir;
            RichItemsControlXmlUtil.Write(itemsSource, configPath);
        }

        private void New()
        {
            WinForm.FolderBrowserDialog folderDialg = new WinForm.FolderBrowserDialog();
            folderDialg.ShowNewFolderButton = true;
            if (folderDialg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                appDirectory.Value = folderDialg.SelectedPath;
                itemsSource = new ObservableCollection<RichItemViewModel>();
                itemsSource.CollectionChanged += ItemsSource_CollectionChanged;
                listView.ItemsSource = itemsSource;// ListView列表初始化
                itemsSourceToDisplay = new ObservableCollection<RichItemViewModel>(itemsSource);
                richItemControl.ItemsSource = itemsSourceToDisplay;
                addMenuItem.IsEnabled = true;
                saveMenuItem.IsEnabled = true;
            }
        }

        private void Open()
        {
            WinForm.OpenFileDialog openFileDialg = new WinForm.OpenFileDialog();
            openFileDialg.Filter = "xml文件格式 (*.xml)|*.xml|任何文件格式 (*.*)|*.*";
            if (openFileDialg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                appDirectory.Value = Path.GetDirectoryName(openFileDialg.FileName);
                itemsSource = RichItemsControlXmlUtil.Read(openFileDialg.FileName);
                itemsSource.CollectionChanged += ItemsSource_CollectionChanged;
                listView.ItemsSource = itemsSource;// ListView列表初始化
                itemsSourceToDisplay = new ObservableCollection<RichItemViewModel>(itemsSource);
                richItemControl.ItemsSource = itemsSourceToDisplay;
                richItemControl.ImageDir = Path.Combine(appDirectory.Value, "Image");
                addMenuItem.IsEnabled = true;
                saveMenuItem.IsEnabled = true;
            }
        }

        private void Add(RichItemViewModel item)
        {
            itemsSource.Add(item);
            listView.ScrollIntoView(item);
            listView.SelectedItem = item;
        }

        private void img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Wpf.Image img = (Wpf.Image)sender;
            itemsSource.Remove((RichItemViewModel)img.Tag);
            dataPanel.DataContext = null;
            dragMgr.CancelDrag();// 取消列表拖拽事件
        }

        string clipBoardTxt = string.Empty;
        private void ListView_ContextMenuOpening(object sender, Wpf.ContextMenuEventArgs e)
        {
            Wpf.ListView listView = (Wpf.ListView)sender;
            if (listView.SelectedIndex != -1)
            {
                Wpf.ListViewItem item = listView.ItemContainerGenerator.ContainerFromIndex(listView.SelectedIndex) as Wpf.ListViewItem;
                foreach (Wpf.TextBlock txtBlock in VisualUtil.FindChildren<Wpf.TextBlock>(item))
                {
                    Point mousePos = MouseUtilities.GetMousePosition(txtBlock);
                    Rect bounds = VisualTreeHelper.GetDescendantBounds(txtBlock);
                    if (bounds.Contains(mousePos))
                    {
                        clipBoardTxt = txtBlock.Text;
                        break;
                    }
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(clipBoardTxt))
                Clipboard.SetDataObject(clipBoardTxt);
        }
    }

    #region 值转换器
    class StringSourceVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is StringComboBoxItemViewModel ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class StringSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<string> itemsSource = value as IEnumerable<string>;
            StringBuilder sb = new StringBuilder();
            foreach (var item in itemsSource)
            {
                sb.Append(item + '\n');
            }
            return sb.ToString().TrimEnd('\n');
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value).Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }

    class ImageSourceVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is ImageComboBoxItemViewModel ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class ImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<ImageComboBoxItemViewModel.ImageAttribute> itemsSource = value as IEnumerable<ImageComboBoxItemViewModel.ImageAttribute>;
            StringBuilder sb = new StringBuilder();
            foreach (var item in itemsSource)
            {
                sb.Append(item.Id.ToString() + ", " + item.Name + ", " + item.Description + ", " + item.Path + '\n');
            }
            return sb.ToString().TrimEnd('\n');
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] imageStrings = ((string)value).Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            List<ImageComboBoxItemViewModel.ImageAttribute> itemsSource = new List<ImageComboBoxItemViewModel.ImageAttribute>();
            foreach (var s in imageStrings)
            {
                string[] attributes = s.Split(',');
                if (attributes.Length == 4)
                {
                    itemsSource.Add(new ImageComboBoxItemViewModel.ImageAttribute(int.Parse(attributes[0]), attributes[1].TrimStart(' '), attributes[2].TrimStart(' '), attributes[3].TrimStart(' ')));
                }
            }
            return itemsSource;
        }
    }

    class ImgTextBoxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? value.ToString() : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(ImageComboBoxItemViewModel.ImageAttribute))
            {
                if (!string.IsNullOrWhiteSpace(MainWindow.ImgsInput))
                {
                    string id = (string)value;
                    if (string.IsNullOrWhiteSpace(id)) return null;
                    string[] imgAttrs = MainWindow.ImgsInput.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var imgAttr in imgAttrs)
                    {
                        if (imgAttr.StartsWith(id))
                        {
                            string[] attr = imgAttr.Split(',');
                            return new ImageComboBoxItemViewModel.ImageAttribute(int.Parse(attr[0]), attr[1].TrimStart(' '), attr[2].TrimStart(' '), attr[3].TrimStart(' '));
                        }
                    }
                }
                return null;
            }
            return string.IsNullOrWhiteSpace((string)value) ? null : value;
        }
    }
    #endregion
}
