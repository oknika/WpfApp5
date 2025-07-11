using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp5.Helper;
using WpfApp5.Models;
using WpfApp5.Services;
using WpfApp5.Views;

namespace WpfApp5.ViewModels
{
    public partial class RO_PrintVM : ObservableObject
    {
        public Action? CloseAction { get; set; }

        public ObservableCollection<GroupingOption> GroupingOptions { get; }

        private readonly IFileDialogService _fileDialogService;

        [ObservableProperty]
        private string buttonCaption = "Print";

        [ObservableProperty]
        private GroupingOption? selectedOption;

        private readonly List<Product> productsToPrint;

        public RO_PrintVM(List<Product> products, string btnCaption, FileDialogService fileDialogService)
        {
            productsToPrint= products ?? throw new ArgumentNullException(nameof(products));

            ButtonCaption = btnCaption ?? throw new ArgumentNullException(nameof(btnCaption));

            _fileDialogService = fileDialogService;

            GroupingOptions = new ObservableCollection<GroupingOption>
            {
                new GroupingOption { DisplayText = "Group by Product Group", IsGroupingEnabled = true },
                new GroupingOption { DisplayText = "No Grouping", IsGroupingEnabled = false }
            };
            // Set default selected option
            SelectedOption = GroupingOptions.FirstOrDefault();
        }

        [RelayCommand]
        private void Print()
        {
            if (SelectedOption == null)
            {
                // Handle case where no option is selected
                return;
            }

            var grouping = SelectedOption?.IsGroupingEnabled ?? false;

            if (ButtonCaption == "Print")
            {
                // If the button caption is "Print", to print
                var reportWindow = new WinReportV2(productsToPrint, grouping);
                reportWindow.WindowState = WindowState.Maximized;
                reportWindow.Show();
            }
            else if (ButtonCaption == "Export to PDF")
            {
                // If the button caption is "Export to PDF", export the report to PDF
                var path = RdlcModifier.ModifyRdlc("Reports/DynamicProductReport.rdlc", showGrouping: grouping, showPgHeader: true);
                string outputPath = _fileDialogService.ShowSaveFileDialog("Product" + (grouping? "Grouping":"Detail") + ".pdf", "PDF Files (*.pdf)|*.pdf");
                if (string.IsNullOrEmpty(outputPath))
                    return;

                RdlcModifier.ExportToPdf(path, productsToPrint, "DataSetCoba", outputPath);

                System.Windows.MessageBox.Show("Exported to PDF!");
            }

            CloseAction?.Invoke();
        }
    }
}
