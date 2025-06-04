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
    /// Interaction logic for LTM.xaml
    /// </summary>
    public partial class LTM : Window
    {
        public LTM()
        {
            InitializeComponent();

            var vm = new LTMVM();
            this.DataContext = vm;

            vm.RequestClose += (dialogResult) =>
            {
                Dispatcher.Invoke(() =>
                {
                    this.DialogResult = dialogResult;
                    this.Close();
                });
            };

            /*if (DataContext is LTMVM vm)
            {
                vm.RequestClose += (dialogResult) =>
                {
                    this.DialogResult = dialogResult;
                    this.Close();
                };
            }*/
        }
    }
}
