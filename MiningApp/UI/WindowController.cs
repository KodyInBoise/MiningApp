using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MiningApp.LoggingUtil;
using Newtonsoft.Json;

namespace MiningApp.UI
{
    public class WindowController
    {
        public static WindowController Instance { get; set; }

        public static UserModel User { get; set; }

        public static List<SessionModel> MiningSessions { get; set; }


        public NavBarVM NavView { get; set; } = null;

        public HomeVM HomeView { get; set; } = null;

        public LoginVM LoginView { get; set; } = null;

        public ConfigsHomeVM ConfigsHomeView { get; set; } = null;

        public MinersHomeVM MinersHomeView { get; set; } = null;

        public BrowseMinersVM BrowseMinersView { get; set; } = null;

        public WalletsHomeVM WalletsHomeView { get; set; } = null;

        public PoolsHomeVM PoolsHomeView { get; set; } = null;

        public PoolSetupVM PoolSetupView { get; set; } = null;

        public LogsHomeVM LogsHomeView { get; set; } = null;

        public SettingsHomeVM SettingsHomeView { get; set; } = null;

        public WalletSetupVM WalletSetupView { get; set; } = null;

        public MinerSetupVM MinerSetupView { get; set; } = null;

        public ConfigSetupVM ConfigSetupView { get; set; } = null;

        public MainWindow Window => MainWindow.Instance;


        private LogHelper _logHelper { get; set; } = null;

        private CryptoHelper _cryptoHelper { get; set; } = null;

        private DataHelper _dataHelper { get; set; } = null;

        private ProcessWatcher _processWatcher { get; set; } = null;


        public WindowController()
        {
            Instance = this;
            Bootstrapper.Startup();

            MiningSessions = new List<SessionModel>();

            _logHelper = new LogHelper();
            _cryptoHelper = new CryptoHelper();
            _dataHelper = new DataHelper();
            _processWatcher = new ProcessWatcher();

            _processWatcher.BlacklistedProcsDelegate += BlacklistedProcsDelegate_Invoked;

            Startup();

            // TESTING
            //LocalClientModel.Test();
        }

        async void Startup()
        {
            NavView = new NavBarVM();

            if (Bootstrapper.User == null)
            {
                ShowLogin();
            }
            else
            {
                if (!String.IsNullOrEmpty(Bootstrapper.User.Email) && !Bootstrapper.User.RequiresLogin)
                {
                    ShowHome();
                }
                else
                {
                    ShowLogin();
                }
            }
        }

        private void DisplayViewModel(ViewModelType viewType, DisplayGrid display = DisplayGrid.Primary,
            SessionModel launchSession = null)
        {
            Grid displayGrid = null;

            if (display == DisplayGrid.Primary)
            {
                MainWindow.Instance.SecondaryBorder.Visibility = System.Windows.Visibility.Collapsed;
                MainWindow.Instance.SecondaryGrid.Visibility = System.Windows.Visibility.Collapsed;

                MainWindow.Instance.PrimaryBorder.Width = ElementValues.Grids.PrimaryNormal;
                MainWindow.Instance.PrimaryStackPanel.Width = ElementValues.Grids.PrimaryNormal;

                displayGrid = Window.PrimaryGrid;
            }
            else if (display == DisplayGrid.Secondary)
            {
                MainWindow.Instance.SecondaryBorder.Visibility = System.Windows.Visibility.Visible;
                MainWindow.Instance.SecondaryGrid.Visibility = System.Windows.Visibility.Visible;

                MainWindow.Instance.SecondaryGrid.Children.Clear();

                MainWindow.Instance.PrimaryBorder.Width = ElementValues.Grids.PrimarySmall;
                MainWindow.Instance.PrimaryStackPanel.Width = ElementValues.Grids.PrimarySmall;

                displayGrid = Window.SecondaryGrid;
            }

            MainWindow.Instance.PrimaryGrid.Children.Clear();
            MainWindow.Instance.SecondaryGrid.Children.Clear();

            
            switch (viewType)
            {
                case ViewModelType.Login:
                    LoginView?.Dispose();
                    LoginView = new LoginVM();
                    break;
                case ViewModelType.Home:
                    HomeView?.Dispose();
                    HomeView = new HomeVM(launchSession);
                    break;
                case ViewModelType.ConfigsHome:
                    ConfigsHomeView?.Dispose();
                    ConfigsHomeView = new ConfigsHomeVM();
                    break;
                case ViewModelType.ConfigSetup:
                    ConfigSetupView?.Dispose();
                    ConfigSetupView = new ConfigSetupVM();
                    break;
                case ViewModelType.MinersHome:
                    MinersHomeView?.Dispose();
                    MinersHomeView = new MinersHomeVM();
                    break;
                case ViewModelType.BrowseMiners:
                    BrowseMinersView?.Dispose();
                    BrowseMinersView = new BrowseMinersVM();
                    break;
                case ViewModelType.MinerSetup:
                    MinerSetupView?.Dispose();
                    MinerSetupView = new MinerSetupVM();
                    break;
                case ViewModelType.WalletsHome:
                    WalletsHomeView?.Dispose();
                    WalletsHomeView = new WalletsHomeVM();
                    break;
                case ViewModelType.WalletSetup:
                    WalletSetupView?.Dispose();
                    WalletSetupView = new WalletSetupVM();
                    break;
                case ViewModelType.PoolsHome:
                    PoolsHomeView?.Dispose();
                    PoolsHomeView = new PoolsHomeVM();
                    break;
                case ViewModelType.PoolSetup:
                    PoolSetupView?.Dispose();
                    PoolSetupView = new PoolSetupVM();
                    break;
                case ViewModelType.LogsHome:
                    LogsHomeView?.Dispose();
                    LogsHomeView = new LogsHomeVM();
                    break;
                case ViewModelType.SettingsHome:
                    SettingsHomeView?.Dispose();
                    SettingsHomeView = new SettingsHomeVM();
                    break;
                default:
                    HomeView?.Dispose();
                    HomeView = new HomeVM();
                    break;
            }
        }

        public void ShowLogin()
        {
            DisplayViewModel(ViewModelType.Login, DisplayGrid.Primary);
        }

        public void ShowHome(SessionModel launchSession = null)
        {
            DisplayViewModel(ViewModelType.Home, DisplayGrid.Primary, launchSession);
        }

        public void ShowConfigurationsHome()
        {
            DisplayViewModel(ViewModelType.ConfigsHome, DisplayGrid.Primary);
        }

        public void ShowMinersHome()
        {
            DisplayViewModel(ViewModelType.MinersHome, DisplayGrid.Primary);
        }

        public void ShowBrowseMiners()
        {
            DisplayViewModel(ViewModelType.BrowseMiners, DisplayGrid.Primary);
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

        public void ShowSettingsHome()
        {
            DisplayViewModel(ViewModelType.SettingsHome, DisplayGrid.Secondary);
        }

        public void ShowWalletSetup()
        {
            DisplayViewModel(ViewModelType.WalletSetup, DisplayGrid.Secondary);
        }

        public void ShowMinerSetup()
        {
            DisplayViewModel(ViewModelType.MinerSetup, DisplayGrid.Secondary);
        }

        public void ShowPoolSetup()
        {
            DisplayViewModel(ViewModelType.PoolSetup, DisplayGrid.Secondary);
        }

        public void ShowConfigSetup()
        {
            DisplayViewModel(ViewModelType.ConfigSetup, DisplayGrid.Secondary);
        }

        public async Task Shutdown()
        {
            CloseSessions();
        }

        private async void CloseSessions()
        {
            var sessions = MiningSessions;

            foreach (var session in MiningSessions.ToList())
            {
                try
                {
                    await session.Stop();
                }
                catch (Exception ex)
                {
                    LogHelper.AddEntry(ex);
                }
            }
        }

        private void BlacklistedProcsDelegate_Invoked(BlacklistedProcessArgs args)
        {
            if (MiningSessions.Any())
            {
                foreach (var session in MiningSessions.ToList())
                {
                    switch (session.CurrentStatus)
                    {
                        case SessionStatusEnum.Running:
                            if (args.BlacklistedProcsRunning)
                            {
                                var pauseMessage = $"Session Paused Due To Blacklist Processes Running: {args.StatusMessage}";
                                session.ToggleStatus(SessionStatusEnum.BlacklistPaused, pauseMessage);
                            }
                            break;
                        case SessionStatusEnum.BlacklistPaused:
                            if (!args.BlacklistedProcsRunning)
                            {
                                var startMessage = "Session Started Due To Blacklist Processes Closing";
                                session.ToggleStatus(SessionStatusEnum.Running, startMessage);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public async void Testing()
        {
            var version = new ServerHelper.VersionHelper.VersionModel()
            {
                Number = 0.1,
                ReleaseTimestamp = DateTime.Now,
                Notes = "Testing release stuff...",
                Urgency = ServerHelper.VersionHelper.VersionModel.UrgencyType.Minor
            };

            Bootstrapper.Settings.App.AppVersion = version;
            Bootstrapper.SaveLocalSettings();
        }

        public static void InvokeOnMainThread(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }
    }
}
