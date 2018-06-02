using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiningApp.UI
{
    public class HomeVM
    {
        TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Home", 40);

        Grid ViewGrid { get; set; } = MainWindow.Instance.PrimaryGrid;

        List<FrameworkElement> ActiveElements { get; set; } = new List<FrameworkElement>();


        ActiveSessionsVM _activeSessionsVM { get; set; }



        double nextLeft = 10;

        double nextTop = 10;

        double padding = 15;


        public HomeVM(SessionModel launchSession = null)
        {
            Show(launchSession);
        }

        private void Show(SessionModel launchSession)
        {
            DisplayElement(TitleTextBlock);

            _activeSessionsVM = new ActiveSessionsVM(ViewGrid, launchSession);
        }

        public void Dispose()
        {
            WindowController.Instance.HomeView = null;
        }

        private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
        {
            element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

            ViewGrid.Children.Add(element);
            ActiveElements.Add(element);

            nextTop = element.Margin.Top + element.Height + padding;
        }
    }
}
