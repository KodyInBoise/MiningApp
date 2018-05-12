using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiningApp.Windows;
using MiningApp.LoggingUtil;

namespace MiningApp
{
    class WindowController
    {

        public static WindowController Instance;

        public static UserModel User;


        private DataHelper _dataHelper { get; set; } = null;

        private ProcessHelper _procHelper { get; set; } = null;

        private CryptoHelper _cryptoHelper { get; set; } = null;

        private ServerHelper _serverHelper { get; set; } = null;

        private MiningHelper _miningHelper { get; set; } = null;

        private WalletHelper _walletHelper { get; set; } = null;

        private PoolHelper _poolHelper { get; set; } = null;

        private FileHelper _fileHelper { get; set; } = null;

        private LogHelper _logHelper { get; set; } = null;


        public ControlBarViewModel ControlBarView { get; set; } = null;

        public HomeViewModel HomeView { get; set; } = null;

        public MinersViewModel MinersView { get; set; } = null;

        public MiningRuleViewModel MiningConfigView { get; set; } = null;

        public ViewMinerViewModel ViewMinerView { get; set; } = null;

        public CryptosViewModel CryptosView { get; set; } = null;

        public UploadMinerViewModel UploadMinerView { get; set; } = null;

        public WalletsHomeViewModel WalletsHomeView { get; set; } = null;

        public WalletConfigViewModel WalletConfigView { get; set; } = null;

        public PoolsHomeViewModel PoolsHomeView { get; set; } = null;

        public PoolConfigViewModel PoolConfigView { get; set; } = null;

        public MinersHomeViewModel MinersHomeView { get; set; } = null;

        public MinerConfigViewModel MinerConfigView { get; set; } = null;


        public ControlBarWindow ControlBarWin { get; set; }

        private HomeWindow _homeWin { get; set; }

        private MinersWindow _minersWin { get; set; }

        private MiningRuleWindow _miningConfigWindow { get; set; }

        private CryptosWindow _cryptosWin { get; set; }

        private UploadMinerWindow _uploadMinerWin { get; set; }

        private WalletsHomeWindow _walletsHomeWin { get; set; }

        private WalletConfigWindow _walletConfigWin { get; set; }

        private PoolsHomeWindow _poolsHomeWin { get; set; }

        private PoolConfigWindow _poolConfigWin { get; set; }

        private MinersHomeWindow _minersHomeWin { get; set; }

        private MinerConfigWindow _minerConfigWin { get; set; }


        public double WindowLeft => ControlBarWin.Left + ControlBarWin.Width;

        public double WindowTop => ControlBarWin.Top;


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
            _serverHelper = new ServerHelper();
            _miningHelper = new MiningHelper();
            _walletHelper = new WalletHelper();
            _poolHelper = new PoolHelper();
            _fileHelper = new FileHelper();
            _logHelper = new LogHelper();

            User = DataHelper.LoadUserSettings();

            ControlBarWin = new ControlBarWindow();
            ControlBarWin.Closing += (s, e) => ShutdownApp();

            ShowHome();

            //Testing
            ControlBarWin.TestButton.Click += (s, e) => TestVoid();
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
            MiningConfigView?.Dispose();

            _miningConfigWindow = new MiningRuleWindow();
        }

        public void ShowEditMiner(MiningRuleModel miner)
        {
            MiningConfigView?.Dispose();

            _miningConfigWindow = new MiningRuleWindow(miner);
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

        public void InsertMiner(MiningRuleModel miner)
        {
            _dataHelper.InsertMiningRule(miner);
        }

        public Task<List<MiningRuleModel>> GetMiners()
        {
            return _dataHelper.GetAllMiningRules();
        }

        public void DeleteMiner(MiningRuleModel miner)
        {
            _dataHelper.DeleteMiningRule(miner.ID);
        }

        public void UpdateMiner(MiningRuleModel miner)
        {
            _dataHelper.UpdateMiningRule(miner);
        }

        public void LaunchMiner(MiningRuleModel miner)
        {
            _procHelper.StartMiner(miner);
        }

        public void ShowCryptos()
        {
            CryptosView?.Dispose();

            _cryptosWin = new CryptosWindow();
        }

        public void ShowUploadMiner()
        {
            UploadMinerView?.Dispose();

            _uploadMinerWin = new UploadMinerWindow();
        }

        public void ShutdownApp()
        {
            User.SaveSettings();

            Environment.Exit(0);
        }

        public void ShowWalletsHome()
        {
            WalletsHomeView?.Dispose();

            _walletsHomeWin = new WalletsHomeWindow();
        }

        public void ShowWalletConfig(WalletConfigModel wallet = null)
        {
            WalletConfigView?.Dispose();

            _walletConfigWin = new WalletConfigWindow(wallet);
        }

        public void ShowPoolsHome()
        {
            PoolsHomeView?.Dispose();

            _poolsHomeWin = new PoolsHomeWindow();
        }

        public void ShowPoolConfig(PoolConfigModel pool = null)
        {
            PoolConfigView?.Dispose();

            _poolConfigWin = new PoolConfigWindow(pool);
        }

        public void ShowMinersHome()
        {
            MinersHomeView?.Dispose();

            _minersHomeWin = new MinersHomeWindow();
        }

        public void ShowMinerConfig(MinerConfigModel miner = null)
        {
            MinerConfigView?.Dispose();

            _minerConfigWin = new MinerConfigWindow(miner);
        }

        public async void TestVoid()
        {
            //ShowUploadMiner();
            var ex = new Exception("Error message :(");

            LogHelper.AddEntry(ex);
        }
    }
}
