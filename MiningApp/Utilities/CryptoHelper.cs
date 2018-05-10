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
        public static CryptoHelper Instance;

        private CoinMarketCapClient _client;
        private List<CryptoModel> _cryptos;

        public Task<List<TickerEntity>> GetTickers() => GetTickerList();

        public List<string> SupportedList = new List<string>()
        {
            "BTC", "ETH", "RDD"
        };

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
    }
}
