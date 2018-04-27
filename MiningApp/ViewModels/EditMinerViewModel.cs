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

        public EditMinerViewModel(EditMinerWindow window, MinerModel miner = null)
        {
            _window = window;
            _miner = miner;

            ShowWindow();
        }

        private void ShowWindow()
        {
            WindowController.Instance.EditMinersView = this;

            if (_miner == null)
            {
                _window.MinerLabel.Visibility =
                _window.MinerComboBox.Visibility = Visibility.Collapsed;

                _miner = new MinerModel();
            }

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
    }
}
