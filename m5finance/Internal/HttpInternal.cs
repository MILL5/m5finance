using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace M5Finance
{
    internal static class HttpInternal
    {
        private const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.72 Safari/537.36 Edg/89.0.774.45";

        static HttpInternal()
        {
            var h = new HttpClientHandler();

            if (h.SupportsAutomaticDecompression)
            {
                h.AutomaticDecompression = DecompressionMethods.GZip |
                                           DecompressionMethods.Deflate;
                h.CookieContainer = new CookieContainer();
            }

            var hc = new HttpClient(h);

            hc.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            hc.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
            hc.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
            hc.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            //hc.DefaultRequestHeaders.Add("", "");
            Client = hc;
        }

        public static HttpClient Client { get; }

        public static async Task<string> DownloadFileAsStringAsync(this HttpClient httpClient, string url)
        {
            CheckIsNotNull(nameof(httpClient), httpClient);
            CheckIsWellFormedUri(nameof(url), url, UriKind.Absolute);

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public static async Task<string> SendAsStringAsync(this HttpClient httpClient, string url, string payLoad)
        {
            CheckIsNotNull(nameof(httpClient), httpClient);
            CheckIsWellFormedUri(nameof(url), url, UriKind.Absolute);

            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                using (var stringContent = new StringContent(payLoad, Encoding.UTF8, "application/json"))
                {
                    request.Content = stringContent;

                    using (var response = await httpClient
                        .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                        .ConfigureAwait(false))
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }
        }
    }
}
