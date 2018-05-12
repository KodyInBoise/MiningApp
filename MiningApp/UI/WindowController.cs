using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiningApp.UI
{
    public class WindowController
    {
        public static WindowController Instance { get; set; }

        public NavBarVM NavView { get; set; } = null;

        public HomeVM HomeView { get; set; } = null;

        public WalletsVM WalletsView { get; set; } = null;

        public WalletSetupVM WalletSetupView { get; set; } = null;

        public MainWindow Window => MainWindow.Instance;


        public WindowController()
        {
            Instance = this;

            NavView = new NavBarVM();

            //TESTING
            MainWindow.Instance.TestButton.Click += (s, e) => TestButton_Clicked();
        }

        private void DisplayViewModel(ViewModelType type, object view, DisplayGrid display = DisplayGrid.Primary)
        {
            if (display == DisplayGrid.Primary)
            {
                MainWindow.Instance.SecondaryBorder.Visibility = System.Windows.Visibility.Collapsed;
                MainWindow.Instance.SecondaryGrid.Visibility = System.Windows.Visibility.Collapsed;

                MainWindow.Instance.PrimaryBorder.Width = 1020;
                MainWindow.Instance.PrimaryGrid.Width = 1020;
            }
            else if (display == DisplayGrid.Secondary)
            {
                MainWindow.Instance.SecondaryBorder.Visibility = System.Windows.Visibility.Visible;
                MainWindow.Instance.SecondaryGrid.Visibility = System.Windows.Visibility.Visible;

                MainWindow.Instance.SecondaryGrid.Children.Clear();

                MainWindow.Instance.PrimaryBorder.Width = 225;
                MainWindow.Instance.PrimaryGrid.Width = 225;
            }
        }

        public void ShowHome()
        {
            HomeView?.Dispose();

            DisplayViewModel(ViewModelType.Home, HomeView = new HomeVM(), DisplayGrid.Primary);
        }

        public void ShowWalletsHome()
        {
            WalletsView?.Dispose();

            WalletsView = new WalletsVM();
        }

        public void ShowWalletSetup()
        {
            WalletSetupView?.Dispose();

            DisplayViewModel(ViewModelType.WalletSetup, WalletSetupView = new WalletSetupVM(), DisplayGrid.Secondary);
        }

        //TESTING METHOD FOR TEST BUTTON
        int testCounter = 0;
        private void TestButton_Clicked()
        {
            if (testCounter % 2 == 0)
            {
                ShowHome();
            }
            else
            {
                ShowWalletSetup();
            }

            testCounter++;
        }
    }
}
