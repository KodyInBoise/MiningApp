using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MiningApp.Windows;

namespace MiningApp
{
    public class CryptosViewModel
    {
        private CryptosWindow _window { get; set; }

        private CryptoHelper _helper { get; set; }

        private CryptoModel _selectedCrypto { get; set; }

        public List<CryptoModel> CryptoList = new List<CryptoModel>();

        public CollectionViewSource GridItems;

        public CryptosViewModel(CryptosWindow window)
        {
            _window = window;
            _helper = new CryptoHelper();

            ShowWindow();
        }

        private void ShowWindow()
        {
            WindowController.Instance.CryptosView = this;

            _window.Left = WindowController.Instance.WindowLeft;
            _window.Top = WindowController.Instance.WindowTop;

            _window.WatchButton.Click += (s, e) => WatchButton_Clicked();

            _window.Show();

            DisplayGrid();
        }

        public void Dispose()
        {
            WindowController.Instance.ControlBarView = null;

            _window.Close();
        }

        private async void DisplayGrid()
        {
            CryptoList = await _helper.GetTopCryptos();

            GridItems = (CollectionViewSource)(_window.FindResource("GridItems"));
            GridItems.Source = CryptoList;

            _window.CryptosDataGrid.Items.Refresh();
        }

        private void WatchButton_Clicked()
        {
            WindowController.User.AddToCryptoWatchList(GetSelectedCrypto());
        }

        private CryptoModel GetSelectedCrypto()
        {
            DataGridCellInfo cellInfo = _window.CryptosDataGrid.SelectedCells[0];
            if (cellInfo == null) return null;

            DataGridBoundColumn column = cellInfo.Column as DataGridBoundColumn;
            if (column == null) return null;

            FrameworkElement element = new FrameworkElement() { DataContext = cellInfo.Item };
            BindingOperations.SetBinding(element, FrameworkElement.TagProperty, column.Binding);
            var symbol = element.Tag.ToString();

            foreach (CryptoModel crypto in CryptoList)
            {
                if (crypto.Symbol == symbol)
                {
                    return crypto;
                }
            }

            return null;
        }
    }
}
