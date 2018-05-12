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
    public class PoolsHomeViewModel
    {
        private PoolsHomeWindow _window { get; set; }

        private List<PoolConfigModel> _pools { get; set; }



        public CollectionViewSource GridItems { get; set; }



        public PoolsHomeViewModel(PoolsHomeWindow window)
        {
            _window = window;

            ShowWindow();
        }

        private void ShowWindow()
        {
            WindowController.Instance.PoolsHomeView = this;

            _window.Left = WindowController.Instance.WindowLeft;
            _window.Top = WindowController.Instance.WindowTop;

            _window.NewButton.Click += (s, e) => NewButton_Clicked();
            _window.EditButton.Click += (s, e) => EditButton_Clicked();

            DisplayGrid();

            _window.Show();
        }

        public void Dispose()
        {
            WindowController.Instance.PoolsHomeView = null;

            _window.Close();
        }

        private void NewButton_Clicked()
        {
            WindowController.Instance.ShowPoolConfig();
        }

        private void EditButton_Clicked()
        {
            WindowController.Instance.ShowPoolConfig(GetSelectedPool());
        }

        private void DisplayGrid()
        {
            _pools = PoolHelper.Instance.LocalPools;

            GridItems = (CollectionViewSource)(_window.FindResource("GridItems"));
            GridItems.Source = _pools;

            _window.PoolsDataGrid.Items.Refresh();
        }

        private PoolConfigModel GetSelectedPool()
        {
            DataGridCellInfo cellInfo = _window.PoolsDataGrid.SelectedCells[0];
            if (cellInfo == null) return null;

            DataGridBoundColumn column = cellInfo.Column as DataGridBoundColumn;
            if (column == null) return null;

            FrameworkElement element = new FrameworkElement() { DataContext = cellInfo.Item };
            BindingOperations.SetBinding(element, FrameworkElement.TagProperty, column.Binding);
            var name = element.Tag.ToString();

            foreach (PoolConfigModel pool in _pools)
            {
                if (pool.Name == name)
                {
                    return pool;
                }
            }

            return null;
        }
    }
}
