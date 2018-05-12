using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MiningApp.Windows;

namespace MiningApp
{
    public class MiningRuleViewModel
    {
        private MiningRuleWindow _window;

        private MiningRuleModel _miner;

        private List<MiningRuleModel> _allMiners;



        public MiningRuleViewModel(MiningRuleWindow window, MiningRuleModel miner = null)
        {
            _window = window;
            _miner = miner;

            ShowWindow();
        }

        private async void ShowWindow()
        {
            OldWindowController.Instance.MiningConfigView = this;

            if (_miner == null)
            {
                //TODO
            }
            else
            {
                
            }

            _window.WalletComboBox.ItemsSource = WalletHelper.Instance.AllWallets;
            _window.PoolComboBox.ItemsSource = PoolHelper.Instance.LocalPools;

            _window.MinerComboBox.DropDownClosed += (s, e) => MinerComboBox_DropDownClosed();
            _window.DeleteButton.Click += (s, e) => DeleteButton_Clicked();
            _window.FinishButton.Click += (s, e) => FinishButton_Clicked();

            _window.Left = OldWindowController.Instance.WindowLeft;
            _window.Top = OldWindowController.Instance.WindowTop;

            _window.Show();
        }

        public void Dispose()
        {
            OldWindowController.Instance.MiningConfigView = null;

            _window.Close();
        }

        string _filePath = "";
        private async void BrowseButton_Clicked()
        {
            _filePath = await OldWindowController.Instance.GetFilePath();

            //_window.PathTextBox.Text = ElementHelper.TrimPath(_filePath);
        }

        private void DeleteButton_Clicked()
        {
            var result = MessageBox.Show("Are you sure you want to delete this miner?", "Delete Miner", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                OldWindowController.Instance.DeleteMiner(_miner);

                OldWindowController.Instance.ShowMiners();
                _window.Close();
                Dispose();
            }
        }

        private void FinishButton_Clicked()
        {
            _window.FinishButton.Visibility = Visibility.Collapsed;

            _miner.Name = _window.NameTextBox.Text;
            _miner.Path = _filePath;
            //_miner.Arguments = _window.ArgumentsTextBox.Text;

            if (_miner.ID > 0)
            {
                Task.Run(() => OldWindowController.Instance.UpdateMiner(_miner));
            }
            else
            {
                _miner.CreatedTimestamp = DateTime.Now;

                Task.Run(() => OldWindowController.Instance.InsertMiner(_miner));
            }
        }

        private void MinerComboBox_DropDownClosed()
        {
            var selectedMiner = (MiningRuleModel)_window.MinerComboBox.SelectedItem;

            if (_miner != selectedMiner)
            {
                DisplayMiner(selectedMiner);
            }
        }

        private void DisplayMiner(MiningRuleModel miner)
        {
            _window.MinerComboBox.Text = miner.Name;
            _window.NameTextBox.Text = miner.Name;
            //_window.ArgumentsTextBox.Text = miner.Arguments;

            _filePath = miner.Path;
            //_window.PathTextBox.Text = ElementHelper.TrimPath(_filePath);

            _miner = miner;
        }
    }
}
