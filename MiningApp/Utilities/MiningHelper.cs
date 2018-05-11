using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    class MiningHelper
    {
        public static MiningHelper Instance { get; set; }

        Dictionary<string, string> CryptosDictionary => GetSupportedCryptosDictionary();

        public MiningHelper()
        {
            Instance = this;
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

        public List<string> GetCryptoNames()
        {
            return CryptosDictionary.Values.ToList();
        }
    }
}
