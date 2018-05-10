using CoinMarketCap.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{

    public class CryptoModel
    {
        private TickerEntity _ticker;

        public string Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public int Rank { get; set; }
        public double PriceUSD { get; set; }
        public double PriceBTC { get; set; }
        public double Volume24Hours { get; set; }
        public double MarketCap { get; set; }
        public double AvailableSupply { get; set; }
        public double TotalSupply { get; set; }
        public double Change1Hour { get; set; }
        public double Change24Hours { get; set; }
        public double Change7Days { get; set; }
        public DateTime LastUpdated { get; set; }

        public CryptoModel(TickerEntity ticker = null)
        {
            _ticker = ticker;

            if (_ticker != null)
            {
                SetTickerInfo();
            }
        }

        private void SetTickerInfo()
        {
            Id = _ticker.Id;
            Name = _ticker.Name;
            Symbol = _ticker.Symbol;
            Rank = _ticker.Rank;
            PriceUSD = _ticker.PriceUsd ?? 0;
            PriceBTC = _ticker.PriceBtc ?? 0;
            Volume24Hours = _ticker.Volume24hUsd ?? 0;
            MarketCap = _ticker.MarketCapUsd ?? 0;
            AvailableSupply = _ticker.AvailableSupply ?? 0;
            TotalSupply = _ticker.TotalSupply ?? 0;
            Change1Hour = _ticker.PercentChange1h ?? 0;
            Change24Hours = _ticker.PercentChange24h ?? 0;
            Change7Days = _ticker.PercentChange7d ?? 0;
            LastUpdated = _ticker.LastUpdated;
        }

        public static CryptoModel CreateFromTicker(TickerEntity _ticker)
        {
            return new CryptoModel()
            {
                Id = _ticker.Id,
                Name = _ticker.Name,
                Symbol = _ticker.Symbol,
                Rank = _ticker.Rank,
                PriceUSD = _ticker.PriceUsd ?? 0,
                PriceBTC = _ticker.PriceBtc ?? 0,
                Volume24Hours = _ticker.Volume24hUsd ?? 0,
                MarketCap = _ticker.MarketCapUsd ?? 0,
                AvailableSupply = _ticker.AvailableSupply ?? 0,
                TotalSupply = _ticker.TotalSupply ?? 0,
                Change1Hour = _ticker.PercentChange1h ?? 0,
                Change24Hours = _ticker.PercentChange24h ?? 0,
                Change7Days = _ticker.PercentChange7d ?? 0,
                LastUpdated = _ticker.LastUpdated
            };
        }
    }
}
