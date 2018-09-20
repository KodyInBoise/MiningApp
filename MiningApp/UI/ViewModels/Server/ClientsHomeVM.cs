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
        }

        public void Dispose()
        {
            Instance = null;

            WindowController.Instance.ClientsHomeView = null;
        }

        public void DisplayPrimary()
        {
            PrimaryGrid.Children.Clear();

            _primaryVM = new PrimaryVM();
        }

        public void DisplaySecondary(LocalClientModel client)
        {
            SecondaryGrid.Children.Clear();

            _secondaryVM = new SecondaryVM(client);
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


            private List<LocalClientModel> _clients { get; set; } = new List<LocalClientModel>();

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

                Instance.DisplaySecondary(LocalClientModel.Instance);

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
                        if (client.ID !=  LocalClientModel.Instance.ID)
                        {
                            var button = ElementHelper.CreateButton(client.ID);
                            _clientButtons.Add(button);
                            _buttonDictionary.Add(button, client.ID);

                            DisplayElement(button);

                            button.Click += ExistingClientClicked;
                        }
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

                Instance.DisplaySecondary(client);
            }

            private void LocalClientButton_Clicked()
            {
                View.DisplaySecondary(LocalClientModel.Instance);
            }
        }

        public class SecondaryVM
        {
            ClientsHomeVM View { get; set; } = Instance;

            Grid ViewGrid { get; set; } = Instance.SecondaryGrid;


            TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("View Client", 40, width: ElementValues.Grids.SecondaryNormal);

            TextBox NameTextBox { get; set; } = ElementHelper.CreateTextBox("Name");

            TextBox PrivateIPTextBox { get; set; } = ElementHelper.CreateTextBox("PrivateIP", readOnly: true);

            TextBox PublicIPTextBox { get; set; } = ElementHelper.CreateTextBox("PublicIP", readOnly: true);

            TextBox LastCheckinTextBox { get; set; } = ElementHelper.CreateTextBox("LastCheckin", readOnly: true);


            Label NameLabel { get; set; }

            Label PrivateIPLabel { get; set; }

            Label PublicIPLabel { get; set; }

            Label LastCheckinLabel { get; set; }

            LocalClientModel _activeClient { get; set; }

            private double nextLeft = 20;

            private double nextTop = 12;

            private double padding = 15;

            private double labelRight = 600;

            private double labelOffset = -5;


            public SecondaryVM(LocalClientModel client)
            {
                _activeClient = client;

                Show();
            }

            private void Show()
            {
                DisplayElement(TitleTextBlock, leftPadding: 25);

                nextLeft = 200;
                nextTop = 250;
                DisplayElement(NameTextBox);

                DisplayElement(PrivateIPTextBox, topPadding: padding * 2);

                DisplayElement(PublicIPTextBox);

                DisplayElement(LastCheckinTextBox);

                nextTop = NameTextBox.Margin.Top;
                NameLabel = ElementHelper.CreateLabel("Name", NameTextBox);
                NameLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(NameLabel, ignoreMargin: true);

                nextTop = PrivateIPTextBox.Margin.Top;
                PrivateIPLabel = ElementHelper.CreateLabel("Private IP", PrivateIPTextBox);
                PrivateIPLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(PrivateIPLabel, ignoreMargin: true);

                nextTop = PublicIPTextBox.Margin.Top;
                PublicIPLabel = ElementHelper.CreateLabel("Public IP", PublicIPTextBox);
                PublicIPLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(PublicIPLabel, ignoreMargin: true);

                nextTop = LastCheckinTextBox.Margin.Top;
                LastCheckinLabel = ElementHelper.CreateLabel("Last Checkin", LastCheckinTextBox);
                LastCheckinLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(LastCheckinLabel, ignoreMargin: true);

                DisplayClient();
            }

            void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0, bool ignoreMargin = false)
            {
                if (!ignoreMargin)
                {
                    element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);
                }

                ViewGrid.Children.Add(element);

                nextTop = element.Margin.Top + element.Height + padding;
            }

            void DisplayClient()
            {
                if (_activeClient.ID == Bootstrapper.Settings.LocalClient.LocalClientID)
                {
                    TitleTextBlock.Text = "Local Client";
                }
                else
                {
                    TitleTextBlock.Text = "Client Details";
                }

                NameTextBox.Text = _activeClient.ID;
                PublicIPTextBox.Text = _activeClient.PublicIP;
                PrivateIPTextBox.Text = _activeClient.PrivateIP;
                LastCheckinTextBox.Text = _activeClient.LastCheckin.ToString();
            }
        }
    }
}
