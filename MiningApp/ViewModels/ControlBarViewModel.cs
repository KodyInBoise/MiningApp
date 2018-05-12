using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiningApp.LoggingUtil;
using MiningApp.Windows;

namespace MiningApp
{
    public class ControlBarViewModel
    {
        private ControlBarWindow _window;

        public ControlBarViewModel(ControlBarWindow window)
        {
            _window = window;

            ShowWindow();
        }

        private void ShowWindow()
        {
            WindowController.Instance.ControlBarView = this;

            _window.Left = WindowController.User.ControlBarLeft ?? 25.0;
            _window.Top = WindowController.User.ControlBarTop ?? 25.0;

            _window.MinersButton.Click += (s, e) => MinersButton_Clicked();
            _window.CryptosButton.Click += (s, e) => CryptosButton_Clicked();
            _window.HomeButton.Click += (s, e) => HomeButton_Clicked();
            _window.WalletsButton.Click += (s, e) => WalletsButton_Clicked();
            _window.PoolsButton.Click += (s, e) => PoolsButton_Clicked();
            _window.LogsButton.Click += (s, e) => LogsButton_Clicked();

            _window.Show();
        }

        public void Dispose()
        {
            WindowController.Instance.ControlBarView = null;

            _window.Close();
        }

        private void MinersButton_Clicked()
        {
            //WindowController.Instance.ShowMiners();

            WindowController.Instance.ShowMinersHome();
        }

        private void CryptosButton_Clicked()
        {
            WindowController.Instance.ShowCryptos();
        }

        private void HomeButton_Clicked()
        {
            WindowController.Instance.ShowHome();
        }

        private void WalletsButton_Clicked()
        {
            WindowController.Instance.ShowWalletsHome();
        }

        private void PoolsButton_Clicked()
        {
            WindowController.Instance.ShowPoolsHome();
        }

        private void LogsButton_Clicked()
        {
            LogHelper.Instance.ShowWindow();
        }
    }
}
