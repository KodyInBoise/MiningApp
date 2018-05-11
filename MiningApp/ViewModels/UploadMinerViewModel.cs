using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiningApp.Windows;

namespace MiningApp
{
    class UploadMinerViewModel
    {
        private UploadMinerWindow _window { get; set; }

        private MinerModel _miner { get; set; }

        public UploadMinerViewModel(UploadMinerWindow window)
        {
            _window = window;
            _miner = new MinerModel();

            ShowWindow();
        }

        private void ShowWindow()
        {
            WindowController.Instance.UploadMinerView = this;

            _window.Left = WindowController.Instance.WindowLeft;
            _window.Top = WindowController.Instance.WindowTop;

            _window.BrowseButton.Click += (s, e) => BrowseButton_Clicked();
            _window.CryptosAddButton.Click += (s, e) => CryptoAddButton_Clicked();

            _window.Show();
        }

        public void Dispose()
        {
            WindowController.Instance.UploadMinerView = null;

            _window.Close();
        }

        private async void BrowseButton_Clicked()
        {
            var path = await WindowController.Instance.GetFilePath();

            _window.PathTextBox.Text = path;
            _window.PathTextBox.ScrollToEnd();
        }

        private async void CryptoAddButton_Clicked()
        {
            if (!String.IsNullOrEmpty(_window.CryptosAddTextBox.Text))
            {
                try
                {
                    var cryptoName = _window.CryptosAddTextBox.Text;

                    if (await _miner.AddCoin(cryptoName))
                    {
                        _window.CryptosListBox.Items.Add(cryptoName);

                        _window.CryptosAddTextBox.Clear();
                        _window.CryptosAddTextBox.Focus();
                    }
                    else
                    {
                        _window.CryptosAddTextBox.Focus();
                        _window.CryptosAddTextBox.SelectAll();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        private void TagAddButton_Clicked()
        {

        }
    }
}
