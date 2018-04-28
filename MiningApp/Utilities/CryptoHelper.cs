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

        public Task<List<TickerEntity>> GetTickers => GetTickerList();

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
    }
}
