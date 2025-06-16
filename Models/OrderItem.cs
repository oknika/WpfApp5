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
        [ObservableProperty] private string _itemName;
        [ObservableProperty] private int _quantity;
        [ObservableProperty] private decimal _price;

        public OrderItem() { }
    }

    public partial class PurchaseOrder : ObservableObject
    {
        /*public string PurchaseOrderID { get; set; }
        public string SupplierName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public ObservableCollection<OrderItem> OrderItems { get; set; } = new();*/
        [ObservableProperty] public string _purchaseOrderID;
        [ObservableProperty] public string _supplierName;
        [ObservableProperty] public DateTime _orderDate;
        [ObservableProperty] public decimal _totalAmount;
        [ObservableProperty] public ObservableCollection<OrderItem> _orderItems = new();
    }

    public partial class Product : ObservableObject
    {
        [ObservableProperty] public string _productID;
        [ObservableProperty] public string _productName;
        [ObservableProperty] public decimal _productQty;
        [ObservableProperty] public string _productUnit;
        [ObservableProperty] public decimal _productPrice;
        [ObservableProperty] public string _productGrp;
        [ObservableProperty] public string _productGrpName;

        public string ProductGroupDisplay => $"{ProductGrp} - {ProductGrpName}";
    }
}
