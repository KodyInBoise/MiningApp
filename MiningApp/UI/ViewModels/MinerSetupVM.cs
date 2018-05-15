using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiningApp.UI
{
    public class MinerSetupVM
    {
        public static MinerSetupVM Instance { get; set; }


        Grid PrimaryGrid { get; set; } = MainWindow.Instance.PrimaryGrid;

        Grid SecondaryGrid { get; set; } = MainWindow.Instance.SecondaryGrid;


        private PrimaryVM _primaryVM { get; set; }

        private SecondaryVM _secondaryVM { get; set; }


        public MinerSetupVM()
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

            WindowController.Instance.MinerSetupView = null;
        }

        public void NewSetup_Clicked()
        {

        }

        public void ExistingSetup_Clicked(object sender, EventArgs e)
        {

        }

        public void DisplaySecondary(MinerConfigModel miner = null)
        {
            SecondaryGrid.Children.Clear();

            _secondaryVM = new SecondaryVM(miner);
        }

        public class PrimaryVM
        {
            MinerSetupVM View { get; set; } = Instance;

            Grid ViewGrid { get; set; } = Instance.PrimaryGrid;

            List<FrameworkElement> ActiveElements { get; set; } = new List<FrameworkElement>();


            TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Miners", 40);

            Button NewButton { get; set; } = ElementHelper.CreateButton("New Miner", height: buttonHeight, style: ButtonStyle.New);


            private static int buttonHeight = 60;

            private double nextLeft = 20;

            private double nextTop = 12;

            private double padding = 15;


            private List<MinerConfigModel> _miners { get; set; } = new List<MinerConfigModel>();

            private List<Button> _minerButtons { get; set; } = new List<Button>();

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
                NewButton.Click += (s, e) => Instance.NewSetup_Clicked();

                DisplayExisting();
            }

            private void DisplayExisting()
            {
                _miners = DataHelper.Instance.GetAllMinerConfigs();

                nextTop = NewButton.Margin.Top + NewButton.Height + padding * 2;

                foreach (var miner in _miners)
                {
                    var button = ElementHelper.CreateButton(miner.Name);
                    _minerButtons.Add(button);
                    _buttonDictionary.Add(button, miner.ID);

                    DisplayElement(button);

                    button.Click += ExistingMinerClicked;
                }
            }

            private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
            {
                element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

                ViewGrid.Children.Add(element);
                ActiveElements.Add(element);

                nextTop = element.Margin.Top + element.Height + padding;
            }

            private void ExistingMinerClicked(object sender, EventArgs e)
            {
                var button = (Button)sender;

                var miner = _miners.Find(x => x.ID == _buttonDictionary[button]);

                Instance.DisplaySecondary(miner);
            }
        }

        public class SecondaryVM
        {
            MinerSetupVM View { get; set; } = Instance;

            Grid ViewGrid { get; set; } = Instance.SecondaryGrid;

            List<FrameworkElement> ActiveElements { get; set; } = new List<FrameworkElement>();


            TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("New Miner", fontSize: 40, width: 400);

            TextBlock StatusTextBlock { get; set; } = ElementHelper.CreateTextBlock("Status", fontSize: 22, width: 725, height: 400);

            TextBox NameTextBox { get; set; } = ElementHelper.CreateTextBox("Name");

            TextBox PathTextBox { get; set; } = ElementHelper.CreateTextBox("Path", width: 350);


            ComboBox PoolsComboBox { get; set; } = ElementHelper.CreateComboBox("Pools", width: 350, isEditable: true);

            ComboBox CryptosComboBox { get; set; } = ElementHelper.CreateComboBox("Cryptos", width: 350);


            ListBox PoolsListBox { get; set; } = ElementHelper.CreateListBox("Pools");

            ListBox CryptosListBox { get; set; } = ElementHelper.CreateListBox("Cryptos");


            Button PoolsAddButton { get; set; } = ElementHelper.CreateButton("+", name: "PoolsAdd", fontSize: 14, style: ButtonStyle.New, 
                width: 40, height: ElementValues.TextBoxs.Height);

            Button PoolsRemoveButton { get; set; } = ElementHelper.CreateButton("-", name: "PoolsAdd", fontSize: 14, style: ButtonStyle.Delete,
                width: 40, height: ElementValues.TextBoxs.Height);

            Button CryptosAddButton { get; set; } = ElementHelper.CreateButton("+", name: "CryptosAdd", fontSize: 14, style: ButtonStyle.New,
                width: 40, height: ElementValues.TextBoxs.Height);

            Button CryptosRemoveButton { get; set; } = ElementHelper.CreateButton("-", name: "CryptosRemove", fontSize: 14, style: ButtonStyle.Delete,
                width: 40, height: ElementValues.TextBoxs.Height);

            Button BrowseButton { get; set; } = ElementHelper.CreateButton("Browse", width: 85, fontSize: 14, height: ElementValues.TextBoxs.Height);

            Button DeleteButton { get; set; } = ElementHelper.CreateButton("Delete", height: buttonHeight,
                width: buttonWidth, style: ButtonStyle.Delete);

            Button FinishButton { get; set; } = ElementHelper.CreateButton("Finish", height: buttonHeight,
                width: buttonWidth, style: ButtonStyle.Finish);



            Label NameLabel { get; set; }

            Label PathLabel { get; set; }

            Label PoolsLabel { get; set; }

            Label CryptosLabel { get; set; }


            private static int buttonWidth = 150;

            private static int buttonHeight = 60;

            private double nextLeft = 15;

            private double nextTop = 12;

            private double padding = 15;

            private double labelRight = 600;

            private double labelOffset = -5;

            private List<string> ViewingCryptos { get; set; } = WindowController.User.WatchingCryptos;


            private MinerConfigModel _miner { get; set; }


            public SecondaryVM(MinerConfigModel miner = null)
            {
                _miner = miner;

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

                DisplayElement(PathTextBox);

                DisplayElement(PoolsComboBox, topPadding: padding);

                DisplayElement(PoolsListBox);

                DisplayElement(CryptosComboBox, topPadding: padding);

                DisplayElement(CryptosListBox);


                nextLeft = PathTextBox.Margin.Left + PathTextBox.Width + padding;
                nextTop = PathTextBox.Margin.Top;
                DisplayElement(BrowseButton);

                nextLeft = PoolsComboBox.Margin.Left + PoolsComboBox.Width + padding;
                nextTop = PoolsComboBox.Margin.Top;
                DisplayElement(PoolsAddButton);

                nextLeft = PoolsAddButton.Margin.Left + PoolsAddButton.Width + 5;
                nextTop = PoolsComboBox.Margin.Top;
                DisplayElement(PoolsRemoveButton);

                nextLeft = CryptosComboBox.Margin.Left + CryptosComboBox.Width + padding;
                nextTop = CryptosComboBox.Margin.Top;
                DisplayElement(CryptosAddButton);

                nextLeft = CryptosAddButton.Margin.Left + CryptosAddButton.Width + 5;
                nextTop = CryptosComboBox.Margin.Top;
                DisplayElement(CryptosRemoveButton);

                nextTop = NameTextBox.Margin.Top;
                NameLabel = ElementHelper.CreateLabel("Name", NameTextBox);
                NameLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(NameLabel, ignoreMargin: true);

                nextTop = PathTextBox.Margin.Top;
                PathLabel = ElementHelper.CreateLabel("Path", PathTextBox);
                PathLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(PathLabel, ignoreMargin: true);

                nextTop = PoolsComboBox.Margin.Top;
                PoolsLabel = ElementHelper.CreateLabel("Pools", PoolsComboBox);
                PoolsLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(PoolsLabel, ignoreMargin: true);

                nextTop = CryptosComboBox.Margin.Top;
                CryptosLabel = ElementHelper.CreateLabel("Cryptos", CryptosComboBox);
                CryptosLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(CryptosLabel, ignoreMargin: true);

                nextLeft = 15;
                nextTop = ViewGrid.Height - DeleteButton.Height - padding;
                DisplayElement(DeleteButton);

                nextLeft = ElementValues.Grids.SecondaryNormal - FinishButton.Width - padding;
                nextTop = DeleteButton.Margin.Top;
                DisplayElement(FinishButton);

                FinishButton.Click += (s, e) => FinishButton_Clicked();

                if (_miner == null)
                {
                    _miner = new MinerConfigModel();
                }
                else
                {
                    TitleTextBlock.Text = "Edit Miner";

                    NameTextBox.Text = _miner.Name;
                    //AddressTextBox.Text = _miner.Address;

                    //ViewingCryptos.Insert(0, _miner.Crypto);
                    //CryptoComboBox.SelectedIndex = 0;
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

            }

            private void DeleteButton_Clicked()
            {
                Delete();
            }

            private void FinishButton_Clicked()
            {
                SetMinerInfo();

                Save();
            }

            private void Delete()
            {
                //CryptoComboBox.Text = _miner.Crypto;
            }

            private void Save()
            {
                //DataHelper.Instance.SaveWallet(_wallet);

                //StatusTextBlock.Text = "Wallet config saved successfully!";
            }

            public void SetMinerInfo()
            {
                /*
                _wallet.CreatedTimestamp = _wallet.ID > 0 ? _wallet.CreatedTimestamp : DateTime.Now;
                _wallet.Name = NameTextBox.Text;
                _wallet.Crypto = CryptoComboBox.Text;
                _wallet.Address = AddressTextBox.Text;
                _wallet.Status = WalletStatus.Active;
                */
            }

        }
    }
}
