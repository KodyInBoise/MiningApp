using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinMarketCap;
using CoinMarketCap.Entities;

namespace MiningApp
{
    public class CryptoHelper
    {
        public static CryptoHelper Instance { get; set; }

        public Task<List<TickerEntity>> GetTickers() => GetTickerList();

        public Dictionary<string, string> CryptosDictionary => GetSupportedCryptosDictionary();



        private CoinMarketCapClient _client { get; set; }

        private List<CryptoModel> _cryptos { get; set; }



        public CryptoHelper()
        {
            Instance = this;

            _client = new CoinMarketCapClient();
        }

        private async Task<List<TickerEntity>> GetTickerList()
        {
            var tickers = await _client.GetTickerListAsync();

            return tickers;
        }

        public void CreateCryptoFromTicker(TickerEntity ticker)
        {
            var crypto = CryptoModel.CreateFromTicker(ticker);

            _cryptos.Add(crypto);
        }

        public async void CreateCryptos()
        {
            var tickers = await GetTickerList();

            foreach (var ticker in tickers)
            {
                var crypto = CryptoModel.CreateFromTicker(ticker);
                _cryptos.Add(crypto);
            }
        }

        public async Task<List<CryptoModel>> GetTopCryptos()
        {
            var cryptos = new List<CryptoModel>();

            var tickers = await GetTickers();
            foreach (var ticker in tickers)
            {
                var crypto = CryptoModel.CreateFromTicker(ticker);
                cryptos.Add(crypto);
            }

            return cryptos;
        }

        public async Task<CryptoModel> CreateCryptoFromName(string name)
        {
            var ticker = await _client.GetTickerAsync(name);
            var crypto = CryptoModel.CreateFromTicker(ticker);

            return crypto;
        }


        private Dictionary<string, string> GetSupportedCryptosDictionary()
        {
            var _dictionary = new Dictionary<string, string>();
            _dictionary.Add("BTC", "Bitcoin");
            _dictionary.Add("ETH", "Ethereum");
            _dictionary.Add("LTC", "Litecoin");
            _dictionary.Add("VTC", "Vertcoin");

            return _dictionary;
        }

        public List<string> GetCryptoSymbols()
        {
            return CryptosDictionary.Keys.ToList();
        }

        public List<string> GetCryptoNames()
        {
            return CryptosDictionary.Values.ToList();
        }

        public string GetCryptoSymbolFromName(string name)
        {
            return CryptosDictionary.FirstOrDefault(x => x.Value == name).Key;
        }

        public string GetCryptoNameFromSymbol(string symbol)
        {
            return CryptosDictionary[symbol];
        }
    }
}
