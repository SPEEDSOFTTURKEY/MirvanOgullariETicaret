using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Collections.Concurrent;

namespace WebApp.Controllers
{
    public class AdminN11Controller : Controller
    {
        private readonly PazarYeriGirisBilgileriRepository _girisBilgileriRepository = new PazarYeriGirisBilgileriRepository();
        private readonly PazarYerleriRepository _pazarYerleriRepository = new PazarYerleriRepository();

        public async Task<IActionResult> Index()
        {
            int? pazarYeriId = 3;
            var pazarYerleris = _pazarYerleriRepository.GetirList(x => x.Durumu == 1);
            ViewBag.Pazaryerleri = pazarYerleris;
            ViewBag.SelectedPazarYeriId = pazarYeriId;

            var products = new List<object>();
            if (pazarYeriId.HasValue)
            {
                var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId.Value).FirstOrDefault();
                if (girisBilgisi != null)
                {
                    var pazarYeri = pazarYerleris.FirstOrDefault(p => p.Id == pazarYeriId.Value);
                    if (pazarYeri != null && pazarYeri.Adi.ToLower().Contains("n11"))
                    {
                        products = await GetN11Products(girisBilgisi);
                    }
                }
            }

            ViewBag.Products = products;
            return View();
        }

        [HttpPost]
        public IActionResult Sil(long productId, int pazarYeriId)
        {
            return DeleteN11Product(productId, pazarYeriId).Result;
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

        [HttpPost]
        public async Task<IActionResult> UpdateN11Product([FromBody] N11ProductRequest request, int pazarYeriId)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            if (string.IsNullOrEmpty(request.stockCode))
                return Json(new { success = false, message = "Stock code is required for updating a product." });

            string apiUrl = "https://api.n11.com/ms/product/tasks/product-update";
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
                                stockCode = request.stockCode,
                                status = request.status,
                                preparingDay = request.preparingDay,
                                shipmentTemplate = request.shipmentTemplate,
                                currencyType = request.currencyType,
                                deleteProductMainId = request.deleteProductMainId,
                                productMainId = request.productMainId,
                                deleteMaxPurchaseQuantity = request.deleteMaxPurchaseQuantity,
                                maxPurchaseQuantity = request.maxPurchaseQuantity,
                                description = request.description,
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
                        return Json(new { success = true, message = "Ürün güncelleme başarıyla işleme alındı. Task ID: " + result.id });
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

        [HttpPost]
        public async Task<IActionResult> DeleteN11Product(long productId, int pazarYeriId)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            string apiUrl = "https://api.n11.com/ms/product/tasks/product-delete";
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
                        productIds = new[] { productId }
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
                        return Json(new { success = true, message = "Ürün silme başarıyla işleme alındı. Task ID: " + result.id });
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