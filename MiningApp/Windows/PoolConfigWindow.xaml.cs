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
    /// Interaction logic for PoolConfigWindow.xaml
    /// </summary>
    public partial class PoolConfigWindow : Window
    {
        private PoolConfigViewModel _view { get; set; }

        public PoolConfigWindow(PoolConfigModel pool)
        {
            InitializeComponent();

            _view = new PoolConfigViewModel(this, pool);
        }
    }
}
