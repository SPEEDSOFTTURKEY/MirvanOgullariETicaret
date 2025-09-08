using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Controllers
{
    public class AdminFarmazonController : Controller
    {
        private readonly PazarYeriGirisBilgileriRepository _girisBilgileriRepository = new PazarYeriGirisBilgileriRepository();

        public async Task<IActionResult> Index(int pazarYeriId)
        {
            pazarYeriId = 10; // Sabit ID for Farmazon
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            var products = new List<FarmazonProductRequest>();
            if (girisBilgisi != null)
            {
                products = await GetFarmazonProducts(girisBilgisi);
            }
            ViewBag.Products = products;
            ViewBag.PazarYeriId = pazarYeriId;
            ViewBag.ApiType = "farmazon";
            return View();
        }

        [HttpPost]
        public IActionResult Sil(string productId, int pazarYeriId)
        {
            return DeleteFarmazonProduct(productId, pazarYeriId).Result;
        }


        private async Task<string> UploadImage(IFormFile file)
        {
            // Implement image upload logic (e.g., to cloud storage or local file system)
            // This is a placeholder; replace with actual implementation
            return file?.FileName;
        }

        private async Task<List<FarmazonProductRequest>> GetFarmazonProducts(PazarYeriGirisBilgileri girisBilgisi)
        {
            string baseUrl = "https://staging.lab.farmazon.com.tr/api";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "API_alvit");

            try
            {
                // Token alma
                var loginData = new Dictionary<string, string>
                {
                    { "username", girisBilgisi.KullaniciAdi },
                    { "password", girisBilgisi.Sifre },
                    { "clientName", girisBilgisi.ApiKey },
                    { "clientSecretKey", girisBilgisi.SecretKey }
                };

                var content = new FormUrlEncodedContent(loginData);
                var loginResponse = await client.PostAsync($"{baseUrl}/v1/account/signin", content);
                if (!loginResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Farmazon Login Error: {loginResponse.StatusCode}");
                    return new List<FarmazonProductRequest>();
                }

                var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
                var loginResult = JsonConvert.DeserializeObject<FarmazonLoginResponse>(loginResponseContent);
                if (loginResult.Result?.Token == null)
                {
                    Console.WriteLine("Farmazon Token alınamadı.");
                    return new List<FarmazonProductRequest>();
                }

                // Ürünleri çekme
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Result.Token);
                var listingsResponse = await client.GetAsync($"{baseUrl}/v2/listings/getlistings");
                if (listingsResponse.IsSuccessStatusCode)
                {
                    var listingsResponseContent = await listingsResponse.Content.ReadAsStringAsync();
                    var listingsResult = JsonConvert.DeserializeObject<FarmazonProductListResponse>(listingsResponseContent);
                    return listingsResult.Result?.Items ?? new List<FarmazonProductRequest>();
                }
                else
                {
                    Console.WriteLine($"Farmazon Listings Error: {listingsResponse.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Farmazon Error: {ex.Message}");
            }
            return new List<FarmazonProductRequest>();
        }

        [HttpPost]
        public async Task<IActionResult> AddFarmazonProduct(FarmazonProductRequest request, int pazarYeriId)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API kimlik bilgileri bulunamadı." });

            string baseUrl = "https://staging.lab.farmazon.com.tr/api";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "API_alvit");

            try
            {
                // Token alma
                var loginData = new Dictionary<string, string>
                {
                    { "username", girisBilgisi.KullaniciAdi },
                    { "password", girisBilgisi.Sifre },
                    { "clientName", girisBilgisi.ApiKey },
                    { "clientSecretKey", girisBilgisi.SecretKey }
                };

                var content = new FormUrlEncodedContent(loginData);
                var loginResponse = await client.PostAsync($"{baseUrl}/v1/account/signin", content);
                if (!loginResponse.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = $"Token alınamadı: {loginResponse.StatusCode}" });
                }

                var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
                var loginResult = JsonConvert.DeserializeObject<FarmazonLoginResponse>(loginResponseContent);
                if (loginResult.Result?.Token == null)
                {
                    return Json(new { success = false, message = "Token alınamadı." });
                }

                // Ürün ekleme
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Result.Token);
                var requestList = new List<FarmazonProductRequest> { request };
                var json = JsonConvert.SerializeObject(requestList);
                var productContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{baseUrl}/v2/listings/createlistings", productContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<FarmazonProductResponse>(responseJson);

                    if (result.StatusCode == 207)
                    {
                        var firstResult = result.Result?.FirstOrDefault();
                        if (firstResult != null)
                        {
                            if (firstResult.Success)
                            {
                                return Json(new { success = true, message = "Ürün başarıyla eklendi." });
                            }
                            else
                            {
                                var error = firstResult.Errors?.FirstOrDefault();
                                if (error?.StatusCode == 1550)
                                {
                                    return Json(new { success = false, message = "Aynı miad bilgisine sahip ilan bulunmaktadır. Lütfen ilanı güncelleyiniz." });
                                }
                                return Json(new { success = false, message = error?.Message ?? "Ürün eklenemedi." });
                            }
                        }
                    }
                    return Json(new { success = false, message = "Yanıt işlenemedi." });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Hata: {(int)response.StatusCode} - {errorContent}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFarmazonProduct([FromBody] FarmazonProductRequest request, int pazarYeriId)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API kimlik bilgileri bulunamadı." });

            string baseUrl = "https://staging.lab.farmazon.com.tr/api";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "API_alvit");

            try
            {
                // Token alma
                var loginData = new Dictionary<string, string>
                {
                    { "username", girisBilgisi.KullaniciAdi },
                    { "password", girisBilgisi.Sifre },
                    { "clientName", girisBilgisi.ApiKey },
                    { "clientSecretKey", girisBilgisi.SecretKey }
                };

                var content = new FormUrlEncodedContent(loginData);
                var loginResponse = await client.PostAsync($"{baseUrl}/v1/account/signin", content);
                if (!loginResponse.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = $"Token alınamadı: {loginResponse.StatusCode}" });
                }

                var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
                var loginResult = JsonConvert.DeserializeObject<FarmazonLoginResponse>(loginResponseContent);
                if (loginResult.Result?.Token == null)
                {
                    return Json(new { success = false, message = "Token alınamadı." });
                }

                // Ürün güncelleme
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Result.Token);
                var requestList = new List<FarmazonProductRequest> { request };
                var json = JsonConvert.SerializeObject(requestList);
                var productContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"{baseUrl}/v2/listings/updatelistings", productContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<FarmazonProductResponse>(responseJson);
                    var firstResult = result.Result?.FirstOrDefault();
                    if (firstResult != null)
                    {
                        if (firstResult.Success)
                        {
                            return Json(new { success = true, message = "Ürün başarıyla güncellendi." });
                        }
                        else
                        {
                            var error = firstResult.Errors?.FirstOrDefault();
                            return Json(new { success = false, message = error?.Message ?? "Ürün güncellenemedi." });
                        }
                    }
                    return Json(new { success = false, message = "Yanıt işlenemedi." });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Hata: {(int)response.StatusCode} - {errorContent}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFarmazonPriceAndStock([FromBody] FarmazonPriceStockRequest request, int pazarYeriId)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API kimlik bilgileri bulunamadı." });

            string baseUrl = "https://staging.lab.farmazon.com.tr/api";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "API_alvit");

            try
            {
                // Token alma
                var loginData = new Dictionary<string, string>
                {
                    { "username", girisBilgisi.KullaniciAdi },
                    { "password", girisBilgisi.Sifre },
                    { "clientName", girisBilgisi.ApiKey },
                    { "clientSecretKey", girisBilgisi.SecretKey }
                };

                var content = new FormUrlEncodedContent(loginData);
                var loginResponse = await client.PostAsync($"{baseUrl}/v1/account/signin", content);
                if (!loginResponse.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = $"Token alınamadı: {loginResponse.StatusCode}" });
                }

                var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
                var loginResult = JsonConvert.DeserializeObject<FarmazonLoginResponse>(loginResponseContent);
                if (loginResult.Result?.Token == null)
                {
                    return Json(new { success = false, message = "Token alınamadı." });
                }

                // Fiyat ve stok güncelleme
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Result.Token);
                var requestList = new List<FarmazonPriceStockRequest> { request };
                var json = JsonConvert.SerializeObject(requestList);
                var productContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"{baseUrl}/v2/listings/UpdateListingsPriceAndStockOnly", productContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<FarmazonPriceStockResponse>(responseJson);
                    return Json(new { success = result.StatusCode == 0, message = result.StatusMessage ?? "Fiyat ve stok güncellendi." });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Hata: {(int)response.StatusCode} - {errorContent}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFarmazonProduct(string productId, int pazarYeriId)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API kimlik bilgileri bulunamadı." });

            string baseUrl = "https://staging.lab.farmazon.com.tr/api";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "API_alvit");

            try
            {
                // Token alma
                var loginData = new Dictionary<string, string>
                {
                    { "username", girisBilgisi.KullaniciAdi },
                    { "password", girisBilgisi.Sifre },
                    { "clientName", girisBilgisi.ApiKey },
                    { "clientSecretKey", girisBilgisi.SecretKey }
                };

                var content = new FormUrlEncodedContent(loginData);
                var loginResponse = await client.PostAsync($"{baseUrl}/v1/account/signin", content);
                if (!loginResponse.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = $"Token alınamadı: {loginResponse.StatusCode}" });
                }

                var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
                var loginResult = JsonConvert.DeserializeObject<FarmazonLoginResponse>(loginResponseContent);

                if (loginResult.Result?.Token == null)
                {
                    return Json(new { success = false, message = "Token alınamadı." });
                }

                // Ürün silme
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Result.Token);
                var response = await client.DeleteAsync($"{baseUrl}/v2/listings/delete/{productId}");

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<FarmazonProductResponse>(responseJson);
                    return Json(new { success = result.Result?.FirstOrDefault()?.Success ?? false, message = result.Result?.FirstOrDefault()?.Errors?.FirstOrDefault()?.Message ?? "Ürün silindi." });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Hata: {(int)response.StatusCode} - {errorContent}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }
    }
}