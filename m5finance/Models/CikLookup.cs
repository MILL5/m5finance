using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using static Pineapple.Common.Preconditions;

namespace M5Finance
{
    public class CikLookup
    {
        private readonly IDictionary<int, List<CikMapping>> _toticker;
        private readonly IDictionary<string, List<CikMapping>> _tocik;

        public CikLookup(IEnumerable<CikMapping> cikMappings)
        {
            CheckIsNotNull(nameof(cikMappings), cikMappings);

            var count = cikMappings.Count();
            var toticker = new Dictionary<int, List<CikMapping>>(count);
            var tocik = new Dictionary<string, List<CikMapping>>(count);

            foreach (var m in cikMappings)
            {
                List<CikMapping> mappings;

                if (toticker.ContainsKey(m.CIK))
                {
                    mappings = toticker[m.CIK];
                }
                else
                {
                    mappings = new List<CikMapping>();
                    toticker.Add(m.CIK, mappings);
                }

                mappings.Add(m);

                if (tocik.ContainsKey(m.Ticker))
                {
                    mappings = tocik[m.Ticker];
                }
                else
                {
                    mappings = new List<CikMapping>();
                    tocik.Add(m.Ticker, mappings);
                }

                mappings.Add(m);
            }

            _toticker = toticker.ToImmutableDictionary();
            _tocik = tocik.ToImmutableDictionary();
        }

        public string[] FromCikToTicker(int cik)
        {
            CheckIsNotLessThanOrEqualTo(nameof(cik), cik, 0);

            _toticker.TryGetValue(cik, out List<CikMapping> cikMapping);

            return cikMapping?.Select(x => x.Ticker).ToArray();
        }

        public int[] FromTickerToCik(string ticker)
        {
            CheckIsNotNullOrWhitespace(nameof(ticker), ticker);

            _tocik.TryGetValue(ticker, out List<CikMapping> cikMapping);

            return cikMapping?.Select(x => x.CIK).ToArray();
        }

        public IEnumerable<IList<CikMapping>> FindOneToManyMappings()
        {
            var badcik = _toticker.Values.Where(x => x.Count > 1).ToList();
            var badticker = _tocik.Values.Where(x => x.Count > 1).ToList();

            var badmappings = new List<IList<CikMapping>>();

            badmappings.AddRange(badcik);
            badmappings.AddRange(badticker);

            return badmappings;
        }
    }
}
