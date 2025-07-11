using Microsoft.Reporting.WinForms;
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
    /// Interaction logic for WinReport.xaml
    /// </summary>
    public partial class WinReport : Window
    {
        public WinReport(IEnumerable<Product> products)
        {
            InitializeComponent();

            var viewer = new ReportViewer
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ProcessingMode = ProcessingMode.Local
            };

            // Load RDLC
            viewer.LocalReport.ReportPath = "Reports/ReportCoba.rdlc";

            // Add data source
            viewer.LocalReport.DataSources.Clear();
            viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetCoba", products));

            viewer.SetDisplayMode(DisplayMode.PrintLayout); // ← THIS LINE
            viewer.ZoomMode = ZoomMode.Percent;
            viewer.ZoomPercent = 100;

            // Refresh
            viewer.RefreshReport();

            // Attach to host
            ReportHost.Child = viewer;
        }
    }
}
