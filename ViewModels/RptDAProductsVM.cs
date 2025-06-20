using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.IO;
using System.Windows.Markup;
using System.Xml;
using WpfApp5.Helper;
using WpfApp5.Models;
using WpfApp5.Views;

namespace WpfApp5.ViewModels
{
    public partial class RptDAProductsVM : ObservableObject
    {
        public ObservableCollection<ProductGroup> GroupedPagedProducts { get; } = new();
        public ObservableCollection<ProductGroup> GroupedAllProducts { get; } = new();

        [ObservableProperty]
        private string connString = Properties.Settings.Default.MainConnString;

        private const int PageSize = 10;

        [ObservableProperty] private int _currentPage = 1;
        public int TotalPages => (AllProducts.Count + PageSize - 1) / PageSize;

        public ObservableCollection<Product> AllProducts { get; } = new();
        public ObservableCollection<Product> PagedProducts { get; } = new();

        private ListCollectionView _pagedView;
        public ICollectionView GroupedView => _pagedView;

        public RptDAProductsVM()
        {
            LoadDataFromSQL();
            LoadPagedProducts();
            SetupGrouping();
            SetupGroupedProductsForPrinting();
        }

        public void SetupGroupedProductsForPrinting()
        {
            GroupedAllProducts.Clear();

            var grouped = PagedProducts
                .GroupBy(p => p.ProductGrp)
                .OrderBy(g => g.Key);

            foreach (var group in grouped)
            {
                GroupedAllProducts.Add(new ProductGroup
                {
                    GroupName = group.First().ProductGrpName,
                    Products = new ObservableCollection<Product>(group)
                });
            }
        }

        public FixedDocument BuildGroupedPrintPreviewDocument()
        {
            var finalDoc = new FixedDocument();
            var pageSize = new Size(816, 1056); // A4 at 96 DPI

            foreach (var group in GroupedAllProducts)
            {
                var control = new SimpleReport
                {
                    DataContext = group,
                    Width = pageSize.Width
                };

                control.Measure(new Size(pageSize.Width, double.PositiveInfinity));
                control.Arrange(new Rect(0, 0, pageSize.Width, control.DesiredSize.Height));
                control.UpdateLayout();

                var groupDoc = PrintPreviewBuilder.BuildFixedDocument(control, pageSize, "");

                foreach (PageContent page in groupDoc.Pages)
                {
                    //finalDoc.Pages.Add(page);
                    var fixedPage = (FixedPage)((PageContent)page).Child;

                    // Deep clone using XAML serialization
                    string xaml = XamlWriter.Save(fixedPage);
                    using StringReader stringReader = new StringReader(xaml);
                    using XmlReader xmlReader = XmlReader.Create(stringReader);
                    FixedPage clonedPage = (FixedPage)XamlReader.Load(xmlReader);

                    PageContent newPageContent = new PageContent();
                    ((IAddChild)newPageContent).AddChild(clonedPage);

                    finalDoc.Pages.Add(newPageContent);
                }
            }

            return finalDoc;
        }

        private void SetupGrouping()
        {
            _pagedView = new ListCollectionView(PagedProducts);
            _pagedView.GroupDescriptions.Clear();
            _pagedView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Product.ProductGrp)));
            //OnPropertyChanged(nameof(GroupedView));
        }

        private void LoadPagedProducts()
        {
            var items = AllProducts
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            PagedProducts.Clear();
            foreach (var item in items)
                PagedProducts.Add(item);
        }

        [RelayCommand(CanExecute = nameof(CanGoNext))]
        private void NextPage()
        {
            CurrentPage++;
            LoadPagedProducts();
            SetupGrouping();
            OnPropertyChanged(nameof(TotalPages));
        }

        [RelayCommand(CanExecute = nameof(CanGoPrev))]
        private void PrevPage()
        {
            CurrentPage--;
            LoadPagedProducts();
            SetupGrouping();
            OnPropertyChanged(nameof(TotalPages));
        }

        private bool CanGoNext() => CurrentPage < TotalPages;
        private bool CanGoPrev() => CurrentPage > 1;

        partial void OnCurrentPageChanged(int value)
        {
            PrevPageCommand.NotifyCanExecuteChanged();
            NextPageCommand.NotifyCanExecuteChanged();
        }

        private void LoadDataFromSQL()
        {
            string connectionString = ConnString;
            string query = "SELECT tP.ProductID, tP.ProductName, tP.ProductQty, tP.ProductUnit, tP.ProductPrice, tG.ProductGrp, tG.ProductGrp_name " +
                "FROM tbl_Products tP INNER JOIN tbl_ProductGroups tG ON tP.ProductGrp = tG.ProductGrp";

            using SqlConnection conn = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            AllProducts.Clear(); // Clear existing products before loading new ones
            int count = 0;
            while (reader.Read())
            {
                AllProducts.Add(new Product
                {
                    ProductID = reader.GetString(0),
                    ProductName = reader.GetString(1),
                    ProductQty = reader.GetDecimal(2),
                    ProductUnit = reader.GetString(3),
                    ProductPrice = reader.GetDecimal(4),
                    ProductGrp = reader.GetString(5),
                    ProductGrpName = reader.GetString(6)
                });
                count++;
            }

            Console.WriteLine($"Loaded {count} products.");
        }
    }
}
