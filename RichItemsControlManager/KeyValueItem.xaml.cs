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
using System.Windows.Shapes;

namespace RichItemsControlManager
{
    /// <summary>
    /// KeyValueItem.xaml 的交互逻辑
    /// </summary>
    public partial class KeyValueItem : UserControl
    {
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(string), typeof(KeyValueItem));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(KeyValueItem));

        public string Key
        {
            get { return (string)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public KeyValueItem()
        {
            InitializeComponent();
        }
    }
}
