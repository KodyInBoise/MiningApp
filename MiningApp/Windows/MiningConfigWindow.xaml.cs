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
    /// Interaction logic for EditMinerWindow.xaml
    /// </summary>
    public partial class MiningConfigWindow : Window
    {
        private MiningConfigViewModel _view;

        public MiningConfigWindow(MiningConfigModel miner = null)
        {
            InitializeComponent();

            _view = new MiningConfigViewModel(this, miner);
        }
    }
}
