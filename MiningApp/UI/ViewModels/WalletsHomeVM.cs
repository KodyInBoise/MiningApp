using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiningApp.UI
{
    public class WalletsHomeVM
    {
        public static WalletsHomeVM Instance { get; set; }

        public Grid ViewGrid => MainWindow.Instance.PrimaryGrid;


        List<FrameworkElement> ActiveElements { get; set; } = new List<FrameworkElement>();

        TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Wallets Home", fontSize: 40);

        Button SetupButton { get; set; } = ElementHelper.CreateButton("Setup");


        double nextLeft = 10;

        double nextTop = 10;

        double padding = 15;


        public WalletsHomeVM()
        {
            Instance = this;

            Show();
        }

        private void Show()
        {
            DisplayElement(TitleTextBlock);

            nextTop = 200;
            nextLeft = 25;
            DisplayElement(SetupButton);
            SetupButton.Click += (s, e) => SetupButton_Clicked();
        }

        public void Dispose()
        {
            Instance = null;

            WindowController.Instance.WalletsHomeView = null;
        }

        private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
        {
            element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

            ViewGrid.Children.Add(element);
            ActiveElements.Add(element);

            nextTop = element.Margin.Top + element.Height + padding;
        }

        private void SetupButton_Clicked()
        {
            WindowController.Instance.ShowWalletSetup();
        }
    }
}
