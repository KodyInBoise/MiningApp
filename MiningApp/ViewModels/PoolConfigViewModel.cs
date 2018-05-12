using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MiningApp.Windows;

namespace MiningApp
{
    public class PoolConfigViewModel
    {
        private PoolConfigWindow _window { get; set; }

        private PoolConfigModel _pool { get; set; }



        public PoolConfigViewModel(PoolConfigWindow window, PoolConfigModel pool = null)
        {
            _window = window;
            _pool = pool;

            ShowWindow();
        }

        private void ShowWindow()
        {
            OldWindowController.Instance.PoolConfigView = this;

            _window.Left = OldWindowController.Instance.WindowLeft;
            _window.Top = OldWindowController.Instance.WindowTop;

            _window.CryptoComboBox.ItemsSource = CryptoHelper.Instance.GetCryptoNames();

            _window.DeleteButton.Click += (s, e) => DeleteButton_Clicked();
            _window.FinishButton.Click += (s, e) => FinishButton_Clicked();
            _window.Closing += (s, e) => Window_Closing();

            if (_pool == null)
            {
                _window.TitleLabel.Content = "New Pool Config";
                _window.DeleteButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                _window.TitleLabel.Content = "Edit Pool Config";
                _window.DeleteButton.Visibility = Visibility.Visible;

                DisplayPool(_pool);
            }

            ShowStatusMessage();
            _window.Show();
        }

        public void Dispose()
        {
            OldWindowController.Instance.PoolConfigView = null;

            _window.Close();
        }

        private void FinishButton_Clicked()
        {
            var message = "";

            if (CanFinish(out message))
            {
                _window.FinishButton.Visibility = Visibility.Collapsed;

                _pool = SetPoolInfo();

                if (_pool.ID > 0)
                {
                    DataHelper.Instance.UpdatePoolConfig(_pool);

                    OldWindowController.Instance.PoolsHomeView?.UpdateGrid();
                }
                else
                {
                    DataHelper.Instance.InsertPoolConfig(_pool);

                    OldWindowController.Instance.PoolsHomeView?.AddPool(_pool);
                }
            }
            else
            {
                ShowStatusMessage(message);
            }
        }

        private void DeleteButton_Clicked()
        {
            var result = MessageBox.Show("Are you sure you want to delete this pool config?", "Delete Pool Config",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Task.Run(() => DataHelper.Instance.DeletePoolConfig(_pool));

                OldWindowController.Instance.PoolsHomeView?.RemovePool(_pool);

                Dispose();
            }
        }

        private void ShowStatusMessage(string message = "")
        {
            _window.MessageTextBlock.Text = message;
        }

        private PoolConfigModel SetPoolInfo()
        {
            if (_pool == null) _pool = new PoolConfigModel { CreatedTimestamp = DateTime.Now };

            _pool.Crypto = _window.CryptoComboBox.Text;
            _pool.Name = _window.NameTextBox.Text;
            _pool.Address = _window.AddressTextBox.Text;
            _pool.Fee = Double.Parse(_window.FeeTextBox.Text);
            _pool.Note = _window.NoteTextBox.Text;

            return _pool;
        }

        private bool CanFinish(out string message)
        {
            message = "";

            if (String.IsNullOrEmpty(_window.CryptoComboBox.Text))
            {
                message = "Pool config must have a crypto type!";
                return false;
            }
            if (String.IsNullOrEmpty(_window.NameTextBox.Text))
            {
                message = "Pool config must have a name!";
                return false;
            }
            if (String.IsNullOrEmpty(_window.AddressTextBox.Text))
            {
                message = "Pool config must have a valid IP address or URL!";
                return false;
            }
            if (!String.IsNullOrEmpty(_window.FeeTextBox.Text))
            {
                try
                {
                    var fee = Double.Parse(_window.FeeTextBox.Text);

                    if (fee < 0.1)
                    {
                        message = "Fee must be a number greater than 0!";
                        return false;
                    }
                }
                catch
                {
                    message = "Fee must be a number percentage greater than 0!";
                    return false;
                }
            }

            message = "success";
            return true;
        }

        private void DisplayPool(PoolConfigModel pool)
        {
            _window.CryptoComboBox.Text = pool.Crypto;
            _window.NameTextBox.Text = pool.Name;
            _window.AddressTextBox.Text = pool.Address;
            _window.FeeTextBox.Text = pool.Fee.ToString();
            _window.NoteTextBox.Text = pool.Note ?? "";
        }

        private void Window_Closing()
        {
            OldWindowController.Instance.PoolsHomeView?.UpdateGrid();
        }
    }
}
