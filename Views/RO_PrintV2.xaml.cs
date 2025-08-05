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
using WpfApp5.Services;
using WpfApp5.ViewModels;

namespace WpfApp5.Views
{
    /// <summary>
    /// Interaction logic for RO_Print.xaml
    /// </summary>
    public partial class RO_PrintV2 : Window
    {
        public RO_PrintV2(List<Product> products, string strButtonCaption)
        {
            InitializeComponent();

            var vm = new RO_PrintV2VM(products, strButtonCaption, new FileDialogService());
            vm.CloseAction = this.Close;

            DataContext = vm;
        }
    }
}
