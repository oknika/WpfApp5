using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using WpfApp5.General;
using WpfApp5.Views;
using WpfApp5.Helper;

namespace WpfApp5.ViewModels
{
    public partial class SidebarMenuVM : ObservableObject
    {
        [ObservableProperty]
        private bool isAnalysisExpanded;
        [ObservableProperty]
        private bool isEditDataExpanded;
        [ObservableProperty]
        private bool isSettingsExpanded;
        [ObservableProperty]
        private bool isAnAnalysisExpanded;
        [ObservableProperty]
        private bool isAnReportExpanded;
        [ObservableProperty]
        private bool isReturnExpanded;


        [ObservableProperty]
        private bool isEDPurchaseOrder;
        [ObservableProperty]
        private bool isAnProducts;
        [ObservableProperty]
        private bool isAnProductsReport;
        [ObservableProperty]
        private bool isAnSampleReport;

        [RelayCommand]
        private void ToggleAnalysis()
        {
            IsAnalysisExpanded = !IsAnalysisExpanded;
        }

        [RelayCommand]
        private void ToggleAnAnalysis()
        {
            IsAnAnalysisExpanded = !IsAnAnalysisExpanded;
        }

        [RelayCommand]
        private void ToggleAnReport()
        {
            IsAnReportExpanded = !IsAnReportExpanded;
        }

        [RelayCommand]
        private void ToggleEditData()
        {
            if (IsReturnExpanded)
            {
                IsReturnExpanded = false;
            }
            IsEditDataExpanded = !IsEditDataExpanded;
        }

        [RelayCommand]
        private void ToggleSettings()
        {
            IsSettingsExpanded = !IsSettingsExpanded;
        }

        [RelayCommand]
        private void ToggleReturn()
        {
            IsReturnExpanded = !IsReturnExpanded;
        }

        [RelayCommand]
        private void ChildEdit(string? destination)
        {
            switch (destination)
            {
                case "PurchaseOrder":
                    IsEDPurchaseOrder = true;

                    foreach (Window xwindow in Application.Current.Windows)
                    {
                        if (xwindow is FrmEDPurchaseOrder)
                        {
                            xwindow.Activate(); // Bring it to the front
                            return; // Prevent opening another one
                        }
                    }

                    var window = new FrmEDPurchaseOrder();
                    window.Owner = Application.Current.MainWindow;
                    window.ShowInTaskbar = false;
                    window.Closed += (s, e) =>
                    {
                        IsEDPurchaseOrder = false;
                    };
                    window.Show();
                    break;

                case "SalesOrder":
                    break;

                case "Return":
                    break;
            }
        }

        [RelayCommand]
        private void ChildAnalysis(string? destination)
        {
            Window window = null;

            switch (destination)
            {
                case "AnProducts":
                    {
                        IsAnProducts = true;

                        foreach (Window xwindow in Application.Current.Windows)
                        {
                            if (xwindow is FrmDAProducts)
                            {
                                xwindow.Activate(); // Bring it to the front
                                return; // Prevent opening another one
                            }
                        }

                        window = new FrmDAProducts();
                        window.Owner = Application.Current.MainWindow;
                        window.ShowInTaskbar = false;
                        window.Closed += (s, e) =>
                        {
                            IsAnProducts = false;
                        };
                        window.Show();
                        break;
                    }
                case "AnProductsReport":
                    {
                        IsAnProductsReport = true;

                        foreach (Window xwindow in Application.Current.Windows)
                        {
                            if (xwindow is RptDAProducts)
                            {
                                xwindow.Activate(); // Bring it to the front
                                return; // Prevent opening another one
                            }
                        }

                        window = new RptDAProducts();
                        window.Owner = Application.Current.MainWindow;
                        window.ShowInTaskbar = false;
                        window.Closed += (s, e) =>
                        {
                            IsAnProductsReport = false;
                        };
                        window.ShowDialog();
                        break;
                    }
                case "AnSampleReport":
                    {
                        IsAnSampleReport = true;

                        /*var dlg = new PrintDialog();
                        if (dlg.ShowDialog() != true)
                            return;*/

                        var report = new SimpleReport(); // Your report UserControl

                        var a4Size = new Size(793.7, 1122.5); // A4 in DIPs

                        // Arrange for printing
                        //report.Measure(new Size(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight));
                        //report.Arrange(new Rect(new Point(0, 0), report.DesiredSize));
                        /*report.Measure(a4Size);
                        report.Arrange(new Rect(new Point(0, 0), a4Size));

                        var doc = new FixedDocument();
                        var page = new PageContent();
                        var fixedPage = new FixedPage
                        {
                            Width = a4Size.Width,
                            Height = a4Size.Height
                        };

                        fixedPage.Children.Add(report);
                        ((IAddChild)page).AddChild(fixedPage);
                        doc.Pages.Add(page);*/

                        foreach (Window xwindow in Application.Current.Windows)
                        {
                            if (xwindow is ReportPreview)
                            {
                                xwindow.Activate(); // Bring it to the front
                                return; // Prevent opening another one
                            }
                        }

                        var doc = PrintPreviewBuilder.BuildFixedDocument(report, a4Size, "Report: Products");
                        var previewWindow = new ReportPreview(doc); 
                        
                        previewWindow.Owner = Application.Current.MainWindow;
                        previewWindow.ShowInTaskbar = false;
                        previewWindow.Closed += (s, e) =>
                        {
                            IsAnSampleReport = false;
                        };
                        previewWindow.ShowDialog();
                        break;
                    }
            }
        }

        [RelayCommand]
        private void Child3()
        {
            // Your logic for Child Button 3
        }

        [RelayCommand]
        private void ToggleSidebar()
        {
            Debug.WriteLine("Sending ToggleSidebarMessage");
            WeakReferenceMessenger.Default.Send(new ToggleSidebarMessage(true));
        }
    }
}
