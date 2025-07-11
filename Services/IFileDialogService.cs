using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp5.Services
{
    public interface IFileDialogService
    {
        string ShowSaveFileDialog(string defaultFileName, string filter);
    }
}
