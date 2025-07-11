﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfApp5.Helper;
using WpfApp5.Models;
using WpfApp5.Services;
using WpfApp5.Views;

namespace WpfApp5.ViewModels
{
    public partial class FrmDAProductsVM : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<Product> products = new(); // Collection of products

        [ObservableProperty]
        private decimal totalQty; // Total quantity of all products

        [ObservableProperty]
        private string connString = Properties.Settings.Default.MainConnString; // Connection string from settings

        [ObservableProperty]
        private string keyword; // Search keyword for filtering products

        public ICollectionView ProductsView { get; } // View for filtering and grouping products

        public FrmDAProductsVM()
        {
            // Initialize view and set filter
            ProductsView = CollectionViewSource.GetDefaultView(Products);
            ProductsView.GroupDescriptions?.Add(new PropertyGroupDescription(nameof(Product.ProductGroupDisplay)));
            ProductsView.Filter = FilterProducts;
        }

        public async Task InitAsync()
        {
            await LoadDataFromSQLAsync();

            // Now that data is loaded, you can subscribe safely
            foreach (var p in Products)
                p.PropertyChanged += Product_PropertyChanged;

            UpdateTotalQty();
        }

        [RelayCommand]
        private void Search() // Triggered when the search button is clicked
        {
            RefreshView();
        }

        [RelayCommand]
        private void ClearSearch() // Triggered when the clear search button is clicked
        {
            Keyword = string.Empty;
            RefreshView();
        }

        [RelayCommand]
        private void PrintRdlcV2()
        {
            var popup = new RO_Print(ProductsView.Cast<Product>().ToList(), "Print"); // pass a copy of the data
            popup.Owner = System.Windows.Application.Current.MainWindow;
            popup.ShowInTaskbar = false;
            popup.ShowDialog();
        }

        [RelayCommand]
        private void Export2PDF()
        {
            var popup = new RO_Print(ProductsView.Cast<Product>().ToList(), "Export to PDF"); // pass a copy of the data
            popup.Owner = System.Windows.Application.Current.MainWindow;
            popup.ShowInTaskbar = false;
            popup.ShowDialog();
        }

        [RelayCommand]
        private void PrintRdlc()
            // Triggered when the print RDLC button is clicked
        {
            var currentProducts = ProductsView.Cast<Product>().ToList(); // Get the currently filtered products
            var reportWindow = new WinReport(currentProducts);
            reportWindow.WindowState= WindowState.Maximized; // Maximize the report window
            reportWindow.Show();
        }

        [RelayCommand]
        private void Print() // Triggered when the print button is clicked
        {
            var currentProducts = ProductsView.Cast<Product>().ToList(); // Get the currently filtered products
            var a4Size = new System.Windows.Size(793.7, 1122.5); // A4 size in WPF units (1/96 inch)

            // Build the paginated report document
            var doc = CommonReportBuilder.BuildPaginationDocument(
                items: currentProducts,
                createPageContent: products =>
                {
                    var stack = new StackPanel();
                    foreach (var p in products)
                        stack.Children.Add(ReportTemplates.CreateProductRow(p));
                    return stack;
                },
                pageSize: a4Size,
                reportTitle: "Product Analysis Report",
                createFirstPageHeader: () => ReportTemplates.CreateBigTitleHeader("Product Analysis Report"),
                createRegularPageHeader: () => ReportTemplates.CreateSmallTitleHeader("Product Analysis Report"),
                createHeaderRow: () => ReportTemplates.CreateProductHeader()
            );

            // Create and show the report preview window
            var previewWindow = new ReportPreview(doc)
            {
                Owner = System.Windows.Application.Current.MainWindow,
                ShowInTaskbar = false
            };
            previewWindow.ShowDialog();
        }

        // Method to filter products based on the search keyword
        private bool FilterProducts(object obj)
        {
            if (obj is not Product product)
                return false;

            if (string.IsNullOrWhiteSpace(Keyword))
                return true;

            string keyword = Keyword.ToLower();
            return product.ProductName?.ToLower().Contains(keyword) == true
                || product.ProductID?.ToLower().Contains(keyword) == true
                || product.ProductGrpName?.ToLower().Contains(keyword) == true
                || product.ProductUnit?.ToLower().Contains(keyword) == true;
        }

        // Method to refresh the view after filtering or searching
        private void RefreshView()
        {
            ProductsView?.Refresh();
        }

        // Event handler for property changes in each product to update total quantity
        private void Product_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Product.ProductQty))
                UpdateTotalQty();
        }

        // Method to update the total quantity of all products
        private void UpdateTotalQty()
        {
            TotalQty = Products.Sum(p => p.ProductQty);
        }

        // Method to load data from SQL Server database
        private async Task LoadDataFromSQLAsync()
        {
            try
            {
                string sql = @"
                    SELECT tP.ProductID, tP.ProductName, tP.ProductQty, tP.ProductUnit, tP.ProductPrice, 
                           tG.ProductGrp, tG.ProductGrp_name 
                    FROM tbl_Products tP 
                    INNER JOIN tbl_ProductGroups tG ON tP.ProductGrp = tG.ProductGrp";

                var data = await DatabaseService.LoadDataFromSQLAsync<Product>(connString, sql, reader => new Product
                {
                    ProductID = reader.GetString(0),
                    ProductName = reader.GetString(1),
                    ProductQty = reader.GetDecimal(2),
                    ProductUnit = reader.GetString(3),
                    ProductPrice = reader.GetDecimal(4),
                    ProductGrp = reader.GetString(5),
                    ProductGrpName = reader.GetString(6)
                });

                Products.Clear();
                foreach (var product in data)
                {
                    Products.Add(product);
                }

                TotalQty = Products.Sum(p => p.ProductQty);
            }
            catch(Exception ex)
            {
                // Handle exceptions (e.g., log the error, show a message to the user)
                System.Windows.MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
