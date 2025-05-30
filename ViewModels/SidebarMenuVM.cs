﻿using CommunityToolkit.Mvvm;
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
using WpfApp5.General;

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

        [RelayCommand]
        private void ToggleAnalysis()
        {
            IsAnalysisExpanded = !IsAnalysisExpanded;
        }

        [RelayCommand]
        private void ToggleEditData()
        {
            IsEditDataExpanded = !IsEditDataExpanded;
        }

        [RelayCommand]
        private void ToggleSettings()
        {
            IsSettingsExpanded = !IsSettingsExpanded;
        }

        [RelayCommand]
        private void Child1()
        {
            // Your logic for Child Button 1
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
