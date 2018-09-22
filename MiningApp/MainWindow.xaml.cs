using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MiningApp.LoggingUtil;
using MiningApp.UI;

namespace MiningApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public const string STARTUP_ARGUMENT_ID = "startup";


        public static MainWindow Instance { get; set; }

        public WindowController Controller { get; set; }

        public static ExceptionUtil ExceptionManager;

        
        private ElementHelper _elementHelper { get; set; }


        
        public MainWindow()
        {
            InitializeComponent();

            Startup();
        }

        private async void Startup()
        {
            Instance = this;
            ExceptionManager = new ExceptionUtil();
          
            _elementHelper = new ElementHelper();

            Controller = new WindowController();

            Closing += (s, e) => Shutdown();

            PrimaryTextBlock.Visibility = Visibility.Hidden;

            var args = await Bootstrapper.GetParseArguments();
            if (args.Any())
            {
                HandleStartupArguments(args);
            }
        }

        void HandleStartupArguments(List<string> args)
        {
            if (args.Contains(STARTUP_ARGUMENT_ID))
            {
                if (Bootstrapper.Settings.General.LaunchOnStartup && Bootstrapper.Settings.General.LaunchConfigID > 0)
                {
                    var config = DataHelper.Instance.GetSessionConfigByID(Bootstrapper.Settings.General.LaunchConfigID);
                    var session = new SessionModel(config);

                    var message = $"Session started with startup argument!";
                    session.ToggleStatus(SessionStatusEnum.Running, message);

                    WindowController.MiningSessions.Add(session);
                    Controller.ShowHome(session);
                }
            }
        }

        public async void Shutdown()
        {
            try
            {
                // Probably need to clean this up eventually 

                WindowController.InvokeOnMainThread(() =>
                {
                    Visibility = Visibility.Collapsed;
                });

                await Task.Run(Controller.Shutdown);

                WindowController.InvokeOnMainThread(() =>
                {
                    Application.Current.Shutdown();
                });
            }
            catch (Exception ex)
            {
                LogHelper.AddEntry(ex);

                WindowController.InvokeOnMainThread(() =>
                {
                    Application.Current.Shutdown();
                });
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Shutdown();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        bool _moving = false;
        TimerModel _movingTimer;
        private void MoveButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            _moving = true;

            _movingTimer = new TimerModel(this, new TimeSpan(0, 0, 0, 0, 250), MoveWindow);
        }

        void MoveWindow()
        {
            WindowController.InvokeOnMainThread(new Action(() => {
                var left = Instance.Left;
                var top = Instance.Top;
            }));
        }

        private void MoveButton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_moving)
            {
                _movingTimer.ToggleStatus(TimerAction.Stop);
            }
        }
    }
}
