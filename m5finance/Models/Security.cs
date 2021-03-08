using M5Finance.Models;
using Newtonsoft.Json;
using HashCode = Pineapple.Common.HashCode;

namespace M5Finance
{
    public class Security : BaseModel
    {
        private bool _isactive = true;
        private string _name;
        private string _ticker;
        private string _exchangeMic;

        [JsonProperty("a")]
        public bool IsActive
        {
            get => _isactive;
            set
            {
                _isactive = value;
            }
        }

        [JsonProperty("n")]
        public string Name
        {
            get => _name;
            set
            {
                _name = string.IsNullOrWhiteSpace(value) ? string.Empty : value;
            }
        }

        [JsonProperty("n")]
        public string Ticker
        {
            get => _ticker;
            set
            {
                _ticker = string.IsNullOrWhiteSpace(value) ? string.Empty : value;
            }
        }

        [JsonProperty("exchmic")]
        public string ExchangeMic
        {
            get => _exchangeMic;
            set
            {
                _exchangeMic = string.IsNullOrWhiteSpace(value) ? null : value;
            }
        }

        protected internal override void ComputeHash(HashCode hash)
        {
            hash.Add(_isactive);
            hash.Add(_name);
            hash.Add(_ticker);
            hash.Add(_exchangeMic);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Security;

            if (other == null)
                return false;

            return other._ticker.Equals(_ticker) &&
                   other._exchangeMic.Equals(_exchangeMic);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();

            hash.Add(_ticker);
            hash.Add(_exchangeMic);

            return hash.ToHashCode();
        }
    }
}
