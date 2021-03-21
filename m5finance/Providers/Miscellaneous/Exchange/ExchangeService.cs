using Microsoft.VisualStudio.Threading;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace M5Finance
{
    public class ExchangeService : IExchangeService
    {
        private class State
        {
            public State(IEnumerable<Exchange> exchanges)
            {
                CheckIsNotNull(nameof(exchanges), exchanges);

                Exchanges = exchanges.ToImmutableList();

                var bymic = new Dictionary<string, Exchange>();
                var byacronym = new Dictionary<string, List<Exchange>>();

                foreach (var e in Exchanges)
                {
                    bymic.Add(e.Mic, e);

                    if (!string.IsNullOrWhiteSpace(e.Acronym))
                    {
                        List<Exchange> acronym;

                        if (byacronym.ContainsKey(e.Acronym))
                        {
                            acronym = byacronym[e.Acronym];
                        }
                        else
                        {
                            acronym = new List<Exchange>();
                            byacronym.Add(e.Acronym, acronym);
                        }

                        acronym.Add(e);
                    }
                }

                ExchangesByMicCode = bymic.ToImmutableSortedDictionary();
                ExchangesByAcronym = byacronym.ToImmutableSortedDictionary();
                MicCodes = bymic.Keys.ToImmutableSortedSet();
                Acronyms = byacronym.Keys.ToImmutableSortedSet();
            }

            public IEnumerable<Exchange> Exchanges { get; }

            public IDictionary<string, Exchange> ExchangesByMicCode { get; }

            public IDictionary<string, List<Exchange>> ExchangesByAcronym { get; }

            public IEnumerable<string> MicCodes { get; }

            public IEnumerable<string> Acronyms { get; }
        }

        private readonly ISO20022Client _client;
        private readonly AsyncLazy<State> _s;

        public ExchangeService()
        {
            _client = new ISO20022Client();
            _s = new AsyncLazy<State>(async () =>
            {
                var exchanges = await _client.GetExchangesAsync();
                return new State(exchanges);
            }, null);
        }

        public ExchangeService(ISO20022Client client)
        {
            CheckIsNotNull(nameof(client), client);
            _client = client;
            _s = new AsyncLazy<State>(async () =>
            {
                var exchanges = await _client.GetExchangesAsync();
                return new State(exchanges);
            }, null);
        }

        public async Task<IEnumerable<Exchange>> GetExchangeByAcronymAsync(string acronym)
        {
            CheckIsNotNullOrWhitespace(nameof(acronym), acronym);

            var state = await _s.GetValueAsync();

            return state.ExchangesByAcronym.TryGetValue(acronym, out var result) ? result : null;
        }

        public async Task<Exchange> GetExchangeByMicCodeAsync(string micCode)
        {
            CheckIsNotNullOrWhitespace(nameof(micCode), micCode);

            var state = await _s.GetValueAsync();

            return state.ExchangesByMicCode.TryGetValue(micCode, out var result) ? result : null;
        }

        public async Task<IEnumerable<Exchange>> GetExchangesAsync()
        {
            var state = await _s.GetValueAsync();

            return state.Exchanges;
        }

        public async Task<IEnumerable<string>> GetExchangeMicCodesAsync()
        {
            var state = await _s.GetValueAsync();

            return state.MicCodes;
        }

        public async Task<IEnumerable<string>> GetExchangeAcronymsAsync()
        {
            var state = await _s.GetValueAsync();

            return state.Acronyms;
        }
    }
}
