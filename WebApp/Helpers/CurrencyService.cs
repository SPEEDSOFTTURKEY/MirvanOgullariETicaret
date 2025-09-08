using System.Text.Json;
using System.Text.Json.Serialization;
namespace WebApp.Helpers
{
    public class CurrencyService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IpService _ipService;

        public CurrencyService(IHttpClientFactory httpClientFactory, IpService ipService)
        {
            _httpClientFactory = httpClientFactory;
            _ipService = ipService;
        }

        public async Task<string> GetUserCurrency()
        {
            string ip = _ipService.GetUserIp();
            if (string.IsNullOrEmpty(ip)) return "USD"; // IP bulunamazsa varsayılan TRY

            string apiUrl = $"http://ip-api.com/json/{ip}?fields=countryCode";
            using var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.GetStringAsync(apiUrl);
                var json = JsonSerializer.Deserialize<IpApiResponse>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (json?.CountryCode == null) return "USD"; // Geçersiz ülke kodu olursa USD döndür

                // Ülke kodları için Dictionary kullanımı
                Dictionary<string, string> currencyMapping = new Dictionary<string, string>
                                    {
                                        // USD Kullanan Ülkeler
                                        { "US", "USD" }, { "CA", "USD" }, { "AU", "USD" }, { "NZ", "USD" },
                                        { "SG", "USD" }, { "HK", "USD" }, { "MY", "USD" }, { "PH", "USD" },
                                        { "ID", "USD" }, { "TH", "USD" }, { "AE", "USD" }, { "SA", "USD" },
                                        { "QA", "USD" }, { "KW", "USD" }, { "BH", "USD" },

                                        // EUR Kullanan Ülkeler
                                        { "DE", "EUR" }, { "FR", "EUR" }, { "IT", "EUR" }, { "ES", "EUR" },
                                        { "NL", "EUR" }, { "BE", "EUR" }, { "AT", "EUR" }, { "FI", "EUR" },
                                        { "GR", "EUR" }, { "IE", "EUR" }, { "PT", "EUR" }, { "EE", "EUR" },
                                        { "LV", "EUR" }, { "LT", "EUR" }, { "SK", "EUR" }, { "SI", "EUR" },
                                        { "MT", "EUR" }, { "LU", "EUR" }, { "CY", "EUR" }, { "MC", "EUR" },
                                        { "SM", "EUR" }, { "VA", "EUR" }, { "AD", "EUR" },

                                        // TRY Kullanan Ülkeler
                                        { "TR", "TRY" }
                                    };

                return currencyMapping.TryGetValue(json.CountryCode, out string currency) ? currency : "USD"; ;
            }
            catch
            {
                return "USD"; // API hatası olursa USD döndür
            }
        }


        private class IpApiResponse
        {
            [JsonPropertyName("countryCode")]
            public string CountryCode { get; set; }
        }
    }

}
