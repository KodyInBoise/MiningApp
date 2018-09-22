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

        public void UpdateButtonContent(string clientID, string newContent)
        {
            _primaryVM.UpdateClientButtonContent(clientID, newContent);
        }

        public void DeleteClient(string clientID)
        {
            _primaryVM.DeleteClient(clientID);
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
                            var button = ElementHelper.CreateButton(client.GetDisplayName());
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

            public void UpdateClientButtonContent(string clientID, string newName)
            {
                var button = _buttonDictionary.FirstOrDefault(x => x.Value == clientID).Key;

                if (button != null)
                {
                    button.Content = newName;
                }
            }

            public void DeleteClient(string clientID)
            {
                var button = _buttonDictionary.FirstOrDefault(x => x.Value == clientID).Key;

                if (button != null)
                {
                    _clients.Remove(_clients.Find(x => x.ID == clientID));
                    _buttonDictionary.Remove(button);

                    ViewGrid.Children.Remove(button);

                    Instance.DisplaySecondary(LocalClientModel.Instance);
                }
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

            ComboBox SessionsComboBox { get; set; } = ElementHelper.CreateComboBox("Sessions");

            Button ToggleStatusButton { get; set; } = ElementHelper.CreateButton("Toggle", width: 150, height: 50);

            Button DeleteButton { get; set; } = ElementHelper.CreateButton("Delete", style: ButtonStyleEnum.Delete);

            Button UpdateButton { get; set; } = ElementHelper.CreateButton("Update", style: ButtonStyleEnum.New);


            Label NameLabel { get; set; }

            Label PrivateIPLabel { get; set; }

            Label PublicIPLabel { get; set; }

            Label LastCheckinLabel { get; set; }

            Label SessionsLabel { get; set; }

            LocalClientModel _activeClient { get; set; }

            List<ClientConfigModel> _clientConfigs { get; set; }

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

                DisplayElement(SessionsComboBox);

                nextLeft = SessionsComboBox.Margin.Left + SessionsComboBox.Width - ToggleStatusButton.Width;
                ToggleStatusButton.Margin = new Thickness(nextLeft, nextTop, 0, 0);

                nextLeft = padding * 2;
                nextTop = ElementValues.Grids.Height - DeleteButton.Height - padding * 2;
                DisplayElement(DeleteButton);
                DeleteButton.Click += (s, e) => DeleteButton_Clicked();

                nextLeft = ElementValues.Grids.SecondaryNormal - UpdateButton.Width - padding * 2;
                nextTop = ElementValues.Grids.Height - UpdateButton.Height - padding * 2;
                DisplayElement(UpdateButton);
                UpdateButton.Click += (s, e) => UpdateButton_Clicked();

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

                nextTop = SessionsComboBox.Margin.Top;
                SessionsLabel = ElementHelper.CreateLabel("Sessions", SessionsComboBox);
                SessionsLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(SessionsLabel, ignoreMargin: true);

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

            async void DisplayClient()
            {
                NameTextBox.Text = _activeClient.GetDisplayName();
                PublicIPTextBox.Text = _activeClient.PublicIP;
                PrivateIPTextBox.Text = _activeClient.PrivateIP;
                LastCheckinTextBox.Text = _activeClient.LastCheckin.ToString();

                if (_activeClient.ID == Bootstrapper.Settings.LocalClient.LocalClientID)
                {
                    TitleTextBlock.Text = "Local Client";

                    //SessionsLabel.Visibility = Visibility.Collapsed;
                    //SessionsComboBox.Visibility = Visibility.Collapsed;
                    //ToggleStatusButton.Visibility = Visibility.Collapsed;
                    DeleteButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    TitleTextBlock.Text = "Client Details";

                    SessionsLabel.Visibility = Visibility.Visible;
                    SessionsComboBox.Visibility = Visibility.Visible;
                    ToggleStatusButton.Visibility = Visibility.Visible;
                    DeleteButton.Visibility = Visibility.Visible;

                    //_clientConfigs = await Task.Run(() => ServerHelper.GetClientConfigs(_activeClient.ID));

                    //SessionsComboBox.ItemsSource = _clientConfigs;
                }

                _clientConfigs = await Task.Run(() => ServerHelper.GetClientConfigs(_activeClient.ID));

                SessionsComboBox.ItemsSource = _clientConfigs;
                if (_clientConfigs.Any())
                {
                    SessionsComboBox.SelectedIndex = 0;
                }

                UpdateToggleStatusButton();
            }

            void DeleteButton_Clicked()
            {
                // Need to possibly check against online clients an not allow deletion if online.
                var result = MessageBox.Show("Are you sure you want to remove this client?", "Delete Client", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteButton.Visibility = Visibility.Collapsed;

                    Task.Run(() => ServerHelper.DeleteClient(_activeClient.ID));

                    Instance.DeleteClient(_activeClient.ID);
                }
            }

            async void UpdateButton_Clicked()
            {
                try
                {
                    UpdateButton.Visibility = Visibility.Collapsed;

                    if (!string.IsNullOrEmpty(NameTextBox.Text) && NameTextBox.Text != _activeClient.ID)
                    {
                        _activeClient.FriendlyName = NameTextBox.Text;

                        if (_activeClient.ID == LocalClientModel.Instance.ID)
                        {
                            LocalClientModel.Instance.FriendlyName = NameTextBox.Text;
                        }
                        else
                        {
                            // Update client button name in PrimaryVM
                            Instance.UpdateButtonContent(_activeClient.ID, NameTextBox.Text);
                        }
                    }

                    await Task.Run(() => ServerHelper.UpdateClient(_activeClient, Bootstrapper.User.ID));

                    UpdateButton.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    ExceptionUtil.Handle(ex, message: $"An error occurred updating client information on the server: {ex.Message}");
                }
            }

            ClientConfigModel GetSelectedConfig()
            {
                return (ClientConfigModel)SessionsComboBox.SelectedItem;
            }

            void UpdateToggleStatusButton()
            {
                var selectedConfig = GetSelectedConfig();
                var margin = ToggleStatusButton.Margin;

                if (ViewGrid.Children.Contains(ToggleStatusButton))
                {
                    ViewGrid.Children.Remove(ToggleStatusButton);
                    ToggleStatusButton = null;
                }

                if (selectedConfig != null)
                {
                    switch ((SessionStatusEnum)selectedConfig.Status)
                    {
                        case SessionStatusEnum.Running:
                            ToggleStatusButton = ElementHelper.CreateButton("Pause", style: ButtonStyleEnum.Orange, width: 150, height: 50);
                            ToggleStatusButton.Click += (s, e) => {
                                Task.Run(() => LocalClientModel.Instance.SendClientMessage(_activeClient.ID, ClientAction.PauseSession, selectedConfig.ServerID));
                            };
                            break;
                        default:
                            break;
                    }

                    ToggleStatusButton.Margin = margin;
                    DisplayElement(ToggleStatusButton, ignoreMargin: true);
                }
            }
        }
    }
}
