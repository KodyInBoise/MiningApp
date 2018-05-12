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

        public List<SplitButton> NavButtons { get; set; }


        private Grid _grid => MainWindow.Instance.NavGrid;


        public NavBarVM()
        {
            Instance = this;

            ShowView();
        }

        private void ShowView()
        {
            NavButtons = ElementHelper.NavBar.ActiveButtons;

            double left = 15;
            double top = 15;
            double padding = 15;

            for (var x = 0; x < NavButtons.Count - 1; x++)
            {
                var button = NavButtons[x];
                if (x == 1) top += padding * 2;
                button.Margin = new Thickness(left, top, 0, 0);

                _grid.Children.Add(button);

                top = button.Margin.Top + button.Height + padding;
            }
        }

        private void NavHomeButton_Clicked()
        {
            Xceed.Wpf.Toolkit.MessageBox.Show(NavButtons[0].Margin.ToString());
        }

    }
}
