using M5Finance.Models;
using Newtonsoft.Json;
using System;
using HashCode = Pineapple.Common.HashCode;

namespace M5Finance
{
    public class Exchange : BaseModel
    {
        private bool _isactive = true;
        private string _mic = string.Empty;
        private string _acronym = string.Empty;
        private string _omic = string.Empty;
        private string _name = string.Empty;
        private string _country = string.Empty;
        private string _isocountrycode = string.Empty;
        private string _website = string.Empty;

        public Exchange() : base()
        {
        }

        [JsonProperty("a")]
        public bool IsActive
        {
            get => _isactive;
            set
            {
                _isactive = value;
            }
        }

        [JsonProperty("acronym")]
        public string Acronym
        {
            get => _acronym;
            set
            {
                _acronym = string.IsNullOrWhiteSpace(value) ? string.Empty : value;
            }
        }

        [JsonProperty("mic")]
        public string Mic
        {
            get => _mic;
            set
            {
                _mic = string.IsNullOrWhiteSpace(value) ? throw new ArgumentNullException() : value;
            }
        }

        [JsonProperty("omic")]
        public string OperatingMic
        {
            get => _omic;
            set
            {
                _omic = string.IsNullOrWhiteSpace(value) ? string.Empty : value;
            }
        }

        [JsonProperty("name")]
        public string Name
        {
            get => _name;
            set
            {
                _name = string.IsNullOrWhiteSpace(value) ? throw new ArgumentNullException() : value;
            }
        }

        [JsonProperty("country")]
        public string Country
        {
            get => _country;
            set
            {
                _country = value ?? string.Empty;
            }
        }

        [JsonProperty("isocountrycode")]
        public string IsoCountryCode
        {
            get => _isocountrycode;
            set
            {
                _isocountrycode = value ?? string.Empty;
            }
        }

        [JsonProperty("website")]
        public string Website
        {
            get => _website;
            set
            {
                _website = string.IsNullOrWhiteSpace(value) ? null : value;
            }
        }

        protected internal override void ComputeHash(HashCode hash)
        {
            hash.Add(_isactive);
            hash.Add(_mic);
            hash.Add(_omic);
            hash.Add(_name);
            hash.Add(_acronym);
            hash.Add(_country);
            hash.Add(_isocountrycode);
            hash.Add(_website);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Exchange;

            if (other == null)
                return false;

            return other._mic.Equals(_mic);
        }


        public override int GetHashCode()
        {
            var hash = new HashCode();

            hash.Add(_mic);

            return hash.ToHashCode();
        }
    }
}
