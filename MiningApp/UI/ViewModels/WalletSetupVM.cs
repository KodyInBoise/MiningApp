using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp.UI
{
    public class WalletSetupVM
    {
        public void Dispose()
        {
            WindowController.Instance.HomeView = null;
        }
    }
}
