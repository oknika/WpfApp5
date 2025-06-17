using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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
using WpfApp5.Helper;

namespace WpfApp5.Views
{
    /// <summary>
    /// Interaction logic for ReportPreview.xaml
    /// </summary>
    public partial class ReportPreview : Window
    {
        public ReportPreview(FixedDocument document)
        {
            InitializeComponent();
            DocumentViewer.Document = document;
        }
    }
}
