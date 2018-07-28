using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MiningApp.LoggingUtil;
using MiningApp.UI;

namespace MiningApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; set; }

        public WindowController Controller { get; set; }

        
        private ElementHelper _elementHelper { get; set; }


        
        public MainWindow()
        {
            InitializeComponent();

            Startup();
        }

        private void Startup()
        {
            Instance = this;

            _elementHelper = new ElementHelper();

            Controller = new WindowController();

            Closing += (s, e) => Shutdown();

            PrimaryTextBlock.Visibility = Visibility.Hidden;
        }

        public async void Shutdown()
        {
            try
            {
                Visibility = Visibility.Collapsed;

                await Controller.Shutdown();

                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                LogHelper.AddEntry(ex);

                Application.Current.Shutdown();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Shutdown();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
