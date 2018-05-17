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

            TextBox CryptoTextBox { get; set; } = ElementHelper.CreateTextBox("Path", width: 350);

            ComboBox MinerComboBox { get; set; } = ElementHelper.CreateComboBox("Miners");

            ComboBox PoolsComboBox { get; set; } = ElementHelper.CreateComboBox("Pools", isEditable: true);

            ComboBox WalletsComboBox { get; set; } = ElementHelper.CreateComboBox("Wallets");

            ComboBox CryptosComboBox { get; set; } = ElementHelper.CreateComboBox("Cryptos");



            Button DeleteButton { get; set; } = ElementHelper.CreateButton("Delete", height: buttonHeight,
                width: buttonWidth, style: ButtonStyle.Delete);

            Button FinishButton { get; set; } = ElementHelper.CreateButton("Finish", height: buttonHeight,
                width: buttonWidth, style: ButtonStyle.Finish);



            Label NameLabel { get; set; }

            Label MinerLabel { get; set; }

            Label PoolsLabel { get; set; }

            Label WalletsLabel { get; set; }

            Label CryptosLabel { get; set; }


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

                DisplayElement(CryptoTextBox);

                DisplayElement(PoolsComboBox, topPadding: padding);
                PoolsComboBox.ItemsSource = ViewingPools;

                DisplayElement(PoolsListBox);

                DisplayElement(WalletsComboBox, topPadding: padding);
                WalletsComboBox.ItemsSource = ViewingCryptos;

                DisplayElement(CryptosListBox);


                nextLeft = CryptoTextBox.Margin.Left + CryptoTextBox.Width + padding;
                nextTop = CryptoTextBox.Margin.Top;
                DisplayElement(BrowseButton);

                nextLeft = PoolsComboBox.Margin.Left + PoolsComboBox.Width + padding;
                nextTop = PoolsComboBox.Margin.Top;
                DisplayElement(PoolsAddButton);

                nextLeft = PoolsAddButton.Margin.Left + PoolsAddButton.Width + 5;
                nextTop = PoolsComboBox.Margin.Top;
                DisplayElement(PoolsRemoveButton);

                nextLeft = WalletsComboBox.Margin.Left + WalletsComboBox.Width + padding;
                nextTop = WalletsComboBox.Margin.Top;
                DisplayElement(CryptosAddButton);

                nextLeft = CryptosAddButton.Margin.Left + CryptosAddButton.Width + 5;
                nextTop = WalletsComboBox.Margin.Top;
                DisplayElement(CryptosRemoveButton);

                nextTop = NameTextBox.Margin.Top;
                NameLabel = ElementHelper.CreateLabel("Name", NameTextBox);
                NameLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(NameLabel, ignoreMargin: true);

                nextTop = CryptoTextBox.Margin.Top;
                PathLabel = ElementHelper.CreateLabel("Path", CryptoTextBox);
                PathLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(PathLabel, ignoreMargin: true);

                nextTop = PoolsComboBox.Margin.Top;
                PoolsLabel = ElementHelper.CreateLabel("Pools", PoolsComboBox);
                PoolsLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(PoolsLabel, ignoreMargin: true);

                nextTop = WalletsComboBox.Margin.Top;
                CryptosLabel = ElementHelper.CreateLabel("Cryptos", WalletsComboBox);
                CryptosLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(CryptosLabel, ignoreMargin: true);

                nextLeft = 15;
                nextTop = ViewGrid.Height - DeleteButton.Height - padding;
                DisplayElement(DeleteButton);

                nextLeft = ElementValues.Grids.SecondaryNormal - FinishButton.Width - padding;
                nextTop = DeleteButton.Margin.Top;
                DisplayElement(FinishButton);

                DeleteButton.Click += (s, e) => DeleteButton_Clicked();
                FinishButton.Click += (s, e) => FinishButton_Clicked();

                if (_config == null)
                {
                    _config = new ConfigModel();
                }
                else
                {
                    TitleTextBlock.Text = "Edit Config";

                    NameTextBox.Text = _config.Name;
                    CryptoTextBox.Text = _config.CryptoName;

                    //PoolsListBox.ItemsSource = _config.Pools;
                    //CryptosListBox.ItemsSource = _config.Cryptos;
                }
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

            }
        }
    }
}
