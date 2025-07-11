using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Size = System.Windows.Size;

namespace WpfApp5.Helper
{
    public static class CommonReportBuilder
    {
        /// <summary>
        /// Builds a paginated FixedDocument for printing or previewing.
        /// </summary>
        /// <typeparam name="T">Type of items to be displayed.</typeparam>
        /// <param name="items">Collection of items to display.</param>
        /// <param name="createPageContent">Function to create the content for each page.</param>
        /// <param name="pageSize">Size of each page.</param>
        /// <param name="reportTitle">Title of the report.</param>
        /// <param name="createHeaderRow">Optional header row.</param>
        /// <returns>A FixedDocument containing the paginated content.</returns>
        public static FixedDocument BuildPaginationDocument<T>(
        IEnumerable<T> items,
        Func<IEnumerable<T>, UIElement> createPageContent,
        System.Windows.Size pageSize,
        string reportTitle,
        Func<UIElement> createFirstPageHeader,
        Func<UIElement> createRegularPageHeader,
        Func<UIElement>? createHeaderRow = null)
        {
            double headerHeight = 40;
            double footerHeight = 40;
            double marginBuffer = 20;
            double contentHeight = pageSize.Height - headerHeight - footerHeight - (marginBuffer * 2);

            var doc = new FixedDocument();

            // Estimate height of one item
            var testItems = items.Take(1).ToList();
            var testPanel = createPageContent(testItems);
            testPanel.Measure(new System.Windows.Size(pageSize.Width, double.PositiveInfinity));
            double estimatedItemHeight = testPanel.DesiredSize.Height;

            double headerRowHeight = 0;

            // If a header row is provided, measure its height
            if (createHeaderRow != null)
            {
                var testHeader = createHeaderRow();
                testHeader.Measure(new System.Windows.Size(pageSize.Width, double.PositiveInfinity));
                headerRowHeight = testHeader.DesiredSize.Height;
            }

            // Items per page (rough estimate)
            int itemsPerPage = Math.Max(1, (int)((contentHeight - headerRowHeight) / estimatedItemHeight));

            // Split into pages
            var pagedData = items
                .Select((item, index) => new { item, index })
                .GroupBy(x => x.index / itemsPerPage)
                .Select(g => g.Select(x => x.item).ToList())
                .ToList();

            for (int i = 0; i < pagedData.Count; i++)
            {
                var fixedPage = new FixedPage
                {
                    Width = pageSize.Width,
                    Height = pageSize.Height
                };

                // Header
                //var header = CreateHeader(reportTitle);
                var header = (i == 0) ? createFirstPageHeader() : createRegularPageHeader();
                FixedPage.SetTop(header, 0);
                fixedPage.Children.Add(header);

                // Vertical stack for header row + items
                var contentStack = new StackPanel { Margin = new Thickness(0) };

                // Header Row (column labels)
                if (createHeaderRow != null)
                {
                    var headerRow = createHeaderRow();
                    contentStack.Children.Add(headerRow);
                }

                // Item Rows
                var itemContent = createPageContent(pagedData[i]);
                contentStack.Children.Add(itemContent);

                contentStack.Measure(new System.Windows.Size(pageSize.Width, contentHeight));
                contentStack.RenderTransform = new TranslateTransform(40, 0);
                contentStack.Arrange(new Rect(40, 0, pageSize.Width - 40, contentHeight));
                contentStack.UpdateLayout();

                FixedPage.SetTop(contentStack, headerHeight + marginBuffer);
                fixedPage.Children.Add(contentStack);

                // Footer
                var footer = CreateFooter(i + 1, pagedData.Count);
                FixedPage.SetTop(footer, pageSize.Height - footerHeight);
                fixedPage.Children.Add(footer);

                var pageContent = new PageContent();
                ((IAddChild)pageContent).AddChild(fixedPage);
                doc.Pages.Add(pageContent);
            }

            return doc;
        }

        /// <summary>
        /// Builds a paginated FixedDocument for grouped items with headers.    
        /// </summary>
        /// <typeparam name="TGroup">Type of the group.</typeparam> 
        /// <typeparam name="TItem">Type of the items within the group.</typeparam>
        /// <param name="groups">Collection of groups.</param>
        /// <param name="getGroupItems">Function to get items for each group.</param>
        /// <param name="createGroupHeader">Function to create the header for each group.</param>
        /// <param name="createItemRow">Function to create the row for each item.</param>
        /// <param name="pageSize">Size of each page.</param>
        /// <param name="reportTitle">Title of the report.</param>
        /// <param name="createHeaderRow">Optinal parameter: content header row.</param>
        /// <returns>A FixedDocument containing the paginated grouped content.</returns>
        public static FixedDocument BuildGroupedPaginationDocument<TGroup, TItem>(
            IEnumerable<TGroup> groups,
            Func<TGroup, IEnumerable<TItem>> getGroupItems,
            Func<TGroup, UIElement> createGroupHeader,
            Func<TItem, UIElement> createItemRow,
            System.Windows.Size pageSize,
            string reportTitle,
            Func<UIElement> createFirstPageHeader,
            Func<UIElement> createRegularPageHeader,
            Func<UIElement>? createHeaderRow = null)
        {
            double headerHeight = 40;
            double footerHeight = 40;
            double marginBuffer = 20;
            double contentHeight = pageSize.Height - headerHeight - footerHeight - (marginBuffer * 2);
            double pageWidth = pageSize.Width;

            var doc = new FixedDocument();
            int pageIndex = 0;

            var currentStack = new StackPanel();
            double currentHeight = 0;

            void FinalizePage(StackPanel pageContentStack)
            {
                var fixedPage = new FixedPage
                {
                    Width = pageSize.Width,
                    Height = pageSize.Height
                };

                //var header = CreateHeader(reportTitle);
                var header = (pageIndex == 0) ? createFirstPageHeader() : createRegularPageHeader();
                FixedPage.SetTop(header, 0);
                fixedPage.Children.Add(header);

                // Use VisualBrush for rendering
                pageContentStack.Measure(new System.Windows.Size(pageSize.Width - 80, contentHeight));
                pageContentStack.Arrange(new Rect(0, 0, pageSize.Width - 80, contentHeight));
                pageContentStack.UpdateLayout();

                var visualBrush = new VisualBrush(pageContentStack)
                {
                    Stretch = Stretch.None,
                    AlignmentX = AlignmentX.Left,
                    AlignmentY = AlignmentY.Top,
                    Viewbox = new Rect(0, 0, pageSize.Width - 80, contentHeight),
                    ViewboxUnits = BrushMappingMode.Absolute
                };

                var container = new Canvas
                {
                    Width = pageWidth - 80,
                    Height = contentHeight,
                    Margin = new Thickness(40, 0, 40, 0)
                };

                var rect = new System.Windows.Shapes.Rectangle
                {
                    Width = pageWidth - 80,
                    Height = contentHeight,
                    Fill = visualBrush
                };

                container.Children.Add(rect);
                FixedPage.SetTop(container, headerHeight + marginBuffer);
                fixedPage.Children.Add(container);

                var footer = CreateFooter(pageIndex + 1, 0); // totalPages will be updated later
                FixedPage.SetTop(footer, pageSize.Height - footerHeight);
                fixedPage.Children.Add(footer);

                var pageContent = new PageContent();
                ((IAddChild)pageContent).AddChild(fixedPage);
                doc.Pages.Add(pageContent);
                pageIndex++;
            }

            foreach (var group in groups)
            {
                var groupHeader = createGroupHeader(group);
                groupHeader.Measure(new Size(pageWidth, double.PositiveInfinity));
                double groupHeaderHeight = groupHeader.DesiredSize.Height;

                if (currentHeight + groupHeaderHeight > contentHeight)
                {
                    FinalizePage(currentStack);
                    currentStack = new StackPanel();
                    currentHeight = 0;
                }

                currentStack.Children.Add(groupHeader);
                currentHeight += groupHeaderHeight;

                // Add optional header row (like column titles)
                if (createHeaderRow != null)
                {
                    var headerRow = createHeaderRow();
                    headerRow.Measure(new Size(pageSize.Width, double.PositiveInfinity));
                    double headerRowHeight = headerRow.DesiredSize.Height;

                    if (currentHeight + headerRowHeight > contentHeight)
                    {
                        FinalizePage(currentStack);
                        currentStack = new StackPanel();
                        currentHeight = 0;

                        // Re-add group header and header row
                        currentStack.Children.Add(groupHeader);
                        currentHeight += groupHeaderHeight;

                        currentStack.Children.Add(headerRow);
                        currentHeight += headerRowHeight;
                    }
                    else
                    {
                        currentStack.Children.Add(headerRow);
                        currentHeight += headerRowHeight;
                    }
                }

                foreach (var item in getGroupItems(group))
                {
                    var row = createItemRow(item);
                    row.Measure(new Size(pageWidth, double.PositiveInfinity));
                    double rowHeight = row.DesiredSize.Height;

                    if (currentHeight + rowHeight > contentHeight)
                    {
                        FinalizePage(currentStack);
                        currentStack = new StackPanel();
                        currentHeight = 0;

                        // Re-add group header if group spans multiple pages
                        groupHeader = createGroupHeader(group);
                        groupHeader.Measure(new Size(pageWidth, double.PositiveInfinity));
                        groupHeaderHeight = groupHeader.DesiredSize.Height;
                        currentStack.Children.Add(groupHeader);
                        currentHeight += groupHeaderHeight;
                    }

                    currentStack.Children.Add(row);
                    currentHeight += rowHeight;
                }
            }

            if (currentStack.Children.Count > 0)
            {
                FinalizePage(currentStack);
            }

            // Update footers with correct total page count
            int totalPages = doc.Pages.Count;
            for (int i = 0; i < totalPages; i++)
            {
                if (doc.Pages[i] is PageContent pc &&
                    pc.Child is FixedPage fp)
                {
                    var footer = CreateFooter(i + 1, totalPages);
                    FixedPage.SetTop(footer, pageSize.Height - footerHeight);
                    fp.Children.RemoveAt(fp.Children.Count - 1); // Remove old footer
                    fp.Children.Add(footer);
                }
            }

            return doc;
        }

        /// <summary>
        /// Creates a header for the report.
        /// </summary>
        /// <param name="title">Title of the report.</param>
        /// <returns>A UIElement representing the header.</returns>
        private static UIElement CreateHeader(string title)
        {
            return new TextBlock
            {
                Text = title,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(40, 20, 0, 0)
            };
        }

        /// <summary>
        /// Creates a footer for the report.
        /// </summary>
        /// <param name="pageNumber">Current page number.</param>
        /// <param name="totalPages">Total number of pages.</param>
        /// <returns>A UIElement representing the footer.</returns>
        private static UIElement CreateFooter(int pageNumber, int totalPages)
        {
            double pageWidth = 793.7;
            double margin = 40;

            var footerGrid = new Grid
            {
                Width = pageWidth - (margin * 2),
                Margin = new Thickness(margin, 0, margin, 20)
            };

            footerGrid.ColumnDefinitions.Add(new ColumnDefinition());
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition());
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition());

            var dateText = new TextBlock
            {
                Text = $"{DateTime.Now:dd MMM yyyy HH:ss}",
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            Grid.SetColumn(dateText, 0);

            var pageText = new TextBlock
            {
                Text = $"{pageNumber} / {totalPages}",
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Grid.SetColumn(pageText, 1);

            var companyText = new TextBlock
            {
                Text = $"© 1998 - 2025 Agrisoft Systems",
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Grid.SetColumn(companyText, 2);

            footerGrid.Children.Add(dateText);
            footerGrid.Children.Add(pageText);
            footerGrid.Children.Add(companyText);

            return footerGrid;
        } 
    }
}
