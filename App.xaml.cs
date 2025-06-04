using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;
using WpfApp5.Views;

namespace WpfApp5
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow(); // or your dashboard/home
            this.MainWindow = mainWindow;

            mainWindow.WindowState = WindowState.Minimized;
            mainWindow.Show();
            mainWindow.Hide();

            string savedConnStr = WpfApp5.Properties.Settings.Default.MainConnString;

            if (string.IsNullOrWhiteSpace(savedConnStr))
            {
                // Show LTM window to set connection
                var ltmWindow = new LTM(); // your connection setup window
                ltmWindow.Owner = mainWindow;
                ltmWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                
                Debug.WriteLine("Showing LTM window...");
                bool? dialogResult = ltmWindow.ShowDialog();
                Debug.WriteLine($"LTM returned DialogResult: {dialogResult}");

                if (dialogResult != true || string.IsNullOrWhiteSpace(WpfApp5.Properties.Settings.Default.MainConnString))
                {
                    // User cancelled or failed to provide connection
                    Shutdown();
                    return;
                }
            }

            // Proceed to main window using saved connection
            Debug.WriteLine("Showing MainWindow...");
            mainWindow.Show();
            mainWindow.WindowState = WindowState.Maximized;
            Debug.WriteLine("MainWindow.Show() called");
        }

    }

}
