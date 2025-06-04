using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp5.General;
using WpfApp5.Views;

namespace WpfApp5.ViewModels
{
    public partial class MainWindowVM : ObservableRecipient
    {
        [ObservableProperty]
        private double sidebarWidth = 250;

        public MainWindowVM()
        {
            /*if (!IsActive)
                IsActive = true;*/
            IsActive = true;
            Debug.WriteLine($"IsActive in MainWindowVM: {IsActive}");

            WeakReferenceMessenger.Default.Register<ToggleSidebarMessage>(this, (r, m) =>
            {
                Console.WriteLine("ToggleSidebarMessage received");
                Debug.WriteLine("ToggleSidebarMessage received");
                SidebarWidth = SidebarWidth == 40 ? 250 : 40;
                Debug.WriteLine(SidebarWidth.ToString());
            });
        }

        [RelayCommand]
        private void Navigate(string? destination)
        {
            switch (destination)
            {
                case "LinkTable":
                    var window = new LTM();
                    window.Owner = Application.Current.MainWindow;
                    window.ShowDialog();
                    break;

                case "Backup":
                    // Optional: Show BackupView
                    break;

                case "Systemsettings":
                    // Optional: Show SystemSettingsView
                    break;
            }
        }

        [RelayCommand]
        private void Quit()
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
