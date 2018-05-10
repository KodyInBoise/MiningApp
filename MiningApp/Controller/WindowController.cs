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
        public static UserModel User;

        private DataHelper _dataHelper { get; set; } = null;
        private ProcessHelper _procHelper { get; set; } = null;
        private CryptoHelper _cryptoHelper { get; set; } = null;

        private ControlBarWindow _controlBarWin { get; set; }
        private HomeWindow _homeWin { get; set;}
        private MinersWindow _minersWin { get; set; }
        private EditMinerWindow _editMinerWin { get; set; }
        private CryptosWindow _cryptosWin { get; set; }

        public ControlBarViewModel ControlBarView { get; set; } = null;
        public HomeViewModel HomeView { get; set; } = null;
        public MinersViewModel MinersView { get; set; } = null;
        public EditMinerViewModel EditMinersView { get; set; } = null;
        public ViewMinerViewModel ViewMinerView { get; set; } = null;
        public CryptosViewModel CryptosView { get; set; } = null;

        public double WindowLeft => _controlBarWin.Left + _controlBarWin.Width;
        public double WindowTop => _controlBarWin.Top;

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

            User = DataHelper.LoadUserSettings();

            _controlBarWin = new ControlBarWindow();
            ShowHome();

            //Testing
            _controlBarWin.TestButton.Click += (s, e) => TestVoid();
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

        public void ShowEditMiner(MinerConfigModel miner)
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

        public void InsertMiner(MinerConfigModel miner)
        {
            _dataHelper.InsertMiner(miner);
        }

        public Task<List<MinerConfigModel>> GetMiners()
        {
            return _dataHelper.GetMiners();
        }

        public void DeleteMiner(MinerConfigModel miner)
        {
            _dataHelper.DeleteMiner(miner.ID);
        }

        public void UpdateMiner(MinerConfigModel miner)
        {
            _dataHelper.UpdateMiner(miner);
        }

        public void LaunchMiner(MinerConfigModel miner)
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
            var crypto = await CryptoHelper.Instance.CreateCryptoFromName(_homeWin.WatchingSymbolsListBox.SelectedItem.ToString());
        }
    }
}
