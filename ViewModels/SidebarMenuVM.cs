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
using WpfApp5.General;
using WpfApp5.Views;

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
        private bool isReturnExpanded;
        [ObservableProperty]
        private bool isEDPurchaseOrder;

        [RelayCommand]
        private void ToggleAnalysis()
        {
            IsAnalysisExpanded = !IsAnalysisExpanded;
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
        private void Child2()
        {
            // Your logic for Child Button 2
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
