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

        TextBlock MinerTextBlock { get; set; } = ElementHelper.CreateTextBlock("Miner: ", width: 450, height: 20);

        TextBlock CryptoTextBlock { get; set; } = ElementHelper.CreateTextBlock("Crypto: ", width: 450, height: 20);

        TextBlock UptimeTextBlock { get; set; } = ElementHelper.CreateTextBlock("Uptime: ", width: 450, height: 20);

        TextBlock LastOutputTextBlock { get; set; } = ElementHelper.CreateTextBlock("Last Output: ", width: 450, height: 20);

        TextBox OutputTextBox { get; set; } = ElementHelper.CreateTextBox("Output", height: 300, width: 500, fontSize: 12, readOnly: true);

        DispatcherTimer ActiveSessionTimer { get; set; } = null;


        private List<MiningSessionModel> _allSessions => WindowController.MiningSessions ?? new List<MiningSessionModel>();

        private MiningSessionModel _activeSession { get; set; } = null;

        private int _currentIndex { get; set; } = 0;

        private string _sessionOutput { get; set; } = string.Empty;


        public ActiveSessionsVM(Grid displayGrid, MiningSessionModel launchSession = null)
        {
            ViewingGrid = displayGrid;

            _activeSession = launchSession;

            Show();
        }

        void Show()
        {
            DisplayElement(TitleTextBlock, leftPadding: 125);
            DisplayElement(ViewingTextBlock, leftPadding: 215);

            DisplayElement(MinerTextBlock);
            DisplayElement(UptimeTextBlock);
            DisplayElement(LastOutputTextBlock);
            DisplayElement(OutputTextBox);

            if (_activeSession != null)
            {
                DisplaySession(_activeSession);
            }
            else if (_allSessions.Any())
            {
                DisplaySession(_allSessions[0]);
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

        void DisplaySession(MiningSessionModel session)
        {
            MinerTextBlock.Text = $"Miner: {session.Config.Miner.Name}";
            CryptoTextBlock.Text = $"Crypto: {session.Config.CryptoName}";
            UptimeTextBlock.Text = $"Uptime: {session.Uptime}";
            LastOutputTextBlock.Text = $"Last Output: {session.LastOutputTimestamp}";

            _sessionOutput = session.AllOutput;
            OutputTextBox.Text = session.AllOutput;
            session.OutputReceived += SessionOutputReceived;

            ActiveSessionTimer = session.GetTimer();
            ActiveSessionTimer.Tick += (s, e) => ActiveSessionTimer_Tick();

            _activeSession = session;
            _currentIndex = _allSessions.IndexOf(_activeSession);

            ViewingTextBlock.Text = $"{_currentIndex + 1} of {_allSessions.Count}";
        }

        string _oldOutput = string.Empty;
        void ActiveSessionTimer_Tick()
        {
            bool outputChanged = _sessionOutput != _oldOutput;

            //Need to only scroll when viewing live output
            if (outputChanged)
            {
                OutputTextBox.Text = _sessionOutput;
                OutputTextBox.ScrollToEnd();

                _oldOutput = _sessionOutput;
            }

            UptimeTextBlock.Text = $"Uptime: {_activeSession.UptimeString}";
            LastOutputTextBlock.Text = $"Last Output: {_activeSession.LastOutputTimestamp.ToString()}";
        }

        void SessionOutputReceived(OutputReceivedArgs args)
        {
            if (!String.IsNullOrEmpty(args.NewOutput))
            {
                _sessionOutput += args.NewOutput + Environment.NewLine;

                //Cannot append textbox text from here due to threading issues with textbox element
                /*
                OutputTextBox.AppendText(newOutput + Environment.NewLine);
                LastOutputTextBlock.Text = $"Last Output: {_activeSession.LastOutputTimestamp}";

                OutputTextBox.ScrollToEnd();
                */
            }
        }
    }
}
