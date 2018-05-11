using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MiningApp.Windows;

namespace MiningApp
{
    public class WalletConfigViewModel
    {
        private WalletConfigWindow _window { get; set; }

        private WalletConfigModel _wallet { get; set; }

        public WalletConfigViewModel(WalletConfigWindow window, WalletConfigModel wallet = null)
        {
            _window = window;
            _wallet = wallet;

            ShowWindow();
        }

        private void ShowWindow()
        {
            WindowController.Instance.WalletConfigView = this;

            _window.Left = WindowController.Instance.WindowLeft;
            _window.Top = WindowController.Instance.WindowTop;

            _window.CryptoComboBox.ItemsSource = MiningHelper.Instance.GetCryptoNames();

            _window.ClientBrowseButton.Click += (s, e) => ClientBrowseButton_Clicked();
            _window.FinishButton.Click += (s, e) => FinishButton_Clicked();

            if (_wallet == null)
            {
                _window.TitleLabel.Content = "New Wallet Config";
            }
            else
            {
                _window.TitleLabel.Content = "Edit Wallet Config";
            }

            ShowStatusMessage();
            _window.Show();
        }

        public void Dispose()
        {
            WindowController.Instance.WalletConfigView = null;

            _window.Close();
        }

        private WalletConfigModel CreateWalletConfig()
        {
            try
            {
                return new WalletConfigModel()
                {
                    CreatedTimestamp = DateTime.Now,
                    Name = _window.NameTextBox.Text,
                    Crypto = _window.CryptoComboBox.Text,
                    Address = _window.AddressTextBox.Text,
                    ClientPath = _window.ClientTextBox.Text
                };
            }
            catch
            {
                return new WalletConfigModel();
            }
        }

        private void FinishButton_Clicked()
        {
            var message = "";

            if (CanFinish(out message))
            {
                _window.FinishButton.Visibility = Visibility.Collapsed;

                _wallet = CreateWalletConfig();

                if (_wallet != null)
                {
                    DataHelper.Instance.InsertWalletConfig(_wallet);
                }
            }
            else
            {
                ShowStatusMessage(message);
            }
        }

        private bool CanFinish(out string message)
        {
            try
            {
                if (String.IsNullOrEmpty(_window.NameTextBox.Text))
                {
                    message = "A name for this wallet configuration is required!";
                    return false;
                }
                if (String.IsNullOrEmpty(_window.CryptoComboBox.Text))
                {
                    message = "A cryptocurrency for this wallet configuration is required!";
                    return false;
                }
                if (String.IsNullOrEmpty(_window.AddressTextBox.Text))
                {
                    message = "A wallet address for this wallet configuration is required!";
                    return false;
                }
                if (_window.AddressTextBox.Text != _window.VerifyTextBox.Text)
                {
                    message = "Please verify that the wallet addresses match!";
                    return false;
                }

                ShowStatusMessage();
                message = "success";
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        private void ShowStatusMessage(string message = "")
        {
            _window.MessageTextBlock.Text = message;
        }

        private string _path = "";
        private async void ClientBrowseButton_Clicked()
        {
            _path = await WindowController.Instance.GetFilePath();

            _window.ClientTextBox.Text = ElementHelper.TrimPath(_path, length: 60);
        }
    }
}
