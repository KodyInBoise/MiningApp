using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MiningApp.UI
{
    public class ActiveSessionsVM
    {
        private double nextLeft = 50;

        private double nextTop = 100;

        private double padding = 15;


        Grid ViewingGrid { get; set; }

        TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Active Sessions", fontSize: 30, width: 300, height: 35);

        TextBlock ViewingTextBlock { get; set; } = ElementHelper.CreateTextBlock("0 of 0", fontSize: 20);

        TextBlock PreviousTextBlock { get; set; } = ElementHelper.CreateTextBlock("<<", fontSize: 20, width: 50);

        TextBlock NextTextBlock { get; set; } = ElementHelper.CreateTextBlock(">>", fontSize: 20, width: 50);

        TextBlock MinerTextBlock { get; set; } = ElementHelper.CreateTextBlock("Miner: ", width: 700, height: 20);

        TextBlock CryptoTextBlock { get; set; } = ElementHelper.CreateTextBlock("Crypto: ", width: 700, height: 20);

        TextBlock UptimeTextBlock { get; set; } = ElementHelper.CreateTextBlock("Uptime: ", width: 700, height: 20);

        TextBlock LastOutputTextBlock { get; set; } = ElementHelper.CreateTextBlock("Last Output: ", width: 450, height: 20);

        TextBox OutputTextBox { get; set; } = ElementHelper.CreateTextBox("Output", height: 300, width: 700, fontSize: 12, readOnly: true,
            contentVerticalAlignment: VerticalAlignment.Top);

        Button StopSessionButton { get; set; } = ElementHelper.CreateButton(content: "Stop", name: "StopSession", style: ButtonStyleEnum.Delete, height: 50, width: 150);

        Button ToggleSessionButton { get; set; } = ElementHelper.CreateButton(content: "Pause", name: "StopSession", style: ButtonStyleEnum.Orange, height: 50, width: 150);

        DispatcherTimer SessionUptimeTimer { get; set; } = null;


        private List<SessionModel> _allSessions => WindowController.MiningSessions ?? new List<SessionModel>();

        private SessionModel _activeSession { get; set; } = null;

        private int _currentIndex { get; set; } = 0;

        private string _sessionOutput { get; set; } = string.Empty;

        private event SessionStatusToggledDelegate _activeSessionStatusToggled;


        public ActiveSessionsVM(Grid displayGrid, SessionModel launchSession = null)
        {
            ViewingGrid = displayGrid;

            _activeSession = launchSession;

            Show();
        }

        void Show()
        {
            DisplayElement(TitleTextBlock, leftPadding: 315);
            DisplayElement(ViewingTextBlock, leftPadding: 405);

            nextTop = ViewingTextBlock.Margin.Top;
            DisplayElement(PreviousTextBlock, leftPadding: 345);
            PreviousTextBlock.MouseDown += (s, e) => PreviousSession();

            nextTop = ViewingTextBlock.Margin.Top;
            DisplayElement(NextTextBlock, leftPadding: 490);
            NextTextBlock.MouseDown += (s, e) => NextSession();

            nextLeft = nextLeft + 100;
            DisplayElement(MinerTextBlock, topPadding: padding * 2);
            DisplayElement(UptimeTextBlock);
            DisplayElement(LastOutputTextBlock);
            DisplayElement(OutputTextBox);

            var buttonTop = OutputTextBox.Margin.Top + OutputTextBox.Height + padding;

            nextTop = buttonTop;
            DisplayElement(StopSessionButton);
            StopSessionButton.Click += (s, e) => StopSessionButton_Clicked();

            nextTop = buttonTop;
            nextLeft = OutputTextBox.Margin.Left + OutputTextBox.Width - ToggleSessionButton.Width;
            DisplayElement(ToggleSessionButton);
            ToggleSessionButton.Click += (s, e) => ToggleSessionButton_Clicked();

            SessionUptimeTimer = new DispatcherTimer();
            SessionUptimeTimer.Interval = new TimeSpan(0, 0, 1);
            SessionUptimeTimer.Tick += (s, e) => ActiveSessionTimer_Tick();
            SessionUptimeTimer.Start();

            if (_activeSession != null)
            {
                DisplaySession(_activeSession);
            }
            else if (_allSessions.Any())
            {
                DisplaySession(_allSessions[0]);
            }
            else
            {
                StopSessionButton.Visibility = Visibility.Collapsed;
                ToggleSessionButton.Visibility = Visibility.Collapsed;
            }
        }

        void StopSessionButton_Clicked()
        {
            _activeSession.ToggleStatus(SessionStatusEnum.Stopped);
        }

        void ToggleSessionButton_Clicked()
        {
            switch (_activeSession.CurrentStatus)
            {
                case SessionStatusEnum.Running:
                    _activeSession.ToggleStatus(SessionStatusEnum.ManuallyPaused);
                    break;
                case SessionStatusEnum.ManuallyPaused:
                case SessionStatusEnum.BlacklistPaused:
                    _activeSession.ToggleStatus(SessionStatusEnum.Running);
                    break;
                default:
                    break;
            }
        }

        public void Dispose()
        {

        }

        private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
        {
            element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

            ViewingGrid.Children.Add(element);

            nextTop = element.Margin.Top + element.Height + padding;
        }

        async void DisplaySession(SessionModel session)
        {
            MinerTextBlock.Text = $"Miner: {session.Config.Miner.Name}";
            CryptoTextBlock.Text = $"Crypto: {session.Config.CryptoName}";
            LastOutputTextBlock.Text = $"Last Output: {session.LastOutputTimestamp}";

            OutputTextBox.Clear();
            OutputTextBox.Text = session.OutputHelper.GetAllOutput();

            session.OutputReceived += SessionOutputReceived;
            session.StatusToggled += SessionStatusToggled;

            _activeSession = session;
            _currentIndex = _allSessions.IndexOf(_activeSession);

            ViewingTextBlock.Text = $"{_currentIndex + 1} of {_allSessions.Count}";

            UpdateStatusButtons(_activeSession.CurrentStatus);
        }

        async Task UpdateSessionUptime()
        {
            var uptime = _activeSession != null ? await _activeSession.GetUptimeString() : string.Empty;

            WindowController.InvokeOnMainThread(new Action(() => {
                UptimeTextBlock.Text = $"Uptime: {uptime}";
            }));
        }

        void ActiveSessionTimer_Tick()
        {
            Task.Run(UpdateSessionUptime);
        }

        void SessionOutputReceived(OutputReceivedArgs args)
        {
            if (args.SessionID == _activeSession.SessionID)
            {
                WindowController.InvokeOnMainThread(new Action(() => UpdateSessionOutput(args)));
            }
        }

        void UpdateSessionOutput(OutputReceivedArgs args)
        {
            LastOutputTextBlock.Text = $"Last Output: {args.Timestamp}";
            OutputTextBox.AppendText(args.NewOutput + Environment.NewLine);
            OutputTextBox.ScrollToEnd();
        }

        void NextSession()
        {
            if (_allSessions.Any())
            {
                _currentIndex = _currentIndex == _allSessions.Count - 1 ? 0 :_currentIndex + 1;

                _activeSession.OutputReceived -= SessionOutputReceived;

                DisplaySession(_allSessions[_currentIndex]);
            }
        }
        
        void PreviousSession()
        {
            if (_allSessions.Any())
            {
                _currentIndex = _currentIndex == 0 ? _allSessions.Count - 1 : _currentIndex - 1;

                _activeSession.OutputReceived -= SessionOutputReceived;

                DisplaySession(_allSessions[_currentIndex]);
            }
        }

        void SessionStatusToggled(SessionStatusToggledArgs args)
        {
            if (args.SessionID == _activeSession.SessionID)
            {              
                StopSessionButton.Visibility = Visibility.Collapsed;
                ToggleSessionButton.Visibility = Visibility.Collapsed;

                UpdateStatusButtons(args.NewStatus);
            }
        }

        void ClearActiveSession()
        {
            SessionUptimeTimer = null;
            OutputTextBox.Text = "";

            if (_allSessions.Contains(_activeSession))
            {
                _allSessions.Remove(_activeSession);
            }

            if (_allSessions.Any())
            {
                if (_allSessions.Count > 1)
                {
                    _currentIndex++;
                }
                else
                {
                    _currentIndex = 0;
                }

                DisplaySession(_allSessions[_currentIndex]);
            }
            else
            {
                _activeSession = null;

                ViewingTextBlock.Text = "0 of 0";
                MinerTextBlock.Text = "Miner:";
                UptimeTextBlock.Text = "Uptime:";
                LastOutputTextBlock.Text = "Last Output:";

                StopSessionButton.Visibility = Visibility.Collapsed;
                ToggleSessionButton.Visibility = Visibility.Collapsed;
            }
        }

        void UpdateStatusButtons(SessionStatusEnum newStatus)
        {
            if (_activeSession != null)
            {
                StopSessionButton.Visibility = newStatus != SessionStatusEnum.Stopped ? Visibility.Visible : Visibility.Collapsed;
                ToggleSessionButton.Visibility = Visibility.Visible;

                switch (newStatus)
                {
                    case SessionStatusEnum.Stopped:
                        ToggleSessionButton.Background = ElementValues.Buttons.Colors.New;
                        ToggleSessionButton.Content = "Start";
                        break;
                    case SessionStatusEnum.Running:
                        ToggleSessionButton.Background = ElementValues.Buttons.Colors.Orange;
                        ToggleSessionButton.Content = "Pause";
                        break;
                    case SessionStatusEnum.ManuallyPaused:
                    case SessionStatusEnum.BlacklistPaused:
                        ToggleSessionButton.Background = ElementValues.Buttons.Colors.New;
                        ToggleSessionButton.Content = "Resume";
                        break;
                    default:
                        StopSessionButton.Visibility = Visibility.Collapsed;
                        ToggleSessionButton.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }
    }
}
