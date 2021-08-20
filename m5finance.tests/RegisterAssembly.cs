using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using static Pineapple.Common.Preconditions;

namespace M5Finance.Tests
{
    [ExcludeFromCodeCoverage]
    public static class RegisterAssembly
    {
        public static void AddApplication(this IServiceCollection services,
                                          IConfiguration config)
        {
            const string OPEN_FIGI_API_KEY = "OpenFigiApiKey";

            CheckIsNotNull(nameof(services), services);
            CheckIsNotNull(nameof(config), config);

            var apiKey = config[OPEN_FIGI_API_KEY];
            services.AddSingleton<IOpenFigiClient>(new OpenFigiClient(apiKey));
        }
    }
}
