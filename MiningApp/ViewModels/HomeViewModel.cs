using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiningApp.Windows;

namespace MiningApp
{
    public class HomeViewModel
    {
        private HomeWindow _window;

        public HomeViewModel(HomeWindow window)
        {
            _window = window;

            ShowWindow();
        }

        private void ShowWindow()
        {
            WindowController.Instance.HomeView = this;

            _window.MinersButton.Click += (s, e) => MinersButton_Clicked();
            _window.CryptosButton.Click += (s, e) => CryptosButton_Clicked();

            _window.Show();
        }

        public void Dispose()
        {
            WindowController.Instance.HomeView = null;

            _window.Close();
        }

        private void MinersButton_Clicked()
        {
            WindowController.Instance.ShowMiners();
        }

        private void CryptosButton_Clicked()
        {
            WindowController.Instance.ShowCryptos();
        }
    }
}
