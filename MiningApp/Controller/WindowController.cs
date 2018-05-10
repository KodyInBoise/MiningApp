using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiningApp.Windows;

namespace MiningApp
{
    class WindowController
    {
        public static WindowController Instance;

        private DataHelper _dataHelper { get; set; } = null;
        private ProcessHelper _procHelper { get; set; } = null;
        private CryptoHelper _cryptoHelper { get; set; } = null;

        private HomeWindow _homeWin { get; set; }
        private MinersWindow _minersWin { get; set; }
        private EditMinerWindow _editMinerWin { get; set; }
        private CryptosWindow _cryptosWin { get; set; }

        public HomeViewModel HomeView { get; set; } = null;
        public MinersViewModel MinersView { get; set; } = null;
        public EditMinerViewModel EditMinersView { get; set; } = null;
        public ViewMinerViewModel ViewMinerView { get; set; } = null;
        public CryptosViewModel CryptosView { get; set; } = null;

        public double WindowLeft => _homeWin.Left + _homeWin.Width;
        public double WindowTop => _homeWin.Top;

        public WindowController()
        {
            Startup();
        }

        private void Startup()
        {
            Instance = this;

            _dataHelper = new DataHelper();
            _procHelper = new ProcessHelper();
            _cryptoHelper = new CryptoHelper();

            ShowHome();

            //Testing
            _homeWin.TestButton.Click += (s, e) => TestVoid();
        }

        public void ShowHome()
        {
            HomeView?.Dispose();

            _homeWin = new HomeWindow();
        }

        public void ShowMiners()
        {
            MinersView?.Dispose();

            _minersWin = new MinersWindow();
        }

        public void ShowNewMiner()
        {
            EditMinersView?.Dispose();

            _editMinerWin = new EditMinerWindow();
        }

        public void ShowEditMiner(MinerModel miner)
        {
            EditMinersView?.Dispose();

            _editMinerWin = new EditMinerWindow(miner);
        }

        public async Task<string> GetFilePath()
        {
            string path = "";

            var fileBrowser = new OpenFileDialog();
            var result = fileBrowser.ShowDialog();

            if (result == DialogResult.OK)
            {
                path = fileBrowser.FileName;
            }

            return path;
        }

        public void InsertMiner(MinerModel miner)
        {
            _dataHelper.InsertMiner(miner);
        }

        public Task<List<MinerModel>> GetMiners()
        {
            return _dataHelper.GetMiners();
        }

        public void DeleteMiner(MinerModel miner)
        {
            _dataHelper.DeleteMiner(miner.ID);
        }

        public void UpdateMiner(MinerModel miner)
        {
            _dataHelper.UpdateMiner(miner);
        }

        public void LaunchMiner(MinerModel miner)
        {
            _procHelper.StartMiner(miner);
        }

        public void ShowCryptos()
        {
            CryptosView?.Dispose();

            _cryptosWin = new CryptosWindow();
        }

        public async void TestVoid()
        {
            var cryptos = await CryptoHelper.Instance.GetTopCryptos();
            var a = "a";
        }
    }
}
