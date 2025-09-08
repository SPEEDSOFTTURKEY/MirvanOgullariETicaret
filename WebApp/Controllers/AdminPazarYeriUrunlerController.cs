using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;
using System.Text;
using Newtonsoft.Json;
using System.ServiceModel;
using System.Net.Http.Headers;
using System.Collections.Concurrent;


namespace WebApp.Controllers
{
    public class AdminPazarYeriUrunlerController : Controller
    {
        private readonly PazarYerleriRepository _pazarYerleriRepository = new PazarYerleriRepository();
        private readonly PazarYeriGirisBilgileriRepository _girisBilgileriRepository = new PazarYeriGirisBilgileriRepository();
        private static readonly ConcurrentDictionary<int, List<N11Category>> _categoryCache = new ConcurrentDictionary<int, List<N11Category>>();

        public async Task<IActionResult> Index(int? pazarYeriId)
        {
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
                    if (pazarYeri != null)
                    {
                        if (pazarYeri.Adi.ToLower().Contains("n11"))
                            products = await GetN11Products(girisBilgisi);
                        else if (pazarYeri.Adi.ToLower().Contains("farma borsa"))
                            products = await GetFarmaborsaProducts(girisBilgisi);
                        else if (pazarYeri.Adi.ToLower().Contains("farmazon"))
                            products = await GetFarmazonProducts(girisBilgisi);
                        else if (pazarYeri.Adi.ToLower().Contains("ptt"))
                            products = await GetPTTProducts(girisBilgisi);
                        else if (pazarYeri.Adi.ToLower().Contains("çiçek sepeti"))
                            products = await GetCicekSepetiProducts(girisBilgisi);
                        else if (pazarYeri.Adi.ToLower().Contains("ıdefix"))
                            products = await GetIdefixProducts(girisBilgisi);
                        else if (pazarYeri.Adi.ToLower().Contains("trendyol"))
                            products = await GetTrendyolProducts(girisBilgisi);
                        else if (pazarYeri.Adi.ToLower().Contains("goturc"))
                            products = await GetGoTurcProducts(girisBilgisi);
                        else if (pazarYeri.Adi.ToLower().Contains("pazarama"))
                            products = await GetPazaramaProducts(girisBilgisi);
                    }
                }
            }

            ViewBag.Products = products;
            return View();
        }

        [HttpPost]
        public IActionResult Sil(int id, int pazarYeriId, string apiType)
        {
            if (apiType.ToLower().Contains("farma borsa"))
                return (IActionResult)DeleteFarmaBorsaProduct(id, pazarYeriId);
      
            return RedirectToAction("Index", new { pazarYeriId });
        }




        private async Task<List<object>> GetFarmaborsaProducts(PazarYeriGirisBilgileri girisBilgisi)
        {
            string url = "https://wapi.farmaborsa.com/api/Entegre/IlanlarimListesi";
            var input = new InputModel
            {
                nick = girisBilgisi.KullaniciAdi,
                parola = girisBilgisi.Sifre,
                apiKey = girisBilgisi.ApiKey,
                pasif = 0,
                ilan = 0,
                tumIlanlarGelsin = true
            };

            using var client = new HttpClient();
            try
            {
                var json = JsonConvert.SerializeObject(input);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<UrunTalepOutput>(responseJson);
                    if (!result.hata)
                        return result.ilanlarimList.Cast<object>().ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Farmaborsa Error: {ex.Message}");
            }
            return new List<object>();
        }
        [HttpPost]
        public async Task<IActionResult> AddFarmaBorsaProduct([FromBody] FarmaBorsaProductRequest request, int pazarYeriId)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            string apiUrl = " https://wapi.farmaborsa.com/api/Entegre/IlanKaydet";
            using var client = new HttpClient();

            try
            {
                var input = new
                {
                    nick = girisBilgisi?.KullaniciAdi ?? "",
                    parola = girisBilgisi?.Sifre ?? "",
                    apiKey = girisBilgisi?.ApiKey ?? "",
                    urunAd = request?.urunAd ?? "",
                    adet = request?.adet ?? 1,
                    maxAdet = request?.maxAdet ?? 1,
                    op = request?.op ?? 0.0,
                    tutar = request?.tutar ?? 0.0,
                    borsadaGoster = request?.borsadaGoster ?? false,
                    miad = request?.miad ?? "",
                    miadTip = request?.miadTip ?? 0,
                    barkod = request?.barkod ?? "",
                    resimUrl = request?.resimUrl ?? ""
                };

                var json = JsonConvert.SerializeObject(input);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<FarmaBorsaProductResponse>(responseJson);
                    if (!result.hata)
                        return Json(new { success = true, message = result.mesaj });
                    else
                        return Json(new { success = false, message = result.mesaj });
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
        public async Task<IActionResult> UpdateFarmaBorsaProduct([FromBody] FarmaBorsaProductRequest request, int pazarYeriId)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            if (request == null || request.kod == 0)
                return Json(new { success = false, message = "Geçersiz istek veya ürün kodu eksik." });

            string apiUrl = "https://wapi.farmaborsa.com/api/Entegre/IlanHizliDuzenle";
            using var client = new HttpClient();

            try
            {
                var input = new
                {
                    nick = girisBilgisi.KullaniciAdi ?? "",
                    parola = girisBilgisi.Sifre ?? "",
                    apiKey = girisBilgisi.ApiKey ?? "",
                    kod = request.kod,
                    urunAd = request.urunAd ?? "",
                    adet = request.adet,
                    maxAdet = request.maxAdet,
                    op = request.op,
                    tutar = request.tutar,
                    borsadaGoster = request.borsadaGoster,
                    miad = request.miad ?? "",
                    miadTip = request.miadTip,
                    barkod = request.barkod ?? "",
                    resimUrl = request.resimUrl ?? "",
                    urunErp = request.barkod ?? "" 
                };

                var json = JsonConvert.SerializeObject(input);
                Console.WriteLine("Giden JSON: " + json); // loglama

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                var responseJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Yanıtı: " + responseJson); // loglama

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<FarmaBorsaProductResponse>(responseJson);

                    if (result != null && !result.hata)
                        return Json(new { success = true, message = result.mesaj });

                    return Json(new { success = false, message = result?.mesaj ?? "Bilinmeyen hata" });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = $"HTTP Error {(int)response.StatusCode}: {response.ReasonPhrase}",
                        details = responseJson
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"İstisna oluştu: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFarmaBorsaProduct(int kod, int pazarYeriId)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            string apiUrl = "https://wapi.farmaborsa.com/api/Entegre/UrunSil";
            using var client = new HttpClient();

            try
            {
                var input = new
                {
                    nick = girisBilgisi.KullaniciAdi,
                    parola = girisBilgisi.Sifre,
                    apiKey = girisBilgisi.ApiKey,
                    kod = kod
                };

                var json = JsonConvert.SerializeObject(input);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<FarmaBorsaProductResponse>(responseJson);
                    if (!result.hata)
                        return Json(new { success = true, message = result.mesaj });
                    else
                        return Json(new { success = false, message = result.mesaj });
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
        [HttpGet]
        public async Task<IActionResult> GetN11Categories(int pazarYeriId)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            // Check cache first
            if (_categoryCache.TryGetValue(pazarYeriId, out var cachedCategories))
            {
                return Json(new { success = true, categories = cachedCategories });
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

                    // Flatten the category tree to get only leaf categories (subCategories: null)
                    var leafCategories = FlattenCategories(categoryResponse.categories)
                        .Where(c => c.subCategories == null || c.subCategories.Count == 0)
                        .ToList();

                    // Cache the categories
                    _categoryCache[pazarYeriId] = leafCategories;

                    return Json(new { success = true, categories = leafCategories });
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

        // Helper method to flatten category tree
        private List<N11Category> FlattenCategories(List<N11Category> categories)
        {
            var result = new List<N11Category>();
            foreach (var category in categories)
            {
                result.Add(category);
                if (category.subCategories != null && category.subCategories.Any())
                {
                    result.AddRange(FlattenCategories(category.subCategories));
                }
            }
            return result;
        }

        // Existing AddN11Product method (from previous response)
        [HttpPost]
        public async Task<IActionResult> AddN11Product([FromBody] N11ProductRequest request, int pazarYeriId)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

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
                        integrator = "YourIntegratorName", // Replace with your integrator name
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
                                        url = request.imageUrl,
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
                        integrator = "YourIntegratorName", // Replace with your integrator name
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
                        integrator = "YourIntegratorName", // Replace with your integrator name
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
        private async Task<List<object>> GetFarmazonProducts(PazarYeriGirisBilgileri girisBilgisi)
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
                    return new List<object>();
                }

                var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
                var loginResult = JsonConvert.DeserializeObject<FarmazonLoginResponse>(loginResponseContent);
                if (loginResult.Result?.Token == null)
                {
                    Console.WriteLine("Farmazon Token alınamadı.");
                    return new List<object>();
                }

                // Ürünleri çekme
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Result.Token);
                var listingsResponse = await client.GetAsync($"{baseUrl}/v2/listings/getlistings");
                if (listingsResponse.IsSuccessStatusCode)
                {
                    var listingsResponseContent = await listingsResponse.Content.ReadAsStringAsync();
                    var listingsResult = JsonConvert.DeserializeObject<FarmazonListingsResponse>(listingsResponseContent);
                    return listingsResult.Result.Items.Cast<object>().ToList();
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
            return new List<object>();
        }

        private async Task<List<object>> GetPTTProducts(PazarYeriGirisBilgileri girisBilgisi)
        {
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential)
            {
                Security = { Message = { ClientCredentialType = BasicHttpMessageCredentialType.UserName } },
                MaxReceivedMessageSize = 20000000
            };
            var endpoint = new EndpointAddress("https://ws.pttavm.com:93/service.svc");
            var client = new PTTServiceReference.ServiceClient(binding, endpoint);
            client.ClientCredentials.UserName.UserName = girisBilgisi.KullaniciAdi;
            client.ClientCredentials.UserName.Password = girisBilgisi.Sifre;

            try
            {
                var sonuc = await client.TedarikciAltKategoriListesiAsync();
                return new List<object>();
            }
            catch (CommunicationException commEx)
            {
                Console.WriteLine($"Communication error: {commEx.Message}");
                client.Abort();
                return new List<object>();
            }
            catch (TimeoutException timeoutEx)
            {
                Console.WriteLine($"Timeout error: {timeoutEx.Message}");
                client.Abort();
                return new List<object>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                client.Abort();
                return new List<object>();
            }
            finally
            {
                try
                {
                    if (client.State != CommunicationState.Faulted)
                    {
                        await client.CloseAsync();
                    }
                    else
                    {
                        client.Abort();
                    }
                }
                catch
                {
                    client.Abort();
                }
            }
        }

        private async Task<List<object>> GetCicekSepetiProducts(PazarYeriGirisBilgileri girisBilgisi)
        {
            string apiUrl = "https://apis.ciceksepeti.com/api/v1/products?page=1&pageSize=5";
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
                    return apiResponse?.ProductCicekSepeti?.Cast<object>().ToList() ?? new List<object>();
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
            return new List<object>();
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
                                Id = p.id?.ToString(),
                                Title = p.title?.ToString(),
                                Barcode = p.barcode?.ToString(),
                                Status = p.status?.ToString(),
                                Price = p.price != null ? Convert.ToDecimal(p.price) : 0,
                                Stock = p.stock != null ? Convert.ToInt32(p.stock) : 0
                            });
                        }
                    }
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

        private async Task<List<object>> GetTrendyolProducts(PazarYeriGirisBilgileri girisBilgisi)
        {
            string apiKey = girisBilgisi.ApiKey;
            string secretKey = girisBilgisi.SecretKey;
            string sellerId = girisBilgisi.KullaniciAdi;
            string url = $"https://apigw.trendyol.com/integration/product/sellers/{sellerId}/products";
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

        private async Task<List<object>> GetGoTurcProducts(PazarYeriGirisBilgileri girisBilgisi)
        {
            string baseUrl = "https://gowa.goturc.com/";
            string token = "";
            using var client = new HttpClient();

            try
            {
                var url = $"{baseUrl}api/Account/Token?apiId={girisBilgisi.ApiKey}&apiSecret={girisBilgisi.SecretKey}";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(json);
                token = result?.token?.ToString();

                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Token is null or empty.");
                    return new List<object>();
                }

                var productUrl = $"{baseUrl}api/Product/List?page=1&size=10";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var productResponse = await client.GetAsync(productUrl);
                productResponse.EnsureSuccessStatusCode();
                var productJson = await productResponse.Content.ReadAsStringAsync();
                var productData = JsonConvert.DeserializeObject<ProductResponseGowa>(productJson);

                return productData?.ProductGowa?.Cast<object>().ToList() ?? new List<object>();
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"GoTurc HTTP Error: {httpEx.Message}");
                return new List<object>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GoTurc Error: {ex.Message}");
                return new List<object>();
            }
        }

        private async Task<List<object>> GetPazaramaProducts(PazarYeriGirisBilgileri girisBilgisi)
        {
            string tokenUrl = "https://isortagimgiris.pazarama.com/connect/token";
            string productListUrl = "https://isortagimapi.pazarama.com/product/products?Approved=true&Size=100&Page=1";
            string token = "";
            using var client = new HttpClient();

            try
            {
                string authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{girisBilgisi.ApiKey}:{girisBilgisi.SecretKey}"));
                var formData = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "scope", "merchantgatewayapi.fullaccess" }
                };
                var content = new FormUrlEncodedContent(formData);
                var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = content;

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var tokenData = JsonConvert.DeserializeObject<PazaramaTokenResponse>(json);
                token = tokenData?.Data?.AccessToken;

                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Pazarama token is null or empty.");
                    return new List<object>();
                }

                var productRequest = new HttpRequestMessage(HttpMethod.Get, productListUrl);
                productRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                productRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                productRequest.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36");

                var productResponse = await client.SendAsync(productRequest);
                productResponse.EnsureSuccessStatusCode();
                var productJson = await productResponse.Content.ReadAsStringAsync();
                var productData = JsonConvert.DeserializeObject<PazaramaProductResponse>(productJson);

                return productData?.Data?.Cast<object>().ToList() ?? new List<object>();
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Pazarama HTTP Error: {httpEx.Message}");
                return new List<object>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pazarama Error: {ex.Message}");
                return new List<object>();
            }
        }

        // Models for n11
        public class N11CategoryResponse
        {
            public List<N11Category> categories { get; set; }
        }

        public class N11Category
        {
            public long id { get; set; }
            public string name { get; set; }
            public List<N11Category> subCategories { get; set; }
        }

        // Existing Models (from previous response)
        public class N11ProductRequest
        {
            public string title { get; set; }
            public string description { get; set; }
            public long categoryId { get; set; }
            public string currencyType { get; set; }
            public string productMainId { get; set; }
            public bool deleteProductMainId { get; set; }
            public int preparingDay { get; set; }
            public string shipmentTemplate { get; set; }
            public int? maxPurchaseQuantity { get; set; }
            public bool deleteMaxPurchaseQuantity { get; set; }
            public string stockCode { get; set; }
            public string barcode { get; set; }
            public int quantity { get; set; }
            public string imageUrl { get; set; }
            public List<N11Attribute> attributes { get; set; }
            public double salePrice { get; set; }
            public double listPrice { get; set; }
            public int vatRate { get; set; }
            public string status { get; set; } // Added for update (Active/Suspended)
        }

        public class N11Attribute
        {
            public long id { get; set; }
            public long? valueId { get; set; }
            public string customValue { get; set; }
        }

        public class N11ProductResponse
        {
            public long id { get; set; }
            public string type { get; set; }
            public string status { get; set; }
            public List<string> reasons { get; set; }
        }
        public class FarmazonLoginResponse
        {
            [JsonProperty("result")]
            public FarmazonLoginResult Result { get; set; }
        }

        public class FarmazonLoginResult
        {
            [JsonProperty("token")]
            public string Token { get; set; }
        }

        public class FarmazonListingsResponse
        {
            [JsonProperty("result")]
            public FarmazonListingsResult Result { get; set; }
        }

        public class FarmazonListingsResult
        {
            [JsonProperty("items")]
            public List<FarmazonProductRequest> Items { get; set; }
        }

        //public class FarmazonProduct
        //{
        //[JsonProperty("id")]
        //public long Id { get; set; }
        //[JsonProperty("price")]
        //public decimal Price { get; set; }
        //[JsonProperty("stock")]
        //public int Stock { get; set; }
        //[JsonProperty("maxCount")]
        //public int MaxCount { get; set; }
        //[JsonProperty("description")]
        //public string Description { get; set; }
        //[JsonProperty("expiration")]
        //public string Expiration { get; set; }
        //[JsonProperty("isFeatured")]
        //public bool IsFeatured { get; set; }
        //[JsonProperty("isDeleted")]
        //public bool IsDeleted { get; set; }
        //[JsonProperty("isBestPrice")]
        //public bool IsBestPrice { get; set; }
        //[JsonProperty("product")]
        //public FarmazonProductDetails Product { get; set; }
        //}

        public class FarmazonProductDetails
        {
            [JsonProperty("id")]
            public long Id { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("vat")]
            public int Vat { get; set; }
            [JsonProperty("listingMinPrice")]
            public decimal ListingMinPrice { get; set; }
            [JsonProperty("sku")]
            public string Sku { get; set; }
            [JsonProperty("barcodes")]
            public List<FarmazonBarcode> Barcodes { get; set; }
        }

        public class FarmazonBarcode
        {
            [JsonProperty("barcode")]
            public string Barcode { get; set; }
            [JsonProperty("isGlobalBarcode")]
            public bool IsGlobalBarcode { get; set; }
            [JsonProperty("status")]
            public string Status { get; set; }
            [JsonProperty("isSelected")]
            public bool IsSelected { get; set; }
        }

        // Models for FarmaBorsa
        public class FarmaBorsaProductRequest
        {
            public string urunAd { get; set; }
            public int adet { get; set; }
            public int maxAdet { get; set; }
            public int op { get; set; }
            public double tutar { get; set; }
            public bool borsadaGoster { get; set; }
            public string miad { get; set; }
            public int miadTip { get; set; }
            public string barkod { get; set; }
            public string resimUrl { get; set; }
            public int? kod { get; set; } // Used for updates
        }

        public class FarmaBorsaProductResponse
        {
            public bool hata { get; set; }
            public string mesaj { get; set; }
        }

        // Existing Models
        public class PazaramaTokenResponse
        {
            [JsonProperty("data")]
            public PazaramaTokenData Data { get; set; }
        }

        public class PazaramaTokenData
        {
            [JsonProperty("accessToken")]
            public string AccessToken { get; set; }
        }

        public class PazaramaProductResponse
        {
            [JsonProperty("data")]
            public List<PazaramaProduct> Data { get; set; }
        }

        public class PazaramaProduct
        {
            [JsonProperty("productId")]
            public string ProductId { get; set; }
            [JsonProperty("title")]
            public string Title { get; set; }
            [JsonProperty("barcode")]
            public string Barcode { get; set; }
            [JsonProperty("status")]
            public string Status { get; set; }
            [JsonProperty("price")]
            public decimal Price { get; set; }
            [JsonProperty("stock")]
            public int Stock { get; set; }
        }

        public class ProductResponseGowa
        {
            public int total { get; set; }
            [JsonProperty("products")]
            public ProductGowa[] ProductGowa { get; set; }
        }

        public class ProductGowa
        {
            public string title { get; set; }
            public string shopProductCode { get; set; }
            public bool isActive { get; set; }
        }

        public class TrendyolProductResponse
        {
            public List<TrendyolProduct> Content { get; set; }
            public int TotalCount { get; set; }
        }

        public class TrendyolProduct
        {
            public long ProductId { get; set; }
            public string Title { get; set; }
            public string Barcode { get; set; }
            public string Status { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
        }

        public class IdefixProduct
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Barcode { get; set; }
            public string Status { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
        }

        public class CicekSepetiDataResponse
        {
            [JsonProperty("products")]
            public List<ProductCicekSepeti> ProductCicekSepeti { get; set; }
        }

        public class ProductCicekSepeti
        {
            [JsonProperty("productName")]
            public string ProductName { get; set; }
            [JsonProperty("productCode")]
            public string ProductCode { get; set; }
            [JsonProperty("stockCode")]
            public string StockCode { get; set; }
            [JsonProperty("isActive")]
            public bool IsActive { get; set; }
            [JsonProperty("categoryId")]
            public int CategoryId { get; set; }
            [JsonProperty("categoryName")]
            public string CategoryName { get; set; }
            [JsonProperty("productStatusType")]
            public string ProductStatusType { get; set; }
            [JsonProperty("isUseStockQuantity")]
            public bool IsUseStockQuantity { get; set; }
            [JsonProperty("stockQuantity")]
            public int StockQuantity { get; set; }
            [JsonProperty("salesPrice")]
            public double SalesPrice { get; set; }
            [JsonProperty("listPrice")]
            public double ListPrice { get; set; }
            [JsonProperty("barcode")]
            public string Barcode { get; set; }
            [JsonProperty("commissionRate")]
            public string CommissionRate { get; set; }
            [JsonProperty("numberOfFavorites")]
            public int NumberOfFavorites { get; set; }
            [JsonProperty("variantName")]
            public string VariantName { get; set; }
            [JsonProperty("images")]
            public List<string> Images { get; set; }
        }

        public class ProductResponse
        {
            public List<Product> Content { get; set; }
            public Pageable Pageable { get; set; }
            public int TotalElements { get; set; }
            public int TotalPages { get; set; }
            public bool Last { get; set; }
            public int NumberOfElements { get; set; }
            public bool First { get; set; }
            public int Size { get; set; }
            public int Number { get; set; }
            public bool Empty { get; set; }
        }

        public class Product
        {
            public long N11ProductId { get; set; }
            public long SellerId { get; set; }
            public string SellerNickname { get; set; }
            public string StockCode { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public long CategoryId { get; set; }
            public object ProductMainId { get; set; }
            public string Status { get; set; }
            public string SaleStatus { get; set; }
            public int PreparingDay { get; set; }
            public string ShipmentTemplate { get; set; }
            public int? MaxPurchaseQuantity { get; set; }
            public List<CustomTextOption> CustomTextOptions { get; set; }
            public long CatalogId { get; set; }
            public string Barcode { get; set; }
            public long GroupId { get; set; }
            public string CurrencyType { get; set; }
            public double SalePrice { get; set; }
            public double ListPrice { get; set; }
            public int Quantity { get; set; }
            public List<ProductAttribute> Attributes { get; set; }
            public List<string> ImageUrls { get; set; }
            public int VatRate { get; set; }
            public double CommissionRate { get; set; }
        }

        public class ProductAttribute
        {
            public long AttributeId { get; set; }
            public string AttributeName { get; set; }
            public string AttributeValue { get; set; }
        }

        public class CustomTextOption
        {
        }

        public class Pageable
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int Offset { get; set; }
            public bool Paged { get; set; }
            public bool Unpaged { get; set; }
        }

        public class InputModel
        {
            public string nick { get; set; }
            public string parola { get; set; }
            public string apiKey { get; set; }
            public int pasif { get; set; }
            public int ilan { get; set; }
            public bool tumIlanlarGelsin { get; set; }
        }

        public class Ilan
        {
            public int sira { get; set; }
            public int kod { get; set; }
            public string urunAd { get; set; }
            public int adet { get; set; }
            public int maxAdet { get; set; }
            public int op { get; set; }
            public double tutar { get; set; }
            public bool borsadaGoster { get; set; }
            public int urun { get; set; }
            public string miad { get; set; }
            public string miadText { get; set; }
            public int miadTip { get; set; }
            public string miadTipAd { get; set; }
            public string barkod { get; set; }
            public object psf { get; set; }
            public string tutarAd { get; set; }
            public string resimUrl { get; set; }
        }

        public class UrunTalepOutput
        {
            public bool hata { get; set; }
            public string mesaj { get; set; }
            public List<Ilan> ilanlarimList { get; set; }
        }
    }
}