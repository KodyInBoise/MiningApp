using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp.UI
{
    public class WindowController
    {
        public static WindowController Instance { get; set; }

        public NavBarVM NavView { get; set; } = null;

        public WalletsVM WalletsView { get; set; } = null;

        public MainWindow Window => MainWindow.Instance;


        public WindowController()
        {
            Instance = this;

            NavView = new NavBarVM();
        }

        public void ShowWalletsHome()
        {
            Window.ContentGrid.Children.Clear();

            WalletsView = new WalletsVM();
        }
    }
}
