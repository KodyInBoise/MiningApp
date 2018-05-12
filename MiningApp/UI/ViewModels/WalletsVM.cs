using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiningApp.UI
{
    public class WalletsVM
    {
        public static WalletsVM Instance { get; set; }

        public Grid ViewGrid => MainWindow.Instance.ContentGrid;


        List<FrameworkElement> ActiveElements { get; set; } = new List<FrameworkElement>();

        TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Wallets", fontSize: 40);


        public WalletsVM()
        {
            Instance = this;

            ShowView();
        }

        private void ShowView()
        {
            DisplayElement(TitleTextBlock, new Thickness(10, 10, 0, 0));
        }

        private void DisplayElement(FrameworkElement element, Thickness margin)
        {
            element.Margin = margin;

            ViewGrid.Children.Add(element);
        }
    }
}
