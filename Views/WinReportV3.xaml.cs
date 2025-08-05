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
using System.Windows.Shapes;
using WpfApp5.Models;

namespace WpfApp5.Views
{
    /// <summary>
    /// Interaction logic for WinReportV3.xaml
    /// </summary>
    public partial class WinReportV3 : Window
    {
        public WinReportV3(IEnumerable<Product> products, string ShowGrp, IEnumerable<GroupedProduct> groupedProducts = null
            , IEnumerable<SubGroupedProduct> subGroupedProducts = null)
        {
            InitializeComponent();
        }
    }
}
