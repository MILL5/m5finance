using System;
using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace M5Finance
{
    public class EdgarFilingClient
    {
        private readonly HttpClient _client;

        public EdgarFilingClient()
        {
            _client = HttpInternal.Client;
        }

        public EdgarFilingClient(HttpClient client)
        {
            CheckIsNotNull(nameof(client), client);

            _client = client;
        }


        public async Task<string> GetEdgarFilingAsync(EdgarFiling filing)
        {
            CheckIsNotNull(nameof(filing), filing);
            
            var text = await _client.DownloadFileAsStringAsync(filing.FilingUrl);

            return text;
        }
    }
}
