using System.Collections.Generic;
using System.Threading.Tasks;

namespace M5Finance
{
    public interface IOpenFigiClient
    {
        IOpenFigiLimits CurrentLimits { get; }
        Task<IEnumerable<OpenFigiInstrument>> GetFigiMappingsAsync(IEnumerable<OpenFigiRequest> openFigiRequestList);
    }
}