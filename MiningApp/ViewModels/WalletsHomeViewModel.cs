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
    public class WalletsHomeViewModel
    {
        private WalletsHomeWindow _window { get; set; }

        private List<WalletConfigModel> _wallets { get; set; }

        private WalletConfigModel _selectedWallet { get; set; }



        public CollectionViewSource GridItems { get; set; }



        public WalletsHomeViewModel(WalletsHomeWindow window)
        {
            _window = window;

            ShowWindow();
        }

        private void ShowWindow()
        {
            OldWindowController.Instance.WalletsHomeView = this;

            _window.Left = OldWindowController.Instance.WindowLeft;
            _window.Top = OldWindowController.Instance.WindowTop;

            _window.NewButton.Click += (s, e) => NewButton_Clicked();
            _window.EditButton.Click += (s, e) => EditButton_Clicked();           

            DisplayGrid();

            _window.Show();
        }

        public void Dispose()
        {
            OldWindowController.Instance.WalletsHomeView = null;

            _window.Close();
        }

        private void DisplayGrid()
        {
            _wallets = WalletHelper.Instance.AllWallets;

            GridItems = (CollectionViewSource)(_window.FindResource("GridItems"));
            GridItems.Source = _wallets;

            _window.WalletsDataGrid.Items.Refresh();           
        }

        private WalletConfigModel GetSelectedWallet()
        {
            DataGridCellInfo cellInfo = _window.WalletsDataGrid.SelectedCells[0];
            if (cellInfo == null) return null;

            DataGridBoundColumn column = cellInfo.Column as DataGridBoundColumn;
            if (column == null) return null;

            FrameworkElement element = new FrameworkElement() { DataContext = cellInfo.Item };
            BindingOperations.SetBinding(element, FrameworkElement.TagProperty, column.Binding);
            var name = element.Tag.ToString();

            foreach (WalletConfigModel wallet in _wallets)
            {
                if (wallet.Name == name)
                {
                    return wallet;
                }
            }

            return null;
        }

        private void NewButton_Clicked()
        {
            OldWindowController.Instance.ShowWalletConfig();
        }

        private void EditButton_Clicked()
        {
            try
            {
                OldWindowController.Instance.ShowWalletConfig(GetSelectedWallet());
            }
            catch
            {

            }
        }

        public void AddWallet(WalletConfigModel wallet)
        {
            try
            {
                _wallets.Add(wallet);

                UpdateGrid();
            }
            catch { }
        }

        public void RemoveWallet(WalletConfigModel wallet)
        {
            try
            {
                _wallets.Remove(wallet);

                UpdateGrid();
            }
            catch { }
        }

        public void UpdateGrid()
        {
            _window.WalletsDataGrid.Items.Refresh();
        }
    }
}
