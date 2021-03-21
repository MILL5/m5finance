using Microsoft.VisualStudio.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;
using MoreLinq.Extensions;

namespace M5Finance
{
    public class SecurityService : ISecurityService
    {
        private class State
        {
            public State(IEnumerable<Security> securities)
            {
                CheckIsNotNull(nameof(securities), securities);

                var aggregate = new List<Security>();
                var byexch = new Dictionary<string, List<Security>>();
                var byticker = new Dictionary<string, List<Security>>();

                foreach (var s in securities)
                {
                    if (aggregate.Contains(s))
                    {
                        continue;
                    }

                    aggregate.Add(s);

                    List<Security> exchange;

                    if (byexch.ContainsKey(s.ExchangeMic))
                    {
                        exchange = byexch[s.ExchangeMic];
                    }
                    else
                    {
                        exchange = new List<Security>();
                        byexch.Add(s.ExchangeMic, exchange);
                    }

                    exchange.Add(s);

                    List<Security> ticker;

                    if (byticker.ContainsKey(s.Ticker))
                    {
                        ticker = byticker[s.Ticker];
                    }
                    else
                    {
                        ticker = new List<Security>();
                        byticker.Add(s.Ticker, ticker);
                    }

                    ticker.Add(s);
                }

                Securities = aggregate.ToImmutableList();
                SecuritiesByTicker = byticker.ToImmutableSortedDictionary();
                SecuritiesByExchange = byexch.ToImmutableSortedDictionary();
                MicCodes = byexch.Keys.ToImmutableSortedSet();
                Tickers = byticker.Keys.ToImmutableSortedSet();
            }

            public IEnumerable<Security> Securities { get; }

            public IDictionary<string, List<Security>> SecuritiesByTicker { get; }

            public IDictionary<string, List<Security>> SecuritiesByExchange { get; }

            public IEnumerable<string> MicCodes { get; }
            public IEnumerable<string> Tickers { get; }
        }

        private readonly List<ISecurityClient> _clients;
        private readonly AsyncLazy<State> _s;

        public SecurityService()
        {
            _clients = new List<ISecurityClient> { new NASDAQClient(),
                                                   new NTNasdaqClient(),
                                                   new NTOtherClient() };

            _s = new AsyncLazy<State>(() =>
            {
                var l = new object();
                List<Security> securities = new List<Security>(10000);

                // TODO: change this out to full async
                Parallel.ForEach(_clients, (c) =>
                {
                      var s = c.GetSecuritiesAsync().Result;

                      lock (l)
                      {
                          securities.AddRange(s);
                      }
                 });

                return Task.FromResult(new State(securities));
            }, null);
        }

        public SecurityService(IEnumerable<ISecurityClient> clients)
        {
            CheckIsNotNull(nameof(clients), clients);
            CheckIsNotLessThan(nameof(clients), clients.Count(), 1);

            _clients = new List<ISecurityClient>(clients);

            _s = new AsyncLazy<State>(() =>
            {
                var l = new object();
                List<Security> securities = new List<Security>(10000);

                // TODO: change this out to full async
                Parallel.ForEach(_clients, (c) =>
                {
                    var s = c.GetSecuritiesAsync().Result;

                    lock (l)
                    {
                        securities.AddRange(s);
                    }
                });

                return Task.FromResult(new State(securities));
            }, null);
        }

        public async Task<IEnumerable<Security>> GetSecuritiesAsync()
        {
            var state = await _s.GetValueAsync();

            return state.Securities;
        }

        public async Task<Security> GetSecuritiesAsync(string ticker, string exchange)
        {
            var state = await _s.GetValueAsync();

            return state.SecuritiesByTicker[ticker].Where(x => x.ExchangeMic == exchange).SingleOrDefault();
        }

        public async Task<IEnumerable<Security>> GetSecuritiesByExchangeAsync(string exchange)
        {
            var state = await _s.GetValueAsync();

            return state.SecuritiesByExchange[exchange].ToList();
        }

        public async Task<IEnumerable<Security>> GetSecuritiesByTickerAsync(string ticker)
        {
            var state = await _s.GetValueAsync();

            return state.SecuritiesByTicker[ticker].ToList();
        }

        public async Task<IEnumerable<string>> GetTickersAsync()
        {
            var state = await _s.GetValueAsync();

            return state.Tickers;
        }

        public async Task<IEnumerable<string>> GetExchangeMicCodesAsync()
        {
            var state = await _s.GetValueAsync();

            return state.MicCodes;
        }
    }
}
