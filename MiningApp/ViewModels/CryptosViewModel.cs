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

        private List<CryptoModel> _cryptos = new List<CryptoModel>();

        private CryptoModel _selectedCrypto { get; set; }



        public CollectionViewSource GridItems { get; set; }



        public CryptosViewModel(CryptosWindow window)
        {
            _window = window;

            ShowWindow();
        }

        private void ShowWindow()
        {
            OldWindowController.Instance.CryptosView = this;

            _window.Left = OldWindowController.Instance.WindowLeft;
            _window.Top = OldWindowController.Instance.WindowTop;

            _window.WatchButton.Click += (s, e) => WatchButton_Clicked();

            DisplayGrid();

            _window.Show();
        }

        public void Dispose()
        {
            OldWindowController.Instance.ControlBarView = null;

            _window.Close();
        }

        private async void DisplayGrid()
        {
            _cryptos = await CryptoHelper.Instance.GetTopCryptos();

            GridItems = (CollectionViewSource)(_window.FindResource("GridItems"));
            GridItems.Source = _cryptos;

            _window.CryptosDataGrid.Items.Refresh();
        }

        private void WatchButton_Clicked()
        {
            OldWindowController.User.AddToCryptoWatchList(GetSelectedCrypto());
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

            foreach (CryptoModel crypto in _cryptos)
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
