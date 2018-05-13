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

        public void NewSetup_Clicked()
        {

        }

        public void ExistingSetup_Clicked(object sender, EventArgs e)
        {

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

            }

            private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
            {
                element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

                ViewGrid.Children.Add(element);
                ActiveElements.Add(element);

                nextTop = element.Margin.Top + element.Height + padding;
            }
        }

        public class SecondaryVM
        {
            WalletSetupVM View { get; set; } = Instance;

            Grid ViewGrid { get; set; } = Instance.SecondaryGrid;

            List<FrameworkElement> ActiveElements { get; set; } = new List<FrameworkElement>();


            TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Setup Wallet", 40);

            TextBox NameTextBox { get; set; } = ElementHelper.CreateTextBox("Name");

            ComboBox CryptoComboBox { get; set; } = ElementHelper.CreateComboBox("Crypto", text: "Select a crypto...");

            TextBox AddressTextBox { get; set; } = ElementHelper.CreateTextBox("Address");

            TextBox VerifyTextBox { get; set; } = ElementHelper.CreateTextBox("Verify");

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


            public SecondaryVM()
            {
                Show();
            }

            private void Show()
            {
                DisplayElement(TitleTextBlock);

                nextLeft = 200;
                nextTop = 250;
                DisplayElement(NameTextBox);

                DisplayElement(CryptoComboBox);

                DisplayElement(AddressTextBox, topPadding: padding * 2);

                DisplayElement(VerifyTextBox);

                nextTop = NameTextBox.Margin.Top;
                NameLabel = ElementHelper.CreateLabel("Name", NameTextBox);
                NameLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(NameLabel, ignoreMargin: true);

                nextTop = CryptoComboBox.Margin.Top;
                NameLabel = ElementHelper.CreateLabel("Crypto", CryptoComboBox);
                NameLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(NameLabel, ignoreMargin: true);

                nextTop = AddressTextBox.Margin.Top;
                NameLabel = ElementHelper.CreateLabel("Address", AddressTextBox);
                NameLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(NameLabel, ignoreMargin: true);

                nextTop = VerifyTextBox.Margin.Top;
                NameLabel = ElementHelper.CreateLabel("Verify", VerifyTextBox);
                NameLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(NameLabel, ignoreMargin: true);

                nextLeft = 15;
                nextTop = ViewGrid.Height - DeleteButton.Height - padding;
                DisplayElement(DeleteButton);

                nextLeft = ElementValues.Grids.SecondaryNormal - FinishButton.Width - padding;
                nextTop = DeleteButton.Margin.Top;
                DisplayElement(FinishButton);
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
        }
    }
}
