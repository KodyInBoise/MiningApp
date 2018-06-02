using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiningApp.UI
{
    public class MinersHomeVM
    {
        public static MinersHomeVM Instance { get; set; }

        public Grid ViewGrid => MainWindow.Instance.PrimaryGrid;


        List<FrameworkElement> ActiveElements { get; set; } = new List<FrameworkElement>();

        TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Miners Home", fontSize: 40, width: 600);

        Button SetupButton { get; set; } = ElementHelper.CreateButton("Setup");

        Button BrowseButton { get; set; } = ElementHelper.CreateButton("Browse");


        double nextLeft = 10;

        double nextTop = 10;

        double padding = 15;


        public MinersHomeVM()
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

            nextTop = SetupButton.Margin.Top;
            nextLeft = nextLeft + SetupButton.Width + padding * 2;
            DisplayElement(BrowseButton);
            BrowseButton.Click += (s, e) => BrowseButton_Clicked();
        }

        public void Dispose()
        {
            Instance = null;

            WindowController.Instance.MinersHomeView = null;
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
            WindowController.Instance.ShowMinerSetup();
        }

        private void BrowseButton_Clicked()
        {
            WindowController.Instance.ShowBrowseMiners();
        }

        private void ShowBrowse()
        {

        }
    }
}
