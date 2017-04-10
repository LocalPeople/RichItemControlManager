using System;
using System.Collections.Generic;
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
using XcWpfControlLib.DataXml;
using XcWpfControlLib.Control;
using WPF.JoshSmith.ServiceProviders.UI;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.IO;

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
        string imgDir;
        /// <summary>
        /// 配置文件路径
        /// </summary>
        string configPath;
        ObservableCollection<RichItemViewModel> itemsSource;
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

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dataPanel.DataContext = listView.SelectedItem;
            ImgsInput = imgsTextBox.Value;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            HeaderedItemsControl menuItem = e.OriginalSource as HeaderedItemsControl;
            if (menuItem != null)
            {
                switch (menuItem.Header.ToString().TrimStart(' '))
                {
                    case "普通文本框":
                        Add(new TextBoxItemViewModel("请输入类别", "请输入名称", ""));
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
            RichItemsControlXmlUtil.Write(itemsSource, configPath);
        }

        private void New()
        {
            SaveFileDialog saveFileDialg = new SaveFileDialog();
            saveFileDialg.AddExtension = true;
            saveFileDialg.DefaultExt = "xml";
            saveFileDialg.Filter = "xml文件格式 (*.xml)|*.xml";
            if (saveFileDialg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                configPath = saveFileDialg.FileName;
                imgDir = Path.Combine(Path.GetDirectoryName(configPath), "Image");
                Directory.CreateDirectory(imgDir);
                itemsSource = new ObservableCollection<RichItemViewModel>();
                listView.ItemsSource = itemsSource;// ListView列表初始化
                richItemControl.ImageDir = imgDir;// 界面预览初始化
                richItemControl.ItemsSource = itemsSource;
                addMenuItem.IsEnabled = true;
                saveMenuItem.IsEnabled = true;
            }
        }

        private void Open()
        {
            OpenFileDialog openFileDialg = new OpenFileDialog();
            openFileDialg.Filter = "xml文件格式 (*.xml)|*.xml|任何文件格式 (*.*)|*.*";
            if (openFileDialg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                configPath = openFileDialg.FileName;
                imgDir = Path.Combine(Path.GetDirectoryName(configPath), "Image");
                itemsSource = RichItemsControlXmlUtil.Read(configPath);
                listView.ItemsSource = itemsSource;// ListView列表初始化
                richItemControl.ImageDir = imgDir;// 界面预览初始化
                richItemControl.ItemsSource = itemsSource;
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
            Image img = (Image)sender;
            itemsSource.Remove((RichItemViewModel)img.Tag);
            dataPanel.DataContext = null;
            dragMgr.CancelDrag();// 取消列表拖拽事件
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
