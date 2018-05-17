using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiningApp.UI
{
    public class ConfigsHomeVM
    {
        public static ConfigsHomeVM Instance { get; set; }

        public Grid ViewGrid => MainWindow.Instance.PrimaryGrid;


        TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Configs Home", fontSize: 40, width: 600);

        Button SetupButton { get; set; } = ElementHelper.CreateButton("Setup");


        double nextLeft = 10;

        double nextTop = 10;

        double padding = 15;


        public ConfigsHomeVM()
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

            WindowController.Instance.ConfigsHomeView = null;
        }

        private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
        {
            element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

            ViewGrid.Children.Add(element);

            nextTop = element.Margin.Top + element.Height + padding;
        }

        private void SetupButton_Clicked()
        {
            WindowController.Instance.ShowConfigSetup();
        }
    }
}
