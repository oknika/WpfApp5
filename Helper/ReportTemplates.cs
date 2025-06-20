using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfApp5.Models;

namespace WpfApp5.Helper
{
    public static class ReportTemplates
    {
        public static UIElement CreateProductGroupHeader(ProductGroup group)
        {
            return new TextBlock
            {
                Text = $"Group: {group.GroupName}",
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Background = Brushes.LightGray,
                Margin = new Thickness(0, 10, 0, 4),
                Padding = new Thickness(4)
            };
        }

        public static UIElement CreateProductGroupFooter(ProductGroup group)
        {
            return new TextBlock
            {
                Text = $"Total Products in Group: {group.Products.Count}",
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                Margin = new Thickness(0, 4, 0, 10),
                Padding = new Thickness(4)
            };
        }

        public static UIElement CreateProductColumnHeader()
        {
            var grid = new Grid
            {
                Background = Brushes.LightSteelBlue,
                Margin = new Thickness(20, 4, 0, 2)
            };

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
            //grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });

            grid.Children.Add(new TextBlock
            {
                Text = "Product ID",
                FontWeight = FontWeights.Bold
            });
            Grid.SetColumn(grid.Children[^1], 0);

            grid.Children.Add(new TextBlock
            {
                Text = "Product Name",
                FontWeight = FontWeights.Bold
            });
            Grid.SetColumn(grid.Children[^1], 1);

            grid.Children.Add(new TextBlock
            {
                Text = "Qty",
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            Grid.SetColumn(grid.Children[^1], 2);

            /*grid.Children.Add(new TextBlock
            {
                Text = "",
                FontWeight = FontWeights.Bold
            });
            Grid.SetColumn(grid.Children[^1], 3);*/

            return grid;
        }

        public static UIElement CreateProductGroupedRow(Product product)
        {
            var grid = new Grid
            {
                Margin = new Thickness(20, 1, 0, 1)
            };

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) }); // ProductID
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Name
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) }); // Qty
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) }); // Unit

            grid.Children.Add(new TextBlock
            {
                Text = product.ProductID,
                FontSize = 12
            });
            Grid.SetColumn(grid.Children[^1], 0);

            grid.Children.Add(new TextBlock
            {
                Text = product.ProductName,
                FontSize = 12
            });
            Grid.SetColumn(grid.Children[^1], 1);

            grid.Children.Add(new TextBlock
            {
                Text = product.ProductQty.ToString("N0"),
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Right
            });
            Grid.SetColumn(grid.Children[^1], 2);

            grid.Children.Add(new TextBlock
            {
                Text = product.ProductUnit,
                FontSize = 12
            });
            Grid.SetColumn(grid.Children[^1], 3);

            return grid;
        }

        public static UIElement CreateProductHeader()
        {
            var panel = new DockPanel { Margin = new Thickness(2) };

            panel.Children.Add(new TextBlock { Text = "ID", Width = 60, FontWeight = FontWeights.Bold });
            panel.Children.Add(new TextBlock { Text = "Name", Width = 150, Margin = new Thickness(5, 0, 0, 0), FontWeight = FontWeights.Bold });
            panel.Children.Add(new TextBlock { Text = "Qty", Width = 60, Margin = new Thickness(5, 0, 0, 0), FontWeight = FontWeights.Bold });
            panel.Children.Add(new TextBlock { Text = "Price", Width = 80, Margin = new Thickness(5, 0, 0, 0), FontWeight = FontWeights.Bold });
            panel.Children.Add(new TextBlock { Text = "Group", Margin = new Thickness(5, 0, 0, 0), FontWeight = FontWeights.Bold });

            return panel;
        }

        public static UIElement CreateProductRow(Product product)
        {
            var panel = new DockPanel { Margin = new Thickness(2) };

            panel.Children.Add(new TextBlock { Text = product.ProductID, Width = 60 });
            panel.Children.Add(new TextBlock { Text = product.ProductName, Width = 150, Margin = new Thickness(5, 0, 0, 0) });
            panel.Children.Add(new TextBlock { Text = product.ProductQty.ToString(), Width = 60, Margin = new Thickness(5, 0, 0, 0) });
            panel.Children.Add(new TextBlock { Text = product.ProductPrice.ToString("N0"), Width = 80, Margin = new Thickness(5, 0, 0, 0) });
            panel.Children.Add(new TextBlock { Text = product.ProductGrpName, Margin = new Thickness(5, 0, 0, 0) });

            return panel;
        }

        public static UIElement CreateBigTitleHeader(string title)
        {
            var stack = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(40, 20, 0, 10),
                VerticalAlignment = VerticalAlignment.Center
            };

            var icon = new Image
            {
                Width = 32,
                Height = 32,
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/MainLogo.png")),
                Margin = new Thickness(0, 0, 10, 0)
            };

            var text = new TextBlock
            {
                Text = title,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center
            };

            stack.Children.Add(icon);
            stack.Children.Add(text);

            return stack;
        }

        public static UIElement CreateSmallTitleHeader(string title) =>
        new TextBlock
        {
            Text = title,
            FontSize = 12,
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(40, 10, 0, 10)
        };

    }
}
