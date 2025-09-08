using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Controllers
{
    public class AdminGoTurcController : Controller
    {
        private readonly PazarYeriGirisBilgileriRepository _girisBilgileriRepository = new PazarYeriGirisBilgileriRepository();

        public async Task<IActionResult> Index(int pazarYeriId = 8)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            var products = new List<GoTurcProduct>();
            string errorMessage = null;

            if (girisBilgisi != null)
            {
                try
                {
                    products = await GetGoTurcProducts(girisBilgisi);
                    if (!products.Any())
                    {
                        errorMessage = "No products found for GoTurc.";
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = $"Error fetching GoTurc products: {ex.Message}";
                }
            }
            else
            {
                errorMessage = "GoTurc API credentials not found.";
            }

            ViewBag.Products = products;
            ViewBag.PazarYeriId = pazarYeriId;
            ViewBag.ApiType = "goturc";
            ViewBag.ErrorMessage = errorMessage;
            return View();
        }

        private async Task<List<GoTurcProduct>> GetGoTurcProducts(PazarYeriGirisBilgileri girisBilgisi)
        {
            string baseUrl = "https://gowa.goturc.com/";
            using var client = new HttpClient();

            try
            {
                var tokenUrl = $"{baseUrl}api/Account/Token?apiId={girisBilgisi.ApiKey}&apiSecret={girisBilgisi.SecretKey}";
                var tokenResponse = await client.GetAsync(tokenUrl);
                tokenResponse.EnsureSuccessStatusCode();
                var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
                dynamic tokenResult = JsonConvert.DeserializeObject(tokenJson);
                string token = tokenResult?.token?.ToString();

                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Token is null or empty.");
                    return new List<GoTurcProduct>();
                }

                var productUrl = $"{baseUrl}api/Product/List?page=1&size=10";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var productResponse = await client.GetAsync(productUrl);
                productResponse.EnsureSuccessStatusCode();
                var productJson = await productResponse.Content.ReadAsStringAsync();
                var productData = JsonConvert.DeserializeObject<GoTurcProductListResponse>(productJson);

                return productData?.Products ?? new List<GoTurcProduct>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching products: {ex.Message}");
                throw; // Re-throw to be caught in Index action
            }
        }

        private async Task<List<GoTurcCategory>> GetGoTurcCategories(PazarYeriGirisBilgileri girisBilgisi, long? categoryId = null, string searchCategoryName = null)
        {
            string baseUrl = "https://gowa.goturc.com/";
            using var client = new HttpClient();

            try
            {
                var tokenUrl = $"{baseUrl}api/Account/Token?apiId={girisBilgisi.ApiKey}&apiSecret={girisBilgisi.SecretKey}";
                var tokenResponse = await client.GetAsync(tokenUrl);
                tokenResponse.EnsureSuccessStatusCode();
                var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
                dynamic tokenResult = JsonConvert.DeserializeObject(tokenJson);
                string token = tokenResult?.token?.ToString();

                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Token is null or empty.");
                    return new List<GoTurcCategory>();
                }

                var categoryUrl = $"{baseUrl}api/category/list";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var requestBody = new Dictionary<string, object>();
                if (categoryId.HasValue)
                {
                    requestBody["categoryId"] = categoryId.Value;
                }
                if (!string.IsNullOrEmpty(searchCategoryName))
                {
                    requestBody["searchCategoryName"] = searchCategoryName; // Arama terimini ekle
                }

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                var categoryResponse = await client.PostAsync(categoryUrl, content);
                categoryResponse.EnsureSuccessStatusCode();
                var categoryJson = await categoryResponse.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<GoTurcCategory>>(categoryJson);

                // Filter to only include leaf categories (no subcategories)
                var leafCategories = new List<GoTurcCategory>();
                foreach (var category in categories)
                {
                    if (category.SubCategory == null || category.SubCategory.Count == 0)
                    {
                        leafCategories.Add(category);
                    }
                    else
                    {
                        leafCategories.AddRange(await GetGoTurcCategories(girisBilgisi, category.Id, searchCategoryName));
                    }
                }

                return leafCategories;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching categories: {ex.Message}");
                return new List<GoTurcCategory>();
            }
        }

        private async Task<List<GoTurcBrand>> GetGoTurcBrands(PazarYeriGirisBilgileri girisBilgisi, string searchBrandName = null)
        {
            string baseUrl = "https://gowa.goturc.com/";
            using var client = new HttpClient();

            try
            {
                var tokenUrl = $"{baseUrl}api/Account/Token?apiId={girisBilgisi.ApiKey}&apiSecret={girisBilgisi.SecretKey}";
                var tokenResponse = await client.GetAsync(tokenUrl);
                tokenResponse.EnsureSuccessStatusCode();
                var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
                dynamic tokenResult = JsonConvert.DeserializeObject(tokenJson);
                string token = tokenResult?.token?.ToString();

                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Token is null or empty.");
                    return new List<GoTurcBrand>();
                }

                var brandUrl = $"{baseUrl}api/Product/Brand/List?page=1&size=50";
                if (!string.IsNullOrEmpty(searchBrandName))
                {
                    brandUrl += $"&searchBrandName={Uri.EscapeDataString(searchBrandName)}";
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var brandResponse = await client.GetAsync(brandUrl);
                brandResponse.EnsureSuccessStatusCode();
                var brandJson = await brandResponse.Content.ReadAsStringAsync();
                var brandData = JsonConvert.DeserializeObject<GoTurcBrandListResponse>(brandJson);

                return brandData?.Brands ?? new List<GoTurcBrand>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching brands: {ex.Message}");
                return new List<GoTurcBrand>();
            }
        }

        private async Task<List<GoTurcDeliveryMethod>> GetGoTurcDeliveryMethods(PazarYeriGirisBilgileri girisBilgisi)
        {
            string baseUrl = "https://gowa.goturc.com/";
            using var client = new HttpClient();

            try
            {
                var tokenUrl = $"{baseUrl}api/Account/Token?apiId={girisBilgisi.ApiKey}&apiSecret={girisBilgisi.SecretKey}";
                var tokenResponse = await client.GetAsync(tokenUrl);
                tokenResponse.EnsureSuccessStatusCode();
                var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
                dynamic tokenResult = JsonConvert.DeserializeObject(tokenJson);
                string token = tokenResult?.token?.ToString();

                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Token is null or empty.");
                    return new List<GoTurcDeliveryMethod>();
                }

                var deliveryMethodUrl = $"{baseUrl}api/Order/DeliveryMethod";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var deliveryMethodResponse = await client.GetAsync(deliveryMethodUrl);
                deliveryMethodResponse.EnsureSuccessStatusCode();
                var deliveryMethodJson = await deliveryMethodResponse.Content.ReadAsStringAsync();
                var deliveryMethods = JsonConvert.DeserializeObject<List<GoTurcDeliveryMethod>>(deliveryMethodJson);

                return deliveryMethods ?? new List<GoTurcDeliveryMethod>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching delivery methods: {ex.Message}");
                return new List<GoTurcDeliveryMethod>();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetGoTurcCategories(int pazarYeriId = 8, string searchCategoryName = null)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            var categories = await GetGoTurcCategories(girisBilgisi, searchCategoryName: searchCategoryName);
            return Json(new { success = true, categories });
        }
        [HttpGet]
        public async Task<IActionResult> GetGoTurcBrands(int pazarYeriId = 8, string searchBrandName = null)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            var brands = await GetGoTurcBrands(girisBilgisi, searchBrandName);
            return Json(new { success = true, brands });
        }

        [HttpGet]
        public async Task<IActionResult> GetGoTurcDeliveryMethods(int pazarYeriId = 8)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            var deliveryMethods = await GetGoTurcDeliveryMethods(girisBilgisi);
            return Json(new { success = true, deliveryMethods });
        }

        [HttpPost]
        public async Task<IActionResult> AddGoTurcProduct([FromBody] GoTurcProductRequest request, int pazarYeriId = 8)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            // Validate request
            if (string.IsNullOrEmpty(request.Title) || request.Title.Length < 5 || request.Title.Length > 65)
                return Json(new { success = false, message = "Product title must be between 5 and 65 characters." });
            if (!string.IsNullOrEmpty(request.SubTitle) && request.SubTitle.Length > 65)
                return Json(new { success = false, message = "Product subtitle must not exceed 65 characters." });
            if (request.DeliveryNo <= 0)
                return Json(new { success = false, message = "Valid delivery number is required." });

            string baseUrl = "https://gowa.goturc.com/";
            using var client = new HttpClient();

            try
            {
                var tokenUrl = $"{baseUrl}api/Account/Token?apiId={girisBilgisi.ApiKey}&apiSecret={girisBilgisi.SecretKey}";
                var tokenResponse = await client.GetAsync(tokenUrl);
                tokenResponse.EnsureSuccessStatusCode();
                var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
                dynamic tokenResult = JsonConvert.DeserializeObject(tokenJson);
                string token = tokenResult?.token?.ToString();

                if (string.IsNullOrEmpty(token))
                    return Json(new { success = false, message = "Failed to obtain token." });

                var apiUrl = $"{baseUrl}api/Product";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var apiRequest = new List<GoTurcProductRequest> { request };
                var json = JsonConvert.SerializeObject(apiRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<GoTurcProductResponse>(responseJson);
                    return Json(new { success = true, message = $"Product added/updated successfully. Report ID: {result.ReportId}" });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"API error: {(int)response.StatusCode} - {errorContent}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGoTurcProduct(string shopProductCode, int pazarYeriId = 8)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            string baseUrl = "https://gowa.goturc.com/";
            using var client = new HttpClient();

            try
            {
                var tokenUrl = $"{baseUrl}api/Account/Token?apiId={girisBilgisi.ApiKey}&apiSecret={girisBilgisi.SecretKey}";
                var tokenResponse = await client.GetAsync(tokenUrl);
                tokenResponse.EnsureSuccessStatusCode();
                var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
                dynamic tokenResult = JsonConvert.DeserializeObject(tokenJson);
                string token = tokenResult?.token?.ToString();

                if (string.IsNullOrEmpty(token))
                    return Json(new { success = false, message = "Failed to obtain token." });

                var apiUrl = $"{baseUrl}api/Product/Delete?shopProductCode={shopProductCode}";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<GoTurcProductResponse>(responseJson);
                    return Json(new { success = true, message = $"Product deleted successfully. Report ID: {result.ReportId}" });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"API error: {(int)response.StatusCode} - {errorContent}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}