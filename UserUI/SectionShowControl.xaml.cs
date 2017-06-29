using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using UserUI.Event;
using XcWpfControlLib.Control;
using XcWpfControlLib.DataXml;

namespace UserUI
{
    /// <summary>
    /// SectionShowControl.xaml 的交互逻辑
    /// </summary>
    public partial class SectionShowControl : UserControl
    {
        static ObservableCollection<RichItemViewModel> EmptyCollection = new ObservableCollection<RichItemViewModel>();
        Data.Section unSaveSection;

        public event EventHandler<SectionChangedEventArgs> SectionChanged;

        public SectionShowControl()
        {
            InitializeComponent();
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveLastSection();
            if (e.AddedItems.Count > 0)
            {
                InitSectionUi(e.AddedItems[0] as Data.Section);
                unSaveSection = e.AddedItems[0] as Data.Section;
                SectionChanged?.Invoke(this, new SectionChangedEventArgs((Data.Section)e.AddedItems[0], e.RemovedItems.Count > 0 ? (Data.Section)e.RemovedItems[0] : null));
            }
        }

        private void InitSectionUi(Data.Section section)
        {
            image.Source = new BitmapImage(new Uri(section.Configuration.Image));
            if (!string.IsNullOrEmpty(section.Configuration.File))
            {
                richItemsControl.ItemsSource = RichItemsControlXmlUtil.Read(section.Configuration.File);
                richItemsControl.ImageDir = System.IO.Path.Combine(section.Directory.FullName, "Image");
            }
            else
            {
                richItemsControl.ItemsSource = EmptyCollection;
            }
        }

        public void SetCaption(string caption)
        {
            this.caption.Text = "当前编辑项：" + caption;
        }

        public void SetComboBoxSelectedItem(object item)
        {
            comboBox.SelectedItem = item;
        }

        public void SaveLastSection()
        {
            if (unSaveSection != null && !string.IsNullOrEmpty(unSaveSection.Configuration.File))
            {
                RichItemsControlXmlUtil.Write(richItemsControl.ItemsSource, unSaveSection.Configuration.File);
                unSaveSection = null;
            }
        }
    }
}
