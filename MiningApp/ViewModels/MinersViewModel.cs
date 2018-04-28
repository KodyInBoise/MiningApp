using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiningApp.Windows;

namespace MiningApp
{
    public class MinersViewModel
    {
        private MinersWindow _window;
        private List<MinerModel> _allMiners;

        public MinersViewModel(MinersWindow window)
        {
            _window = window;

            ShowWindow();
        }

        private async void ShowWindow()
        {
            WindowController.Instance.MinersView = this;

            _allMiners = await WindowController.Instance.LoadMiners();

            _window.NewButton.Click += (s, e) => NewButton_Clicked();
            _window.EditButton.Click += (s, e) => EditButton_Clicked();
            _window.LaunchButton.Click += (s, e) => LaunchButton_Clicked();
            _window.PreviousButton.Click += (s, e) => PreviousButton_Clicked();
            _window.NextButton.Click += (s, e) => NextButton_Clicked();

            _window.Left = WindowController.Instance.WindowLeft;
            _window.Top = WindowController.Instance.WindowTop;

            DisplayMiner(_allMiners[_index]);

            _window.Show();
        }

        public void NewButton_Clicked()
        {
            WindowController.Instance.ShowNewMiner();

            Dispose();
        }

        public void EditButton_Clicked()
        {
            WindowController.Instance.ShowEditMiner(_allMiners[_index]);

            Dispose();
        }

        public void LaunchButton_Clicked()
        {

        }

        public void Dispose()
        {
            WindowController.Instance.MinersView = null;

            _window.Close();
        }

        private void DisplayMiner(MinerModel miner)
        {
            _window.NameLabel.Content = $"Name: {miner.Name}";
            _window.PathLabel.Content = $"Path: {ElementHelper.TrimPath(miner.Path)}";
            _window.StatusLabel.Content = $"Status: {miner.Status}";
            _window.OutputLabel.Content = $"Output: {miner.Output}";

            _window.ViewingLabel.Content = $"{_index + 1} of {_allMiners.Count}";
        }

        int _index = 0;
        private void PreviousButton_Clicked()
        {
            if (_index == 0) _index = _allMiners.Count - 1;
            else _index--;

            DisplayMiner(_allMiners[_index]);
        }

        private void NextButton_Clicked()
        {
            if (_index == _allMiners.Count - 1) _index = 0;
            else _index++;

            DisplayMiner(_allMiners[_index]);
        }
    }
}
