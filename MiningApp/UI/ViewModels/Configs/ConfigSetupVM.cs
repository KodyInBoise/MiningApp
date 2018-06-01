using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiningApp.UI
{
    public class ConfigSetupVM
    {
        public static ConfigSetupVM Instance { get; set; }


        Grid PrimaryGrid { get; set; } = MainWindow.Instance.PrimaryGrid;

        Grid SecondaryGrid { get; set; } = MainWindow.Instance.SecondaryGrid;


        private PrimaryVM _primaryVM { get; set; }

        private SecondaryVM _secondaryVM { get; set; }


        public ConfigSetupVM()
        {
            Instance = this;

            Show();
        }

        private void Show()
        {
            _primaryVM = new PrimaryVM();
            _secondaryVM = new SecondaryVM();
        }

        public void Dispose()
        {
            Instance = null;

            WindowController.Instance.ConfigSetupView = null;
        }

        public void DisplayPrimary()
        {
            PrimaryGrid.Children.Clear();

            _primaryVM = new PrimaryVM();
        }

        public void DisplaySecondary(ConfigModel config = null)
        {
            SecondaryGrid.Children.Clear();

            _secondaryVM = new SecondaryVM(config);
        }

        public class PrimaryVM
        {
            ConfigSetupVM View { get; set; } = Instance;

            Grid ViewGrid { get; set; } = Instance.PrimaryGrid;


            TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Configs", 40);

            Button NewButton { get; set; } = ElementHelper.CreateButton("New Config", height: buttonHeight, style: ButtonStyle.New);


            private static int buttonHeight = 60;

            private double nextLeft = 20;

            private double nextTop = 12;

            private double padding = 15;


            private List<ConfigModel> _configs { get; set; } = new List<ConfigModel>();

            private List<Button> _configButtons { get; set; } = new List<Button>();

            private Dictionary<Button, int> _buttonDictionary { get; set; } = new Dictionary<Button, int>();


            public PrimaryVM()
            {
                Show();
            }

            private void Show()
            {
                DisplayElement(TitleTextBlock, leftPadding: 25);
                nextTop = 75;

                DisplayElement(NewButton);
                NewButton.Click += (s, e) => NewButton_Clicked();

                DisplayExisting();
            }

            private async void DisplayExisting()
            {
                _configs = await DataHelper.Instance.GetAllConfigs();

                nextTop = NewButton.Margin.Top + NewButton.Height + padding * 2;

                foreach (var config in _configs)
                {
                    var button = ElementHelper.CreateButton(config.Name);
                    _configButtons.Add(button);
                    _buttonDictionary.Add(button, config.ID);

                    DisplayElement(button);

                    button.Click += ExistingConfigClicked;
                }
            }

            private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
            {
                element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

                ViewGrid.Children.Add(element);

                nextTop = element.Margin.Top + element.Height + padding;
            }

            private void ExistingConfigClicked(object sender, EventArgs e)
            {
                var button = (Button)sender;

                var config = _configs.Find(x => x.ID == _buttonDictionary[button]);

                Instance.DisplaySecondary(config);
            }

            public void ShowNewMiner(ConfigModel config)
            {
                var button = ElementHelper.CreateButton(config.Name);
                _configButtons.Add(button);
                _buttonDictionary.Add(button, config.ID);

                DisplayElement(button);

                button.Click += ExistingConfigClicked;

                _configs.Add(config);
            }

            private void NewButton_Clicked()
            {
                Instance.DisplaySecondary();
            }
        }

        public class SecondaryVM
        {
            ConfigSetupVM View { get; set; } = Instance;

            Grid ViewGrid { get; set; } = Instance.SecondaryGrid;

            List<FrameworkElement> ActiveElements { get; set; } = new List<FrameworkElement>();


            TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("New Config", fontSize: 40, width: 400);

            TextBlock StatusTextBlock { get; set; } = ElementHelper.CreateTextBlock("Status", fontSize: 22, width: 725, height: 400);

            TextBox NameTextBox { get; set; } = ElementHelper.CreateTextBox("Name");

            ComboBox MinersComboBox { get; set; } = ElementHelper.CreateComboBox("Miners", width: ElementValues.TextBoxs.Width);

            ComboBox PoolsComboBox { get; set; } = ElementHelper.CreateComboBox("Pools", isEditable: true);

            ComboBox WalletsComboBox { get; set; } = ElementHelper.CreateComboBox("Wallets");

            ComboBox CryptosComboBox { get; set; } = ElementHelper.CreateComboBox("Cryptos");

            TextBox InactiveThresholdTextBox { get; set; } = ElementHelper.CreateTextBox("InactiveThreshold", width: 100);



            Button DeleteButton { get; set; } = ElementHelper.CreateButton("Delete", height: buttonHeight,
                width: buttonWidth, style: ButtonStyle.Delete);

            Button LaunchButton { get; set; } = ElementHelper.CreateButton("Launch", height: buttonHeight,
                width: buttonWidth, style: ButtonStyle.Normal);

            Button FinishButton { get; set; } = ElementHelper.CreateButton("Finish", height: buttonHeight,
                width: buttonWidth, style: ButtonStyle.Finish);



            Label NameLabel { get; set; }

            Label MinersLabel { get; set; }

            Label PoolsLabel { get; set; }

            Label WalletsLabel { get; set; }

            Label CryptosLabel { get; set; }

            Label InactiveThresholdLabel { get; set; }

            Label MinutesLabel { get; set; }


            private static int buttonWidth = 150;

            private static int buttonHeight = 60;

            private double nextLeft = 15;

            private double nextTop = 12;

            private double padding = 15;

            private double labelRight = 600;

            private double labelOffset = -5;

            private List<MinerConfigModel> ViewingMiners { get; set; }

            private List<PoolConfigModel> ViewingPools { get; set; }

            private List<WalletConfigModel> ViewingWallets { get; set; }

            private List<string> ViewingCryptos { get; set; }


            private ConfigModel _config { get; set; }


            public SecondaryVM(ConfigModel config = null)
            {
                _config = config;

                Show();
            }

            private void Show()
            {
                DisplayElement(TitleTextBlock);

                nextTop = 75;
                DisplayElement(StatusTextBlock, leftPadding: 10);
                StatusTextBlock.Visibility = Visibility.Collapsed;

                nextLeft = 200;
                nextTop = 250;
                DisplayElement(NameTextBox);

                DisplayElement(MinersComboBox, topPadding: padding * 2);
                MinersComboBox.ItemsSource = ViewingMiners;

                DisplayElement(PoolsComboBox);
                PoolsComboBox.ItemsSource = ViewingPools;

                DisplayElement(WalletsComboBox);
                WalletsComboBox.ItemsSource = ViewingCryptos;

                DisplayElement(InactiveThresholdTextBox, leftPadding: 250, topPadding: padding * 4);


                nextTop = NameTextBox.Margin.Top;
                NameLabel = ElementHelper.CreateLabel("Name", NameTextBox);
                NameLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(NameLabel, ignoreMargin: true);

                nextTop = MinersComboBox.Margin.Top;
                MinersLabel = ElementHelper.CreateLabel("Miner", MinersComboBox);
                MinersLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(MinersLabel, ignoreMargin: true);

                nextTop = PoolsComboBox.Margin.Top;
                PoolsLabel = ElementHelper.CreateLabel("Pool", PoolsComboBox);
                PoolsLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(PoolsLabel, ignoreMargin: true);

                nextTop = WalletsComboBox.Margin.Top;
                CryptosLabel = ElementHelper.CreateLabel("Wallet", WalletsComboBox);
                CryptosLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(CryptosLabel, ignoreMargin: true);

                nextTop = InactiveThresholdTextBox.Margin.Top;
                InactiveThresholdLabel = ElementHelper.CreateLabel("Inactivity Threshold:", InactiveThresholdTextBox);
                InactiveThresholdLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight - 225, 0);
                DisplayElement(InactiveThresholdLabel, ignoreMargin: true);

                nextTop = InactiveThresholdTextBox.Margin.Top;
                MinutesLabel = ElementHelper.CreateLabel("Minutes", InactiveThresholdTextBox);
                MinutesLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight - 475, 0);
                DisplayElement(MinutesLabel, ignoreMargin: true);

                nextLeft = 15;
                nextTop = ViewGrid.Height - DeleteButton.Height - padding;
                DisplayElement(DeleteButton);

                nextLeft = ElementValues.Grids.SecondaryNormal - FinishButton.Width - padding;
                nextTop = DeleteButton.Margin.Top;
                DisplayElement(FinishButton);

                MinersComboBox.DropDownClosed += (s, e) => MinersComboBox_Closed();
                DeleteButton.Click += (s, e) => DeleteButton_Clicked();
                LaunchButton.Click += (s, e) => LaunchButton_Clicked();
                FinishButton.Click += (s, e) => FinishButton_Clicked();

                if (_config == null)
                {
                    _config = new ConfigModel();
                }
                else
                {
                    TitleTextBlock.Text = "Edit Config";

                    NameTextBox.Text = _config.Name;

                    nextLeft = DeleteButton.Margin.Left + DeleteButton.Width + padding * 4;
                    nextTop = DeleteButton.Margin.Top;
                    DisplayElement(LaunchButton);
                }

                LoadItems();
            }

            private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0, bool ignoreMargin = false)
            {
                if (!ignoreMargin)
                {
                    element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);
                }

                ViewGrid.Children.Add(element);
                ActiveElements.Add(element);

                nextTop = element.Margin.Top + element.Height + padding;
            }

            private async void LoadItems()
            {
                ViewingMiners = await DataHelper.Instance.GetMiners();
                ViewingMiners.ForEach(x => MinersComboBox.Items.Add(x));
                MinersComboBox.SelectedItem = ViewingMiners.Find(x => x.ID == _config.Miner?.ID);

                ViewingPools = _config.Miner?.Pools ?? new List<PoolConfigModel>();
                ViewingPools.ForEach(x => PoolsComboBox.Items.Add(x));
                PoolsComboBox.SelectedItem = ViewingPools.Find(x => x.ID == _config.Pool?.ID);

                ViewingWallets = await DataHelper.Instance.GetWallets();
                ViewingWallets.ForEach(x => WalletsComboBox.Items.Add(x));
                WalletsComboBox.SelectedItem = ViewingWallets.Find(x => x.ID == _config.Wallet?.ID);

                var index = ViewingMiners.IndexOf(_config.Miner);
                PoolsComboBox.SelectedItem = _config.Pool;
                WalletsComboBox.SelectedItem = _config.Wallet;

                InactiveThresholdTextBox.Text = _config.StaleOutputThreshold.ToString();
            }

            private void RadioButton_Toggled()
            {
                /*
                if (UserCryptosRadioButton.IsChecked == true)
                {
                    ViewingCryptos = WindowController.User.WatchingCryptos;
                }
                else
                {
                    ViewingCryptos = CryptoHelper.Instance.GetCryptoNames();
                }

                CryptoComboBox.ItemsSource = ViewingCryptos;
                */
            }

            private void DeleteButton_Clicked()
            {
                Delete();
            }

            void LaunchButton_Clicked()
            {
                _config.StartSession();
            }

            private void FinishButton_Clicked()
            {
                SetConfigInfo();

                Save();
            }

            private void Delete()
            {
                DataHelper.Instance.DeleteConfig(_config);

                View.DisplayPrimary();
                View.DisplaySecondary();
            }

            private void Save()
            {
                DataHelper.Instance.SaveConfig(_config);

                StatusTextBlock.Text = "Config saved successfully!";
                TitleTextBlock.Text = "Edit Config";

                View.DisplayPrimary();
            }

            public void SetConfigInfo()
            {
                _config.CreatedTimestamp = _config.ID > 0 ? _config.CreatedTimestamp : DateTime.Now;
                _config.Name = NameTextBox.Text;
                _config.Miner = (MinerConfigModel)MinersComboBox.SelectedItem;
                _config.Pool = (PoolConfigModel)PoolsComboBox.SelectedItem;
                _config.Wallet = (WalletConfigModel)WalletsComboBox.SelectedItem;

                double staleThreshold;
                try
                {
                    Double.TryParse(InactiveThresholdTextBox.Text, out staleThreshold);

                    if (staleThreshold > 0)
                    {
                        _config.StaleOutputThreshold = staleThreshold;
                    }
                    else
                    {
                        _config.StaleOutputThreshold = -1;
                    }
                }
                catch { _config.StaleOutputThreshold = -1; }               
            }

            private void MinersComboBox_Closed()
            {
                PoolsComboBox.Items.Clear();

                var miner = (MinerConfigModel)MinersComboBox.SelectedItem;

                ViewingPools = miner?.Pools ?? new List<PoolConfigModel>();
                ViewingPools.ForEach(x => PoolsComboBox.Items.Add(x));
                PoolsComboBox.SelectedItem = ViewingPools.Find(x => x.ID == _config.Pool?.ID);
            }
        }
    }
}
