using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace WebApp.Controllers
{
    public class AdminPazaramaController : Controller
    {
        private readonly PazarYeriGirisBilgileriRepository _girisBilgileriRepository;
        private readonly HttpClient _httpClient;
        private const string TokenUrl = "https://isortagimgiris.pazarama.com/connect/token";
        private const string ProductListUrl = "https://isortagimapi.pazarama.com/product/products?Approved=true&Size=100&Page=1";
        private const string CategoryUrl = "https://isortagimapi.pazarama.com/category/getCategoryTree";
        private const string BrandUrl = "https://isortagimapi.pazarama.com/brand/getBrands?Page={0}&Size={1}";
        private const string ProductCreateUrl = "https://isortagimapi.pazarama.com/product/create";
        private const string PriceUpdateUrl = "https://isortagimapi.pazarama.com/product/updatePrice";
        private const string StockUpdateUrl = "https://isortagimapi.pazarama.com/product/updateStock";
        private const string ProductDeleteUrl = "https://isortagimapi.pazarama.com/product/delete/{0}";

        public AdminPazaramaController()
        {
            _girisBilgileriRepository = new PazarYeriGirisBilgileriRepository();
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36");
        }

        public async Task<IActionResult> Index(int pazarYeriId)
        {
            pazarYeriId = 9; // Should be dynamic in production
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            var products = girisBilgisi != null ? await GetPazaramaProducts(girisBilgisi) : new List<object>();

            if (products.Count == 0 && girisBilgisi == null)
            {
                Console.WriteLine("Pazarama Error: Giriş bilgileri bulunamadı.");
            }

            ViewBag.Products = products;
            ViewBag.PazarYeriId = pazarYeriId;
            ViewBag.ApiType = "pazarama";
            return View();
        }

        private async Task<List<object>> GetPazaramaProducts(PazarYeriGirisBilgileri girisBilgisi)
        {
            try
            {
                string token = await GetAccessToken(girisBilgisi.ApiKey, girisBilgisi.SecretKey);
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Pazarama Error: Token alınamadı.");
                    return new List<object>();
                }

                var response = await SendGetRequest(ProductListUrl, token);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Pazarama Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    return new List<object>();
                }

                string responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<PazaramaProductListResponse>(responseJson);

                if (result?.Data?.Any() != true)
                {
                    Console.WriteLine("Pazarama Error: Ürün listesi alınamadı veya veri formatı hatalı.");
                    return new List<object>();
                }

                var categoryDict = await GetCategoryDictionary(9);
                foreach (var product in result.Data)
                {
                    product.CategoryName = categoryDict.GetValueOrDefault(product.CategoryId, "Bilinmiyor");
                }

                return result.Data.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pazarama Error: {ex.Message}");
                return new List<object>();
            }
        }

        private async Task<string> GetAccessToken(string clientId, string clientSecret)
        {
            try
            {
                var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
                var formData = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "scope", "merchantgatewayapi.fullaccess" }
                };

                var request = new HttpRequestMessage(HttpMethod.Post, TokenUrl)
                {
                    Content = new FormUrlEncodedContent(formData)
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Pazarama Error: Token alınamadı: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    return null;
                }

                var jsonDoc = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                return jsonDoc?.data?.accessToken?.ToString() ?? throw new Exception("accessToken key bulunamadı.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pazarama Error: Token alma hatası: {ex.Message}");
                return null;
            }
        }

        private async Task<HttpResponseMessage> SendGetRequest(string url, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await _httpClient.SendAsync(request);
        }

        private async Task<Dictionary<string, string>> GetCategoryDictionary(int pazarYeriId)
        {
            var categoryDict = new Dictionary<string, string>();
            var categoriesResult = await GetPazaramaCategories(pazarYeriId);
            if (categoriesResult is JsonResult jsonResult)
            {
                var categories = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(jsonResult.Value));
                if (categories.success == true)
                {
                    foreach (var category in categories.categories)
                    {
                        categoryDict[category.id.ToString()] = category.name.ToString();
                    }
                }
            }
            return categoryDict;
        }

        [HttpGet]
        public async Task<IActionResult> GetPazaramaCategories(int pazarYeriId)
        {
            try
            {
                var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
                if (girisBilgisi == null)
                    return Json(new { success = false, message = "Giriş bilgileri bulunamadı." });

                string token = await GetAccessToken(girisBilgisi.ApiKey, girisBilgisi.SecretKey);
                if (string.IsNullOrEmpty(token))
                    return Json(new { success = false, message = "Token alınamadı." });

                var response = await SendGetRequest(CategoryUrl, token);
                if (!response.IsSuccessStatusCode)
                    return Json(new { success = false, message = $"Kategori listesi alınamadı: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}" });

                string responseJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Pazarama Category Response: {responseJson}");

                var result = JsonConvert.DeserializeObject<PazaramaCategoryListResponse>(responseJson);
                var categories = MapToCategoryDto(result?.Data); // Map to CategoryDto with hierarchy

                return Json(new { success = true, categories });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pazarama Category Error: {ex.Message}");
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        private List<CategoryDto> MapToCategoryDto(List<PazaramaCategory> categories)
        {
            if (categories == null) return new List<CategoryDto>();
            return categories.Select(c => new CategoryDto
            {
                id = c.Id,
                name = c.DisplayName,
                isLeaf = c.Leaf,
                subCategories = MapToCategoryDto(c.SubCategories)
            }).ToList();
        }

        [HttpGet]
        public async Task<IActionResult> GetPazaramaBrands(int pazarYeriId, int page = 1, int size = 100)
        {
            try
            {
                var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
                if (girisBilgisi == null)
                    return Json(new { success = false, message = "Giriş bilgileri bulunamadı." });

                string token = await GetAccessToken(girisBilgisi.ApiKey, girisBilgisi.SecretKey);
                if (string.IsNullOrEmpty(token))
                    return Json(new { success = false, message = "Token alınamadı." });

                var response = await SendGetRequest(string.Format(BrandUrl, page, size), token);
                if (!response.IsSuccessStatusCode)
                    return Json(new { success = false, message = $"Marka listesi alınamadı: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}" });

                var result = JsonConvert.DeserializeObject<PazaramaBrandListResponse>(await response.Content.ReadAsStringAsync());
              var brands = result?.Data
    ?.Select(b => new BrandDto { id = b.Id.ToString(), name = b.Name })
    .ToList() ?? new List<BrandDto>();


                return Json(new { success = true, brands });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPazaramaProduct(int pazarYeriId, [FromBody] PazaramaProduct productData)
        {
            try
            {
                var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
                if (girisBilgisi == null)
                    return Json(new { success = false, message = "Giriş bilgileri bulunamadı." });

                string token = await GetAccessToken(girisBilgisi.ApiKey, girisBilgisi.SecretKey);
                if (string.IsNullOrEmpty(token))
                    return Json(new { success = false, message = "Token alınamadı." });

                var categoriesResult = await GetPazaramaCategories(pazarYeriId);
                if (categoriesResult is JsonResult jsonResult)
                {
                    var categories = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(jsonResult.Value));
                    if (categories.success != true || !((IEnumerable<dynamic>)categories.categories).Any(c => c.id.ToString() == productData.CategoryId))
                        return Json(new { success = false, message = "Seçilen kategori geçersiz veya leaf: true değil." });
                }
                else
                {
                    return Json(new { success = false, message = "Kategori listesi alınamadı." });
                }

                var request = new HttpRequestMessage(HttpMethod.Post, ProductCreateUrl)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(productData), Encoding.UTF8, "application/json")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                    return Json(new { success = true, message = "Ürün başarıyla eklendi." });

                return Json(new { success = false, message = $"Ürün eklenemedi: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePazaramaPrice(int pazarYeriId, [FromBody] List<PazaramaPriceUpdateRequest> priceUpdates)
        {
            if (priceUpdates == null || !priceUpdates.Any())
                return Json(new { success = false, message = "Geçerli fiyat güncelleme verisi sağlanmadı." });

            try
            {
                var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
                if (girisBilgisi == null)
                    return Json(new { success = false, message = "Giriş bilgileri bulunamadı." });

                string token = await GetAccessToken(girisBilgisi.ApiKey, girisBilgisi.SecretKey);
                if (string.IsNullOrEmpty(token))
                    return Json(new { success = false, message = "Token alınamadı." });

                var request = new HttpRequestMessage(HttpMethod.Post, PriceUpdateUrl)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { items = priceUpdates }), Encoding.UTF8, "application/json")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                    return Json(new { success = true, data = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync()) });

                return Json(new { success = false, message = $"Fiyat güncellenemedi: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePazaramaStock(int pazarYeriId, [FromBody] List<PazaramaStockUpdateRequest> stockUpdates)
        {
            if (stockUpdates == null || !stockUpdates.Any())
                return Json(new { success = false, message = "Geçerli stok güncelleme verisi sağlanmadı." });

            try
            {
                var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
                if (girisBilgisi == null)
                    return Json(new { success = false, message = "Giriş bilgileri bulunamadı." });

                string token = await GetAccessToken(girisBilgisi.ApiKey, girisBilgisi.SecretKey);
                if (string.IsNullOrEmpty(token))
                    return Json(new { success = false, message = "Token alınamadı." });

                var request = new HttpRequestMessage(HttpMethod.Post, StockUpdateUrl)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { items = stockUpdates }), Encoding.UTF8, "application/json")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                    return Json(new { success = true, data = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync()) });

                return Json(new { success = false, message = $"Stok güncellenemedi: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Sil(string code, int pazarYeriId)
        {
            try
            {
                var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
                if (girisBilgisi == null)
                    return Json(new { success = false, message = "Giriş bilgileri bulunamadı." });

                string token = await GetAccessToken(girisBilgisi.ApiKey, girisBilgisi.SecretKey);
                if (string.IsNullOrEmpty(token))
                    return Json(new { success = false, message = "Token alınamadı." });

                var request = new HttpRequestMessage(HttpMethod.Delete, string.Format(ProductDeleteUrl, code));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index", new { pazarYeriId });

                return Json(new { success = false, message = $"Ürün silinemedi: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }
    }
}
public class PazaramaCategoryListResponse
{
    public List<PazaramaCategory> Data { get; set; }
}

public class PazaramaCategory
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
    public bool Leaf { get; set; }
    public List<PazaramaCategory> SubCategories { get; set; } // Support nested categories
}

public class CategoryDto
{
    public string id { get; set; }
    public string name { get; set; }
    public bool isLeaf { get; set; } // Add isLeaf to match frontend expectation
    public List<CategoryDto> subCategories { get; set; } // Add subCategories for hierarchy
}
public class BrandDto
{
    public string id { get; set; }
    public string name { get; set; }
}
