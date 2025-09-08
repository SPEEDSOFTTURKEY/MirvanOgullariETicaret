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
    public class AdminCicekSepetiController : Controller
    {
        private readonly PazarYeriGirisBilgileriRepository _girisBilgileriRepository = new PazarYeriGirisBilgileriRepository();

        public async Task<IActionResult> Index(int pazarYeriId)
        {
            pazarYeriId = 5;
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            var products = new List<object>();
            if (girisBilgisi != null)
            {
                products = (await GetCicekSepetiProducts(girisBilgisi)).Cast<object>().ToList();
            }
            ViewBag.Products = products;
            ViewBag.PazarYeriId = pazarYeriId;
            ViewBag.ApiType = "cicek sepeti";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Sil(string productId, int pazarYeriId)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            string apiUrl = $"https://api.ciceksepeti.com/products/{productId}";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-api-key", girisBilgisi.ApiKey);

            try
            {
                var response = await client.DeleteAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Product deleted successfully." });
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

        private async Task<List<ProductCicekSepeti>> GetCicekSepetiProducts(PazarYeriGirisBilgileri girisBilgisi)
        {
            string apiUrl = "https://api.ciceksepeti.com/api/v1/products?page=1&pageSize=50";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-api-key", girisBilgisi.ApiKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<CicekSepetiDataResponse>(jsonResponse);
                    return apiResponse?.ProductCicekSepeti ?? new List<ProductCicekSepeti>();
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {(int)response.StatusCode} - {response.ReasonPhrase}");
                    Console.WriteLine($"Error Details: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Çiçek Sepeti Error: {ex.ToString()}");
            }
            return new List<ProductCicekSepeti>();
        }

        [HttpPost]
        public async Task<IActionResult> AddCicekSepetiProduct([FromBody] ProductCicekSepeti request, int pazarYeriId)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            string apiUrl = "https://api.ciceksepeti.com/api/v1/products";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-api-key", girisBilgisi.ApiKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(responseJson);
                    return Json(new { success = true, message = "Product added successfully." });
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
        public async Task<IActionResult> UpdateCicekSepetiProduct([FromBody] ProductCicekSepeti request, int pazarYeriId)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            string apiUrl = $"https://api.ciceksepeti.com/api/v1/products/{request.ProductCode}";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-api-key", girisBilgisi.ApiKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Product updated successfully." });
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

        public async Task<IActionResult> GetCategories(int pazarYeriId)
        {
            pazarYeriId = 5; // Çiçek Sepeti PazarYeriId
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            var categories = await GetCicekSepetiCategories(girisBilgisi);
            return Json(new { success = true, categories = categories });
        }

        private async Task<List<CicekSepetiCategory>> GetCicekSepetiCategories(PazarYeriGirisBilgileri girisBilgisi)
        {
            string apiUrl = "https://api.ciceksepeti.com/api/v1/Categories";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-api-key", girisBilgisi.ApiKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<CicekSepetiCategoryResponse>(jsonResponse);
                    return apiResponse?.Categories ?? new List<CicekSepetiCategory>();
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {(int)response.StatusCode} - {response.ReasonPhrase}");
                    Console.WriteLine($"Error Details: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Çiçek Sepeti Categories Error: {ex.ToString()}");
            }
            return new List<CicekSepetiCategory>();
        }
    }
}