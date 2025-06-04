using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp5.Views;

namespace WpfApp5.ViewModels
{
    public partial class LTMVM : ObservableObject
    {
        public event Action<bool>? RequestClose;

        [ObservableProperty]
        private string serverName;

        [ObservableProperty]
        private string databaseName;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool useWindowsAuth = true;

        [ObservableProperty]
        private ObservableCollection<string> tableNames = new();

        [ObservableProperty]
        private string selectedTable;

        [ObservableProperty]
        private DataTable tablePreview;

        public LTMVM()
        {
            //Debug.WriteLine("LTMVM constructor called.");

            ServerName = Properties.Settings.Default.ServerName;
            DatabaseName = Properties.Settings.Default.DatabaseName;
            Username = Properties.Settings.Default.Username;
            Password = Properties.Settings.Default.Password;
            UseWindowsAuth = Properties.Settings.Default.UseWindowsAuth;

            Debug.WriteLine($"Loaded Settings: ServerName={ServerName}, DatabaseName={DatabaseName}, Username={Username}, UseWindowsAuth={UseWindowsAuth}");
        }

        private string BuildConnectionString()
        {
            if (UseWindowsAuth)
            {
                return $"Server={ServerName};Database={DatabaseName};Integrated Security=True;TrustServerCertificate=True;";
            }
            else
            {
                return $"Server={ServerName};Database={DatabaseName};User Id={Username};Password={Password};TrustServerCertificate=True;";
            }
        }

        [RelayCommand]
        private void Connect()
        {
            Debug.WriteLine($"password: {Password}, Win auth: {UseWindowsAuth}");
            TableNames.Clear();
            try
            {
                string dynamicConnStr = BuildConnectionString();

                Properties.Settings.Default.ServerName = ServerName;
                Properties.Settings.Default.DatabaseName = DatabaseName;
                Properties.Settings.Default.Username = Username;
                Properties.Settings.Default.Password = Password;
                Properties.Settings.Default.UseWindowsAuth = UseWindowsAuth;
                Properties.Settings.Default.MainConnString = dynamicConnStr;
                Properties.Settings.Default.Save();

                using SqlConnection conn = new(dynamicConnStr);
                conn.Open();

                DataTable schema = conn.GetSchema("Tables");

                foreach (DataRow row in schema.Rows)
                {
                    string tableName = row["TABLE_NAME"].ToString();
                    TableNames.Add(tableName);
                }
                System.Windows.MessageBox.Show("Connection successful!");

                bool success = true; // say connection succeeded

                if (success)
                {
                    // Ask the View to close with DialogResult=true
                    Debug.WriteLine("RequestClose called, setting DialogResult...");
                    RequestClose?.Invoke(true);
                }
                else
                {
                    // Show error or stay open
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Connection failed: {ex.Message}");
            }
        }

        [RelayCommand]
        private void LoadTablePreview()
        {
            if (string.IsNullOrEmpty(SelectedTable)) return;

            try
            {
                string connStr = Properties.Settings.Default.MainConnString;
                using SqlConnection conn = new(connStr);
                conn.Open();
                SqlDataAdapter adapter = new($"SELECT TOP 10 * FROM [{SelectedTable}]", conn);
                DataTable dt = new();
                adapter.Fill(dt);
                TablePreview = dt;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to load table preview: {ex.Message}");
            }
        }
    }
}
