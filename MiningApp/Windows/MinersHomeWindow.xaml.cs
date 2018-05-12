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
    /// Interaction logic for BrowseMinersWindow.xaml
    /// </summary>
    public partial class MinersHomeWindow : Window
    {
        public CollectionViewSource GridItems => _view.GridItems;



        private MinersHomeViewModel _view { get; set; }



        public MinersHomeWindow()
        {
            InitializeComponent();

            _view = new MinersHomeViewModel(this);
        }
    }
}
