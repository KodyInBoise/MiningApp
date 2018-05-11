using MiningApp.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class HomeViewModel
    {
        private HomeWindow _window { get; set; }

        private UserModel _user { get; set; }

        private CryptoModel _selectedCrypto { get; set; }

        private List<CryptoModel> _watchingCryptos { get; set; }

        public HomeViewModel(HomeWindow window)
        {
            _window = window;
            _user = WindowController.User;

            ShowWindow();
        }

        private void ShowWindow()
        {
            WindowController.Instance.HomeView = this;

            _window.Left = WindowController.Instance.WindowLeft;
            _window.Top = WindowController.Instance.WindowTop;

            _window.WatchingSymbolsListBox.SelectionChanged += (s, e) => CryptoListBox_SelectionChanged();

            LoadUserInfo();

            _window.Show();
        }

        public void Dispose()
        {
            WindowController.Instance.HomeView = null;

            _window.Close();
        }

        private void DisplayCrypto(int index)
        {
            _selectedCrypto = _watchingCryptos[index];

            _window.NameTextBlock.Text = $"Name: {_selectedCrypto.Name}";
            _window.PriceUSDTextBlock.Text = $"Price USD: ${_selectedCrypto.PriceUSD}";
            _window.PriceBTCTextBlock.Text = $"Price BTC: {_selectedCrypto.PriceBTC}";
            _window.VolumeTextBlock.Text = $"24 Hour Volume: {_selectedCrypto.Volume24Hours}";
            _window.HourlyChangeTextBlock.Text = $"Hourly Change: {_selectedCrypto.Change1Hour}%";
            _window.DailyChangeTextBlock.Text = $"Daily Change: {_selectedCrypto.Change24Hours}%";
            _window.WeeklyChangeTextBlock.Text = $"Weekly Change: {_selectedCrypto.Change7Days}%";
        }

        private async void LoadUserInfo()
        {
            _window.WatchingSymbolsListBox.ItemsSource = _user.WatchingCryptos;

            await Task.Run(LoadCryptos);
        }

        private async Task LoadCryptos()
        {
            _watchingCryptos = new List<CryptoModel>();
            var symbols = _user.WatchingCryptos;

            foreach (var symbol in symbols)
            {
                _watchingCryptos.Add(await CryptoHelper.Instance.CreateCryptoFromName(symbol));
            }
        }

        private void CryptoListBox_SelectionChanged()
        {
            try
            {
                DisplayCrypto(_window.WatchingSymbolsListBox.SelectedIndex);
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }
    }
}
