using M5Finance.Clients.OPENFIGI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace M5Finance
{
    public interface IOpenFigiClient
    {
        IOpenFigiLimits CurrentLimits { get; }
        Task<IEnumerable<OpenFigiInstrument>> GetFigiMappingsAsync(IEnumerable<OpenFigiRequest> openFigiRequestList);
        Task<OpenFigiResponseV3> GetFigiMappingsForExchangeAsync(
            string exchangeCode,
            string marketSector = OpenFigiConsts.OpenFigiMarketSector.Equity,
            string securityType = OpenFigiConsts.OpenFigiSecurityTypes.CommonStock,
            string next = null);
    }
}