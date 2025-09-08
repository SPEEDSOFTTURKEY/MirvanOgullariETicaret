using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    public class AdminTrendyolController : Controller
    {
        private readonly PazarYeriGirisBilgileriRepository _girisBilgileriRepository = new PazarYeriGirisBilgileriRepository();

        public class Category
        {
            public int Id { get; set; }
            public int? ParentId { get; set; }
            public string Name { get; set; }
        }

        // Define the TrendyolProductRequest class to match the client-side payload
        public class TrendyolProductRequestUpdate
        {
            public string ProductId { get; set; }
            public string Title { get; set; }
            public string Barcode { get; set; }
            public int StockQuantity { get; set; }
            public double SalePrice { get; set; }
            public double ListPrice { get; set; }
            public string Description { get; set; }
            public string ImageUrl { get; set; }
            public int CategoryId { get; set; }
            public int VatRate { get; set; }
        }
        public class TrendyolProductResponseUpdate
        {
            public List<TrendyolProductRequestUpdate> Content { get; set; }
            public int TotalCount { get; set; }
            public bool Success { get; set; }
            public string Message { get; set; }
            public int ProductId { get; set; }
            [JsonProperty("batchRequestId")]
            public string BatchRequestId { get; set; }
        }


        private async Task<List<Category>> GetTrendyolCategories()
        {
            string url = "https://api.trendyol.com/sapigw/product-categories";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var jsonData = JsonConvert.DeserializeObject<dynamic>(responseBody);
                var categories = new List<Category>();

                void ParseCategories(dynamic categoryList, int? parentId = null)
                {
                    foreach (var cat in categoryList)
                    {
                        categories.Add(new Category
                        {
                            Id = (int)cat.id,
                            ParentId = cat.parentId != null ? (int?)cat.parentId : null,
                            Name = (string)cat.name
                        });
                        if (cat.subCategories != null)
                        {
                            ParseCategories(cat.subCategories, (int)cat.id);
                        }
                    }
                }

                ParseCategories(jsonData.categories);
                return categories;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching categories: {ex.Message}");
                return new List<Category>();
            }
        }

        public async Task<IActionResult> Index(int pazarYeriId)
        {
            pazarYeriId = 7;
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            var products = new List<object>();
            var categories = new List<Category>();

            if (girisBilgisi != null)
            {
                products = await GetTrendyolProducts(girisBilgisi);
                categories = await GetTrendyolCategories();
            }

            ViewBag.Products = products;
            ViewBag.PazarYeriId = pazarYeriId;
            ViewBag.ApiType = "trendyol";
            ViewBag.Categories = categories; // Pass all categories for the form
            return View();
        }

        [HttpPost]
        public IActionResult Sil(string productId, int pazarYeriId)
        {
            return DeleteTrendyolProduct(productId, pazarYeriId).Result;
        }

        private async Task<List<object>> GetTrendyolProducts(PazarYeriGirisBilgileri girisBilgisi)
        {
            string apiKey = girisBilgisi.ApiKey;
            string secretKey = girisBilgisi.SecretKey;
            string sellerId = girisBilgisi.KullaniciAdi;
            string url = $"https://api.trendyol.com/sapigw/suppliers/{sellerId}/products";
            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:{secretKey}"));

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            client.DefaultRequestHeaders.Add("User-Agent", $"{sellerId} - SelfIntegration");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<TrendyolProductResponse>(responseBody);
                return responseData?.Content?.Cast<object>().ToList() ?? new List<object>();
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Trendyol HTTP Error: {httpEx.Message}");
                return new List<object>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Trendyol Error: {ex.Message}");
                return new List<object>();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddTrendyolProduct([FromBody] TrendyolProductRequest request, int pazarYeriId)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();

            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            string supplierId = girisBilgisi.KullaniciAdi;
            string apiKey = girisBilgisi.ApiKey;
            string apiSecret = girisBilgisi.SecretKey;

            var credentials = $"{apiKey}:{apiSecret}";
            var base64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

            string apiUrl = $"https://api.trendyol.com/sapigw/suppliers/{supplierId}/v2/products";

            // Prepare the payload to match Trendyol API structure
            var payload = new
            {
                items = new[]
                {
                    new
                    {
                        barcode = request.Barcode,
                        title = request.Title,
                        productMainId = request.Barcode, // Assuming barcode as productMainId
                        brandId = 1, // You may need to fetch or pass brandId dynamically
                        categoryId = request.CategoryId,
                        quantity = request.StockQuantity,
                        stockCode = request.Barcode, // Assuming barcode as stockCode
                        dimensionalWeight = 1, // Default value, adjust as needed
                        description = request.Description,
                        currencyType = "TRY",
                        listPrice = request.ListPrice,
                        salePrice = request.SalePrice,
                        vatRate = request.VatRate,
                        cargoCompanyId = 10, // Default value, adjust as needed
                        deliveryOption = new
                        {
                            deliveryDuration = 1,
                            fastDeliveryType = "SAME_DAY_SHIPPING|FAST_DELIVERY"
                        },
                        images = new[]
                        {
                            new { url = request.ImageUrl ?? "https://example.com/default.jpg" }
                        },
                    }
                }
            };

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<TrendyolProductResponse>(responseJson);
                    return Json(new { success = true, message = "Ürün başarıyla yüklendi.", response = result });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"API Hatası: {(int)response.StatusCode}", error = errorContent });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"İstisna: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTrendyolProduct([FromBody] TrendyolProductRequestUpdate request, int pazarYeriId)
        {
            pazarYeriId = 7;
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            string apiUrl = $"https://api.trendyol.com/sapigw/suppliers/{girisBilgisi.KullaniciAdi}/v2/products";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{girisBilgisi.ApiKey}:{girisBilgisi.SecretKey}")));

            try
            {
                var payload = new
                {
                    items = new[]
                    {
                        new
                        {
                            barcode = request.Barcode,
                            title = request.Title,
                            productMainId = request.Barcode,
                            brandId = 1,
                            categoryId = request.CategoryId,
                            quantity = request.StockQuantity,
                            stockCode = request.Barcode,
                            dimensionalWeight = 1,
                            description = request.Description,
                            currencyType = "TRY",
                            listPrice = request.ListPrice,
                            salePrice = request.SalePrice,
                            vatRate = request.VatRate,
                            cargoCompanyId = 10,
                            deliveryOption = new
                            {
                                deliveryDuration = 1,
                                fastDeliveryType = "SAME_DAY_SHIPPING|FAST_DELIVERY"
                            },
                            images = new[]
                            {
                                new { url = request.ImageUrl ?? "https://example.com/default.jpg" }
                            },
                        }
                    }
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<TrendyolProductResponseUpdate>(responseJson);

                    return Json(new { success = true, message = "Ürün güncellendi.", batchRequestId = result?.BatchRequestId });
                }

                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Error: {(int)response.StatusCode} - {errorContent}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTrendyolProduct(string productId, int pazarYeriId)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            string apiUrl = $"https://api.trendyol.com/sapigw/suppliers/{girisBilgisi.KullaniciAdi}/products/{productId}";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{girisBilgisi.ApiKey}:{girisBilgisi.SecretKey}")));

            try
            {
                var response = await client.DeleteAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<TrendyolProductResponse>(responseJson);
                    if (result.Success)
                        return Json(new { success = true, message = result.Message });
                    else
                        return Json(new { success = false, message = result.Message });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Error: {(int)response.StatusCode} - {errorContent}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}