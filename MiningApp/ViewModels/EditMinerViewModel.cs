using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MiningApp.Windows;

namespace MiningApp
{
    public class EditMinerViewModel
    {
        private EditMinerWindow _window;
        private MinerModel _miner;
        private List<MinerModel> _allMiners;

        public EditMinerViewModel(EditMinerWindow window, MinerModel miner = null)
        {
            _window = window;
            _miner = miner;

            ShowWindow();
        }

        private async void ShowWindow()
        {
            WindowController.Instance.EditMinersView = this;

            if (_miner == null)
            {
                _window.MinerLabel.Visibility =
                _window.MinerComboBox.Visibility = Visibility.Collapsed;

                _miner = new MinerModel();
            }
            else
            {
                _allMiners = await WindowController.Instance.LoadMiners();
                _allMiners.ForEach(x => _window.MinerComboBox.Items.Add(x));

                _window.MinerLabel.Visibility =
                _window.MinerComboBox.Visibility = Visibility.Visible;

                DisplayMiner(_miner);
            }

            _window.MinerComboBox.DropDownClosed += (s, e) => MinerComboBox_DropDownClosed();
            _window.BrowseButton.Click += (s, e) => BrowseButton_Clicked();
            _window.FinishButton.Click += (s, e) => FinishButton_Clicked();

            _window.Left = WindowController.Instance.WindowLeft;
            _window.Top = WindowController.Instance.WindowTop;

            _window.Show();
        }

        public void Dispose()
        {
            WindowController.Instance.EditMinersView = null;

            _window.Close();
        }
        
        private async void BrowseButton_Clicked()
        {
            var path = await WindowController.Instance.GetFilePath();

            _window.PathTextBox.Text = ElementHelper.TrimPath(path);
        }

        private void FinishButton_Clicked()
        {
            _window.FinishButton.Visibility = Visibility.Collapsed;

            _miner.Created = DateTime.Now;
            _miner.Name = _window.NameTextBox.Text;
            _miner.Path = _window.PathTextBox.Text;
            _miner.Arguments = _window.ArgumentsTextBox.Text;

            Task.Run(() => WindowController.Instance.InsertMiner(_miner));
        }

        private void MinerComboBox_DropDownClosed()
        {
            var selectedMiner = (MinerModel)_window.MinerComboBox.SelectedItem;

            if (_miner != selectedMiner)
            {
                DisplayMiner(selectedMiner);
            }
        }

        private void DisplayMiner(MinerModel miner)
        {
            _window.NameTextBox.Text = miner.Name;
            _window.PathTextBox.Text = ElementHelper.TrimPath(miner.Path);
            _window.ArgumentsTextBox.Text = miner.Arguments;

            _miner = miner;
        }
    }
}
