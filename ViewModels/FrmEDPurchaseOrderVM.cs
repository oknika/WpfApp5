using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp5.Generator;
using WpfApp5.Models;
using MessageBox = System.Windows.MessageBox;

namespace WpfApp5.ViewModels
{
    public partial class FrmEDPurchaseOrderVM : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<OrderItem> orderItems = new();

        [ObservableProperty]
        private ObservableCollection<Product> availableItems = new();

        [ObservableProperty]
        private string orderId;
        [ObservableProperty]
        private string supplierName;
        [ObservableProperty]
        private DateTime orderDate = DateTime.Today;
        [ObservableProperty]
        private decimal totalAmount;
        [ObservableProperty]
        private string itemName;
        [ObservableProperty]
        private int quantity;
        [ObservableProperty]
        private decimal price;

        [ObservableProperty]
        private string connString = Properties.Settings.Default.MainConnString;

        public FrmEDPurchaseOrderVM()
        {
            _ = InitializeAsync();
            orderItems.CollectionChanged += OrderItems_CollectionChanged;
            AvailableItems = new ObservableCollection<Product>();

            LoadAvailableItemsFromSql();
        }

        private void LoadAvailableItemsFromSql()
        {
            string connectionString = connString;
            string query = "SELECT ProductName, ProductPrice FROM tbl_Products";

            using SqlConnection conn = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                AvailableItems.Add(new Product
                {
                    ProductName = reader.GetString(0),
                    ProductPrice = reader.GetDecimal(1)
                });
            }
        }

        private void OrderItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (OrderItem item in e.NewItems)
                {
                    item.PropertyChanged += OrderItem_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (OrderItem item in e.OldItems)
                {
                    item.PropertyChanged -= OrderItem_PropertyChanged;
                }
            }

            RecalculateTotal();
        }

        private void OrderItem_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is OrderItem item)
            {
                if (e.PropertyName == nameof(OrderItem.ItemName))
                {
                    UpdatePrice(item);
                }

                if (e.PropertyName == nameof(OrderItem.Quantity) || e.PropertyName == nameof(OrderItem.Price))
                {
                    RecalculateTotal();
                }
            }
        }

        private void UpdatePrice(OrderItem item)
        {
            var selectedProduct = AvailableItems.FirstOrDefault(p => p.ProductName == item.ItemName);
            if (selectedProduct != null)
            {
                item.Price = selectedProduct.ProductPrice;
            }
            else
            {
                item.Price = 0;
            }
        }

        private void RecalculateTotal()
        {
            TotalAmount = OrderItems.Sum(i => i.Price * i.Quantity);
        }

        private async Task InitializeAsync()
        {
            string lastId = await GetLastTransID.GetLastTransIdFromDatabase(connString, "PurchaseOrders");
            OrderId = IDPurchaseGen.GenerateNextPurchaseId("PO", lastId);
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            var po = new PurchaseOrder
            {
                PurchaseOrderID = this.OrderId,
                SupplierName = this.SupplierName,
                OrderDate = this.OrderDate,
                TotalAmount = this.TotalAmount,
                OrderItems = new ObservableCollection<OrderItem>(this.OrderItems)
            };

            try
            {
                await SavePurchaseOrderAsync(po);
                MessageBox.Show("Purchase Order saved successfully!","Success",MessageBoxButton.OK,MessageBoxImage.Information);
                await ClearFormAfterSave();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error: {sqlEx.Message}","Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ClearFormAfterSave()
        {
            string lastId = await GetLastTransID.GetLastTransIdFromDatabase(connString, "PurchaseOrders");
            OrderId = IDPurchaseGen.GenerateNextPurchaseId("PO", lastId);
            SupplierName = string.Empty;
            OrderDate = DateTime.Today;
            TotalAmount = 0;
            OrderItems.Clear();
            ItemName = string.Empty;
            Quantity = 0;
            Price = 0;
        }

        private async Task SavePurchaseOrderAsync(PurchaseOrder po)
        {
            using var conn = new SqlConnection(connString);
            await conn.OpenAsync();

            // Insert purchase order and get inserted ID
            var cmd = new SqlCommand("INSERT INTO PurchaseOrders (PurchaseOrderID, SupplierName, OrderDate, Status, TotalAmount) OUTPUT INSERTED.PurchaseOrderID " +
                                    "VALUES (@PurchaseOrderID, @SupplierName, @OrderDate, @Status, @TotalAmount)", conn);
            cmd.Parameters.AddWithValue("@PurchaseOrderID", po.PurchaseOrderID);
            cmd.Parameters.AddWithValue("@SupplierName", po.SupplierName);
            cmd.Parameters.AddWithValue("@OrderDate", po.OrderDate);
            cmd.Parameters.AddWithValue("@Status", "Lunas");
            cmd.Parameters.AddWithValue("@TotalAmount", po.TotalAmount);

            await cmd.ExecuteScalarAsync();

            // Insert order items
            int i= 1;
            foreach (var item in po.OrderItems)
            {
                var itemCmd = new SqlCommand("INSERT INTO PurchaseOrderDetails (DetailID, PurchaseOrderID, ItemName, Quantity, UnitPrice) " +
                                    "VALUES (@DetailID, @PurchaseOrderID, @ItemName, @Quantity, @Price)", conn);
                itemCmd.Parameters.AddWithValue("@DetailID", "D" + i.ToString("D4"));
                itemCmd.Parameters.AddWithValue("@PurchaseOrderID", po.PurchaseOrderID);
                itemCmd.Parameters.AddWithValue("@ItemName", item.ItemName);
                itemCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                itemCmd.Parameters.AddWithValue("@Price", item.Price);
                await itemCmd.ExecuteNonQueryAsync();

                var stockpdateCmd = new SqlCommand("UPDATE tbl_Products SET [ProductQty] = [ProductQty] - @Quantity WHERE ProductName = @ProductName", conn);
                stockpdateCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                stockpdateCmd.Parameters.AddWithValue("@ProductName", item.ItemName);
                await stockpdateCmd.ExecuteNonQueryAsync();

                i++;
            }
        }
    }
}
