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
using WpfApp5.Models;
using Application = System.Windows.Application;

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
            if(IsAnAnalysisExpanded)
            {
                IsAnAnalysisExpanded = false;
            }
            if(IsAnReportExpanded)
            {
                IsAnReportExpanded = false;
            }
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

        /// <summary>
        /// Command to handle child menu actions.
        /// </summary>
        /// <param name="destination"></param>
        [RelayCommand]
        private void ChildEdit(string? destination)
        {
            switch (destination)
            {
                case "PurchaseOrder":
                    IsEDPurchaseOrder = true;

                    // Check if the window is already open
                    foreach (Window xwindow in Application.Current.Windows)
                    {
                        if (xwindow is FrmEDPurchaseOrder)
                        {
                            xwindow.Activate(); // Bring it to the front
                            return; // Prevent opening another one
                        }
                    }

                    // If not open, create a new instance
                    var window = new FrmEDPurchaseOrder();
                    window.Owner = Application.Current.MainWindow;
                    window.ShowInTaskbar = false;
                    // Attach a closed event handler to reset the property
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

                        // Create a ViewModel instance to load data
                        var vm = new SimpleReportVM();
                        // Load data from SQL
                        vm.LoadDataFromSQL();
                        var groupedData = vm?.GroupedProducts;

                        // If the data is not loaded, show a message and return
                        var report = new SimpleReport
                        {
                            DataContext = vm
                        };

                        // Define the A4 size in pixels (assuming 96 DPI)
                        var a4Size = new System.Windows.Size(793.7, 1122.5);

                        foreach (Window xwindow in Application.Current.Windows)
                        {
                            if (xwindow is ReportPreview)
                            {
                                xwindow.Activate(); // Bring it to the front
                                return; // Prevent opening another one
                            }
                        }

                        var doc = CommonReportBuilder.BuildGroupedPaginationDocument<ProductGroup, Product>(
                            groups: groupedData,
                            getGroupItems: g => g.Products,
                            createGroupHeader: ReportTemplates.CreateProductGroupHeader,
                            createItemRow: ReportTemplates.CreateProductGroupedRow,
                            pageSize: new System.Windows.Size(793.7, 1122.5),
                            reportTitle: "Grouped Product Report",
                            createFirstPageHeader: () => ReportTemplates.CreateBigTitleHeader("Product Report: Grouped"),
                            createRegularPageHeader: () => ReportTemplates.CreateSmallTitleHeader("Product Report: Grouped"),
                            createHeaderRow: ReportTemplates.CreateProductColumnHeader
                        );

                        // Create a new ReportPreview window
                        var previewWindow = new ReportPreview(doc)
                        {
                            Owner = Application.Current.MainWindow,
                            ShowInTaskbar = false
                        };
                        previewWindow.Closed += (s, e) => { IsAnProductsReport = false; };
                        previewWindow.ShowDialog();
                        break;
                    }
                case "AnSampleReport":
                    {
                        IsAnSampleReport = true;

                        // Create a ViewModel instance to load data
                        var vm = new SimpleReportVM();
                        // Load data from SQL
                        vm.LoadDataFromSQL();

                        // If the data is not loaded, show a message and return
                        var report = new SimpleReport
                        {
                            DataContext = vm
                        };

                        // Define the A4 size in pixels (assuming 96 DPI)
                        var a4Size = new System.Windows.Size(793.7, 1122.5);

                        // Check if the ReportPreview window is already open
                        foreach (Window xwindow in Application.Current.Windows)
                        {
                            if (xwindow is ReportPreview)
                            {
                                xwindow.Activate();
                                return;
                            }
                        }

                        // Build paginated report from the ViewModel data
                        var doc = CommonReportBuilder.BuildPaginationDocument(
                            items: vm.AllProducts,
                            createPageContent: products =>
                            {
                                var stack = new StackPanel();
                                foreach (var p in products)
                                    stack.Children.Add(ReportTemplates.CreateProductRow(p));
                                return stack;
                            },
                            pageSize: a4Size,
                            reportTitle: "Product Report",
                            createFirstPageHeader: () => ReportTemplates.CreateBigTitleHeader("Product Report: Flat"),
                            createRegularPageHeader: () => ReportTemplates.CreateSmallTitleHeader("Product Report: Flat"),
                            createHeaderRow: () => ReportTemplates.CreateProductHeader()
                        );

                        // Create a new ReportPreview window
                        var previewWindow = new ReportPreview(doc)
                        {
                            Owner = Application.Current.MainWindow,
                            ShowInTaskbar = false
                        };

                        // Attach a closed event handler to reset the property
                        previewWindow.Closed += (s, e) => { IsAnSampleReport = false; };
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

        // Command to toggle the sidebar visibility.
        [RelayCommand]
        private void ToggleSidebar()
        {
            Debug.WriteLine("Sending ToggleSidebarMessage");
            // Send a message to toggle the sidebar visibility.
            WeakReferenceMessenger.Default.Send(new ToggleSidebarMessage(true));
        }
    }
}
