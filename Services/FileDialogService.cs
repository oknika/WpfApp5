using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp5.Services;
using System.IO;

namespace WpfApp5.Services
{
    public class FileDialogService : IFileDialogService
    {
        public string ShowSaveFileDialog(string defaultFileName, string filter)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Export Report",
                Filter = filter,
                FileName = defaultFileName,
                DefaultExt = Path.GetExtension(defaultFileName)
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }
    }
}

