﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MiningApp.Windows;

namespace MiningApp
{
    public class MinerConfigViewModel
    {

        private MinerConfigWindow _window { get; set; }

        private MinerConfigModel _miner { get; set; }

        private List<string> _allCryptoNames { get; set; }


        public MinerConfigViewModel(MinerConfigWindow window, MinerConfigModel miner = null)
        {
            _window = window;

            ShowWindow();
        }

        private void ShowWindow()
        {
            WindowController.Instance.MinerConfigView = this;

            _window.Left = WindowController.Instance.WindowLeft;
            _window.Top = WindowController.Instance.WindowTop;

            _allCryptoNames = CryptoHelper.Instance.GetCryptoNames();
            _window.CryptosComboBox.ItemsSource = _allCryptoNames;

            _window.PathBrowseButton.Click += (s, e) => PathBrowseButton_Clicked();
            _window.CryptosAddButton.Click += (s, e) => CryptosAddButton_Clicked();
            _window.CryptosRemoveButton.Click += (s, e) => CryptosRemoveButton_Clicked();
            _window.DeleteButton.Click += (s, e) => DeleteButton_Clicked();
            _window.FinishButton.Click += (s, e) => FinishButton_Clicked();

            _window.Show();
        }

        public void Dispose()
        {
            WindowController.Instance.MinerConfigView = null;

            _window.Close();
        }

        private void ShowMessage(string message = "")
        {
            _window.MessageTextBlock.Text = message;
        }

        string _path = "";
        private async void PathBrowseButton_Clicked()
        {
            _path = await WindowController.Instance.GetFilePath();

            _window.PathTextBox.Text = ElementHelper.TrimPath(_path, 50);
        }

        private async void CryptosAddButton_Clicked()
        {
            var cryptoName = _window.CryptosComboBox.Text;

            if (!String.IsNullOrEmpty(cryptoName))
            {
                ShowMessage("Adding crypto...");

                var crypto = await CryptoHelper.Instance.CreateCryptoFromName(cryptoName);

                if (crypto != null)
                {
                    _window.CryptosListBox.Items.Add(cryptoName);

                    _allCryptoNames.Remove(cryptoName);
                    _window.CryptosComboBox.Items.Refresh();

                    ShowMessage();
                }
                else
                {
                    _allCryptoNames.Remove(cryptoName);
                    _window.CryptosComboBox.Items.Refresh();

                    ShowMessage("Failed to add crypto!");
                }
            }
        }

        private void CryptosRemoveButton_Clicked()
        {
            var cryptoName = (string)_window.CryptosListBox.SelectedItem;

            if (!String.IsNullOrEmpty(cryptoName))
            {
                ShowMessage("Removing crypto...");

                _window.CryptosListBox.Items.Remove(cryptoName);

                _allCryptoNames.Add(cryptoName);
                _window.CryptosComboBox.Items.Refresh();

                ShowMessage();
            }
        }

        private void DeleteButton_Clicked()
        {

        }

        private void FinishButton_Clicked()
        {
            ShowMessage("Saving miner config...");

            _window.FinishButton.Visibility = Visibility.Collapsed;

            _miner = SetMinerInfo();

            var message = "";

            if (CanFinish(out message))
            {
                if (_miner.ID > 0)
                {
                    DataHelper.Instance.UpdateMinerConfig(_miner);
                }
                else
                {
                    DataHelper.Instance.InsertMinerConfig(_miner);
                }

                ShowMessage();
            }
            else
            {
                ShowMessage(message);

                _window.FinishButton.Visibility = Visibility.Visible;
            }
        }

        private bool CanFinish(out string message)
        {
            message = "";

            if (String.IsNullOrEmpty(_window.NameTextBox.Text))
            {
                message = "Please give a name to this miner config!";
                return false;
            }
            if (String.IsNullOrEmpty(_window.PathTextBox.Text))
            {
                message = "Please give a file path for this miner config!";
                return false;
            }
            if (_window.CryptosListBox.Items.Count == 0)
            {
                message = "Please add a crypto to this miner config!";
                return false;
            }

            return true;
        }

        private MinerConfigModel SetMinerInfo()
        {
            try
            {
                if (_miner == null) _miner = new MinerConfigModel();

                _miner.Name = _window.NameTextBox.Text;
                _miner.FilePath = _window.PathTextBox.Text;
                _miner.Cryptos = _window.CryptosListBox.Items.Cast<string>().ToList();

                return _miner;
            }
            catch (Exception ex)
            {
                ShowMessage($"Failed to create miner config: {ex.Message}");

                return null;
            }
        }
    }
}