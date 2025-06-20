using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp5.Models;

namespace WpfApp5.ViewModels
{
    public partial class SimpleReportVM : ObservableObject
    {
        public ObservableCollection<Product> AllProducts { get; } = new();
        public ObservableCollection<ProductGroup> GroupedProducts { get; } = new();

        [ObservableProperty]
        private string connString = Properties.Settings.Default.MainConnString;

        public SimpleReportVM()
        {
            LoadDataFromSQL();
        }

        public void LoadDataFromSQL()
        {
            string connectionString = ConnString;
            string query = "SELECT tP.ProductID, tP.ProductName, tP.ProductQty, tP.ProductUnit, tP.ProductPrice, tG.ProductGrp, tG.ProductGrp_name " +
                "FROM tbl_Products tP INNER JOIN tbl_ProductGroups tG ON tP.ProductGrp = tG.ProductGrp";

            using SqlConnection conn = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            AllProducts.Clear(); // Clear existing products before loading new ones
            GroupedProducts.Clear();

            var all = new List<Product>();

            int count = 0;
            while (reader.Read())
            {
                /*AllProducts.Add(new Product
                {
                    ProductID = reader.GetString(0),
                    ProductName = reader.GetString(1),
                    ProductQty = reader.GetDecimal(2),
                    ProductUnit = reader.GetString(3),
                    ProductPrice = reader.GetDecimal(4),
                    ProductGrp = reader.GetString(5),
                    ProductGrpName = reader.GetString(6)
                });*/

                var product = new Product
                {
                    ProductID = reader.GetString(0),
                    ProductName = reader.GetString(1),
                    ProductQty = reader.GetDecimal(2),
                    ProductUnit = reader.GetString(3),
                    ProductPrice = reader.GetDecimal(4),
                    ProductGrp = reader.GetString(5),
                    ProductGrpName = reader.GetString(6)
                };

                AllProducts.Add(product);
                all.Add(product);

                count++;
            }

            // Grouping logic
            var grouped = all
                .GroupBy(p => p.ProductGrpName)
                .Select(g => new ProductGroup
                {
                    GroupName = g.Key,
                    Products = new ObservableCollection<Product>(g)
                });

            foreach (var group in grouped)
            {
                GroupedProducts.Add(group);
            }

            Console.WriteLine($"Loaded {count} products and grouped product.");
        }
    }
}
