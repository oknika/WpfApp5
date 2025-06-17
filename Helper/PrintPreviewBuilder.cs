using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp5.Helper
{
    public static class PrintPreviewBuilder
    {
        public static FixedDocument BuildFixedDocument(UIElement content, Size pageSize, string reportTitle)
        {
            double headerHeight = 40;
            double footerHeight = 40;
            double contentHeight = pageSize.Height - headerHeight - footerHeight - 45;

            content.Measure(new Size(pageSize.Width, double.PositiveInfinity));
            content.Arrange(new Rect(0, 0, pageSize.Width, content.DesiredSize.Height));
            content.UpdateLayout();

            double totalHeight = content.DesiredSize.Height;
            int pageCount = (int)Math.Ceiling(totalHeight / contentHeight);

            var doc = new FixedDocument();
            //double totalHeight = content.DesiredSize.Height;
            //int pageCount = (int)Math.Ceiling(totalHeight / pageSize.Height);

            for (int i = 0; i < pageCount; i++)
            {
                var fixedPage = new FixedPage
                {
                    Width = pageSize.Width,
                    Height = pageSize.Height
                };

                var visualBrush = new VisualBrush(content)
                {
                    Stretch = Stretch.None,
                    AlignmentX = AlignmentX.Left,
                    AlignmentY = AlignmentY.Top,
                    //Viewbox = new Rect(0, i * pageSize.Height, pageSize.Width, pageSize.Height),
                    Viewbox = new Rect(0, i * contentHeight, pageSize.Width, contentHeight),
                    ViewboxUnits = BrushMappingMode.Absolute
                };

                /*var pageVisual = new Rectangle
                {
                    Width = pageSize.Width,
                    Height = pageSize.Height,
                    Fill = visualBrush
                };
                
                 fixedPage.Children.Add(pageVisual);
                AddPageHeader(fixedPage, i + 1, pageCount);
                AddPageFooter(fixedPage);*/

                var contentRect = new Rectangle
                {
                    Width = pageSize.Width,
                    Height = contentHeight,
                    Fill = visualBrush,
                    Clip = new RectangleGeometry(new Rect(0, 0, pageSize.Width, contentHeight))
                };

                // Position content with margin under header
                var contentContainer = new Canvas
                {
                    Width = pageSize.Width,
                    Height = contentHeight,
                    Margin = new Thickness(0, headerHeight+20, 0, footerHeight+20)
                };
                contentContainer.Children.Add(contentRect);

                var header = CreateHeader(reportTitle);
                FixedPage.SetTop(header, 0);
                fixedPage.Children.Add(header);

                fixedPage.Children.Add(contentContainer);

                var footer = CreateFooter(i + 1, pageCount);
                FixedPage.SetTop(footer, pageSize.Height - footerHeight);
                fixedPage.Children.Add(footer);

                var pageContent = new PageContent();
                ((IAddChild)pageContent).AddChild(fixedPage);
                doc.Pages.Add(pageContent);
            }

            return doc;
        }

        private static UIElement CreateHeader(string reportTitle)
        {
            return new TextBlock
            {
                Text = reportTitle,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(40, 20, 0, 0)
            };
        }

        private static UIElement CreateFooter(int pageNumber, int totalPages)
        {
            double pageWidth = 793.7; // A4 width in 96 DPI
            double margin = 40;

            var footerGrid = new Grid
            {
                Width = pageWidth - (margin * 2), // ensure it fits inside margins
                Margin = new Thickness(margin, 0, margin, 20),
                VerticalAlignment = VerticalAlignment.Bottom
            };

            //footerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }); // Left (date)
            //footerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }); // Right (page)
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition());
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition());

            var dateText = new TextBlock
            {
                Text = $"Printed on {DateTime.Now:dd MMM yyyy}",
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            Grid.SetColumn(dateText, 0);

            var pageText = new TextBlock
            {
                Text = $"Page {pageNumber} of {totalPages}",
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Grid.SetColumn(pageText, 1);

            footerGrid.Children.Add(dateText);
            footerGrid.Children.Add(pageText);

            return footerGrid;
        }

        /*private static void AddPageHeader(FixedPage page, int pageIndex, int totalPages)
        {
            var header = new TextBlock
            {
                Text = $"Report Title - Page {pageIndex} of {totalPages}",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(40, 20, 0, 0)
            };
            page.Children.Add(header);
        }

        private static void AddPageFooter(FixedPage page)
        {
            var footer = new TextBlock
            {
                Text = $"Printed on {DateTime.Now:dd MMM yyyy}",
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 40, 20)
            };
            FixedPage.SetBottom(footer, 0);
            page.Children.Add(footer);
        }*/
    }
}
