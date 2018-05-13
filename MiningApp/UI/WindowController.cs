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

        public ConfigsHomeVM ConfigsHomeView { get; set; } = null;

        public MinersHomeVM MinersHomeView { get; set; } = null;

        public WalletsHomeVM WalletsHomeView { get; set; } = null;

        public PoolsHomeVM PoolsHomeView { get; set; } = null;

        public LogsHomeVM LogsHomeView { get; set; } = null;

        public WalletSetupVM WalletSetupView { get; set; } = null;

        public MainWindow Window => MainWindow.Instance;


        public WindowController()
        {
            Instance = this;

            NavView = new NavBarVM();

            //TESTING
            MainWindow.Instance.TestButton.Click += (s, e) => TestButton_Clicked();
        }

        private void DisplayViewModel(ViewModelType viewType, DisplayGrid display = DisplayGrid.Primary)
        {
            Grid displayGrid = null;

            if (display == DisplayGrid.Primary)
            {
                MainWindow.Instance.SecondaryBorder.Visibility = System.Windows.Visibility.Collapsed;
                MainWindow.Instance.SecondaryGrid.Visibility = System.Windows.Visibility.Collapsed;

                MainWindow.Instance.PrimaryBorder.Width = 1025;
                MainWindow.Instance.PrimaryStackPanel.Width = 1025;
                //MainWindow.Instance.PrimaryGrid.Width = 1025;

                displayGrid = Window.PrimaryGrid;
            }
            else if (display == DisplayGrid.Secondary)
            {
                MainWindow.Instance.SecondaryBorder.Visibility = System.Windows.Visibility.Visible;
                MainWindow.Instance.SecondaryGrid.Visibility = System.Windows.Visibility.Visible;

                MainWindow.Instance.SecondaryGrid.Children.Clear();

                MainWindow.Instance.PrimaryBorder.Width = 225;
                MainWindow.Instance.PrimaryStackPanel.Width = 225;

                displayGrid = Window.SecondaryGrid;
            }

            displayGrid?.Children.Clear();

            
            switch (viewType)
            {
                case ViewModelType.Home:
                    HomeView?.Dispose();
                    HomeView = new HomeVM();
                    break;
                case ViewModelType.ConfigsHome:
                    ConfigsHomeView?.Dispose();
                    ConfigsHomeView = new ConfigsHomeVM();
                    break;
                case ViewModelType.MinersHome:
                    MinersHomeView?.Dispose();
                    MinersHomeView = new MinersHomeVM();
                    break;
                case ViewModelType.WalletsHome:
                    WalletsHomeView?.Dispose();
                    WalletsHomeView = new WalletsHomeVM();
                    break;
                case ViewModelType.PoolsHome:
                    PoolsHomeView?.Dispose();
                    PoolsHomeView = new PoolsHomeVM();
                    break;
                case ViewModelType.LogsHome:
                    LogsHomeView?.Dispose();
                    LogsHomeView = new LogsHomeVM();
                    break;
                default:
                    HomeView?.Dispose();
                    HomeView = new HomeVM();
                    break;
            }
        }

        public void ShowHome()
        {
            DisplayViewModel(ViewModelType.Home, DisplayGrid.Primary);
        }

        public void ShowConfigurationsHome()
        {
            DisplayViewModel(ViewModelType.ConfigsHome, DisplayGrid.Primary);
        }

        public void ShowMinersHome()
        {
            DisplayViewModel(ViewModelType.MinersHome, DisplayGrid.Primary);
        }

        public void ShowWalletsHome()
        {
            DisplayViewModel(ViewModelType.WalletsHome, DisplayGrid.Primary);           
        }

        public void ShowPoolsHome()
        {
            DisplayViewModel(ViewModelType.PoolsHome, DisplayGrid.Primary);
        }

        public void ShowLogsHome()
        {
            DisplayViewModel(ViewModelType.LogsHome, DisplayGrid.Primary);
        }

        public void ShowWalletSetup()
        {
            DisplayViewModel(ViewModelType.WalletSetup, DisplayGrid.Secondary);
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
