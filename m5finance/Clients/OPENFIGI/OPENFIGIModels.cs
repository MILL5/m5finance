using Newtonsoft.Json;
using System.Collections.Generic;

namespace M5Finance
{
    public class OpenFigiRequest
    {
        private OpenFigiRequest()
        {

        }

        public OpenFigiRequest(string idType, string idValue)
            : this()
        {
            this.IdType = idType;
            this.IdValue = idValue;
        }

        public OpenFigiRequest WithExchangeCode(string exchCode)
        {
            this.ExchangeCode = exchCode;
            return this;
        }

        public OpenFigiRequest WithMicCode(string micCode)
        {
            this.MicCode = micCode;
            return this;
        }

        public OpenFigiRequest WithCurrency(string currency)
        {
            this.Currency = currency;
            return this;
        }

        public OpenFigiRequest WithMarketSectorDescription(string marketSectorDescription)
        {
            MarketSectorDescription = marketSectorDescription;
            return this;
        }

        [JsonProperty("idType")]
        public string IdType { get; set; }

        [JsonProperty("idValue")]
        public string IdValue { get; set; }

        [JsonProperty("exchCode")]
        public string ExchangeCode { get; set; }

        [JsonProperty("micCode")]
        public string MicCode { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("marketSecDes")]
        public string MarketSectorDescription { get; set; }
    }

    public class OpenFigiInstrument
    {
        [JsonProperty("figi")]
        public string figi { get; set; }
        [JsonProperty("securityType")]
        public string SecurityType { get; set; }
        [JsonProperty("marketSector")]
        public string MarketSector { get; set; }
        [JsonProperty("ticker")]
        public string Ticker { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("uniqueID")]
        public string uniqueID { get; set; }
        [JsonProperty("exchCode")]
        public string exchCode { get; set; }
        [JsonProperty("shareClassFIGI")]
        public string ShareClassFIGI { get; set; }
        [JsonProperty("compositeFIGI")]
        public string CompositeFIGI { get; set; }
        [JsonProperty("securityType2")]
        public string SecurityType2 { get; set; }
        [JsonProperty("securityDescription")]
        public string SecurityDescription { get; set; }
        [JsonProperty("uniqueIDFutOpt")]
        public string uniqueIDFutOpt { get; set; }

        [JsonProperty("tickerComplete")]
        public string TickerComplete
        {
            get { return MarketSector == "Equity" ? string.Format("{0} {1} {2}", Ticker, exchCode, MarketSector) : string.Format("{0} {1}", Ticker, MarketSector); }
        }
    }

    public class OpenFigiArrayResponse
    {
        [JsonProperty("data")]
        public List<OpenFigiInstrument> Data { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
