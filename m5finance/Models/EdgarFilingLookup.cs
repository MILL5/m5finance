using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static Pineapple.Common.Preconditions;

namespace M5Finance
{
    public class EdgarFilingLookup
    {
        private readonly ImmutableList<EdgarFiling> _all;
        private readonly ImmutableSortedDictionary<string, List<EdgarFiling>> _filingsDb;

        public EdgarFilingLookup(IEnumerable<EdgarFiling> filings)
        {
            CheckIsNotNull(nameof(filings), filings);

            var filingsDb = new Dictionary<string, List<EdgarFiling>>();

            foreach (var f in filings)
            {
                List<EdgarFiling> formFilings;

                if (filingsDb.ContainsKey(f.FormType))
                {
                    formFilings = filingsDb[f.FormType];
                }
                else
                {
                    formFilings = new List<EdgarFiling>();
                    filingsDb.Add(f.FormType, formFilings);
                }

                formFilings.Add(f);
            }

            _filingsDb = filingsDb.ToImmutableSortedDictionary();

            var sortedFilings = new List<EdgarFiling>();
            foreach (var ft in _filingsDb.Keys)
            {
                sortedFilings.AddRange(_filingsDb[ft]);
            }
            _all = sortedFilings.ToImmutableList();
        }

        private string[] ResolveKey(string formType)
        {
            string[] keys = null;

            if (_filingsDb.ContainsKey(formType))
                keys = new[] { formType };
            else
            {
                keys = _filingsDb.Keys.Where(x => x.Contains(formType)).ToArray();
            }

            return keys;
        }

        public IEnumerable<EdgarFiling> GetAllFilings()
        {
            return _all;
        }

        public IEnumerable<EdgarFiling> GetFilingsByForm(string formType)
        {
            CheckIsNotNullOrWhitespace(nameof(formType), formType);

            var keys = ResolveKey(formType);

            CheckIsNotNull(nameof(formType), keys);
            CheckIsNotCondition(nameof(formType), keys.Length > 1, $"{formType} matched multiples form types.");
            CheckIsEqualTo(nameof(formType), keys.Length, 1);

            return _filingsDb[formType];
        }
    }
}
