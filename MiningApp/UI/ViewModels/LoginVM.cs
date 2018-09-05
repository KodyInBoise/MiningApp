using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiningApp.UI
{
    public class LoginVM
    {
        TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Login", 40);

        Grid ViewGrid { get; set; } = MainWindow.Instance.PrimaryGrid;


        ActiveSessionsVM _activeSessionsVM { get; set; }



        double nextLeft = 10;

        double nextTop = 10;

        double padding = 15;


        public LoginVM()
        {
            Show();
        }

        private void Show()
        {
            DisplayElement(TitleTextBlock);

            //_activeSessionsVM = new ActiveSessionsVM(ViewGrid);
        }

        public void Dispose()
        {
            WindowController.Instance.LoginView = null;
        }

        private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
        {
            element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

            ViewGrid.Children.Add(element);

            nextTop = element.Margin.Top + element.Height + padding;
        }
    }
}

