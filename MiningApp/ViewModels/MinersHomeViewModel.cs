using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MiningApp.Windows;

namespace MiningApp
{
    public class MinersHomeViewModel
    {
        public CollectionViewSource GridItems { get; set; }



        private MinersHomeWindow _window { get; set; }

        private List<MinerConfigModel> _localMiners { get; set; }

        private List<MinerConfigModel> _allMiners { get; set; }



        public MinersHomeViewModel(MinersHomeWindow window)
        {
            _window = window;

            ShowWindow();
        }

        private void ShowWindow()
        {
            OldWindowController.Instance.MinersHomeView = this;

            _window.Left = OldWindowController.Instance.WindowLeft;
            _window.Top = OldWindowController.Instance.WindowTop;

            _window.NewButton.Click += (s, e) => NewButton_Clicked();
            _window.EditButton.Click += (s, e) => EditButton_Clicked();

            DisplayGrid();

            _window.Show();
        }

        public void Dispose()
        {
            OldWindowController.Instance.MinersHomeView = null;

            _window.Close();
        }

        private async void DisplayGrid(bool allMiners = false)
        { 
            GridItems = (CollectionViewSource)(_window.FindResource("GridItems"));

            if (allMiners)
            {
                _allMiners = ServerHelper.Instance.GetMiners();

                GridItems.Source = _allMiners;
            }
            else
            {
                _localMiners = await DataHelper.Instance.GetAllMinerConfigs();

                GridItems.Source = _localMiners;
            }

            _window.MinersDataGrid.Items.Refresh();
        }

        private MinerConfigModel GetSelectedMiner()
        {
            DataGridCellInfo cellInfo = _window.MinersDataGrid.SelectedCells[0];
            if (cellInfo == null) return null;

            DataGridBoundColumn column = cellInfo.Column as DataGridBoundColumn;
            if (column == null) return null;

            FrameworkElement element = new FrameworkElement() { DataContext = cellInfo.Item };
            BindingOperations.SetBinding(element, FrameworkElement.TagProperty, column.Binding);
            var name = element.Tag.ToString();

            foreach (MinerConfigModel miner in _localMiners)
            {
                if (miner.Name == name)
                {
                    return miner;
                }
            }

            return null;
        }

        public void AddMiner(MinerConfigModel miner)
        {
            try
            {
                _localMiners.Add(miner);

                UpdateGrid();
            }
            catch { }
        }

        public void RemoveMinerConfig(MinerConfigModel miner)
        {
            try
            {
                _localMiners.Remove(miner);

                UpdateGrid();
            }
            catch { }
        }

        public void UpdateGrid()
        {
            _window.MinersDataGrid.Items.Refresh();
        }

        private void ViewingMiners_Changed()
        {
            if (_window.AllMinersRadioButton.IsChecked == true)
            {
                DisplayGrid(true);
            }
            else
            {
                DisplayGrid();
            }
        }

        private void NewButton_Clicked()
        {
            OldWindowController.Instance.ShowMinerConfig();
        }

        private void EditButton_Clicked()
        {
            OldWindowController.Instance.ShowMinerConfig(GetSelectedMiner());
        }
    }
}
