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
using System.Windows.Shapes;

namespace MiningApp.Windows
{
    /// <summary>
    /// Interaction logic for WalletConfigWindow.xaml
    /// </summary>
    public partial class WalletConfigWindow : Window
    {
        private WalletConfigViewModel _view { get; set; }

        public WalletConfigWindow(WalletConfigModel wallet = null)
        {
            InitializeComponent();

            _view = new WalletConfigViewModel(this, wallet);
        }
    }
}
