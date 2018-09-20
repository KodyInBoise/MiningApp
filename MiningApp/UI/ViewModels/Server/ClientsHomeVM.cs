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
    public class ClientsHomeVM
    {
        public static ClientsHomeVM Instance { get; set; }


        Grid PrimaryGrid { get; set; } = MainWindow.Instance.PrimaryGrid;

        Grid SecondaryGrid { get; set; } = MainWindow.Instance.SecondaryGrid;


        private PrimaryVM _primaryVM { get; set; }

        private SecondaryVM _secondaryVM { get; set; }


        public ClientsHomeVM()
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

        public void DisplaySecondary(ServerClientModel client)
        {
            SecondaryGrid.Children.Clear();

            _secondaryVM = new SecondaryVM();
        }

        public class PrimaryVM
        {
            ClientsHomeVM View { get; set; } = Instance;

            Grid ViewGrid { get; set; } = Instance.PrimaryGrid;


            TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Clients", 40);

            Button LoalClientButton { get; set; } = ElementHelper.CreateButton("Local Client", height: buttonHeight, style: ButtonStyleEnum.New);


            private static int buttonHeight = 60;

            private double nextLeft = 20;

            private double nextTop = 12;

            private double padding = 15;


            private List<ServerClientModel> _clients { get; set; } = new List<ServerClientModel>();

            private List<Button> _clientButtons { get; set; } = new List<Button>();

            private Dictionary<Button, string> _buttonDictionary { get; set; } = new Dictionary<Button, string>();


            public PrimaryVM()
            {
                Show();
            }

            private void Show()
            {
                DisplayElement(TitleTextBlock, leftPadding: 25);
                nextTop = 75;

                DisplayElement(LoalClientButton);
                LoalClientButton.Click += (s, e) => LocalClientButton_Clicked();

                DisplayExisting();
            }

            private async void DisplayExisting()
            {
                if (Bootstrapper.User != null)
                {
                    _clients = await Task.Run(() => ServerHelper.GetUserClients(Bootstrapper.User.ID));

                    nextTop = LoalClientButton.Margin.Top + LoalClientButton.Height + padding * 2;

                    foreach (var client in _clients)
                    {
                        var button = ElementHelper.CreateButton(client.ID);
                        _clientButtons.Add(button);
                        _buttonDictionary.Add(button, client.ID);

                        DisplayElement(button);

                        button.Click += ExistingClientClicked;
                    }
                }
                else
                {
                    // NEED TO ADD CODE TO HANDLE CLIENT DISPLAY WHEN THERE'S NO ACTIVE USER
                }
            }

            private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
            {
                element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

                ViewGrid.Children.Add(element);

                nextTop = element.Margin.Top + element.Height + padding;
            }

            private void ExistingClientClicked(object sender, EventArgs e)
            {
                var button = (Button)sender;

                var client = _clients.Find(x => x.ID == _buttonDictionary[button]);
            }

            private void LocalClientButton_Clicked()
            {
                View.DisplaySecondary(null);
            }
        }

        public class SecondaryVM
        {
            ClientsHomeVM View { get; set; } = Instance;

            Grid ViewGrid { get; set; } = Instance.SecondaryGrid;


            TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("View Client", 40, width: ElementValues.Grids.SecondaryNormal);


            private static int buttonHeight = 60;

            private double nextLeft = 20;

            private double nextTop = 12;

            private double padding = 15;


            public SecondaryVM()
            {
                Show();
            }

            private void Show()
            {
                DisplayElement(TitleTextBlock, leftPadding: 25);
                nextTop = 75;


            }

            private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
            {
                element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

                ViewGrid.Children.Add(element);

                nextTop = element.Margin.Top + element.Height + padding;
            }
        }
    }
}
