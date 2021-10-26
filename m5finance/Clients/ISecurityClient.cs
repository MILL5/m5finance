using System.Collections.Generic;
using System.Threading.Tasks;

namespace M5Finance
{
    public interface ISecurityClient
    {
        Task<IEnumerable<Security>> GetSecuritiesAsync();

        Task<IEnumerable<NasdaqSecurityExchange>> GetSecuritiesByExchangeAsync(string exchangeMic, string exchangeAcronym);
    }
}