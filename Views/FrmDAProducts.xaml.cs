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
using WpfApp5.ViewModels;

namespace WpfApp5.Views
{
    /// <summary>
    /// Interaction logic for FrmDAProducts.xaml
    /// </summary>
    public partial class FrmDAProducts : Window
    {
        private readonly FrmDAProductsVM _vm;

        public FrmDAProducts()
        {
            InitializeComponent();

            _vm = new FrmDAProductsVM();
            this.DataContext = _vm;

            // Hook the Loaded event to call InitAsync
            this.Loaded += FrmDAProducts_Loaded;
        }

        private async void FrmDAProducts_Loaded(object sender, RoutedEventArgs e)
        {
            await _vm.InitAsync();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
