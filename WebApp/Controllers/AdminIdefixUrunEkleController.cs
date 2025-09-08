using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Text;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminIdefixUrunEkleController : Controller
    {
        private readonly PazarYeriGirisBilgileriRepository _girisBilgileriRepository = new();
        private static readonly ConcurrentDictionary<int, List<IdefixCategory>> _categoryCache = new();
        private static readonly ConcurrentDictionary<int, List<IdefixBrand>> _brandCache = new();
        private static readonly ConcurrentDictionary<int, List<IdefixCargoCompany>> _cargoCache = new();
        private static readonly ConcurrentDictionary<int, List<IdefixVendorProfile>> _profileCache = new();

        public IActionResult Index()
        {
            ViewBag.PazarYeriId = 6; // Idefix-specific ID
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetIdefixCategories(int pazarYeriId, string searchCategoryName = "")
        {
            pazarYeriId = 6; // Idefix-specific ID
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            if (_categoryCache.TryGetValue(pazarYeriId, out var cachedCategories))
            {
                var filteredCategories = string.IsNullOrEmpty(searchCategoryName)
                    ? cachedCategories
                    : cachedCategories.Where(c => c.Name.Contains(searchCategoryName, StringComparison.OrdinalIgnoreCase)).ToList();
                var treeCategories = BuildCategoryTree(filteredCategories);
                return Json(new { success = true, categories = treeCategories });
            }

            string vendorId = girisBilgisi.KullaniciAdi;
            string apiKey = girisBilgisi.ApiKey;
            string apiPass = girisBilgisi.SecretKey;
            string vendorToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:{apiPass}"));
            string url = "https://merchantapi.idefix.com/pim/product-category";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("X-API-KEY", vendorToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var categories = JsonConvert.DeserializeObject<List<IdefixCategory>>(responseJson);
                    var rootCategories = BuildHierarchicalTree(categories);
                    _categoryCache[pazarYeriId] = rootCategories;
                    var filteredCategories = string.IsNullOrEmpty(searchCategoryName)
                        ? rootCategories
                        : rootCategories.Where(c => c.Name.Contains(searchCategoryName, StringComparison.OrdinalIgnoreCase)).ToList();
                    var treeCategories = BuildCategoryTree(filteredCategories);
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
        [HttpGet]
        public async Task<IActionResult> GetIdefixCategoryAttributes(int pazarYeriId, long categoryId)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
            {
                return Json(new { success = false, message = "API kimlik bilgileri bulunamadı." });
            }

            string vendorId = girisBilgisi.KullaniciAdi;
            string apiKey = girisBilgisi.ApiKey;
            string apiPass = girisBilgisi.SecretKey;
            string vendorToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:{apiPass}"));
            string url = $"https://merchantapi.idefix.com/pim/category-attribute/{categoryId}";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", vendorToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var response = await client.GetAsync(url);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    try
                    {
                        if (jsonObject.categoryAttributes == null)
                        {
                            return Json(new { success = false, message = "categoryAttributes bulunamadı." });
                        }

                        var formattedAttributes = ((IEnumerable<dynamic>)jsonObject.categoryAttributes).Select(attr => new
                        {
                            attributeId = (long)attr.attributeId,
                            name = (string)attr.attributeTitle,
                            required = (bool)attr.required,
                            values = ((IEnumerable<dynamic>)attr.attributeValues).Select(v => new
                            {
                                attributeValueId = (long)v.id,
                                value = (string)v.name
                            }).ToList()
                        }).ToList();

                        return Json(new { success = true, data = formattedAttributes });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = "Error processing attributes", error = ex.Message });
                    }
                }

                return Json(new { success = false, message = "API request failed", statusCode = response.StatusCode });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Request exception", error = ex.Message });
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetIdefixBrands(int pazarYeriId)
        {
            pazarYeriId = 6; // Idefix-specific ID
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            if (_brandCache.TryGetValue(pazarYeriId, out var cachedBrands))
            {
                var brandList = cachedBrands.Select(b => new { id = b.Id, title = b.Title }).OrderBy(b => b.title).ToList();
                return Json(new { success = true, brands = brandList });
            }

            string vendorId = girisBilgisi.KullaniciAdi;
            string apiKey = girisBilgisi.ApiKey;
            string apiPass = girisBilgisi.SecretKey;
            string vendorToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:{apiPass}"));
            string url = "https://merchantapi.idefix.com/pim/brand?page=1&size=1000";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("X-API-KEY", vendorToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var brands = JsonConvert.DeserializeObject<List<IdefixBrand>>(responseJson);
                    _brandCache[pazarYeriId] = brands;
                    var brandList = brands.Select(b => new { id = b.Id, title = b.Title }).OrderBy(b => b.title).ToList();
                    return Json(new { success = true, brands = brandList });
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

        [HttpGet]
        public async Task<IActionResult> GetIdefixCargoCompanies(int pazarYeriId)
        {
            pazarYeriId = 6; // Idefix-specific ID
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            if (_cargoCache.TryGetValue(pazarYeriId, out var cachedCargoCompanies))
            {
                var cargoList = cachedCargoCompanies.Select(c => new { id = c.Id, title = c.Title }).OrderBy(c => c.title).ToList();
                return Json(new { success = true, cargoCompanies = cargoList });
            }

            string vendorId = girisBilgisi.KullaniciAdi;
            string apiKey = girisBilgisi.ApiKey;
            string apiPass = girisBilgisi.SecretKey;
            string vendorToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:{apiPass}"));
            string url = "https://merchantapi.idefix.com/pim/cargo-company";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("X-API-KEY", vendorToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var cargoCompanies = JsonConvert.DeserializeObject<List<IdefixCargoCompany>>(responseJson);
                    _cargoCache[pazarYeriId] = cargoCompanies;
                    var cargoList = cargoCompanies.Select(c => new { id = c.Id, title = c.Title }).OrderBy(c => c.title).ToList();
                    return Json(new { success = true, cargoCompanies = cargoList });
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

        [HttpGet]
        public async Task<IActionResult> GetIdefixVendorProfiles(int pazarYeriId)
        {
            pazarYeriId = 6; // Idefix-specific ID
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            if (_profileCache.TryGetValue(pazarYeriId, out var cachedProfiles))
            {
                var profileList = cachedProfiles.Select(p => new { id = p.Id, title = p.CargoCompany.Title }).OrderBy(p => p.title).ToList();
                return Json(new { success = true, vendorProfiles = profileList });
            }

            string vendorId = girisBilgisi.KullaniciAdi;
            string apiKey = girisBilgisi.ApiKey;
            string apiPass = girisBilgisi.SecretKey;
            string vendorToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:{apiPass}"));
            string url = $"https://merchantapi.idefix.com/pim/cargo-company/{vendorId}/profile/list";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("X-API-KEY", vendorToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var profiles = JsonConvert.DeserializeObject<List<IdefixVendorProfile>>(responseJson);
                    _profileCache[pazarYeriId] = profiles;
                    var profileList = profiles.Select(p => new { id = p.Id, title = p.CargoCompany.Title }).OrderBy(p => p.title).ToList();
                    return Json(new { success = true, vendorProfiles = profileList });
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

        private List<IdefixCategory> BuildHierarchicalTree(List<IdefixCategory> flatCategories)
        {
            var categoryMap = flatCategories.ToDictionary(c => c.Id, c => c);
            var rootCategories = new List<IdefixCategory>();

            foreach (var category in flatCategories)
            {
                if (category.ParentId == 0 || !categoryMap.ContainsKey(category.ParentId))
                {
                    rootCategories.Add(category);
                }
                else
                {
                    var parent = categoryMap[category.ParentId];
                    parent.Subs = parent.Subs ?? new List<IdefixCategory>();
                    parent.Subs.Add(category);
                }
            }

            return rootCategories;
        }

        private List<object> BuildCategoryTree(List<IdefixCategory> categories)
        {
            var result = new List<object>();
            foreach (var category in categories)
            {
                var node = new
                {
                    id = category.Id,
                    name = category.Name,
                    fullPath = GetCategoryPath(category, new List<IdefixCategory>()),
                    isLeaf = category.Subs == null || category.Subs.Count == 0,
                    subCategories = category.Subs != null && category.Subs.Any()
                        ? BuildCategoryTree(category.Subs)
                        : null
                };
                result.Add(node);
            }
            return result;
        }

        private string GetCategoryPath(IdefixCategory category, List<IdefixCategory> path)
        {
            path.Insert(0, category);
            if (category.ParentId != 0 && _categoryCache.TryGetValue(6, out var allCategories))
            {
                var parent = allCategories.FirstOrDefault(c => c.Id == category.ParentId);
                if (parent != null)
                {
                    return GetCategoryPath(parent, path);
                }
            }
            return string.Join(" > ", path.Select(c => c.Name));
        }

        [HttpPost]
        public async Task<IActionResult> AddIdefixProduct(int pazarYeriId, [FromForm] UnifiedMarketplaceModel request)
        {
            pazarYeriId = 6; // Idefix-specific ID
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "Pazar yeri bilgileri bulunamadı." });

            string vendorId = girisBilgisi.KullaniciAdi;
            string apiKey = girisBilgisi.ApiKey;
            string apiPass = girisBilgisi.SecretKey;
            string vendorToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:{apiPass}"));
            string url = $"https://merchantapi.idefix.com/pim/pool/{vendorId}/create";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("X-API-KEY", vendorToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Process images from IFormFile to URLs (assuming they are uploaded and converted to URLs)
            List<string> imageUrls = new List<string>();
            if (request.ImgFiles != null && request.ImgFiles.Any())
            {
                // Placeholder for image upload logic (e.g., save to server or cloud and get URLs)
                // For this example, assume URLs are provided or handled elsewhere
                imageUrls = request.ImgFiles.Select((file, index) => $"https://example.com/images/{file.FileName}").ToList();
            }

            var product = new
            {
                barcode = request.Barcode,
                title = request.Title,
                productMainId = request.Idefix.ProductMainId,
                brandId = request.Idefix.BrandId,
                categoryId = request.Idefix.CategoryId,
                inventoryQuantity = request.Idefix.StockQuantity,
                vendorStockCode = request.Idefix.VendorStockCode,
                weight = request.Idefix.Weight ?? 0,
                description = request.Description,
                price = request.Idefix.SalePrice,
                comparePrice = request.Idefix.ComparePrice,
                vatRate = request.VatRate,
                deliveryDuration = request.Idefix.DeliveryDuration,
                deliveryType ="regular",
                cargoCompanyId = request.Idefix.CargoCompanyId,
                shipmentAddressId = request.Idefix.ShipmentAddressId,
                returnAddressId = request.Idefix.ReturnAddressId,
                images = imageUrls?.Select((url, index) => new { url, order = index + 1 }).ToArray(),
                originCountryId = "request.Idefix.OriginCountryId",
                manufacturer ="request.Idefix.Manufacturer",
                importer = "request.Idefix.Importer",
                ceCompatibility = "request.Idefix.CeCompatibility",

            };

            var requestData = new
            {
                products = new[] { product }
            };

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    dynamic json = JsonConvert.DeserializeObject(responseContent);
                    return Json(new { success = true, message = "Ürün başarıyla eklendi.", batchRequestId = json.batchRequestId?.ToString() });
                }
                else
                {
                    return Json(new { success = false, message = $"Idefix API Error: {response.StatusCode} - {responseContent}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }
    }
}