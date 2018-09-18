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

        ActiveSessionsVM _activeSessionsVM { get; set; }

        Border HomeButtonsBorder { get; set; }

        Button ClientsButton { get; set; } = ElementHelper.CreateButton("Clients");



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

            HomeButtonsBorder = ElementHelper.CreateBorder(height: 100, width: (int)(ElementValues.Grids.PrimaryNormalWidth - padding * 4));

            nextLeft = ElementValues.Grids.PrimaryNormalWidth - HomeButtonsBorder.Width - padding * 5;
            nextTop = ElementValues.Grids.Height - HomeButtonsBorder.Height- padding * 3;
            DisplayElement(HomeButtonsBorder);

            nextLeft = HomeButtonsBorder.Margin.Left + (padding * 3) + 5;
            nextTop = HomeButtonsBorder.Margin.Top + (padding * 2) + 5;
            DisplayElement(ClientsButton);
        }

        public void Dispose()
        {
            WindowController.Instance.HomeView = null;
        }

        private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
        {
            element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

            ViewGrid.Children.Add(element);

            nextTop = element.Margin.Top + element.Height + padding;
        }
    }
}
