using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiningApp.UI
{
    public class ActiveSessionsVM
    {
        private double nextLeft = 50;

        private double nextTop = 150;

        private double padding = 15;


        Grid ViewingGrid { get; set; }

        TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Active Sessions", fontSize: 30, width: 300, height: 35);

        TextBlock ViewingTextBlock { get; set; } = ElementHelper.CreateTextBlock("0 of 0", fontSize: 20);

        TextBlock MinerTextBlock { get; set; } = ElementHelper.CreateTextBlock("Miner: ", height: 20);

        TextBlock CryptoTextBlock { get; set; } = ElementHelper.CreateTextBlock("Crypto: ", height: 20);

        TextBlock UptimeTextBlock { get; set; } = ElementHelper.CreateTextBlock("Uptime: ", height: 20);

        TextBlock LastOutputTextBlock { get; set; } = ElementHelper.CreateTextBlock("Last Output: ", height: 20);

        TextBox OutputTextBox { get; set; } = ElementHelper.CreateTextBox("Output", height: 300);


        private List<MiningSessionModel> _allSessions => WindowController.MiningSessions ?? new List<MiningSessionModel>();

        private MiningSessionModel _activeSession { get; set; } 


        public ActiveSessionsVM(Grid displayGrid)
        {
            ViewingGrid = displayGrid;

            Show();
        }

        void Show()
        {
            DisplayElement(TitleTextBlock, leftPadding: 75);

            DisplayElement(ViewingTextBlock, leftPadding: 160);
            DisplayElement(MinerTextBlock);
            DisplayElement(UptimeTextBlock);
            DisplayElement(LastOutputTextBlock);
            DisplayElement(OutputTextBox);
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
    }
}
