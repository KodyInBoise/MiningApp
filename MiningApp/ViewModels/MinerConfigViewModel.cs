using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiningApp.Windows;

namespace MiningApp
{
    public class MinerConfigViewModel
    {
        private MinerConfigWindow _window { get; set; }

        public MinerConfigViewModel(MinerConfigWindow window)
        {
            _window = window;

            ShowWindow();
        }

        private void ShowWindow()
        {

        }
    }
}
