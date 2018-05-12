using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace MiningApp.UI
{
    public class NavBarVM
    {
        public static NavBarVM Instance { get; set; }


        Grid ViewGrid => MainWindow.Instance.NavGrid;

        List<FrameworkElement> ActiveElements { get; set; } = new List<FrameworkElement>();

        SplitButton HomeButton { get; set; } = ElementHelper.NavBar.NavButtonTemplate("Home");

        SplitButton ConfigurationsButton { get; set; } = ElementHelper.NavBar.NavButtonTemplate("Configurations");

        SplitButton MinersButton { get; set; } = ElementHelper.NavBar.NavButtonTemplate("Miners");

        SplitButton WalletsButton { get; set; } = ElementHelper.NavBar.NavButtonTemplate("Wallets");

        SplitButton PoolsButton { get; set; } = ElementHelper.NavBar.NavButtonTemplate("Pools");

        SplitButton LogsButton { get; set; } = ElementHelper.NavBar.NavButtonTemplate("Logs");


        double nextLeft = 10;
        double nextTop = 15;
        double padding = 25;


        public NavBarVM()
        {
            Instance = this;

            ShowView();

            WindowController.Instance.ShowHome();
        }

        private void ShowView()
        {
            DisplayElement(HomeButton);

            DisplayElement(ConfigurationsButton, topPadding: padding * 2);

            DisplayElement(MinersButton);

            DisplayElement(WalletsButton);
            WalletsButton.Click += (s, e) => WalletsButton_Clicked();

            DisplayElement(PoolsButton);

            DisplayElement(LogsButton);

            /*
            NavButtons = ElementHelper.NavBar.ActiveButtons;

            double left = 15;
            double top = 15;
            double padding = 15;

            for (var x = 0; x < NavButtons.Count - 1; x++)
            {
                var button = NavButtons[x];
                if (x == 1) top += padding * 2;
                button.Margin = new Thickness(left, top, 0, 0);

                ViewGrid.Children.Add(button);

                top = button.Margin.Top + button.Height + padding;
            }
            */
        }

        private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
        {
            element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

            ViewGrid.Children.Add(element);
            ActiveElements.Add(element);

            nextTop = element.Margin.Top + element.Height + padding;
        }

        private void HomeButton_Clicked()
        {
            WindowController.Instance.ShowHome();
        }

        private void WalletsButton_Clicked()
        {
            WindowController.Instance.ShowWalletsHome();
        }
    }
}
