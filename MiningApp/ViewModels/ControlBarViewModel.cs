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
            OldWindowController.Instance.ControlBarView = this;

            _window.Left = OldWindowController.User.ControlBarLeft ?? 25.0;
            _window.Top = OldWindowController.User.ControlBarTop ?? 25.0;

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
            OldWindowController.Instance.ControlBarView = null;

            _window.Close();
        }

        private void MinersButton_Clicked()
        {
            //WindowController.Instance.ShowMiners();

            OldWindowController.Instance.ShowMinersHome();
        }

        private void CryptosButton_Clicked()
        {
            OldWindowController.Instance.ShowCryptos();
        }

        private void HomeButton_Clicked()
        {
            OldWindowController.Instance.ShowHome();
        }

        private void WalletsButton_Clicked()
        {
            OldWindowController.Instance.ShowWalletsHome();
        }

        private void PoolsButton_Clicked()
        {
            OldWindowController.Instance.ShowPoolsHome();
        }

        private void LogsButton_Clicked()
        {
            LogHelper.Instance.ShowWindow();
        }
    }
}
