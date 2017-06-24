using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public SectionShowControl()
        {
            InitializeComponent();
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Data.Section section = comboBox.SelectedItem as Data.Section;
            if (section != null)
            {
                InitSectionUi(section);
            }
        }

        private void InitSectionUi(Data.Section section)
        {
            image.Source = new BitmapImage(new Uri(section.Configuration.Image));
            richItemsControl.ItemsSource = RichItemsControlXmlUtil.Read(section.Configuration.File);
            richItemsControl.ImageDir = System.IO.Path.Combine(section.Directory.FullName, "Image");
        }
    }
}
