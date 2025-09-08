using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;

namespace WebApp.Controllers
{
    public class AdminFarmaBorsaController : Controller
    {
        private readonly PazarYeriGirisBilgileriRepository _girisBilgileriRepository = new PazarYeriGirisBilgileriRepository();

        public async Task<IActionResult> Index(int pazarYeriId = 10)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            var products = new List<UnifiedMarketplaceModel>();
            if (girisBilgisi != null)
            {
                var farmaBorsaProducts = await GetFarmaborsaProducts(girisBilgisi);
                products = farmaBorsaProducts.Select(p => new UnifiedMarketplaceModel
                {
                    Title = p.urunAd,
                    ProductCode = p.kod.ToString(),
                    Barcode = p.barkod,
                    Description = p.urunAd, // Açıklama yoksa ürün adı kullanılır
                    FarmaBorsa = new FarmaBorsaUrunModel
                    {
                        SalePrice = (decimal?)p.tutar,
                        StockQuantity = p.adet,
                        MaxQuantity = p.maxAdet,
                        ExpirationDate = p.miad,
                        ShowInBorsa = p.borsadaGoster,
                        MiadTip = p.miadTip
                    }
                }).ToList();
            }
            ViewBag.PazarYeriId = pazarYeriId;
            ViewBag.ApiType = "farma borsa";
            return View(products);
        }

        [HttpPost]
        public IActionResult Sil(int kod, int pazarYeriId)
        {
            return DeleteFarmaBorsaProduct(kod, pazarYeriId).Result;
        }

        private async Task<List<Ilan>> GetFarmaborsaProducts(PazarYeriGirisBilgileri girisBilgisi)
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
                        return result.ilanlarimList;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Farmaborsa Error: {ex.Message}");
            }
            return new List<Ilan>();
        }

        [HttpPost]
        public async Task<IActionResult> AddFarmaBorsaProduct([FromBody] UnifiedMarketplaceModel model, int pazarYeriId = 10)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            string apiUrl = "https://wapi.farmaborsa.com/api/Entegre/IlanKaydet";
            using var client = new HttpClient();

            try
            {
                // Görselleri işleme (sadece ilk görsel kullanılıyor)
                string resimUrl = "";
                if (model.ImgFiles != null && model.ImgFiles.Any())
                {
                    var file = model.ImgFiles.First();
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        resimUrl = $"/uploads/{fileName}";
                    }
                }

                var input = new
                {
                    nick = girisBilgisi.KullaniciAdi ?? "",
                    parola = girisBilgisi.Sifre ?? "",
                    apiKey = girisBilgisi.ApiKey ?? "",
                    urunAd = model.Title ?? "",
                    adet = model.FarmaBorsa?.StockQuantity ?? 1,
                    maxAdet = model.FarmaBorsa?.MaxQuantity ?? 1,
                    op = 0.0,
                    tutar = model.FarmaBorsa?.SalePrice ,
                    borsadaGoster = model.FarmaBorsa?.ShowInBorsa ?? false,
                    miad = model.FarmaBorsa?.ExpirationDate ?? "",
                    miadTip = model.FarmaBorsa?.MiadTip ?? 0,
                    barkod = model.Barcode ?? "",
                    resimUrl = resimUrl
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
        public async Task<IActionResult> UpdateFarmaBorsaProduct([FromBody] UnifiedMarketplaceModel model, int pazarYeriId = 10)
        {
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            //if (model.FarmaBorsa == null || model.ProductCode || model.FarmaBorsa.Kod == 0)
            //    return Json(new { success = false, message = "Geçersiz istek veya ürün kodu eksik." });

            string apiUrl = "https://wapi.farmaborsa.com/api/Entegre/IlanHizliDuzenle";
            using var client = new HttpClient();

            try
            {
                // Görselleri işleme (sadece ilk görsel kullanılıyor)
                string resimUrl = "";
                if (model.ImgFiles != null && model.ImgFiles.Any())
                {
                    var file = model.ImgFiles.First();
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        resimUrl = $"/Uploads/{fileName}";
                    }
                }

                var input = new
                {
                    nick = girisBilgisi.KullaniciAdi ?? "",
                    parola = girisBilgisi.Sifre ?? "",
                    apiKey = girisBilgisi.ApiKey ?? "",
                    kod = model.ProductCode,
                    urunAd = model.Title ?? "",
                    adet = model.FarmaBorsa.StockQuantity ?? 1,
                    maxAdet = model.FarmaBorsa.MaxQuantity ?? 1,
                    op = 0.0,
                    tutar =model.FarmaBorsa.SalePrice,
                    borsadaGoster = model.FarmaBorsa.ShowInBorsa,
                    miad = model.FarmaBorsa.ExpirationDate ?? "",
                    miadTip = model.FarmaBorsa.MiadTip ?? 0,
                    barkod = model.Barcode ?? "",
                    resimUrl = resimUrl,
                    urunErp = model.Barcode ?? ""
                };

                var json = JsonConvert.SerializeObject(input);
                Console.WriteLine("Giden JSON: " + json);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                var responseJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Yanıtı: " + responseJson);

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

        [HttpGet]
        public async Task<IActionResult> DeleteFarmaBorsaProduct(int kod, int pazarYeriId = 10)
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

    }
}