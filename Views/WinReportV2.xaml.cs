using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using WpfApp5.Helper;
using WpfApp5.Models;

namespace WpfApp5.Views
{
    /// <summary>
    /// Interaction logic for WinReport.xaml
    /// </summary>
    public partial class WinReportV2 : Window
    {
        private readonly string _tempRdlcPath;

        public WinReportV2(IEnumerable<Product> products, bool ShowGrp)
        {
            InitializeComponent();

            var viewer = new ReportViewer
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ProcessingMode = ProcessingMode.Local
            };

            var path = RdlcModifier.ModifyRdlc("Reports/DynamicProductReport.rdlc", showGrouping: ShowGrp, showPgHeader: true);
            _tempRdlcPath = path;

            // Load RDLC
            //viewer.LocalReport.ReportPath = "Report/ReportCoba.rdlc";
            viewer.LocalReport.ReportPath = path;

            // Add data source
            viewer.LocalReport.DataSources.Clear();
            viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetCoba", products));

            viewer.LocalReport.SetParameters(new[]
            {
                new ReportParameter("ShowGrouping", ShowGrp.ToString()) // Or "False"
            });

            viewer.SetDisplayMode(DisplayMode.PrintLayout);
            viewer.ZoomMode = ZoomMode.Percent;
            viewer.ZoomPercent = 100;

            // Refresh
            viewer.RefreshReport();

            // Attach to host
            ReportHost.Child = viewer;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // 🧽 Clean up temp file safely
            try
            {
                if (File.Exists(_tempRdlcPath))
                    File.Delete(_tempRdlcPath);
            }
            catch (Exception ex)
            {
                // Optional: log or ignore
                Debug.WriteLine("Temp file deletion failed: " + ex.Message);
            }
        }
    }
}
