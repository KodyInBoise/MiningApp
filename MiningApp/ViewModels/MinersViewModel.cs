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

        public MinersViewModel(MinersWindow window)
        {
            _window = window;

            ShowWindow();
        }

        private void ShowWindow()
        {
            WindowController.Instance.MinersView = this;

            _window.NewButton.Click += (s, e) => NewButton_Clicked();
            _window.EditButton.Click += (s, e) => EditButton_Clicked();
            _window.LaunchButton.Click += (s, e) => LaunchButton_Clicked();

            _window.Left = WindowController.Instance.WindowLeft;
            _window.Top = WindowController.Instance.WindowTop;

            _window.Show();
        }

        public void NewButton_Clicked()
        {
            WindowController.Instance.ShowNewMiner();

            Dispose();
        }

        public void EditButton_Clicked()
        {
            WindowController.Instance.ShowEditMiner();

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
    }
}
