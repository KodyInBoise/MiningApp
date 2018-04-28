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
            _window.DeleteButton.Click += (s, e) => DeleteButton_Clicked();
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

        string _filePath = "";
        private async void BrowseButton_Clicked()
        {
            _filePath = await WindowController.Instance.GetFilePath();

            _window.PathTextBox.Text = ElementHelper.TrimPath(_filePath);
        }

        private void DeleteButton_Clicked()
        {
            var result = MessageBox.Show("Are you sure you want to delete this miner?", "Delete Miner", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                WindowController.Instance.DeleteMiner(_miner);

                WindowController.Instance.ShowMiners();
                _window.Close();
                Dispose();
            }
        }

        private void FinishButton_Clicked()
        {
            _window.FinishButton.Visibility = Visibility.Collapsed;

            _miner.Name = _window.NameTextBox.Text;
            _miner.Path = _filePath;
            _miner.Arguments = _window.ArgumentsTextBox.Text;

            if (_miner.ID > 0)
            {
                Task.Run(() => WindowController.Instance.UpdateMiner(_miner));
            }
            else
            {
                _miner.Created = DateTime.Now;

                Task.Run(() => WindowController.Instance.InsertMiner(_miner));
            }
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
            _window.MinerComboBox.Text = miner.Name;
            _window.NameTextBox.Text = miner.Name;
            _window.ArgumentsTextBox.Text = miner.Arguments;

            _filePath = miner.Path;
            _window.PathTextBox.Text = ElementHelper.TrimPath(_filePath);

            _miner = miner;
        }
    }
}
