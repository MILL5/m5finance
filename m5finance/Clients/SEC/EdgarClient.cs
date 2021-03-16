using System;
using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;
using System.Diagnostics;

namespace M5Finance
{
    // CIK Mapping based on SEC Filings
    //
    // Based on https://github.com/leoliu0/cik-cusip-mapping
    //
    public class EdgarClient
    {
        private readonly HttpClient _client;

        public EdgarClient()
        {
            _client = HttpInternal.Client;
        }

        private string GetMasterIndexUrl(int year, int quarter)
        {
            CheckIsNotLessThan(nameof(year), year, 1993);
            CheckIsNotGreaterThan(nameof(year), year, DateTime.Now.Year);
            CheckIsNotLessThan(nameof(quarter), quarter, 1);
            CheckIsNotGreaterThan(nameof(quarter), quarter, 4);

            return $@"https://www.sec.gov/Archives/edgar/full-index/{year}/QTR{quarter}/master.idx";
        }

        private int GetMaxQuarterForCurrentYear()
        {
            var now = DateTime.Now;
            var month = now.Month;

            return month / 4 + 1;
        }

        private string[] GetMasterIndexUrls()
        {
            var urls = new List<string>();

            int currentYear = DateTime.Now.Year;
            int maxQuarterForCurrentYear = GetMaxQuarterForCurrentYear();

            for (int year = 1993; year <= currentYear; year++)
            {
                for (int quarter = 1; quarter <= 4; quarter++)
                {
                    if (year == currentYear)
                    {
                        if (quarter > maxQuarterForCurrentYear)
                            break;
                    }

                    urls.Add(GetMasterIndexUrl(year, quarter));
                }
            }

            return urls.ToArray();
        }

        public EdgarClient(HttpClient client)
        {
            CheckIsNotNull(nameof(client), client);

            _client = client;
        }


        // CIK|Company Name|Form Type|Date Filed|Filename
        // --------------------------------------------------------------------------------
        // 1000027|MUELLER PAUL /MO|SC 13G|1996-02-02|edgar/data/1000027/0001000027-96-000002.txt
        // 1000027|MUELLER PAUL /MO|SC 13G|1996-02-02|edgar/data/1000027/0001000027-96-000003.txt
        // 1000037|IFGP CORP /SC/|SC 13D/A|1996-02-26|edgar/data/1000037/0000897446-96-000222.txt
        // 1000037|IFGP CORP /SC/|SC 13D/A|1996-02-26|edgar/data/1000037/0000897446-96-000230.txt
        //
        public async Task<EdgarFilingLookup> GetEdgarFilingsAsync()
        {
            var urls = GetMasterIndexUrls();

            var listOfFilings = new List<EdgarFiling>(183000 * urls.Length);

            object l = new object();

            Parallel.ForEach(urls, (u) =>
            {
                try
                {
                    var fileMappings = new List<EdgarFiling>(183000);

                    var masterIdx = _client.DownloadFileAsStringAsync(u).Result;

                    using (var textReader = new StringReader(masterIdx))
                    {
                        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                        {
                            HasHeaderRecord = false,
                            TrimOptions = TrimOptions.Trim,
                            IgnoreBlankLines = true,
                            Delimiter = $"|",
                            BadDataFound = null
                        };

                        using (var csvReader = new CsvReader(textReader, config))
                        {
                            for (int i = 0; i < 10; i++)
                                _ = csvReader.ReadAsync().Result;

                            while (csvReader.ReadAsync().Result)
                            {
                                var m = new EdgarFiling
                                {
                                    CIK = csvReader.GetField<int>(0),
                                    CompanyName = csvReader.GetField<string>(1),
                                    FormType = csvReader.GetField<string>(2),
                                    DateFiled = csvReader.GetField<string>(3),
                                    Filing = csvReader.GetField<string>(4)
                                };

                                fileMappings.Add(m);
                            }
                        }

                        lock (l)
                        {
                            listOfFilings.AddRange(fileMappings);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            });

            await Task.CompletedTask;

            return new EdgarFilingLookup(listOfFilings);
        }
    }
}
