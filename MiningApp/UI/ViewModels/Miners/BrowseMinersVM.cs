using MiningApp.LoggingUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiningApp.UI
{
    public class BrowseMinersVM
    {
        Grid ViewGrid { get; set; } = MainWindow.Instance.PrimaryGrid;

        TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Browse Miners", 40, width: 300);

        TextBlock CryptoTextBlock { get; set; } = ElementHelper.CreateTextBlock("Crypto", fontSize: 28, height: 40, width: 130);

        ComboBox CryptoComboBox { get; set; } = ElementHelper.CreateComboBox("Crypto", width: 250);

        DataGrid DataGrid { get; set; } = ElementHelper.CreateDataGrid("BrowseMiners");

        DataGridColumn TimestampColumn { get; set; } = ElementHelper.CreateGridTextColumn("Name", binding: "Name", width: 175);

        DataGridColumn MessageColumn { get; set; } = ElementHelper.CreateGridTextColumn("Cryptos", binding: "CryptosString", width: 740);

        double nextLeft = 10;

        double nextTop = 10;

        double padding = 15;


        public BrowseMinersVM()
        {
            Show();
        }

        private void Show()
        {
            DisplayElement(TitleTextBlock);

            nextLeft = nextLeft + padding * 2;
            nextTop = 125;
            DisplayElement(CryptoTextBlock);

            nextTop = CryptoTextBlock.Margin.Top + 5;
            nextLeft = CryptoTextBlock.Margin.Left + CryptoTextBlock.Width;
            DisplayElement(CryptoComboBox);

            nextLeft = TitleTextBlock.Margin.Left + padding * 2;
            DisplayElement(DataGrid, topPadding: padding);
            DataGrid.Columns.Add(TimestampColumn);
            DataGrid.Columns.Add(MessageColumn);

            CryptoComboBox.ItemsSource = CryptoHelper.Instance.GetCryptoNames();
            CryptoComboBox.SelectedIndex = 0;

            ShowMiners();
        }

        public void Dispose()
        {
            WindowController.Instance.BrowseMinersView = null;
        }

        private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
        {
            element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

            ViewGrid.Children.Add(element);

            nextTop = element.Margin.Top + element.Height + padding;
        }

        void ShowMiners()
        {
            DataGrid.ItemsSource = ServerHelper.GetMiners();
        }
    }
}
