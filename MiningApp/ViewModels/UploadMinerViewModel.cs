using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using MiningApp.Windows;

namespace MiningApp
{
    class UploadMinerViewModel
    {
        private UploadMinerWindow _window { get; set; }

        private MinerConfigModel _miner { get; set; }

        public UploadMinerViewModel(UploadMinerWindow window)
        {
            _window = window;
            _miner = new MinerConfigModel();

            ShowWindow();
        }

        private void ShowWindow()
        {
            WindowController.Instance.UploadMinerView = this;

            _window.Left = WindowController.Instance.WindowLeft;
            _window.Top = WindowController.Instance.WindowTop;

            _window.BrowseButton.Click += (s, e) => BrowseButton_Clicked();
            _window.CryptosAddButton.Click += (s, e) => CryptoAddButton_Clicked();
            _window.CryptosAddTextBox.KeyDown += CryptoAddTextBox_KeyDown;
            _window.TagsAddButton.Click += (s, e) => TagAddButton_Clicked();
            _window.TagsAddTextBox.KeyDown += TagsAddTextBox_KeyDown;
            _window.FinishButton.Click += (s, e) => FinishButton_Clicked();
            

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

        private void CryptoAddTextBox_KeyDown(object s, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CryptoAddButton_Clicked();
            }
        }

        private void TagAddButton_Clicked()
        {
            var tag = _window.TagsAddTextBox.Text;

            if (!String.IsNullOrEmpty(tag) && !_window.TagsListBox.Items.Contains(tag))
            {
                _window.TagsListBox.Items.Add(tag);

                _window.TagsAddTextBox.Clear();
                _window.TagsAddTextBox.Focus();
            }
            else
            {
                _window.TagsAddTextBox.Focus();
                _window.TagsAddTextBox.SelectAll();
            }       
        }

        private void TagsAddTextBox_KeyDown(object s, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TagAddButton_Clicked();
            }
        }

        private MinerConfigModel CreateNewMiner()
        {
            try
            {
                return new MinerConfigModel()
                {
                    ServerID = ServerHelper.Instance.GetUniqueMinerID(),
                    CreatedTimestamp = DateTime.Now,
                    Name = _window.NameTextBox.Text,
                    Cryptos = _window.CryptosListBox.Items.Cast<String>().ToList(),
                    Tags = _window.TagsListBox.Items.Cast<String>().ToList(),
                    FilePath = _window.PathTextBox.Text
                };
            }
            catch
            {
                return new MinerConfigModel();
            }
        }

        private void FinishButton_Clicked()
        {
            try
            {
                _miner = CreateNewMiner();

                ServerHelper.Instance.UploadMiner(_miner);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
