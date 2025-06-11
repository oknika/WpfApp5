using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp5.Models
{
    public partial class OrderItem : ObservableObject
    {
        [ObservableProperty] private string itemName;
        [ObservableProperty] private int quantity;
        [ObservableProperty] private decimal price;

        public OrderItem() { }
    }

    public partial class PurchaseOrder : ObservableObject
    {
        /*public string PurchaseOrderID { get; set; }
        public string SupplierName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public ObservableCollection<OrderItem> OrderItems { get; set; } = new();*/
        [ObservableProperty] public string purchaseOrderID;
        [ObservableProperty] public string supplierName;
        [ObservableProperty] public DateTime orderDate;
        [ObservableProperty] public decimal totalAmount;
        [ObservableProperty] public ObservableCollection<OrderItem> orderItems = new();
    }

    public partial class Product : ObservableObject
    {
        [ObservableProperty] public string productName;
        [ObservableProperty] public decimal productPrice;
    }
}
