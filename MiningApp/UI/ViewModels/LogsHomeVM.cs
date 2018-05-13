using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiningApp.UI
{
    public class LogsHomeVM
    {
        TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Logs Home", 40);

        Grid ViewGrid { get; set; } = MainWindow.Instance.PrimaryGrid;

        List<FrameworkElement> ActiveElements { get; set; } = new List<FrameworkElement>();


        double nextLeft = 10;

        double nextTop = 10;

        double padding = 15;


        public LogsHomeVM()
        {
            Show();
        }

        private void Show()
        {
            DisplayElement(TitleTextBlock);
        }

        public void Dispose()
        {
            WindowController.Instance.LogsHomeView = null;
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
