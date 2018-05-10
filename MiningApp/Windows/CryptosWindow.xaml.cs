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
    /// Interaction logic for CryptosWindow.xaml
    /// </summary>
    public partial class CryptosWindow : Window
    {
        private CryptosViewModel _view;
        private CollectionViewSource GridItems => _view.GridItems;

        public CryptosWindow()
        {
            InitializeComponent();

            _view = new CryptosViewModel(this);
        }
    }
}
