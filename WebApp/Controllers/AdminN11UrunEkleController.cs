using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Text;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminN11UrunEkleController : Controller
    {
        private readonly PazarYeriGirisBilgileriRepository _girisBilgileriRepository = new();
        private readonly PazarYerleriRepository _pazarYerleriRepository = new();
        private static readonly ConcurrentDictionary<int, List<N11Category>> _categoryCache = new();

        public IActionResult Index()
        {
            int? pazarYeriId = 3; // N11 için sabit ID
            var pazarYerleris = _pazarYerleriRepository.GetirList(x => x.Durumu == 1);
            ViewBag.Pazaryerleri = pazarYerleris;
            ViewBag.SelectedPazarYeriId = pazarYeriId;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetN11Categories(int pazarYeriId)
        {
            pazarYeriId = 3; // N11 için sabit ID
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            if (_categoryCache.TryGetValue(pazarYeriId, out var cachedCategories))
            {
                var treeCategories = BuildCategoryTree(cachedCategories);
                return Json(new { success = true, categories = treeCategories });
            }

            string apiUrl = "https://api.n11.com/cdn/categories";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("appKey", girisBilgisi.ApiKey);

            try
            {
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var categoryResponse = JsonConvert.DeserializeObject<N11CategoryResponse>(responseJson);

                    var rootCategories = BuildHierarchicalTree(categoryResponse.categories);
                    _categoryCache[pazarYeriId] = rootCategories;
                    var treeCategories = BuildCategoryTree(rootCategories);
                    return Json(new { success = true, categories = treeCategories });
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

        private List<N11Category> BuildHierarchicalTree(List<N11Category> flatCategories)
        {
            var categoryMap = flatCategories.ToDictionary(c => c.id, c => c);
            var rootCategories = new List<N11Category>();

            foreach (var category in flatCategories)
            {
                if (category.parentId == null || !categoryMap.ContainsKey(category.parentId ?? 0))
                {
                    rootCategories.Add(category);
                }
                else
                {
                    var parent = categoryMap[category.parentId ?? 0];
                    parent.subCategories = parent.subCategories ?? new List<N11Category>();
                    parent.subCategories.Add(category);
                }
            }

            return rootCategories;
        }

        private List<object> BuildCategoryTree(List<N11Category> categories)
        {
            var result = new List<object>();
            foreach (var category in categories)
            {
                var node = new
                {
                    id = category.id,
                    name = category.name,
                    fullPath = GetCategoryPath(category, new List<N11Category>()),
                    isLeaf = category.subCategories == null || category.subCategories.Count == 0,
                    subCategories = category.subCategories != null && category.subCategories.Any()
                        ? BuildCategoryTree(category.subCategories)
                        : null
                };
                result.Add(node);
            }
            return result;
        }

        private string GetCategoryPath(N11Category category, List<N11Category> path)
        {
            path.Insert(0, category);
            if (category.parentId.HasValue && _categoryCache.TryGetValue(3, out var allCategories))
            {
                var parent = allCategories.FirstOrDefault(c => c.id == category.parentId);
                if (parent != null)
                {
                    return GetCategoryPath(parent, path);
                }
            }
            return string.Join(" > ", path.Select(c => c.name));
        }

        private async Task<List<object>> GetN11Products(PazarYeriGirisBilgileri girisBilgisi)
        {
            string apiUrl = "https://api.n11.com/ms/product-query?page=0&size=20";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("appKey", girisBilgisi.ApiKey);
            client.DefaultRequestHeaders.Add("appSecret", girisBilgisi.SecretKey);

            try
            {
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var productData = JsonConvert.DeserializeObject<ProductResponse>(jsonResponse);
                    return productData.Content.Cast<object>().ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"N11 Error: {ex.Message}");
            }
            return new List<object>();
        }

        [HttpGet]
        public async Task<IActionResult> GetN11ShipmentTemplates(int pazarYeriId)
        {
            pazarYeriId = 3; // N11 için sabit ID
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            try
            {
                var products = await GetN11Products(girisBilgisi);
                var shipmentTemplates = products
                    .Select(p => p.GetType().GetProperty("ShipmentTemplate")?.GetValue(p)?.ToString())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Distinct()
                    .OrderBy(s => s)
                    .ToList();

                return Json(new { success = true, shipmentTemplates });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddN11Product([FromForm] N11ProductRequest request, int pazarYeriId)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            // Initialize attributes if null
            request.attributes = request.attributes ?? new List<N11Attribute>();

      

            string apiUrl = "https://api.n11.com/ms/product/tasks/product-create";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("appKey", girisBilgisi.ApiKey);
            client.DefaultRequestHeaders.Add("appSecret", girisBilgisi.SecretKey);

            try
            {
                var payload = new
                {
                    payload = new
                    {
                        integrator = "YourIntegratorName",
                        skus = new[]
                        {
                            new
                            {
                                title = request.title,
                                description = request.description,
                                categoryId = request.categoryId,
                                currencyType = request.currencyType,
                                productMainId = request.productMainId,
                                preparingDay = request.preparingDay,
                                shipmentTemplate = request.shipmentTemplate,
                                maxPurchaseQuantity = request.maxPurchaseQuantity,
                                stockCode = request.stockCode,
                                barcode = request.barcode,
                                quantity = request.quantity,
                                images = new[]
                                {
                                    new
                                    {
                                        url =  request.Images,
                                        order = 1
                                    }
                                },
                                attributes = request.attributes.Select(attr => new
                                {
                                    id = attr.id,
                                    valueId = attr.valueId,
                                    customValue = attr.customValue
                                }).ToArray(),
                                salePrice = request.salePrice,
                                listPrice = request.listPrice,
                                vatRate = request.vatRate
                            }
                        }
                    }
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<N11ProductResponse>(responseJson);
                    if (result.status == "IN_QUEUE")
                        return Json(new { success = true, message = "Ürün başarıyla işleme alındı. Task ID: " + result.id });
                    else
                        return Json(new { success = false, message = "Hata: " + string.Join(", ", result.reasons) });
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