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

        public string Id;
        public string Name;
        public string Symbol;
        public int Rank;
        public double PriceUSD;
        public double PriceBTC;
        public double Volume24Hours;
        public double MarketCap;
        public double AvailableSupply;
        public double TotalSupply;
        public double Change1Hour;
        public double Change24Hours;
        public double Change7Days;
        public DateTime LastUpdated;

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
