using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using MiningApp.Windows;

namespace MiningApp
{
    public class CryptosViewModel
    {
        private CryptosWindow _window;
        private CryptoHelper _helper;

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

            _window.Show();

            DisplayGrid();
        }

        public void Dispose()
        {
            WindowController.Instance.HomeView = null;

            _window.Close();
        }

        private async void DisplayGrid()
        {
            CryptoList = await _helper.GetTopCryptos();

            GridItems = (CollectionViewSource)(_window.FindResource("GridItems"));
            GridItems.Source = CryptoList;

            _window.CryptosDataGrid.Items.Refresh();
        }
    }
}
