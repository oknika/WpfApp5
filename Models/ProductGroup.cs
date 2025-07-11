using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp5.Models
{
    public class ProductGroup
    {
        public string GroupName { get; set; }
        public ObservableCollection<Product> Products { get; set; }
    }

    public partial class GroupingOption : ObservableObject
    {
        [ObservableProperty]
        private string _displayText;

        [ObservableProperty]
        private bool _isGroupingEnabled;
    }

}
