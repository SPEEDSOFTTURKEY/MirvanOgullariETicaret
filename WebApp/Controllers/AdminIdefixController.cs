using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebApp.Controllers
{
    public class IdefixProduct
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "";
        public string Barcode { get; set; } = "";
        public string? VendorStockCode { get; set; }
        public string? Status { get; set; }
        public decimal Price { get; set; }
        public decimal? ComparePrice { get; set; }
        public int Stock { get; set; }
        public int? DeliveryDuration { get; set; }
        public string? DeliveryType { get; set; }
    }

    public class AdminIdefixController : Controller
    {
        private readonly PazarYeriGirisBilgileriRepository _girisBilgileriRepository = new PazarYeriGirisBilgileriRepository();

        public async Task<IActionResult> Index(int pazarYeriId)
        {
            pazarYeriId = 6; // Idefix-specific ID
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            var products = new List<object>();
            if (girisBilgisi != null)
            {
                products = await GetIdefixProducts(girisBilgisi);
            }
            ViewBag.Products = products;
            ViewBag.PazarYeriId = pazarYeriId;
            ViewBag.ApiType = "idefix";
            return View();
        }

        private async Task<List<object>> GetIdefixProducts(PazarYeriGirisBilgileri girisBilgisi)
        {
            string vendorId = girisBilgisi.KullaniciAdi;
            string apiKey = girisBilgisi.ApiKey;
            string apiPass = girisBilgisi.SecretKey;
            string vendorToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:{apiPass}"));
            string url = $"https://merchantapi.idefix.com/pim/pool/{vendorId}/list?page=1&limit=10";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("X-API-KEY", vendorToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var response = await client.GetAsync(url);
                string content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Response: " + content);

                if (response.IsSuccessStatusCode)
                {
                    dynamic json = JsonConvert.DeserializeObject(content);
                    var products = new List<object>();
                    if (json.products != null)
                    {
                        foreach (var p in json.products)
                        {
                            products.Add(new IdefixProduct
                            {
                                Id = p.id?.ToString() ?? Guid.NewGuid().ToString(),
                                Title = p.title?.ToString() ?? "Bilinmeyen Ürün",
                                Barcode = p.barcode?.ToString() ?? "",
                                Status = p.status?.ToString() ?? "unknown",
                                Price = p.price != null ? Convert.ToDecimal(p.price) : 0,
                                Stock = p.stock != null ? Convert.ToInt32(p.stock) : 0,
                                VendorStockCode = p.vendorStockCode?.ToString(),
                                ComparePrice = p.comparePrice != null ? Convert.ToDecimal(p.comparePrice) : null,
                                DeliveryDuration = p.deliveryDuration != null ? Convert.ToInt32(p.deliveryDuration) : null,
                                DeliveryType = p.deliveryType?.ToString()
                            });
                        }
                    }
                    Console.WriteLine("Products: " + JsonConvert.SerializeObject(products));
                    return products;
                }
                else
                {
                    Console.WriteLine($"Idefix API Error: {response.StatusCode} - {content}");
                    return new List<object>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Idefix Error: {ex.Message}");
                return new List<object>();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateIdefixProduct(int pazarYeriId, [FromBody] IdefixProduct product)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
            {
                return Json(new { success = false, message = "Pazar yeri bilgileri bulunamadı." });
            }

            string vendorId = girisBilgisi.KullaniciAdi;
            string apiKey = girisBilgisi.ApiKey;
            string apiPass = girisBilgisi.SecretKey;
            string vendorToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:{apiPass}"));
            string url = $"https://merchantapi.idefix.com/pim/catalog/{vendorId}/inventory-upload";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("X-API-KEY", vendorToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var requestData = new
            {
                items = new[]
                {
                    new
                    {
                        barcode = product.Barcode,
                        price = product.Price,
                        comparePrice = product.ComparePrice,
                        inventoryQuantity = product.Stock,
                        deliveryDuration = product.DeliveryDuration,
                        deliveryType = product.DeliveryType
                    }
                }
            };

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    dynamic json = JsonConvert.DeserializeObject(responseContent);
                    return Json(new { success = true, message = "Ürün başarıyla güncellendi.", batchRequestId = json.batchRequestId?.ToString() });
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