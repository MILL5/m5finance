using System.Collections.Generic;
using System.Threading.Tasks;

namespace M5Finance
{
    public interface IExchangeService
    {
        Task<IEnumerable<Exchange>> GetExchangesAsync();
        Task<Exchange> GetExchangeByMicCodeAsync(string micCode);
        Task<IEnumerable<Exchange>> GetExchangeByAcronymAsync(string acronym);
        Task<IEnumerable<string>> GetExchangeMicCodesAsync();
        Task<IEnumerable<string>> GetExchangeAcronymsAsync();
    }
}
