using Newtonsoft.Json;
using System.Collections.Generic;

namespace M5Finance
{
    public class OpenFigiRequest
    {
        [JsonProperty("idType")]
        public string IdType { get; set; }

        [JsonProperty("idValue")]
        public string IdValue { get; set; }

        [JsonProperty("exchCode", NullValueHandling = NullValueHandling.Ignore)]
        public string ExchCode { get; set; }

        [JsonProperty("micCode", NullValueHandling = NullValueHandling.Ignore)]
        public string MicCode { get; set; }

        [JsonProperty("currency", NullValueHandling = NullValueHandling.Ignore)]
        public string Currency { get; set; }

        [JsonProperty("marketSecDes", NullValueHandling = NullValueHandling.Ignore)]
        public string MarketSecDes { get; set; }

        [JsonProperty("securityType", NullValueHandling = NullValueHandling.Ignore)]
        public string SecurityType { get; set; }

        [JsonProperty("securityType2", NullValueHandling = NullValueHandling.Ignore)]
        public string SecurityType2 { get; set; }

        [JsonProperty("includeUnlistedEquities", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IncludeUnlistedEquities { get; set; }

        [JsonProperty("optionType", NullValueHandling = NullValueHandling.Ignore)]
        public string OptionType { get; set; }

        [JsonProperty("strike", NullValueHandling = NullValueHandling.Ignore)]
        public long?[] Strike { get; set; }
        
        [JsonProperty("contractSize", NullValueHandling = NullValueHandling.Ignore)]
        public long?[] ContractSize { get; set; }

        [JsonProperty("coupon", NullValueHandling = NullValueHandling.Ignore)]
        public long?[] Coupon { get; set; }

        [JsonProperty("expiration", NullValueHandling = NullValueHandling.Ignore)]
        public long?[] Expiration { get; set; }

        [JsonProperty("maturity", NullValueHandling = NullValueHandling.Ignore)]
        public long?[] Maturity { get; set; }

        [JsonProperty("stateCode", NullValueHandling = NullValueHandling.Ignore)]
        public string StateCode { get; set; }
    }

    public class OpenFigiInstrument
    {
        [JsonProperty("figi")]
        public string Figi { get; set; }
        [JsonProperty("securityType")]
        public string SecurityType { get; set; }
        [JsonProperty("marketSector")]
        public string MarketSector { get; set; }
        [JsonProperty("exchCode")]
        public string ExchCode { get; set; }
        [JsonProperty("securityType2")]
        public string SecurityType2 { get; set; }
        [JsonProperty("ticker")]
        public string Ticker { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("uniqueID")]
        public string UniqueID { get; set; }
        [JsonProperty("shareClassFIGI")]
        public string ShareClassFIGI { get; set; }
        [JsonProperty("compositeFIGI")]
        public string CompositeFIGI { get; set; }
        [JsonProperty("securityDescription")]
        public string SecurityDescription { get; set; }
        [JsonProperty("uniqueIDFutOpt")]
        public string UniqueIDFutOpt { get; set; }
    }

    public class OpenFigiArrayResponse
    {
        [JsonProperty("data")]
        public List<OpenFigiInstrument> Data { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }

    public class OpenFigiRequestV3
    {
        [JsonProperty("exchCode")]
        public string ExchCode { get; set; }

        [JsonProperty("marketSecDes")]
        public string MarketSectorDesc { get; set; }

        [JsonProperty("securityType")]
        public string SecurityType { get; set; }

        [JsonProperty("start", NullValueHandling = NullValueHandling.Ignore)]
        public string Start { get; set; }
    }

    public class OpenFigiResponseV3
    {
        [JsonProperty("data")]
        public List<OpenFigiInstrumentV3> Data { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }

        [JsonProperty("total")]
        public string Total { get; set; }
    }

    public class OpenFigiInstrumentV3
    {
        [JsonProperty("figi")]
        public string Figi { get; set; }

        [JsonProperty("securityType")]
        public string SecurityType { get; set; }

        [JsonProperty("marketSector")]
        public string MarketSector { get; set; }

        [JsonProperty("ticker")]
        public string Ticker { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("exchCode")]
        public string ExchCode { get; set; }

        [JsonProperty("shareClassFIGI")]
        public string ShareClassFIGI { get; set; }

        [JsonProperty("compositeFIGI")]
        public string CompositeFIGI { get; set; }

        [JsonProperty("securityType2")]
        public string SecurityType2 { get; set; }

        [JsonProperty("securityDescription")]
        public string SecurityDescription { get; set; }
    }
}