using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiningApp.Windows;

namespace MiningApp
{
    public class PoolsHomeViewModel
    {
        private PoolsHomeWindow _window { get; set; }

        public PoolsHomeViewModel()
        {
            WindowController.Instance.PoolsHomeView = this;
        }

        private void ShowWindow()
        {

        }

        public void Dispose()
        {

        }
    }
}
