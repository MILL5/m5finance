using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace M5Finance
{
    public interface ISecurityService
    {
        Task<IEnumerable<Security>> GetSecuritiesAsync();
        Task<IEnumerable<Security>> GetSecuritiesByExchangeAsync(string exchange);
        Task<IEnumerable<Security>> GetSecuritiesByTickerAsync(string ticker);
        Task<Security> GetSecuritiesAsync(string ticker, string exchange);
        Task<IEnumerable<string>> GetTickersAsync();
        Task<IEnumerable<string>> GetExchangeMicCodesAsync();
    }
}
