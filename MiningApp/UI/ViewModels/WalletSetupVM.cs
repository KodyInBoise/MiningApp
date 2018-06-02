using MiningApp.LoggingUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiningApp.UI
{
    public class WalletSetupVM
    {
        public static WalletSetupVM Instance { get; set; }


        Grid PrimaryGrid { get; set; } = MainWindow.Instance.PrimaryGrid;

        Grid SecondaryGrid { get; set; } = MainWindow.Instance.SecondaryGrid;


        private PrimaryVM _primaryVM { get; set; }

        private SecondaryVM _secondaryVM { get; set; }


        public WalletSetupVM()
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

            WindowController.Instance.WalletSetupView = null;
        }

        public void DisplayPrimary()
        {
            PrimaryGrid.Children.Clear();

            _primaryVM = new PrimaryVM();
        }

        public void DisplaySecondary(WalletConfigModel wallet = null)
        {
            SecondaryGrid.Children.Clear();

            _secondaryVM = new SecondaryVM(wallet);
        }

        public class PrimaryVM
        {
            WalletSetupVM View { get; set; } = Instance;

            Grid ViewGrid { get; set; } = Instance.PrimaryGrid;

            List<FrameworkElement> ActiveElements { get; set; } = new List<FrameworkElement>();


            TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Wallets", 40);

            Button NewButton { get; set; } = ElementHelper.CreateButton("New Wallet", height: buttonHeight, style: ButtonStyle.New);


            private static int buttonHeight = 60;
            
            private double nextLeft = 20;

            private double nextTop = 12;

            private double padding = 15;


            private List<WalletConfigModel> _wallets { get; set; } = new List<WalletConfigModel>();

            private List<Button> _walletButtons { get; set; } = new List<Button>();

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

            private void DisplayExisting()
            {
                _wallets = DataHelper.Instance.GetWallets().Result;

                nextTop = NewButton.Margin.Top + NewButton.Height + padding * 2;

                foreach (var wallet in _wallets)
                {
                    var button = ElementHelper.CreateButton(wallet.Name);
                    _walletButtons.Add(button);
                    _buttonDictionary.Add(button, wallet.ID);

                    DisplayElement(button);

                    button.Click += ExistingWalletClicked;
                }
            }

            private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
            {
                element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

                ViewGrid.Children.Add(element);
                ActiveElements.Add(element);

                nextTop = element.Margin.Top + element.Height + padding;
            }

            private void ExistingWalletClicked(object sender, EventArgs e)
            {
                var button = (Button)sender;

                var wallet = _wallets.Find(x => x.ID == _buttonDictionary[button]);

                Instance.DisplaySecondary(wallet);
            }

            private void NewButton_Clicked()
            {
                View.DisplaySecondary();
            }
        }

        public class SecondaryVM
        {
            WalletSetupVM View { get; set; } = Instance;

            Grid ViewGrid { get; set; } = Instance.SecondaryGrid;

            List<FrameworkElement> ActiveElements { get; set; } = new List<FrameworkElement>();


            TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("New Wallet",fontSize: 40, width: 400);

            TextBlock StatusTextBlock { get; set; } = ElementHelper.CreateTextBlock("Status", fontSize: 22, width: 725, height: 400);

            TextBox NameTextBox { get; set; } = ElementHelper.CreateTextBox("Name");

            ComboBox CryptosComboBox { get; set; } = ElementHelper.CreateComboBox("Crypto", text: "Select a crypto...", fontSize: 16);

            TextBox AddressTextBox { get; set; } = ElementHelper.CreateTextBox("Address");

            TextBox VerifyTextBox { get; set; } = ElementHelper.CreateTextBox("Verify");

            RadioButton UserCryptosRadioButton { get; set; } = ElementHelper.CreateRadioButton("UserCryptos", content: "Watchlist");

            RadioButton AllCryptosRadioButton { get; set; } = ElementHelper.CreateRadioButton("AllCryptos", content: "All Cryptos");


            Button DeleteButton { get; set; } = ElementHelper.CreateButton("Delete", height: buttonHeight,
                width: buttonWidth, style: ButtonStyle.Delete);

            Button FinishButton { get; set; } = ElementHelper.CreateButton("Finish", height: buttonHeight,
                width: buttonWidth, style: ButtonStyle.Finish);


            Label NameLabel { get; set; }

            Label CryptoLabel { get; set; }

            Label AddressLabel { get; set; }

            Label VerifyLabel { get; set; }


            private static int buttonWidth = 150;

            private static int buttonHeight = 60;

            private double nextLeft = 15;

            private double nextTop = 12;

            private double padding = 15;

            private double labelRight = 600;

            private double labelOffset = -5;

            private List<string> ViewingCryptos { get; set; } = WindowController.User.WatchingCryptos;


            private WalletConfigModel _wallet { get; set; }


            public SecondaryVM(WalletConfigModel wallet = null)
            {
                _wallet = wallet;

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

                DisplayElement(CryptosComboBox);
                CryptosComboBox.ItemsSource = ViewingCryptos;

                DisplayElement(UserCryptosRadioButton, leftPadding: padding * 4);
                UserCryptosRadioButton.IsChecked = true;

                nextTop = CryptosComboBox.Margin.Top + CryptosComboBox.Height + padding;
                DisplayElement(AllCryptosRadioButton, leftPadding: padding * 14);

                DisplayElement(AddressTextBox, topPadding: padding * 2);

                DisplayElement(VerifyTextBox);


                nextTop = NameTextBox.Margin.Top;
                NameLabel = ElementHelper.CreateLabel("Name", NameTextBox);
                NameLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(NameLabel, ignoreMargin: true);

                nextTop = CryptosComboBox.Margin.Top;
                CryptoLabel = ElementHelper.CreateLabel("Crypto", CryptosComboBox);
                CryptoLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(CryptoLabel, ignoreMargin: true);

                nextTop = AddressTextBox.Margin.Top;
                AddressLabel = ElementHelper.CreateLabel("Address", AddressTextBox);
                AddressLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(AddressLabel, ignoreMargin: true);

                nextTop = VerifyTextBox.Margin.Top;
                VerifyLabel = ElementHelper.CreateLabel("Verify", VerifyTextBox);
                VerifyLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(VerifyLabel, ignoreMargin: true);

                nextLeft = 15;
                nextTop = ViewGrid.Height - DeleteButton.Height - padding;
                DisplayElement(DeleteButton);

                nextLeft = ElementValues.Grids.SecondaryNormal - FinishButton.Width - padding;
                nextTop = DeleteButton.Margin.Top;
                DisplayElement(FinishButton);

                UserCryptosRadioButton.Checked += (s, e) => RadioButton_Toggled();
                AllCryptosRadioButton.Checked += (s, e) => RadioButton_Toggled();

                DeleteButton.Click += (s, e) => DeleteButton_Clicked();
                FinishButton.Click += (s, e) => FinishButton_Clicked();

                if (_wallet == null)
                {
                    _wallet = new WalletConfigModel();
                }
                else
                {
                    TitleTextBlock.Text = "Edit Wallet";

                    NameTextBox.Text = _wallet.Name;
                    AddressTextBox.Text = _wallet.Address;
                    VerifyTextBox.Text = _wallet.Address;

                    ViewingCryptos.Insert(0, _wallet.Crypto);
                    CryptosComboBox.SelectedIndex = 0;
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
                if (UserCryptosRadioButton.IsChecked == true)
                {
                    ViewingCryptos = WindowController.User.WatchingCryptos;
                }
                else
                {
                    ViewingCryptos = CryptoHelper.Instance.GetCryptoNames();
                }

                CryptosComboBox.ItemsSource = ViewingCryptos;
            }

            private void DeleteButton_Clicked()
            {
                Delete();
            }

            private void FinishButton_Clicked()
            {
                SetWalletInfo();

                Save();
            }

            private void Delete()
            {
                DataHelper.Instance.DeleteWalletConfig(_wallet);

                Task.Run(() => LogHelper.AddEntry(LogType.General, $"Deleted Wallet Config: \"{_wallet.Name}\""));

                View.DisplayPrimary();
                View.DisplaySecondary();
            }

            private void Save()
            {
                DataHelper.Instance.SaveWallet(_wallet);

                Task.Run(() => LogHelper.AddEntry(LogType.General, $"Saved Wallet Config: \"{_wallet.Name}\""));

                StatusTextBlock.Text = "Wallet config saved successfully!";
                TitleTextBlock.Text = "Edit Wallet";

                View.DisplayPrimary();
            }

            public void SetWalletInfo()
            {
                _wallet.CreatedTimestamp = _wallet.ID > 0 ? _wallet.CreatedTimestamp : DateTime.Now;
                _wallet.Name = NameTextBox.Text;
                _wallet.Crypto = CryptosComboBox.Text;
                _wallet.Address = AddressTextBox.Text;
                _wallet.Status = WalletStatus.Active;
            }

        }
    }
}
