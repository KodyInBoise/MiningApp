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

            _window.CryptoComboBox.ItemsSource = CryptoHelper.Instance.GetCryptoNames();

            _window.ClientBrowseButton.Click += (s, e) => ClientBrowseButton_Clicked();
            _window.DeleteButton.Click += (s, e) => DeleteButton_Clicked();
            _window.FinishButton.Click += (s, e) => FinishButton_Clicked();

            if (_wallet == null)
            {
                _window.TitleLabel.Content = "New Wallet Config";
                _window.DeleteButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                _window.TitleLabel.Content = "Edit Wallet Config";
                _window.DeleteButton.Visibility = Visibility.Visible;

                DisplayWallet(_wallet);
            }

            ShowStatusMessage();
            _window.Show();
        }

        public void Dispose()
        {
            WindowController.Instance.WalletConfigView = null;

            _window.Close();
        }

        private WalletConfigModel SetWalletInfo()
        {
            try
            {
                if (_wallet == null) _wallet = new WalletConfigModel { CreatedTimestamp = DateTime.Now };

                _wallet.Name = _window.NameTextBox.Text;
                _wallet.Crypto = _window.CryptoComboBox.Text;
                _wallet.Address = _window.AddressTextBox.Text;
                _wallet.ClientPath = _path;
                _wallet.Status = WalletStatus.Active;

                return _wallet;
            }
            catch
            {
                return new WalletConfigModel();
            }
        }

        private void DeleteButton_Clicked()
        {
            var result = MessageBox.Show("Are you sure you want to delete this wallet config?", "Delete Wallet Config", 
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Task.Run(() => DataHelper.Instance.DeleteWalletConfig(_wallet));

                WindowController.Instance.WalletsHomeView?.RemoveWallet(_wallet);

                Dispose();
            }
        }

        private void FinishButton_Clicked()
        {
            var message = "";

            if (CanFinish(out message))
            {
                _window.FinishButton.Visibility = Visibility.Collapsed;

                _wallet = SetWalletInfo();

                if (_wallet.ID > 0)
                {
                    DataHelper.Instance.UpdateWalletConfig(_wallet);

                    WindowController.Instance.WalletsHomeView?.UpdateGrid();
                }
                else
                {
                    DataHelper.Instance.InsertWalletConfig(_wallet);

                    WindowController.Instance.WalletsHomeView?.AddWallet(_wallet);
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
                if (WalletHelper.Instance.WalletNameTaken(_window.NameTextBox.Text))
                {
                    message = "A wallet with that name already exists!";
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

            _window.ClientPathTextBox.Text = ElementHelper.TrimPath(_path, length: 60);
        }

        private void DisplayWallet(WalletConfigModel wallet)
        {
            _window.NameTextBox.Text = wallet.Name;
            _window.CryptoComboBox.Text = wallet.Crypto;
            _window.AddressTextBox.Text = wallet.Address;
            _window.VerifyTextBox.Text = wallet.Address;

            _path = wallet.ClientPath;
            _window.ClientPathTextBox.Text = ElementHelper.TrimPath(_path, 60);
        }
    }
}
