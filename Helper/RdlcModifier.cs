using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shell;
using System.Xml.Linq;

namespace WpfApp5.Helper
{
    public static class RdlcModifier
    {
        public static class RdlcNs
        {
            public static XNamespace CurrentNs { get; private set; }

            public static void InitFrom(XDocument doc)
            {
                CurrentNs = doc.Root?.Name.Namespace
                            ?? throw new InvalidOperationException("Could not determine RDLC namespace from document.");
            }

            public static XName Name(string localName) => CurrentNs + localName;
        }

        public static string ModifyRdlc(string originalPath, bool showGrouping, bool showPgHeader)
        {
            var doc = XDocument.Load(originalPath);
            //var ns = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition";
            RdlcNs.InitFrom(doc);

            // Set header height
            var headerHeight = showPgHeader ? "1cm" : "0cm";
            var header = doc.Descendants(RdlcNs.Name("PageHeader")).FirstOrDefault();
            header?.Element(RdlcNs.Name("Height"))?.SetValue(headerHeight);

            // Set BreakLocation for groups
            var breakLocations = doc
                .Descendants(RdlcNs.Name("Group"))
                .Elements(RdlcNs.Name("PageBreak"))
                .Elements(RdlcNs.Name("BreakLocation"));

            foreach (var breakLocation in breakLocations)
            {
                breakLocation.Value = showGrouping ? "Between" : "None";
            }

            if (!showGrouping)
            {
                var productIdGroupMember = doc.Descendants(RdlcNs.Name("TablixMember"))
                .FirstOrDefault(m => m.Element(RdlcNs.Name("Group"))?.Attribute("Name")?.Value == "Group1");

                if (productIdGroupMember != null)
                {
                    // Remove the <Group> element
                    productIdGroupMember.Element(RdlcNs.Name("Group"))?.Remove();

                    // Remove the <SortExpressions> element
                    productIdGroupMember.Element(RdlcNs.Name("SortExpressions"))?.Remove();
                }
            }

            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".rdlc");
            doc.Save(tempPath);
            return tempPath;
        }

        public static void ExportToPdf<T>(string rdlcPath, IEnumerable<T> data, string datasetName, string outputPdfPath)
        {
            try
            {
                // Create and configure LocalReport
                LocalReport report = new LocalReport();
                report.ReportPath = rdlcPath;

                report.DataSources.Clear();
                report.DataSources.Add(new ReportDataSource(datasetName, data));

                // Render as PDF
                Warning[] warnings;
                string[] streamIds;
                string mimeType, encoding, extension;

                byte[] pdfBytes = report.Render(
                    "PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                // Save to disk
                File.WriteAllBytes(outputPdfPath, pdfBytes);
            }
            finally
            {
                if (File.Exists(rdlcPath))
                    File.Delete(rdlcPath);
            }
        }
    }
}
