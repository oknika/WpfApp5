using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp5.Models;

namespace WpfApp5.ViewModels
{
    public partial class FrmDAProductsVM : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<Product> products = new();

        [ObservableProperty]
        private decimal totalQty;

        [ObservableProperty]
        private string connString = Properties.Settings.Default.MainConnString;

        public FrmDAProductsVM()
        {
            LoadDataFromSQL();

            foreach (var p in Products)
                p.PropertyChanged += Product_PropertyChanged;

            UpdateTotalQty();
        }

        private void Product_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Product.ProductQty))
                UpdateTotalQty();
        }

        private void UpdateTotalQty()
        {
            TotalQty = Products.Sum(p => p.ProductQty);
        }

        private void LoadDataFromSQL()
        {
            string connectionString = connString;
            string query = "SELECT tP.ProductID, tP.ProductName, tP.ProductQty, tP.ProductUnit, tP.ProductPrice, tG.ProductGrp, tG.ProductGrp_name " +
                "FROM tbl_Products tP INNER JOIN tbl_ProductGroups tG ON tP.ProductGrp = tG.ProductGrp";

            using SqlConnection conn = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            Products.Clear(); // Clear existing products before loading new ones
            while (reader.Read())
            {
                Products.Add(new Product
                {
                    ProductID = reader.GetString(0),
                    ProductName = reader.GetString(1),
                    ProductQty = reader.GetDecimal(2),
                    ProductUnit = reader.GetString(3),
                    ProductPrice = reader.GetDecimal(4),
                    ProductGrp = reader.GetString(5),
                    ProductGrpName = reader.GetString(6)
                });
            }
            TotalQty = Products.Sum(p => p.ProductQty);
        }
    }
}
