using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiningApp.Windows;
using MiningApp.UI;

namespace MiningApp
{
    public class MinersViewModel
    {
        private MinersWindow _window;
        private List<MiningRuleModel> _allMiners;

        public MinersViewModel(MinersWindow window)
        {
            _window = window;

            ShowWindow();
        }

        private async void ShowWindow()
        {
            OldWindowController.Instance.MinersView = this;

            _allMiners = await OldWindowController.Instance.GetMiners();

            _window.NewButton.Click += (s, e) => NewButton_Clicked();
            _window.EditButton.Click += (s, e) => EditButton_Clicked();
            _window.LaunchButton.Click += (s, e) => LaunchButton_Clicked();
            _window.PreviousButton.Click += (s, e) => PreviousButton_Clicked();
            _window.NextButton.Click += (s, e) => NextButton_Clicked();
            _window.UploadButton.Click += (s, e) => UploadButton_Clicked();

            _window.Left = OldWindowController.Instance.WindowLeft;
            _window.Top = OldWindowController.Instance.WindowTop;

            DisplayMiner(_allMiners.Count > 0 ?_allMiners[_index] : new MiningRuleModel());

            _window.Show();
        }

        public void NewButton_Clicked()
        {
            OldWindowController.Instance.ShowNewMiner();

            Dispose();
        }

        public void EditButton_Clicked()
        {
            OldWindowController.Instance.ShowEditMiner(_allMiners[_index]);

            Dispose();
        }

        public void LaunchButton_Clicked()
        {
            OldWindowController.Instance.LaunchMiner(_allMiners[_index]);
        }

        public void Dispose()
        {
            OldWindowController.Instance.MinersView = null;

            _window.Close();
        }

        private void DisplayMiner(MiningRuleModel miner)
        {
            _window.NameLabel.Content = $"Name: {miner.Name}";
            _window.PathLabel.Content = $"Path: {ElementHelper.TrimPath(miner.Path)}";
            _window.StatusLabel.Content = $"Status: {miner.Status}";
            _window.OutputLabel.Content = $"Output: {miner.Output}";

            int current = _allMiners.Count > 0 ? _index + 1 : 0;
            _window.ViewingLabel.Content = $"{current} of {_allMiners.Count}";
        }

        int _index = 0;
        private void PreviousButton_Clicked()
        {
            try
            {
                if (_index == 0) _index = _allMiners.Count - 1;
                else _index--;

                DisplayMiner(_allMiners[_index]);
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        private void NextButton_Clicked()
        {
            try
            {
                if (_index == _allMiners.Count - 1) _index = 0;
                else _index++;

                DisplayMiner(_allMiners[_index]);
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        private void UploadButton_Clicked()
        {
            OldWindowController.Instance.ShowUploadMiner();

            Dispose();
        }
    }
}
