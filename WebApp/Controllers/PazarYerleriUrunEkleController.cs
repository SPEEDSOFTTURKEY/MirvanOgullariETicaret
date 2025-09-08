using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;
using System.ServiceModel;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using OfficeOpenXml;
using System.IO;
using WebApp.Services;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using NuGet.Protocol.Core.Types;
using PTTServiceReference;
using System.Globalization;
using System;
using System.Drawing;

namespace WebApp.Controllers
{
    public class PazarYerleriUrunEkleController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly PazaramaService _pazaramaService;
        private readonly PazarYeriGirisBilgileriRepository _girisBilgileriRepository = new();
        UnifiedMarketplaceRepository _repository = new UnifiedMarketplaceRepository();
        private readonly UrunRepository _urunRepository = new();
        private const string ProductCreateUrl = "https://isortagimapi.pazarama.com/product/create";

        PazarYeriGorselRepository _pazarYeriGorselRepository = new PazarYeriGorselRepository();

        public IActionResult Index()
        {
            List<Urun> uruns = _urunRepository.GetirList(x => x.Durumu == 1);
            ViewBag.Urun = uruns;
            return View();
        }
        public async Task<IActionResult> Kaydet(UnifiedMarketplaceModel request)
        {
            List<string> imageUrls = new List<string>();

            // İlk olarak ürünün veritabanına eklenmesi
            var entity = new UnifiedMarketplace
            {
                Title = request.Title,
                UrunId = request.UrunId,
                ProductCode = request.ProductCode,
                Barcode = request.Barcode,
                Description = request.Description,
                ListPrice = request.ListPrice,
                VatRate = request.VatRate,

                N11_CategoryId = request.N11.CategoryId,
                N11_SalePrice = request.N11.SalePrice,
                N11_StockQuantity = request.N11.StockQuantity,
                N11_EklenmeTarihi = DateTime.Now,
                N11_GuncellenmeTarihi = DateTime.Now,
                N11_Gonderildi = request.N11.CategoryId != null ? 0 : null,
                N11_Success = request.N11.CategoryId != null ? false : null,
                N11_Message = request.N11.CategoryId != null ? "Ürün henüz gönderilmedi." : null,

                GoTurc_CategoryId = request.GoTurc.CategoryId,
                GoTurc_BrandId = request.GoTurc.BrandId,
                GoTurc_DeliveryNo = request.GoTurc.DeliveryNo,
                GoTurc_SalePrice = request.GoTurc.SalePrice,
                GoTurc_StockQuantity = request.GoTurc.StockQuantity,
                GoTurc_EklenmeTarihi = DateTime.Now,
                GoTurc_GuncellenmeTarihi = DateTime.Now,
                GoTurc_Gonderildi = request.GoTurc.CategoryId != null ? 0 : null,
                GoTurc_Success = request.GoTurc.CategoryId != null ? false : null,
                GoTurc_Message = request.GoTurc.CategoryId != null ? "Ürün henüz gönderilmedi." : null,

                Idefix_CategoryId = request.Idefix.CategoryId,
                Idefix_BrandId = request.Idefix.BrandId,
                Idefix_SalePrice = request.Idefix.SalePrice,
                Idefix_StockQuantity = request.Idefix.StockQuantity,
                Idefix_VendorStockCode = request.Idefix.VendorStockCode,
                Idefix_Weight = request.Idefix.Weight,
                Idefix_ComparePrice = request.Idefix.ComparePrice,
                Idefix_ProductMainId = request.Idefix.ProductMainId,
                Idefix_DeliveryDuration = request.Idefix.DeliveryDuration,
                Idefix_CargoCompanyId = request.Idefix.CargoCompanyId,
                Idefix_ShipmentAddressId = request.Idefix.ShipmentAddressId,
                Idefix_ReturnAddressId = request.Idefix.ReturnAddressId,
                Idefix_AttributesJson = JsonConvert.SerializeObject(request.Idefix.Attributes),
                Idefix_EklenmeTarihi = DateTime.Now,
                Idefix_GuncellenmeTarihi = DateTime.Now,
                Idefix_Gonderildi = request.Idefix.CategoryId != null ? 0 : null,
                Idefix_Success = request.Idefix.CategoryId != null ? false : null,
                Idefix_Message = request.Idefix.CategoryId != null ? "Ürün henüz gönderilmedi." : null,

                PTT_CategoryId = request.PTT.CategoryId,
                PTT_SalePrice = request.PTT.SalePrice,
                PTT_PriceWithVat = request.PTT.PriceWithVat,
                PTT_StockQuantity = request.PTT.StockQuantity,
                PTT_Desi = request.PTT.Desi,
                PTT_Discount = request.PTT.Discount,
                PTT_EstimatedCourierDelivery = request.PTT.EstimatedCourierDelivery,
                PTT_WarrantyDuration = request.PTT.WarrantyDuration,
                PTT_CargoProfileId = request.PTT.CargoProfileId,
                PTT_BasketMaxQuantity = request.PTT.BasketMaxQuantity,
                PTT_VariantsJson = JsonConvert.SerializeObject(request.PTT.Variants),
                PTT_EklenmeTarihi = DateTime.Now,
                PTT_GuncellenmeTarihi = DateTime.Now,
                PTT_Gonderildi = request.PTT.CategoryId != null ? 0 : null,
                PTT_Success = request.PTT.CategoryId != null ? false : null,
                PTT_Message = request.PTT.CategoryId != null ? "Ürün henüz gönderilmedi." : null,

                Pazarama_CategoryId = request.Pazarama.CategoryId,
                Pazarama_BrandId = request.Pazarama.BrandId,
                Pazarama_SalePrice = request.Pazarama.SalePrice,
                Pazarama_StockQuantity = request.Pazarama.StockQuantity,
                Pazarama_EklenmeTarihi = DateTime.Now,
                Pazarama_GuncellenmeTarihi = DateTime.Now,
                Pazarama_Gonderildi = request.Pazarama.CategoryId != null ? 0 : null,
                Pazarama_Success = request.Pazarama.CategoryId != null ? false : null,
                Pazarama_Message = request.Pazarama.CategoryId != null ? "Ürün henüz gönderilmedi." : null,

                CicekSepeti_CategoryId = request.CicekSepeti.CategoryId,
                CicekSepeti_SalePrice = request.CicekSepeti.SalePrice,
                CicekSepeti_StockQuantity = request.CicekSepeti.StockQuantity,
                CicekSepeti_StockCode = request.CicekSepeti.StockCode,
                CicekSepeti_CategoryName = request.CicekSepeti.CategoryName,
                CicekSepeti_ProductStatusType = request.CicekSepeti.ProductStatusType,
                CicekSepeti_VariantName = request.CicekSepeti.VariantName,
                CicekSepeti_EklenmeTarihi = DateTime.Now,
                CicekSepeti_GuncellenmeTarihi = DateTime.Now,
                CicekSepeti_Gonderildi = request.CicekSepeti.CategoryId != null ? 0 : null,
                CicekSepeti_Success = request.CicekSepeti.CategoryId != null ? false : null,
                CicekSepeti_Message = request.CicekSepeti.CategoryId != null ? "Ürün henüz gönderilmedi." : null,

                Farmazon_Id = request.Farmazon.Id,
                Farmazon_Price = request.Farmazon.Price,
                Farmazon_Stock = request.Farmazon.Stock,
                Farmazon_State = request.Farmazon.State,
                Farmazon_Expiration = request.Farmazon.Expiration,
                Farmazon_MaxCount = request.Farmazon.MaxCount,
                Farmazon_IsFeatured = request.Farmazon.IsFeatured,
                Farmazon_FarmazonEnabled = request.Farmazon.FarmazonEnabled,
                Farmazon_StockCode = request.Farmazon.StockCode,
                Farmazon_EklenmeTarihi = DateTime.Now,
                Farmazon_GuncellenmeTarihi = DateTime.Now,
                Farmazon_Gonderildi = request.Farmazon.Id != null ? 0 : null,
                Farmazon_Success = request.Farmazon.Id != null ? false : null,
                Farmazon_Message = request.Farmazon.Id != null ? "Ürün henüz gönderilmedi." : null,

                FarmaBorsa_SalePrice = request.FarmaBorsa.SalePrice,
                FarmaBorsa_StockQuantity = request.FarmaBorsa.StockQuantity,
                FarmaBorsa_MaxQuantity = request.FarmaBorsa.MaxQuantity,
                FarmaBorsa_ExpirationDate = request.FarmaBorsa.ExpirationDate,
                FarmaBorsa_ShowInBorsa = true,
                FarmaBorsa_FarmaBorsaEnabled = request.FarmaBorsa.FarmaBorsaEnabled,
                FarmaBorsa_MiadTip = 0,
                FarmaBorsa_EklenmeTarihi = DateTime.Now,
                FarmaBorsa_GuncellenmeTarihi = DateTime.Now,
                FarmaBorsa_Gonderildi = request.FarmaBorsa.StockQuantity != null ? 0 : null,
                FarmaBorsa_Success = request.FarmaBorsa.StockQuantity != null ? false : null,
                FarmaBorsa_Message = request.FarmaBorsa.StockQuantity != null ? "Ürün henüz gönderilmedi." : null,

                Trendyol_CategoryId = request.Trendyol.CategoryId,
                Trendyol_BrandId = request.Trendyol.BrandId,
                Trendyol_SalePrice = request.Trendyol.SalePrice,
                Trendyol_StockQuantity = request.Trendyol.StockQuantity,
                Trendyol_EklenmeTarihi = DateTime.Now,
                Trendyol_GuncellenmeTarihi = DateTime.Now,
                Trendyol_Gonderildi = request.Trendyol.CategoryId != null ? 0 : null,
                Trendyol_Success = request.Trendyol.CategoryId != null ? false : null,
                Trendyol_Message = request.Trendyol.CategoryId != null ? "Ürün henüz gönderilmedi." : null,

                Durumu = 1,
                GuncellenmeTarihi = DateTime.Now
            };

            _repository.Ekle(entity);

            // Görselleri kaydet
            if (request.ImgFiles != null && request.ImgFiles.Any())
            {
                List<string> imageUrlsBuyuk = new List<string>();

                string buyukPath = Path.Combine("wwwroot/urungorsel/Buyuk");
                string kucukPath = Path.Combine("wwwroot/urungorsel/Kucuk");
                Directory.CreateDirectory(buyukPath);
                Directory.CreateDirectory(kucukPath);

                foreach (var file in request.ImgFiles)
                {
                    if (file != null && file.Length > 0)
                    {
                        string extension = Path.GetExtension(file.FileName);
                        string newImageName = Guid.NewGuid() + extension;

                        string filePathBuyuk = Path.Combine(buyukPath, newImageName);
                        string filePathKucuk = Path.Combine(kucukPath, newImageName);

                        using (var stream = new FileStream(filePathBuyuk, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                            stream.Position = 0;

                            using (Bitmap original = new Bitmap(stream))
                            using (Bitmap kucuk = new Bitmap(original, new Size(270, 334)))
                            {
                                kucuk.Save(filePathKucuk);
                            }
                        }

                        string relativePathBuyuk = $"/urungorsel/Buyuk/{newImageName}";
                        imageUrlsBuyuk.Add(relativePathBuyuk);
                    }
                }

                // Veritabanına görselleri kaydet
                foreach (var imgPath in imageUrlsBuyuk)
                {
                    var gorsel = new PazarYeriGorsel
                    {
                        UnifiedMarketplaceId = entity.Id,
                        FotografYolu = imgPath,
                        Durumu = 1,
                        EklenmeTarihi = DateTime.Now,
                        GuncellenmeTarihi = DateTime.Now
                    };
                    _pazarYeriGorselRepository.Ekle(gorsel);
                }

                // İlk görseli ana görsel olarak ata
                if (imageUrlsBuyuk.Any())
                {
                    entity.UrunGorsel = imageUrlsBuyuk.First();
                    entity.ImgFilesJson = JsonConvert.SerializeObject(imageUrlsBuyuk);
                    _repository.Guncelle(entity);
                }
            }

            return RedirectToAction("Index", "PazarYerleriUrun");
        }
        [HttpPost]
        public async Task<IActionResult> Gonder(UnifiedMarketplaceModel request, int PazarYeriUrunId)
        {
            var results = new List<dynamic>();

            if (PazarYeriUrunId != 0)
            {
                UnifiedMarketplaceRepository unifiedMarketplaceRepository = new UnifiedMarketplaceRepository();
                UnifiedMarketplace unifiedMarketplace = unifiedMarketplaceRepository.Getir(x => x.Id == PazarYeriUrunId);

                try
                {
                    var imageUrls = new List<string>();
                    if (unifiedMarketplace.ImgFilesJson != null && !string.IsNullOrEmpty(unifiedMarketplace.ImgFilesJson))
                    {
                        imageUrls = JsonConvert.DeserializeObject<List<string>>(unifiedMarketplace.ImgFilesJson) ?? new List<string>();
                    }
                    if (request.ImgFiles != null && request.ImgFiles.Any())
                    {
                        foreach (var file in request.ImgFiles)
                        {
                            if (file.Length > 0)
                            {
                                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/urungorsel", fileName);
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }
                                imageUrls.Add($"https://ultrasonluzibin.com/urungorsel/{fileName}");
                         
                            }
                        }
                        unifiedMarketplace.ImgFilesJson = JsonConvert.SerializeObject(imageUrls);
                    }

                    // PTT ürün ekleme
                    if (unifiedMarketplace.PTT_CategoryId != null)
                    {
                        var pazarYeriId = 4;
                        var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
                        if (girisBilgisi == null)
                        {
                            unifiedMarketplace.PTT_Gonderildi = 0;
                            unifiedMarketplace.PTT_Success = false;
                            unifiedMarketplace.PTT_Message = "API kimlik bilgileri bulunamadı.";
                            unifiedMarketplace.PTT_TrackingId = string.Empty;
                            unifiedMarketplace.PTT_GuncellenmeTarihi = DateTime.Now;
                            unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                            results.Add(new { marketplace = "PTT", success = false, message = "API kimlik bilgileri bulunamadı." });
                        }
                        else
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
                                await client.OpenAsync();
                                var productRequest = new PTTServiceReference.ProductV3Request
                                {
                                    Active = true,
                                    Barcode = unifiedMarketplace.Barcode,
                                    BasketMaxQuantity = unifiedMarketplace.PTT_BasketMaxQuantity ?? 1000,
                                    CargoProfileId = unifiedMarketplace.PTT_CargoProfileId ?? 0,
                                    CategoryId = (int?)unifiedMarketplace.PTT_CategoryId,
                                    Desi = (double?)unifiedMarketplace.PTT_Desi,
                                    Discount = unifiedMarketplace.PTT_Discount ?? 0,
                                    EstimatedCourierDelivery = unifiedMarketplace.PTT_EstimatedCourierDelivery ?? 5,
                                    Images = imageUrls.Select(url => new PTTServiceReference.ProductImageV3 { Url = url }).ToArray(),
                                    LongDescription = unifiedMarketplace.Description,
                                    Name = unifiedMarketplace.Title,
                                    NoShippingProduct = false,
                                    PriceWithVat = unifiedMarketplace.PTT_PriceWithVat ?? 0,
                                    PriceWithoutVat = unifiedMarketplace.PTT_SalePrice,
                                    ProductCode = unifiedMarketplace.Barcode,
                                    Quantity = unifiedMarketplace.PTT_StockQuantity,
                                    SingleBox = false,
                                    VATRate = unifiedMarketplace.VatRate,
                                    WarrantyDuration = unifiedMarketplace.PTT_WarrantyDuration ?? 0,
                                };

                                var response = await client.UpdateProductsV3Async(new[] { productRequest });

                                unifiedMarketplace.PTT_Gonderildi = 1;
                                unifiedMarketplace.PTT_Success = response.Success;
                                unifiedMarketplace.PTT_Message = response.Success ? "Ürün başarıyla eklendi/güncellendi" : response.Message;
                                unifiedMarketplace.PTT_TrackingId = response.TrackingId ?? string.Empty;
                                unifiedMarketplace.PTT_GuncellenmeTarihi = DateTime.Now;
                                unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                results.Add(new
                                {
                                    marketplace = "PTT",
                                    success = response.Success,
                                    message = unifiedMarketplace.PTT_Message,
                                    trackingId = unifiedMarketplace.PTT_TrackingId
                                });
                            }
                            catch (Exception ex)
                            {
                                unifiedMarketplace.PTT_Gonderildi = 0;
                                unifiedMarketplace.PTT_Success = false;
                                unifiedMarketplace.PTT_Message = $"Ürün eklenirken hata oluştu: {ex.Message}";
                                unifiedMarketplace.PTT_TrackingId = string.Empty;
                                unifiedMarketplace.PTT_GuncellenmeTarihi = DateTime.Now;
                                unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                results.Add(new
                                {
                                    marketplace = "PTT",
                                    success = false,
                                    message = unifiedMarketplace.PTT_Message,
                                    trackingId = string.Empty
                                });
                            }
                            finally
                            {
                                try
                                {
                                    if (client.State != CommunicationState.Faulted)
                                        await client.CloseAsync();
                                    else
                                        client.Abort();
                                }
                                catch
                                {
                                    client.Abort();
                                }
                            }
                        }
                    }
                    else
                    {
                        unifiedMarketplace.PTT_Gonderildi = null;
                        unifiedMarketplace.PTT_Success = null;
                        unifiedMarketplace.PTT_Message = null;
                        unifiedMarketplace.PTT_TrackingId = null;
                        unifiedMarketplace.PTT_GuncellenmeTarihi = DateTime.Now;
                        unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                    }

                    // N11 ürün ekleme
                    if (unifiedMarketplace.N11_CategoryId != null)
                    {
                        var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == 3).FirstOrDefault();
                        if (girisBilgisi == null)
                        {
                            unifiedMarketplace.N11_Gonderildi = 0;
                            unifiedMarketplace.N11_Success = false;
                            unifiedMarketplace.N11_Message = "API kimlik bilgileri bulunamadı.";
                            unifiedMarketplace.N11_ReportId = 0;
                            unifiedMarketplace.N11_GuncellenmeTarihi = DateTime.Now;
                            unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                            results.Add(new { marketplace = "N11", success = false, message = "API kimlik bilgileri bulunamadı." });
                        }
                        else
                        {
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
                                        title = unifiedMarketplace.Title,
                                        description = unifiedMarketplace.Description,
                                        categoryId = unifiedMarketplace.N11_CategoryId,
                                        currencyType = "TL",
                                        productMainId = unifiedMarketplace.Barcode,
                                        preparingDay = unifiedMarketplace.PTT_EstimatedCourierDelivery ?? 5,
                                        shipmentTemplate = "ALVİT",
                                        maxPurchaseQuantity = unifiedMarketplace.PTT_BasketMaxQuantity,
                                        stockCode = unifiedMarketplace.CicekSepeti_StockCode,
                                        barcode = unifiedMarketplace.Barcode,
                                        quantity = unifiedMarketplace.N11_StockQuantity,
                                        images = imageUrls.Select((url, index) => new { url, order = index + 1 }).ToArray(),
                                        salePrice = unifiedMarketplace.N11_SalePrice,
                                        listPrice = unifiedMarketplace.ListPrice,
                                        vatRate = unifiedMarketplace.VatRate
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

                                    unifiedMarketplace.N11_Gonderildi = result.status == "IN_QUEUE" ? 1 : 0;
                                    unifiedMarketplace.N11_Success = result.status == "IN_QUEUE";
                                    unifiedMarketplace.N11_Message = result.status == "IN_QUEUE" ? $"Ürün başarıyla işleme alındı. Task ID: {result.id}" : $"Hata: {string.Join(", ", result.reasons)}";
                                    unifiedMarketplace.N11_ReportId = result.id;
                                    unifiedMarketplace.N11_GuncellenmeTarihi = DateTime.Now;
                                    unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                    results.Add(new
                                    {
                                        marketplace = "N11",
                                        success = result.status == "IN_QUEUE",
                                        message = unifiedMarketplace.N11_Message,
                                        reportId = unifiedMarketplace.N11_ReportId
                                    });
                                }
                                else
                                {
                                    var errorContent = await response.Content.ReadAsStringAsync();
                                    unifiedMarketplace.N11_Gonderildi = 0;
                                    unifiedMarketplace.N11_Success = false;
                                    unifiedMarketplace.N11_Message = $"Hata: {(int)response.StatusCode} - {errorContent}";
                                    unifiedMarketplace.N11_ReportId = 0;
                                    unifiedMarketplace.N11_GuncellenmeTarihi = DateTime.Now;
                                    unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                    results.Add(new
                                    {
                                        marketplace = "N11",
                                        success = false,
                                        message = unifiedMarketplace.N11_Message
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                unifiedMarketplace.N11_Gonderildi = 0;
                                unifiedMarketplace.N11_Success = false;
                                unifiedMarketplace.N11_Message = $"Hata: {ex.Message}";
                                unifiedMarketplace.N11_ReportId = 0;
                                unifiedMarketplace.N11_GuncellenmeTarihi = DateTime.Now;
                                unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                results.Add(new
                                {
                                    marketplace = "N11",
                                    success = false,
                                    message = unifiedMarketplace.N11_Message
                                });
                            }
                        }
                    }
                    else
                    {
                        unifiedMarketplace.N11_Gonderildi = null;
                        unifiedMarketplace.N11_Success = null;
                        unifiedMarketplace.N11_Message = null;
                        unifiedMarketplace.N11_ReportId = null;
                        unifiedMarketplace.N11_GuncellenmeTarihi = DateTime.Now;
                        unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                    }

                    // GoTurc ürün ekleme
                    if (unifiedMarketplace.GoTurc_CategoryId != null)
                    {
                        var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == 8).FirstOrDefault();
                        if (girisBilgisi == null)
                        {
                            unifiedMarketplace.GoTurc_Gonderildi = 0;
                            unifiedMarketplace.GoTurc_Success = false;
                            unifiedMarketplace.GoTurc_Message = "API kimlik bilgileri bulunamadı.";
                            unifiedMarketplace.GoTurc_ReportId = string.Empty;
                            unifiedMarketplace.GoTurc_GuncellenmeTarihi = DateTime.Now;
                            unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                            results.Add(new { marketplace = "GoTurc", success = false, message = "API kimlik bilgileri bulunamadı." });
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(unifiedMarketplace.Title) || unifiedMarketplace.Title.Length < 5 || unifiedMarketplace.Title.Length > 65)
                            {
                                unifiedMarketplace.GoTurc_Gonderildi = 0;
                                unifiedMarketplace.GoTurc_Success = false;
                                unifiedMarketplace.GoTurc_Message = "Ürün başlığı 5 ile 65 karakter arasında olmalıdır.";
                                unifiedMarketplace.GoTurc_ReportId = string.Empty;
                                unifiedMarketplace.GoTurc_GuncellenmeTarihi = DateTime.Now;
                                unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                                results.Add(new { marketplace = "GoTurc", success = false, message = "Ürün başlığı 5 ile 65 karakter arasında olmalıdır." });
                            }
                            else if (unifiedMarketplace.GoTurc_DeliveryNo <= 0)
                            {
                                unifiedMarketplace.GoTurc_Gonderildi = 0;
                                unifiedMarketplace.GoTurc_Success = false;
                                unifiedMarketplace.GoTurc_Message = "Geçerli teslimat numarası gereklidir.";
                                unifiedMarketplace.GoTurc_ReportId = string.Empty;
                                unifiedMarketplace.GoTurc_GuncellenmeTarihi = DateTime.Now;
                                unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                                results.Add(new { marketplace = "GoTurc", success = false, message = "Geçerli teslimat numarası gereklidir." });
                            }
                            else
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
                                        unifiedMarketplace.GoTurc_Gonderildi = 0;
                                        unifiedMarketplace.GoTurc_Success = false;
                                        unifiedMarketplace.GoTurc_Message = "Token alınamadı.";
                                        unifiedMarketplace.GoTurc_ReportId = string.Empty;
                                        unifiedMarketplace.GoTurc_GuncellenmeTarihi = DateTime.Now;
                                        unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                                        results.Add(new { marketplace = "GoTurc", success = false, message = "Token alınamadı." });
                                    }
                                    else
                                    {
                                        var apiUrl = $"{baseUrl}api/Product";
                                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                                        GoTurcProduct product = new GoTurcProduct
                                        {
                                            ShopProductCode = unifiedMarketplace.Barcode,
                                            Title = unifiedMarketplace.Title,
                                            SubTitle = unifiedMarketplace.Title,
                                            StockQuantity = unifiedMarketplace.GoTurc_StockQuantity ?? 0,
                                            Price = unifiedMarketplace.GoTurc_SalePrice ?? 0,
                                            DescriptionHTML = unifiedMarketplace.Description,
                                            ImageUrl = imageUrls,
                                            Status = "Aktif",
                                            CategoryId = unifiedMarketplace.GoTurc_CategoryId ?? 0,
                                            BrandId = unifiedMarketplace.GoTurc_BrandId ?? 0,
                                            DeliveryNo = Convert.ToInt32(unifiedMarketplace.GoTurc_DeliveryNo)
                                        };

                                        var apiRequest = new List<GoTurcProduct> { product };
                                        var json = JsonConvert.SerializeObject(apiRequest);
                                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                                        var response = await client.PostAsync(apiUrl, content);

                                        if (response.IsSuccessStatusCode)
                                        {
                                            var responseJson = await response.Content.ReadAsStringAsync();
                                            var result = JsonConvert.DeserializeObject<GoTurcProductResponse>(responseJson);
                                            unifiedMarketplace.GoTurc_Gonderildi = 1;
                                            unifiedMarketplace.GoTurc_Success = true;
                                            unifiedMarketplace.GoTurc_Message = $"Ürün başarıyla eklendi/güncellendi. Rapor ID: {result.ReportId}";
                                            unifiedMarketplace.GoTurc_ReportId = result.ReportId;
                                            unifiedMarketplace.GoTurc_GuncellenmeTarihi = DateTime.Now;
                                            unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                            results.Add(new
                                            {
                                                marketplace = "GoTurc",
                                                success = true,
                                                message = unifiedMarketplace.GoTurc_Message,
                                                reportId = unifiedMarketplace.GoTurc_ReportId
                                            });
                                        }
                                        else
                                        {
                                            var errorContent = await response.Content.ReadAsStringAsync();
                                            unifiedMarketplace.GoTurc_Gonderildi = 0;
                                            unifiedMarketplace.GoTurc_Success = false;
                                            unifiedMarketplace.GoTurc_Message = $"API hatası: {(int)response.StatusCode} - {errorContent}";
                                            unifiedMarketplace.GoTurc_ReportId = string.Empty;
                                            unifiedMarketplace.GoTurc_GuncellenmeTarihi = DateTime.Now;
                                            unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                            results.Add(new
                                            {
                                                marketplace = "GoTurc",
                                                success = false,
                                                message = unifiedMarketplace.GoTurc_Message
                                            });
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    unifiedMarketplace.GoTurc_Gonderildi = 0;
                                    unifiedMarketplace.GoTurc_Success = false;
                                    unifiedMarketplace.GoTurc_Message = $"Hata: {ex.Message}";
                                    unifiedMarketplace.GoTurc_ReportId = string.Empty;
                                    unifiedMarketplace.GoTurc_GuncellenmeTarihi = DateTime.Now;
                                    unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                    results.Add(new
                                    {
                                        marketplace = "GoTurc",
                                        success = false,
                                        message = unifiedMarketplace.GoTurc_Message
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        unifiedMarketplace.GoTurc_Gonderildi = null;
                        unifiedMarketplace.GoTurc_Success = null;
                        unifiedMarketplace.GoTurc_Message = null;
                        unifiedMarketplace.GoTurc_ReportId = null;
                        unifiedMarketplace.GoTurc_GuncellenmeTarihi = DateTime.Now;
                        unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                    }

                    // Idefix ürün ekleme
                    if (unifiedMarketplace.Idefix_CategoryId != null)
                    {
                        var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == 6).FirstOrDefault();
                        if (girisBilgisi == null)
                        {
                            unifiedMarketplace.Idefix_Gonderildi = 0;
                            unifiedMarketplace.Idefix_Success = false;
                            unifiedMarketplace.Idefix_Message = "Pazar yeri bilgileri bulunamadı.";
                            unifiedMarketplace.Idefix_ReportId = string.Empty;
                            unifiedMarketplace.Idefix_GuncellenmeTarihi = DateTime.Now;
                            unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                            results.Add(new { marketplace = "Idefix", success = false, message = "Pazar yeri bilgileri bulunamadı." });
                        }
                        else
                        {
                            string vendorId = girisBilgisi.KullaniciAdi;
                            string apiKey = girisBilgisi.ApiKey;
                            string apiPass = girisBilgisi.SecretKey;
                            string vendorToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:{apiPass}"));
                            string url = $"https://merchantapi.idefix.com/pim/pool/{vendorId}/create";

                            using var client = new HttpClient();
                            client.DefaultRequestHeaders.Clear();
                            client.DefaultRequestHeaders.Add("X-API-KEY", vendorToken);
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            var product = new
                            {
                                barcode = unifiedMarketplace.Barcode,
                                title = unifiedMarketplace.Title,
                                productMainId = unifiedMarketplace.ProductCode,
                                brandId = unifiedMarketplace.Idefix_BrandId,
                                categoryId = unifiedMarketplace.Idefix_CategoryId,
                                inventoryQuantity = unifiedMarketplace.Idefix_StockQuantity,
                                vendorStockCode = unifiedMarketplace.Idefix_VendorStockCode,
                                desi = unifiedMarketplace.PTT_Desi ?? 0,
                                weight = unifiedMarketplace.Idefix_Weight ?? 0,
                                description = unifiedMarketplace.Description,
                                price = unifiedMarketplace.ListPrice,
                                comparePrice = unifiedMarketplace.Idefix_ComparePrice,
                                vatRate = unifiedMarketplace.VatRate,
                                deliveryDuration = unifiedMarketplace.Idefix_DeliveryDuration,
                                deliveryType = "regular",
                                cargoCompanyId = unifiedMarketplace.Idefix_CargoCompanyId ?? null,
                                shipmentAddressId = unifiedMarketplace.Idefix_ShipmentAddressId ?? null,
                                returnAddressId = unifiedMarketplace.Idefix_ReturnAddressId ?? null,
                                images = imageUrls.Select(url => new { url }).ToArray(),
                                originCountryId = 792,
                                manufacturer = (string)null,
                                importer = (string)null,
                                ceCompatibility = (bool?)null
                            };

                            var requestData = new { products = new[] { product } };

                            try
                            {
                                var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                                var response = await client.PostAsync(url, content);
                                string responseContent = await response.Content.ReadAsStringAsync();

                                if (response.IsSuccessStatusCode)
                                {
                                    dynamic json = JsonConvert.DeserializeObject(responseContent);
                                    unifiedMarketplace.Idefix_Gonderildi = 1;
                                    unifiedMarketplace.Idefix_Success = true;
                                    unifiedMarketplace.Idefix_Message = "Ürün başarıyla eklendi/güncellendi";
                                    unifiedMarketplace.Idefix_ReportId = json.batchRequestId?.ToString() ?? string.Empty;
                                    unifiedMarketplace.Idefix_GuncellenmeTarihi = DateTime.Now;
                                    unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                    results.Add(new
                                    {
                                        marketplace = "Idefix",
                                        success = true,
                                        message = unifiedMarketplace.Idefix_Message,
                                        batchRequestId = unifiedMarketplace.Idefix_ReportId
                                    });
                                }
                                else
                                {
                                    unifiedMarketplace.Idefix_Gonderildi = 0;
                                    unifiedMarketplace.Idefix_Success = false;
                                    unifiedMarketplace.Idefix_Message = $"Idefix API Hatası: {(int)response.StatusCode} - {responseContent}";
                                    unifiedMarketplace.Idefix_ReportId = string.Empty;
                                    unifiedMarketplace.Idefix_GuncellenmeTarihi = DateTime.Now;
                                    unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                    results.Add(new
                                    {
                                        marketplace = "Idefix",
                                        success = false,
                                        message = unifiedMarketplace.Idefix_Message
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                unifiedMarketplace.Idefix_Gonderildi = 0;
                                unifiedMarketplace.Idefix_Success = false;
                                unifiedMarketplace.Idefix_Message = $"Hata: {ex.Message}";
                                unifiedMarketplace.Idefix_ReportId = string.Empty;
                                unifiedMarketplace.Idefix_GuncellenmeTarihi = DateTime.Now;
                                unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                results.Add(new
                                {
                                    marketplace = "Idefix",
                                    success = false,
                                    message = unifiedMarketplace.Idefix_Message
                                });
                            }
                        }
                    }
                    else
                    {
                        unifiedMarketplace.Idefix_Gonderildi = null;
                        unifiedMarketplace.Idefix_Success = null;
                        unifiedMarketplace.Idefix_Message = null;
                        unifiedMarketplace.Idefix_ReportId = null;
                        unifiedMarketplace.Idefix_GuncellenmeTarihi = DateTime.Now;
                        unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                    }

                    // Pazarama ürün ekleme
                    if (unifiedMarketplace.Pazarama_CategoryId != null)
                    {
                        var pazarYeriId = 9;
                        var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
                        if (girisBilgisi == null)
                        {
                            unifiedMarketplace.Pazarama_Gonderildi = 0;
                            unifiedMarketplace.Pazarama_Success = false;
                            unifiedMarketplace.Pazarama_Message = "API kimlik bilgileri bulunamadı.";
                            unifiedMarketplace.Pazarama_ReportId = string.Empty;
                            unifiedMarketplace.Pazarama_GuncellenmeTarihi = DateTime.Now;
                            unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                            results.Add(new { marketplace = "Pazarama", success = false, message = "API kimlik bilgileri bulunamadı." });
                        }
                        else
                        {
                            try
                            {
                                string token = await _pazaramaService.GetAccessToken(girisBilgisi.ApiKey, girisBilgisi.SecretKey);
                                if (string.IsNullOrEmpty(token))
                                {
                                    unifiedMarketplace.Pazarama_Gonderildi = 0;
                                    unifiedMarketplace.Pazarama_Success = false;
                                    unifiedMarketplace.Pazarama_Message = "Token alınamadı.";
                                    unifiedMarketplace.Pazarama_ReportId = string.Empty;
                                    unifiedMarketplace.Pazarama_GuncellenmeTarihi = DateTime.Now;
                                    unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                                    results.Add(new { marketplace = "Pazarama", success = false, message = "Token alınamadı." });
                                }
                                else
                                {
                                    var productData = new
                                    {
                                        categoryId = unifiedMarketplace.Pazarama_CategoryId,
                                        brandId = unifiedMarketplace.Pazarama_BrandId,
                                        barcode = unifiedMarketplace.Barcode,
                                        title = unifiedMarketplace.Title,
                                        description = unifiedMarketplace.Description,
                                        salePrice = unifiedMarketplace.Pazarama_SalePrice,
                                        listPrice = unifiedMarketplace.ListPrice,
                                        stockQuantity = unifiedMarketplace.Pazarama_StockQuantity,
                                        vatRate = unifiedMarketplace.VatRate,
                                        images = imageUrls.Select((url, index) => new { url, order = index + 1 }).ToArray()
                                    };

                                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.pazarama.com/products")
                                    {
                                        Content = new StringContent(JsonConvert.SerializeObject(productData), Encoding.UTF8, "application/json")
                                    };
                                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                                    requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                                    var response = await _httpClient.SendAsync(requestMessage);
                                    var responseContent = await response.Content.ReadAsStringAsync();

                                    if (response.IsSuccessStatusCode)
                                    {
                                        var result = JsonConvert.DeserializeObject<dynamic>(responseContent);
                                        unifiedMarketplace.Pazarama_Gonderildi = 1;
                                        unifiedMarketplace.Pazarama_Success = true;
                                        unifiedMarketplace.Pazarama_Message = "Ürün başarıyla eklendi/güncellendi";
                                        unifiedMarketplace.Pazarama_ReportId = result?.id?.ToString() ?? string.Empty;
                                        unifiedMarketplace.Pazarama_GuncellenmeTarihi = DateTime.Now;
                                        unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                        results.Add(new
                                        {
                                            marketplace = "Pazarama",
                                            success = true,
                                            message = unifiedMarketplace.Pazarama_Message,
                                            reportId = unifiedMarketplace.Pazarama_ReportId
                                        });
                                    }
                                    else
                                    {
                                        unifiedMarketplace.Pazarama_Gonderildi = 0;
                                        unifiedMarketplace.Pazarama_Success = false;
                                        unifiedMarketplace.Pazarama_Message = $"Hata: {(int)response.StatusCode} - {responseContent}";
                                        unifiedMarketplace.Pazarama_ReportId = string.Empty;
                                        unifiedMarketplace.Pazarama_GuncellenmeTarihi = DateTime.Now;
                                        unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                        results.Add(new
                                        {
                                            marketplace = "Pazarama",
                                            success = false,
                                            message = unifiedMarketplace.Pazarama_Message
                                        });
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                unifiedMarketplace.Pazarama_Gonderildi = 0;
                                unifiedMarketplace.Pazarama_Success = false;
                                unifiedMarketplace.Pazarama_Message = $"Hata: {ex.Message}";
                                unifiedMarketplace.Pazarama_ReportId = string.Empty;
                                unifiedMarketplace.Pazarama_GuncellenmeTarihi = DateTime.Now;
                                unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                results.Add(new
                                {
                                    marketplace = "Pazarama",
                                    success = false,
                                    message = unifiedMarketplace.Pazarama_Message
                                });
                            }
                        }
                    }
                    else
                    {
                        unifiedMarketplace.Pazarama_Gonderildi = null;
                        unifiedMarketplace.Pazarama_Success = null;
                        unifiedMarketplace.Pazarama_Message = null;
                        unifiedMarketplace.Pazarama_ReportId = null;
                        unifiedMarketplace.Pazarama_GuncellenmeTarihi = DateTime.Now;
                        unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                    }

                    // ÇiçekSepeti ürün ekleme
                    if (unifiedMarketplace.CicekSepeti_CategoryId != null)
                    {
                        var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == 5).FirstOrDefault();
                        if (girisBilgisi == null)
                        {
                            unifiedMarketplace.CicekSepeti_Gonderildi = 0;
                            unifiedMarketplace.CicekSepeti_Success = false;
                            unifiedMarketplace.CicekSepeti_Message = "API kimlik bilgileri bulunamadı.";
                            unifiedMarketplace.CicekSepeti_ReportId = string.Empty;
                            unifiedMarketplace.CicekSepeti_GuncellenmeTarihi = DateTime.Now;
                            unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                            results.Add(new { marketplace = "CicekSepeti", success = false, message = "API kimlik bilgileri bulunamadı." });
                        }
                        else
                        {
                            string apiUrl = "https://api.ciceksepeti.com/api/v1/products";
                            using var client = new HttpClient();
                            client.DefaultRequestHeaders.Add("x-api-key", girisBilgisi.ApiKey);
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            try
                            {
                                var productData = new
                                {
                                    ProductName = unifiedMarketplace.Title,
                                    ProductCode = unifiedMarketplace.ProductCode,
                                    StockCode = unifiedMarketplace.CicekSepeti_StockCode,
                                    IsActive = true,
                                    CategoryId = unifiedMarketplace.CicekSepeti_CategoryId ?? 0,
                                    ProductStatusType = unifiedMarketplace.CicekSepeti_ProductStatusType,
                                    IsUseStockQuantity = true,
                                    StockQuantity = unifiedMarketplace.CicekSepeti_StockQuantity ?? 0,
                                    SalesPrice = Convert.ToDouble(unifiedMarketplace.CicekSepeti_SalePrice),
                                    ListPrice = Convert.ToDouble(unifiedMarketplace.ListPrice),
                                    Barcode = unifiedMarketplace.Barcode,
                                    Images = imageUrls
                                };

                                var json = JsonConvert.SerializeObject(productData);
                                var content = new StringContent(json, Encoding.UTF8, "application/json");
                                var response = await client.PostAsync(apiUrl, content);
                                var responseContent = await response.Content.ReadAsStringAsync();

                                if (response.IsSuccessStatusCode)
                                {
                                    var result = JsonConvert.DeserializeObject<dynamic>(responseContent);
                                    unifiedMarketplace.CicekSepeti_Gonderildi = 1;
                                    unifiedMarketplace.CicekSepeti_Success = true;
                                    unifiedMarketplace.CicekSepeti_Message = "Ürün başarıyla eklendi/güncellendi";
                                    unifiedMarketplace.CicekSepeti_ReportId = result?.id?.ToString() ?? string.Empty;
                                    unifiedMarketplace.CicekSepeti_GuncellenmeTarihi = DateTime.Now;
                                    unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                    results.Add(new
                                    {
                                        marketplace = "CicekSepeti",
                                        success = true,
                                        message = unifiedMarketplace.CicekSepeti_Message,
                                        reportId = unifiedMarketplace.CicekSepeti_ReportId
                                    });
                                }
                                else
                                {
                                    unifiedMarketplace.CicekSepeti_Gonderildi = 0;
                                    unifiedMarketplace.CicekSepeti_Success = false;
                                    unifiedMarketplace.CicekSepeti_Message = $"Hata: {(int)response.StatusCode} - {responseContent}";
                                    unifiedMarketplace.CicekSepeti_ReportId = string.Empty;
                                    unifiedMarketplace.CicekSepeti_GuncellenmeTarihi = DateTime.Now;
                                    unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                    results.Add(new
                                    {
                                        marketplace = "CicekSepeti",
                                        success = false,
                                        message = unifiedMarketplace.CicekSepeti_Message
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                unifiedMarketplace.CicekSepeti_Gonderildi = 0;
                                unifiedMarketplace.CicekSepeti_Success = false;
                                unifiedMarketplace.CicekSepeti_Message = $"Hata: {ex.Message}";
                                unifiedMarketplace.CicekSepeti_ReportId = string.Empty;
                                unifiedMarketplace.CicekSepeti_GuncellenmeTarihi = DateTime.Now;
                                unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                results.Add(new
                                {
                                    marketplace = "CicekSepeti",
                                    success = false,
                                    message = unifiedMarketplace.CicekSepeti_Message
                                });
                            }
                        }
                    }
                    else
                    {
                        unifiedMarketplace.CicekSepeti_Gonderildi = null;
                        unifiedMarketplace.CicekSepeti_Success = null;
                        unifiedMarketplace.CicekSepeti_Message = null;
                        unifiedMarketplace.CicekSepeti_ReportId = null;
                        unifiedMarketplace.CicekSepeti_GuncellenmeTarihi = DateTime.Now;
                        unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                    }

                    // Farmazon ürün ekleme
                    if (unifiedMarketplace.Farmazon_Stock != null)
                    {
                        var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == 10).FirstOrDefault();
                        if (girisBilgisi == null)
                        {
                            unifiedMarketplace.Farmazon_Gonderildi = 0;
                            unifiedMarketplace.Farmazon_Success = false;
                            unifiedMarketplace.Farmazon_Message = "API kimlik bilgileri bulunamadı.";
                            unifiedMarketplace.Farmazon_ReportId = string.Empty;
                            unifiedMarketplace.Farmazon_GuncellenmeTarihi = DateTime.Now;
                            unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                            results.Add(new { marketplace = "Farmazon", success = false, message = "API kimlik bilgileri bulunamadı." });
                        }
                        else
                        {
                            string baseUrl = "https://staging.lab.farmazon.com.tr/api";
                            using var client = new HttpClient();
                            client.DefaultRequestHeaders.Add("User-Agent", "API_alvit");

                            try
                            {
                                var loginData = new Dictionary<string, string>
                        {
                            { "username", girisBilgisi.KullaniciAdi },
                            { "password", girisBilgisi.Sifre },
                            { "clientName", girisBilgisi.ApiKey },
                            { "clientSecretKey", girisBilgisi.SecretKey }
                        };

                                var content = new FormUrlEncodedContent(loginData);
                                var loginResponse = await client.PostAsync($"{baseUrl}/v1/account/signin", content);
                                var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();

                                if (!loginResponse.IsSuccessStatusCode)
                                {
                                    unifiedMarketplace.Farmazon_Gonderildi = 0;
                                    unifiedMarketplace.Farmazon_Success = false;
                                    unifiedMarketplace.Farmazon_Message = $"Token alınamadı: {loginResponse.StatusCode} - {loginResponseContent}";
                                    unifiedMarketplace.Farmazon_ReportId = string.Empty;
                                    unifiedMarketplace.Farmazon_GuncellenmeTarihi = DateTime.Now;
                                    unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                                    results.Add(new { marketplace = "Farmazon", success = false, message = unifiedMarketplace.Farmazon_Message });
                                }
                                else
                                {
                                    var loginResult = JsonConvert.DeserializeObject<FarmazonLoginResponse>(loginResponseContent);
                                    if (loginResult.Result?.Token == null)
                                    {
                                        unifiedMarketplace.Farmazon_Gonderildi = 0;
                                        unifiedMarketplace.Farmazon_Success = false;
                                        unifiedMarketplace.Farmazon_Message = "Token alınamadı.";
                                        unifiedMarketplace.Farmazon_ReportId = string.Empty;
                                        unifiedMarketplace.Farmazon_GuncellenmeTarihi = DateTime.Now;
                                        unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                                        results.Add(new { marketplace = "Farmazon", success = false, message = "Token alınamadı." });
                                    }
                                    else
                                    {
                                        var productData = new FarmazonProductRequest
                                        {
                                            Id = unifiedMarketplace.Farmazon_Id,
                                            Price = unifiedMarketplace.Farmazon_Price,
                                            Stock = unifiedMarketplace.Farmazon_Stock,
                                            State = 1,
                                            Expiration = unifiedMarketplace.FarmaBorsa_ExpirationDate,
                                            MaxCount = unifiedMarketplace.Farmazon_MaxCount ?? 0,
                                            Description = unifiedMarketplace.Description,
                                            IsFeatured = unifiedMarketplace.Farmazon_IsFeatured,
                                            Vat = unifiedMarketplace.VatRate,
                                            Sku = unifiedMarketplace.Farmazon_StockCode
                                        };

                                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Result.Token);
                                        var requestList = new List<FarmazonProductRequest> { productData };
                                        var json = JsonConvert.SerializeObject(requestList);
                                        var productContent = new StringContent(json, Encoding.UTF8, "application/json");
                                        var response = await client.PostAsync($"{baseUrl}/v2/listings/createlistings", productContent);
                                        var responseContent = await response.Content.ReadAsStringAsync();

                                        if (response.IsSuccessStatusCode)
                                        {
                                            var result = JsonConvert.DeserializeObject<FarmazonProductResponse>(responseContent);
                                            var firstResult = result.Result?.FirstOrDefault();

                                            if (firstResult?.Success == true)
                                            {
                                                unifiedMarketplace.Farmazon_Gonderildi = 1;
                                                unifiedMarketplace.Farmazon_Success = true;
                                                unifiedMarketplace.Farmazon_Message = "Ürün başarıyla eklendi/güncellendi";
                                                unifiedMarketplace.Farmazon_ReportId = firstResult.RequestItem.ToString();
                                                unifiedMarketplace.Farmazon_GuncellenmeTarihi = DateTime.Now;
                                                unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                                results.Add(new
                                                {
                                                    marketplace = "Farmazon",
                                                    success = true,
                                                    message = unifiedMarketplace.Farmazon_Message,
                                                    reportId = unifiedMarketplace.Farmazon_ReportId
                                                });
                                            }
                                            else
                                            {
                                                var error = firstResult?.Errors?.FirstOrDefault();
                                                unifiedMarketplace.Farmazon_Gonderildi = 0;
                                                unifiedMarketplace.Farmazon_Success = false;
                                                unifiedMarketplace.Farmazon_Message = error?.Message ?? "Ürün eklenemedi";
                                                unifiedMarketplace.Farmazon_ReportId = string.Empty;
                                                unifiedMarketplace.Farmazon_GuncellenmeTarihi = DateTime.Now;
                                                unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                                results.Add(new
                                                {
                                                    marketplace = "Farmazon",
                                                    success = false,
                                                    message = unifiedMarketplace.Farmazon_Message
                                                });
                                            }
                                        }
                                        else
                                        {
                                            unifiedMarketplace.Farmazon_Gonderildi = 0;
                                            unifiedMarketplace.Farmazon_Success = false;
                                            unifiedMarketplace.Farmazon_Message = $"Hata: {(int)response.StatusCode} - {responseContent}";
                                            unifiedMarketplace.Farmazon_ReportId = string.Empty;
                                            unifiedMarketplace.Farmazon_GuncellenmeTarihi = DateTime.Now;
                                            unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                            results.Add(new
                                            {
                                                marketplace = "Farmazon",
                                                success = false,
                                                message = unifiedMarketplace.Farmazon_Message
                                            });
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                unifiedMarketplace.Farmazon_Gonderildi = 0;
                                unifiedMarketplace.Farmazon_Success = false;
                                unifiedMarketplace.Farmazon_Message = $"Hata: {ex.Message}";
                                unifiedMarketplace.Farmazon_ReportId = string.Empty;
                                unifiedMarketplace.Farmazon_GuncellenmeTarihi = DateTime.Now;
                                unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                results.Add(new
                                {
                                    marketplace = "Farmazon",
                                    success = false,
                                    message = unifiedMarketplace.Farmazon_Message
                                });
                            }
                        }
                    }
                    else
                    {
                        unifiedMarketplace.Farmazon_Gonderildi = null;
                        unifiedMarketplace.Farmazon_Success = null;
                        unifiedMarketplace.Farmazon_Message = null;
                        unifiedMarketplace.Farmazon_ReportId = null;
                        unifiedMarketplace.Farmazon_GuncellenmeTarihi = DateTime.Now;
                        unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                    }

                    // FarmaBorsa ürün ekleme
                    if (unifiedMarketplace.FarmaBorsa_StockQuantity != null)
                    {
                        var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == 1).FirstOrDefault();
                        if (girisBilgisi == null)
                        {
                            unifiedMarketplace.FarmaBorsa_Gonderildi = 0;
                            unifiedMarketplace.FarmaBorsa_Success = false;
                            unifiedMarketplace.FarmaBorsa_Message = "API kimlik bilgileri bulunamadı.";
                            unifiedMarketplace.FarmaBorsa_GuncellenmeTarihi = DateTime.Now;
                            unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                            results.Add(new { marketplace = "FarmaBorsa", success = false, message = "API kimlik bilgileri bulunamadı." });
                        }
                        else
                        {
                            string apiUrl = "https://wapi.farmaborsa.com/api/Entegre/IlanKaydet";
                            using var client = new HttpClient();

                            try
                            {
                                var input = new
                                {
                                    nick = girisBilgisi.KullaniciAdi ?? "",
                                    parola = girisBilgisi.Sifre ?? "",
                                    apiKey = girisBilgisi.ApiKey ?? "",
                                    urunAd = unifiedMarketplace.Title ?? "",
                                    adet = unifiedMarketplace.FarmaBorsa_StockQuantity ?? 1,
                                    maxAdet = unifiedMarketplace.FarmaBorsa_MaxQuantity ?? 1,
                                    op = 0.0,
                                    tutar = unifiedMarketplace.FarmaBorsa_SalePrice,
                                    borsadaGoster = true,
                                    miad = unifiedMarketplace.FarmaBorsa_ExpirationDate ?? "",
                                    miadTip = unifiedMarketplace.FarmaBorsa_MiadTip ?? 0,
                                    barkod = unifiedMarketplace.Barcode ?? "",
                                    resimUrl = imageUrls.FirstOrDefault() ?? ""
                                };

                                var json = JsonConvert.SerializeObject(input);
                                var content = new StringContent(json, Encoding.UTF8, "application/json");
                                var response = await client.PostAsync(apiUrl, content);
                                var responseContent = await response.Content.ReadAsStringAsync();

                                if (response.IsSuccessStatusCode)
                                {
                                    var result = JsonConvert.DeserializeObject<FarmaBorsaProductResponse>(responseContent);
                                    unifiedMarketplace.FarmaBorsa_Gonderildi = !result.hata ? 1 : 0;
                                    unifiedMarketplace.FarmaBorsa_Success = !result.hata;
                                    unifiedMarketplace.FarmaBorsa_Message = result.mesaj;
                                    unifiedMarketplace.FarmaBorsa_GuncellenmeTarihi = DateTime.Now;
                                    unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                    results.Add(new
                                    {
                                        marketplace = "FarmaBorsa",
                                        success = !result.hata,
                                        message = unifiedMarketplace.FarmaBorsa_Message
                                    });
                                }
                                else
                                {
                                    unifiedMarketplace.FarmaBorsa_Gonderildi = 0;
                                    unifiedMarketplace.FarmaBorsa_Success = false;
                                    unifiedMarketplace.FarmaBorsa_Message = $"Hata: {(int)response.StatusCode} - {responseContent}";
                                    unifiedMarketplace.FarmaBorsa_GuncellenmeTarihi = DateTime.Now;
                                    unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                    results.Add(new
                                    {
                                        marketplace = "FarmaBorsa",
                                        success = false,
                                        message = unifiedMarketplace.FarmaBorsa_Message
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                unifiedMarketplace.FarmaBorsa_Gonderildi = 0;
                                unifiedMarketplace.FarmaBorsa_Success = false;
                                unifiedMarketplace.FarmaBorsa_Message = $"Hata: {ex.Message}";
                                unifiedMarketplace.FarmaBorsa_GuncellenmeTarihi = DateTime.Now;
                                unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                                results.Add(new
                                {
                                    marketplace = "FarmaBorsa",
                                    success = false,
                                    message = unifiedMarketplace.FarmaBorsa_Message
                                });
                            }
                        }
                    }
                    else
                    {
                        unifiedMarketplace.FarmaBorsa_Gonderildi = null;
                        unifiedMarketplace.FarmaBorsa_Success = null;
                        unifiedMarketplace.FarmaBorsa_Message = null;
                        unifiedMarketplace.FarmaBorsa_GuncellenmeTarihi = DateTime.Now;
                        unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                    }

                    // Trendyol ürün ekleme (örnek olarak eklenmedi, gerekirse eklenebilir)
                    if (unifiedMarketplace.Trendyol_CategoryId != null)
                    {
                        unifiedMarketplace.Trendyol_Gonderildi = 0;
                        unifiedMarketplace.Trendyol_Success = false;
                        unifiedMarketplace.Trendyol_Message = "Trendyol entegrasyonu henüz uygulanmadı.";
                        unifiedMarketplace.Trendyol_GuncellenmeTarihi = DateTime.Now;
                        unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                        results.Add(new { marketplace = "Trendyol", success = false, message = "Trendyol entegrasyonu henüz uygulanmadı." });
                    }
                    else
                    {
                        unifiedMarketplace.Trendyol_Gonderildi = null;
                        unifiedMarketplace.Trendyol_Success = null;
                        unifiedMarketplace.Trendyol_Message = null;
                        unifiedMarketplace.Trendyol_GuncellenmeTarihi = DateTime.Now;
                        unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                    }

                    // Genel Gonderildi durumunu kontrol et
                    unifiedMarketplace.Gonderildi = new[]
                    {
                unifiedMarketplace.PTT_Gonderildi,
                unifiedMarketplace.N11_Gonderildi,
                unifiedMarketplace.GoTurc_Gonderildi,
                unifiedMarketplace.Idefix_Gonderildi,
                unifiedMarketplace.Pazarama_Gonderildi,
                unifiedMarketplace.CicekSepeti_Gonderildi,
                unifiedMarketplace.Farmazon_Gonderildi,
                unifiedMarketplace.FarmaBorsa_Gonderildi,
                unifiedMarketplace.Trendyol_Gonderildi
            }.Any(g => g == 1) ? 1 : 0;

                    unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);

                    if (!results.Any())
                    {
                        return Json(new { success = false, message = "Hiçbir pazar yeri işlenmedi." });
                    }

                    return Json(new { success = true, results });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Ürün kaydedilirken bir hata oluştu: {ex.Message}");
                    return View("Index", request);
                }
            }

            try
            {
                var imageUrls = new List<string>();
                if (request.ImgFiles != null && request.ImgFiles.Any())
                {
                    foreach (var file in request.ImgFiles)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/urungorsel", fileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            imageUrls.Add($"https://ultrasonluzibin.com/urungorsel/{fileName}");

                        }
                    }
                }

                var entity = new UnifiedMarketplace
                {
                    Title = request.Title,
                    UrunId = request.UrunId,
                    ProductCode = request.ProductCode,
                    Barcode = request.Barcode,
                    Description = request.Description,
                    ListPrice = request.ListPrice,
                    VatRate = request.VatRate,
                    ImgFilesJson = JsonConvert.SerializeObject(imageUrls),
                    Durumu = 1,

                    // PTT
                    PTT_CategoryId = request.PTT?.CategoryId,
                    PTT_SalePrice = request.PTT?.SalePrice,
                    PTT_PriceWithVat = request.PTT?.PriceWithVat,
                    PTT_StockQuantity = request.PTT?.StockQuantity,
                    PTT_Desi = request.PTT?.Desi,
                    PTT_Discount = request.PTT?.Discount,
                    PTT_EstimatedCourierDelivery = request.PTT?.EstimatedCourierDelivery,
                    PTT_WarrantyDuration = request.PTT?.WarrantyDuration,
                    PTT_CargoProfileId = request.PTT?.CargoProfileId,
                    PTT_BasketMaxQuantity = request.PTT?.BasketMaxQuantity,
                    PTT_VariantsJson = JsonConvert.SerializeObject(request.PTT?.Variants),
                    PTT_EklenmeTarihi = DateTime.Now,
                    PTT_GuncellenmeTarihi = DateTime.Now,
                    PTT_Gonderildi = request.PTT?.CategoryId != null ? 0 : null,
                    PTT_Success = request.PTT?.CategoryId != null ? false : null,
                    PTT_Message = request.PTT?.CategoryId != null ? "Ürün henüz gönderilmedi." : null,

                    // N11
                    N11_CategoryId = request.N11?.CategoryId,
                    N11_SalePrice = request.N11?.SalePrice,
                    N11_StockQuantity = request.N11?.StockQuantity,
                    N11_EklenmeTarihi = DateTime.Now,
                    N11_GuncellenmeTarihi = DateTime.Now,
                    N11_Gonderildi = request.N11?.CategoryId != null ? 0 : null,
                    N11_Success = request.N11?.CategoryId != null ? false : null,
                    N11_Message = request.N11?.CategoryId != null ? "Ürün henüz gönderilmedi." : null,

                    // GoTurc
                    GoTurc_CategoryId = request.GoTurc?.CategoryId,
                    GoTurc_BrandId = request.GoTurc?.BrandId,
                    GoTurc_DeliveryNo = request.GoTurc?.DeliveryNo,
                    GoTurc_SalePrice = request.GoTurc?.SalePrice,
                    GoTurc_StockQuantity = request.GoTurc?.StockQuantity,
                    GoTurc_EklenmeTarihi = DateTime.Now,
                    GoTurc_GuncellenmeTarihi = DateTime.Now,
                    GoTurc_Gonderildi = request.GoTurc?.CategoryId != null ? 0 : null,
                    GoTurc_Success = request.GoTurc?.CategoryId != null ? false : null,
                    GoTurc_Message = request.GoTurc?.CategoryId != null ? "Ürün henüz gönderilmedi." : null,

                    // Idefix
                    Idefix_CategoryId = request.Idefix?.CategoryId,
                    Idefix_BrandId = request.Idefix?.BrandId,
                    Idefix_SalePrice = request.Idefix?.SalePrice,
                    Idefix_StockQuantity = request.Idefix?.StockQuantity,
                    Idefix_VendorStockCode = request.Idefix?.VendorStockCode,
                    Idefix_Weight = request.Idefix?.Weight,
                    Idefix_ComparePrice = request.Idefix?.ComparePrice,
                    Idefix_ProductMainId = request.Idefix?.ProductMainId,
                    Idefix_DeliveryDuration = request.Idefix?.DeliveryDuration,
                    Idefix_CargoCompanyId = request.Idefix?.CargoCompanyId,
                    Idefix_ShipmentAddressId = request.Idefix?.ShipmentAddressId,
                    Idefix_ReturnAddressId = request.Idefix?.ReturnAddressId,
                    Idefix_AttributesJson = JsonConvert.SerializeObject(request.Idefix?.Attributes),
                    Idefix_EklenmeTarihi = DateTime.Now,
                    Idefix_GuncellenmeTarihi = DateTime.Now,
                    Idefix_Gonderildi = request.Idefix?.CategoryId != null ? 0 : null,
                    Idefix_Success = request.Idefix?.CategoryId != null ? false : null,
                    Idefix_Message = request.Idefix?.CategoryId != null ? "Ürün henüz gönderilmedi." : null,

                    // Pazarama
                    Pazarama_CategoryId = request.Pazarama?.CategoryId,
                    Pazarama_BrandId = request.Pazarama?.BrandId,
                    Pazarama_SalePrice = request.Pazarama?.SalePrice,
                    Pazarama_StockQuantity = request.Pazarama?.StockQuantity,
                    Pazarama_EklenmeTarihi = DateTime.Now,
                    Pazarama_GuncellenmeTarihi = DateTime.Now,
                    Pazarama_Gonderildi = request.Pazarama?.CategoryId != null ? 0 : null,
                    Pazarama_Success = request.Pazarama?.CategoryId != null ? false : null,
                    Pazarama_Message = request.Pazarama?.CategoryId != null ? "Ürün henüz gönderilmedi." : null,

                    // ÇiçekSepeti
                    CicekSepeti_CategoryId = request.CicekSepeti?.CategoryId,
                    CicekSepeti_SalePrice = request.CicekSepeti?.SalePrice,
                    CicekSepeti_StockQuantity = request.CicekSepeti?.StockQuantity,
                    CicekSepeti_StockCode = request.CicekSepeti?.StockCode,
                    CicekSepeti_CategoryName = request.CicekSepeti?.CategoryName,
                    CicekSepeti_ProductStatusType = request.CicekSepeti?.ProductStatusType,
                    CicekSepeti_VariantName = request.CicekSepeti?.VariantName,
                    CicekSepeti_EklenmeTarihi = DateTime.Now,
                    CicekSepeti_GuncellenmeTarihi = DateTime.Now,
                    CicekSepeti_Gonderildi = request.CicekSepeti?.CategoryId != null ? 0 : null,
                    CicekSepeti_Success = request.CicekSepeti?.CategoryId != null ? false : null,
                    CicekSepeti_Message = request.CicekSepeti?.CategoryId != null ? "Ürün henüz gönderilmedi." : null,

                    // Farmazon
                    Farmazon_Id = request.Farmazon?.Id,
                    Farmazon_Price = request.Farmazon?.Price,
                    Farmazon_Stock = request.Farmazon?.Stock,
                    Farmazon_State = request.Farmazon?.State,
                    Farmazon_Expiration = request.Farmazon?.Expiration,
                    Farmazon_MaxCount = request.Farmazon?.MaxCount,
                    Farmazon_IsFeatured = request.Farmazon?.IsFeatured,
                    Farmazon_FarmazonEnabled = request.Farmazon?.FarmazonEnabled,
                    Farmazon_StockCode = request.Farmazon?.StockCode,
                    Farmazon_EklenmeTarihi = DateTime.Now,
                    Farmazon_GuncellenmeTarihi = DateTime.Now,
                    Farmazon_Gonderildi = request.Farmazon?.Id != null ? 0 : null,
                    Farmazon_Success = request.Farmazon?.Id != null ? false : null,
                    Farmazon_Message = request.Farmazon?.Id != null ? "Ürün henüz gönderilmedi." : null,

                    // FarmaBorsa
                    FarmaBorsa_SalePrice = request.FarmaBorsa?.SalePrice,
                    FarmaBorsa_StockQuantity = request.FarmaBorsa?.StockQuantity,
                    FarmaBorsa_MaxQuantity = request.FarmaBorsa?.MaxQuantity,
                    FarmaBorsa_ExpirationDate = request.FarmaBorsa?.ExpirationDate,
                    FarmaBorsa_ShowInBorsa = true,
                    FarmaBorsa_FarmaBorsaEnabled = request.FarmaBorsa?.FarmaBorsaEnabled,
                    FarmaBorsa_MiadTip = request.FarmaBorsa?.MiadTip,
                    FarmaBorsa_EklenmeTarihi = DateTime.Now,
                    FarmaBorsa_GuncellenmeTarihi = DateTime.Now,
                    FarmaBorsa_Gonderildi = request.FarmaBorsa?.StockQuantity != null ? 0 : null,
                    FarmaBorsa_Success = request.FarmaBorsa?.StockQuantity != null ? false : null,
                    FarmaBorsa_Message = request.FarmaBorsa?.StockQuantity != null ? "Ürün henüz gönderilmedi." : null,

                    // Trendyol
                    Trendyol_CategoryId = request.Trendyol?.CategoryId,
                    Trendyol_BrandId = request.Trendyol?.BrandId,
                    Trendyol_SalePrice = request.Trendyol?.SalePrice,
                    Trendyol_StockQuantity = request.Trendyol?.StockQuantity,
                    Trendyol_EklenmeTarihi = DateTime.Now,
                    Trendyol_GuncellenmeTarihi = DateTime.Now,
                    Trendyol_Gonderildi = request.Trendyol?.CategoryId != null ? 0 : null,
                    Trendyol_Success = request.Trendyol?.CategoryId != null ? false : null,
                    Trendyol_Message = request.Trendyol?.CategoryId != null ? "Ürün henüz gönderilmedi." : null
                };

                _repository.Ekle(entity);

                if (imageUrls.Any())
                {
                    foreach (var url in imageUrls)
                    {
                        var gorsel = new PazarYeriGorsel
                        {
                            UnifiedMarketplaceId = entity.Id,
                            FotografYolu = url,
                            Durumu = 1,
                            EklenmeTarihi = DateTime.Now,
                            GuncellenmeTarihi = DateTime.Now
                        };
                        _pazarYeriGorselRepository.Ekle(gorsel);
                    }
                }
                // PTT ürün ekleme
                if (request.PTT?.CategoryId != null)
                {
                    var pazarYeriId = 4;
                    var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
                    if (girisBilgisi == null)
                    {
                        entity.PTT_Gonderildi = 0;
                        entity.PTT_Success = false;
                        entity.PTT_Message = "API kimlik bilgileri bulunamadı.";
                        entity.PTT_GuncellenmeTarihi = DateTime.Now;
                        _repository.Guncelle(entity);
                        results.Add(new { marketplace = "PTT", success = false, message = "API kimlik bilgileri bulunamadı." });
                    }
                    else
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
                            await client.OpenAsync();
                            var productRequest = new PTTServiceReference.ProductV3Request
                            {
                                Active = true,
                                Barcode = request.Barcode,
                                BasketMaxQuantity = request.PTT.BasketMaxQuantity ?? 1000,
                                CargoProfileId = request.PTT.CargoProfileId ?? 0,
                                CategoryId = (int?)request.PTT.CategoryId,
                                Desi = (double?)request.PTT.Desi,
                                Discount = request.PTT.Discount ?? 0,
                                EstimatedCourierDelivery = request.PTT.EstimatedCourierDelivery ?? 5,
                                Images = imageUrls.Select(url => new PTTServiceReference.ProductImageV3 { Url = url }).ToArray(),
                                LongDescription = request.Description,
                                Name = request.Title,
                                NoShippingProduct = false,
                                PriceWithVat = request.PTT.PriceWithVat ?? 0,
                                PriceWithoutVat = request.PTT.SalePrice,
                                ProductCode = request.Barcode,
                                Quantity = request.PTT.StockQuantity,
                                SingleBox = false,
                                VATRate = request.VatRate,
                                WarrantyDuration = request.PTT.WarrantyDuration ?? 0,
                                Variants = request.PTT.Variants?.Select(v => new PTTServiceReference.VariantV3
                                {
                                    VariantBarcode = v.Barcode,
                                    Price = v.Price ?? 0,
                                    Quantity = v.Quantity ?? 0,
                                    Attributes = v.Attributes?.Select(a => new PTTServiceReference.VariantAttrV3
                                    {
                                        Definition = a.Definition,
                                        Value = a.Value
                                    }).ToArray()
                                }).ToArray()
                            };

                            var response = await client.UpdateProductsV3Async(new[] { productRequest });
                            entity.PTT_Gonderildi = response.Success ? 1 : 0;
                            entity.PTT_Success = response.Success;
                            entity.PTT_Message = response.Success ? "Ürün başarıyla eklendi/güncellendi" : response.Message;
                            entity.PTT_TrackingId = response.TrackingId ?? string.Empty;
                            entity.PTT_GuncellenmeTarihi = DateTime.Now;
                            _repository.Guncelle(entity);

                            results.Add(new
                            {
                                marketplace = "PTT",
                                success = response.Success,
                                message = entity.PTT_Message,
                                trackingId = entity.PTT_TrackingId
                            });
                        }
                        catch (Exception ex)
                        {
                            entity.PTT_Gonderildi = 0;
                            entity.PTT_Success = false;
                            entity.PTT_Message = $"Ürün eklenirken hata oluştu: {ex.Message}";
                            entity.PTT_TrackingId = string.Empty;
                            entity.PTT_GuncellenmeTarihi = DateTime.Now;
                            _repository.Guncelle(entity);

                            results.Add(new
                            {
                                marketplace = "PTT",
                                success = false,
                                message = entity.PTT_Message
                            });
                        }
                        finally
                        {
                            try
                            {
                                if (client.State != CommunicationState.Faulted)
                                    await client.CloseAsync();
                                else
                                    client.Abort();
                            }
                            catch
                            {
                                client.Abort();
                            }
                        }
                    }
                }
                else
                {
                    entity.PTT_Gonderildi = null;
                    entity.PTT_Success = null;
                    entity.PTT_Message = null;
                    entity.PTT_TrackingId = null;
                    entity.PTT_GuncellenmeTarihi = DateTime.Now;
                    _repository.Guncelle(entity);
                }

                // N11 ürün ekleme
                if (request.N11?.CategoryId != null)
                {
                    var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == 3).FirstOrDefault();
                    if (girisBilgisi == null)
                    {
                        entity.N11_Gonderildi = 0;
                        entity.N11_Success = false;
                        entity.N11_Message = "API kimlik bilgileri bulunamadı.";
                        entity.N11_ReportId = 0;
                        entity.N11_GuncellenmeTarihi = DateTime.Now;
                        _repository.Guncelle(entity);
                        results.Add(new { marketplace = "N11", success = false, message = "API kimlik bilgileri bulunamadı." });
                    }
                    else
                    {
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
                                    title = entity.Title,
                                    description = entity.Description,
                                    categoryId = entity.N11_CategoryId,
                                    currencyType = "TL",
                                    productMainId = entity.Barcode,
                                    preparingDay = entity.PTT_EstimatedCourierDelivery ?? 5,
                                    shipmentTemplate = "ALVİT",
                                    maxPurchaseQuantity = entity.PTT_BasketMaxQuantity,
                                    stockCode = entity.CicekSepeti_StockCode,
                                    barcode = entity.Barcode,
                                    quantity = entity.N11_StockQuantity,
                                    images = imageUrls.Select((url, index) => new { url, order = index + 1 }).ToArray(),
                                    salePrice = entity.N11_SalePrice,
                                    listPrice = entity.ListPrice,
                                    vatRate = entity.VatRate
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

                                entity.N11_Gonderildi = result.status == "IN_QUEUE" ? 1 : 0;
                                entity.N11_Success = result.status == "IN_QUEUE";
                                entity.N11_Message = result.status == "IN_QUEUE" ? $"Ürün başarıyla işleme alındı. Task ID: {result.id}" : $"Hata: {string.Join(", ", result.reasons)}";
                                entity.N11_ReportId = result.id;
                                entity.N11_GuncellenmeTarihi = DateTime.Now;
                                _repository.Guncelle(entity);

                                results.Add(new
                                {
                                    marketplace = "N11",
                                    success = result.status == "IN_QUEUE",
                                    message = entity.N11_Message,
                                    reportId = entity.N11_ReportId
                                });
                            }
                            else
                            {
                                var errorContent = await response.Content.ReadAsStringAsync();
                                entity.N11_Gonderildi = 0;
                                entity.N11_Success = false;
                                entity.N11_Message = $"Hata: {(int)response.StatusCode} - {errorContent}";
                                entity.N11_ReportId = 0;
                                entity.N11_GuncellenmeTarihi = DateTime.Now;
                                _repository.Guncelle(entity);

                                results.Add(new
                                {
                                    marketplace = "N11",
                                    success = false,
                                    message = entity.N11_Message
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            entity.N11_Gonderildi = 0;
                            entity.N11_Success = false;
                            entity.N11_Message = $"Hata: {ex.Message}";
                            entity.N11_ReportId = 0;
                            entity.N11_GuncellenmeTarihi = DateTime.Now;
                            _repository.Guncelle(entity);

                            results.Add(new
                            {
                                marketplace = "N11",
                                success = false,
                                message = entity.N11_Message
                            });
                        }
                    }
                }
                else
                {
                    entity.N11_Gonderildi = null;
                    entity.N11_Success = null;
                    entity.N11_Message = null;
                    entity.N11_ReportId = null;
                    entity.N11_GuncellenmeTarihi = DateTime.Now;
                    _repository.Guncelle(entity);
                }

                // GoTurc ürün ekleme
                if (request.GoTurc?.CategoryId != null)
                {
                    var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == 8).FirstOrDefault();
                    if (girisBilgisi == null)
                    {
                        entity.GoTurc_Gonderildi = 0;
                        entity.GoTurc_Success = false;
                        entity.GoTurc_Message = "API kimlik bilgileri bulunamadı.";
                        entity.GoTurc_ReportId = string.Empty;
                        entity.GoTurc_GuncellenmeTarihi = DateTime.Now;
                        _repository.Guncelle(entity);
                        results.Add(new { marketplace = "GoTurc", success = false, message = "API kimlik bilgileri bulunamadı." });
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(entity.Title) || entity.Title.Length < 5 || entity.Title.Length > 65)
                        {
                            entity.GoTurc_Gonderildi = 0;
                            entity.GoTurc_Success = false;
                            entity.GoTurc_Message = "Ürün başlığı 5 ile 65 karakter arasında olmalıdır.";
                            entity.GoTurc_ReportId = string.Empty;
                            entity.GoTurc_GuncellenmeTarihi = DateTime.Now;
                            _repository.Guncelle(entity);
                            results.Add(new { marketplace = "GoTurc", success = false, message = "Ürün başlığı 5 ile 65 karakter arasında olmalıdır." });
                        }
                        else if (entity.GoTurc_DeliveryNo <= 0)
                        {
                            entity.GoTurc_Gonderildi = 0;
                            entity.GoTurc_Success = false;
                            entity.GoTurc_Message = "Geçerli teslimat numarası gereklidir.";
                            entity.GoTurc_ReportId = string.Empty;
                            entity.GoTurc_GuncellenmeTarihi = DateTime.Now;
                            _repository.Guncelle(entity);
                            results.Add(new { marketplace = "GoTurc", success = false, message = "Geçerli teslimat numarası gereklidir." });
                        }
                        else
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
                                    entity.GoTurc_Gonderildi = 0;
                                    entity.GoTurc_Success = false;
                                    entity.GoTurc_Message = "Token alınamadı.";
                                    entity.GoTurc_ReportId = string.Empty;
                                    entity.GoTurc_GuncellenmeTarihi = DateTime.Now;
                                    _repository.Guncelle(entity);
                                    results.Add(new { marketplace = "GoTurc", success = false, message = "Token alınamadı." });
                                }
                                else
                                {
                                    var apiUrl = $"{baseUrl}api/Product";
                                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                                    GoTurcProduct product = new GoTurcProduct
                                    {
                                        ShopProductCode = entity.Barcode,
                                        Title = entity.Title,
                                        SubTitle = entity.Title,
                                        StockQuantity = entity.GoTurc_StockQuantity ?? 0,
                                        Price = entity.GoTurc_SalePrice ?? 0,
                                        DescriptionHTML = entity.Description,
                                        ImageUrl = imageUrls,
                                        Status = "Aktif",
                                        CategoryId = entity.GoTurc_CategoryId ?? 0,
                                        BrandId = entity.GoTurc_BrandId ?? 0,
                                        DeliveryNo = Convert.ToInt32(entity.GoTurc_DeliveryNo)
                                    };

                                    var apiRequest = new List<GoTurcProduct> { product };
                                    var json = JsonConvert.SerializeObject(apiRequest);
                                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                                    var response = await client.PostAsync(apiUrl, content);

                                    if (response.IsSuccessStatusCode)
                                    {
                                        var responseJson = await response.Content.ReadAsStringAsync();
                                        var result = JsonConvert.DeserializeObject<GoTurcProductResponse>(responseJson);
                                        entity.GoTurc_Gonderildi = 1;
                                        entity.GoTurc_Success = true;
                                        entity.GoTurc_Message = $"Ürün başarıyla eklendi/güncellendi. Rapor ID: {result.ReportId}";
                                        entity.GoTurc_ReportId = result.ReportId;
                                        entity.GoTurc_GuncellenmeTarihi = DateTime.Now;
                                        _repository.Guncelle(entity);

                                        results.Add(new
                                        {
                                            marketplace = "GoTurc",
                                            success = true,
                                            message = entity.GoTurc_Message,
                                            reportId = entity.GoTurc_ReportId
                                        });
                                    }
                                    else
                                    {
                                        var errorContent = await response.Content.ReadAsStringAsync();
                                        entity.GoTurc_Gonderildi = 0;
                                        entity.GoTurc_Success = false;
                                        entity.GoTurc_Message = $"API hatası: {(int)response.StatusCode} - {errorContent}";
                                        entity.GoTurc_ReportId = string.Empty;
                                        entity.GoTurc_GuncellenmeTarihi = DateTime.Now;
                                        _repository.Guncelle(entity);

                                        results.Add(new
                                        {
                                            marketplace = "GoTurc",
                                            success = false,
                                            message = entity.GoTurc_Message
                                        });
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                entity.GoTurc_Gonderildi = 0;
                                entity.GoTurc_Success = false;
                                entity.GoTurc_Message = $"Hata: {ex.Message}";
                                entity.GoTurc_ReportId = string.Empty;
                                entity.GoTurc_GuncellenmeTarihi = DateTime.Now;
                                _repository.Guncelle(entity);

                                results.Add(new
                                {
                                    marketplace = "GoTurc",
                                    success = false,
                                    message = entity.GoTurc_Message
                                });
                            }
                        }
                    }
                }
                else
                {
                    entity.GoTurc_Gonderildi = null;
                    entity.GoTurc_Success = null;
                    entity.GoTurc_Message = null;
                    entity.GoTurc_ReportId = null;
                    entity.GoTurc_GuncellenmeTarihi = DateTime.Now;
                    _repository.Guncelle(entity);
                }

                // Idefix ürün ekleme
                if (request.Idefix?.CategoryId != null)
                {
                    var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == 6).FirstOrDefault();
                    if (girisBilgisi == null)
                    {
                        entity.Idefix_Gonderildi = 0;
                        entity.Idefix_Success = false;
                        entity.Idefix_Message = "Pazar yeri bilgileri bulunamadı.";
                        entity.Idefix_ReportId = string.Empty;
                        entity.Idefix_GuncellenmeTarihi = DateTime.Now;
                        _repository.Guncelle(entity);
                        results.Add(new { marketplace = "Idefix", success = false, message = "Pazar yeri bilgileri bulunamadı." });
                    }
                    else
                    {
                        string vendorId = girisBilgisi.KullaniciAdi;
                        string apiKey = girisBilgisi.ApiKey;
                        string apiPass = girisBilgisi.SecretKey;
                        string vendorToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:{apiPass}"));
                        string url = $"https://merchantapi.idefix.com/pim/pool/{vendorId}/create";

                        using var client = new HttpClient();
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Add("X-API-KEY", vendorToken);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var product = new
                        {
                            barcode = entity.Barcode,
                            title = entity.Title,
                            productMainId = entity.ProductCode,
                            brandId = entity.Idefix_BrandId,
                            categoryId = entity.Idefix_CategoryId,
                            inventoryQuantity = entity.Idefix_StockQuantity,
                            vendorStockCode = entity.Idefix_VendorStockCode,
                            desi = entity.PTT_Desi ?? 0,
                            weight = entity.Idefix_Weight ?? 0,
                            description = entity.Description,
                            price = entity.ListPrice,
                            comparePrice = entity.Idefix_ComparePrice,
                            vatRate = entity.VatRate,
                            deliveryDuration = entity.Idefix_DeliveryDuration,
                            deliveryType = "regular",
                            cargoCompanyId = entity.Idefix_CargoCompanyId ?? null,
                            shipmentAddressId = entity.Idefix_ShipmentAddressId ?? null,
                            returnAddressId = entity.Idefix_ReturnAddressId ?? null,
                            images = imageUrls.Select(url => new { url }).ToArray(),
                            originCountryId = 792,
                            manufacturer = (string)null,
                            importer = (string)null,
                            ceCompatibility = (bool?)null
                        };

                        var requestData = new { products = new[] { product } };

                        try
                        {
                            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                            var response = await client.PostAsync(url, content);
                            string responseContent = await response.Content.ReadAsStringAsync();

                            if (response.IsSuccessStatusCode)
                            {
                                dynamic json = JsonConvert.DeserializeObject(responseContent);
                                entity.Idefix_Gonderildi = 1;
                                entity.Idefix_Success = true;
                                entity.Idefix_Message = "Ürün başarıyla eklendi/güncellendi";
                                entity.Idefix_ReportId = json.batchRequestId?.ToString() ?? string.Empty;
                                entity.Idefix_GuncellenmeTarihi = DateTime.Now;
                                _repository.Guncelle(entity);

                                results.Add(new
                                {
                                    marketplace = "Idefix",
                                    success = true,
                                    message = entity.Idefix_Message,
                                    batchRequestId = entity.Idefix_ReportId
                                });
                            }
                            else
                            {
                                entity.Idefix_Gonderildi = 0;
                                entity.Idefix_Success = false;
                                entity.Idefix_Message = $"Idefix API Hatası: {(int)response.StatusCode} - {responseContent}";
                                entity.Idefix_ReportId = string.Empty;
                                entity.Idefix_GuncellenmeTarihi = DateTime.Now;
                                _repository.Guncelle(entity);

                                results.Add(new
                                {
                                    marketplace = "Idefix",
                                    success = false,
                                    message = entity.Idefix_Message
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            entity.Idefix_Gonderildi = 0;
                            entity.Idefix_Success = false;
                            entity.Idefix_Message = $"Hata: {ex.Message}";
                            entity.Idefix_ReportId = string.Empty;
                            entity.Idefix_GuncellenmeTarihi = DateTime.Now;
                            _repository.Guncelle(entity);

                            results.Add(new
                            {
                                marketplace = "Idefix",
                                success = false,
                                message = entity.Idefix_Message
                            });
                        }
                    }
                }
                else
                {
                    entity.Idefix_Gonderildi = null;
                    entity.Idefix_Success = null;
                    entity.Idefix_Message = null;
                    entity.Idefix_ReportId = null;
                    entity.Idefix_GuncellenmeTarihi = DateTime.Now;
                    _repository.Guncelle(entity);
                }

                // Pazarama ürün ekleme
                if (request.Pazarama?.CategoryId != null)
                {
                    var pazarYeriId = 9;
                    var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
                    if (girisBilgisi == null)
                    {
                        entity.Pazarama_Gonderildi = 0;
                        entity.Pazarama_Success = false;
                        entity.Pazarama_Message = "API kimlik bilgileri bulunamadı.";
                        entity.Pazarama_ReportId = string.Empty;
                        entity.Pazarama_GuncellenmeTarihi = DateTime.Now;
                        _repository.Guncelle(entity);
                        results.Add(new { marketplace = "Pazarama", success = false, message = "API kimlik bilgileri bulunamadı." });
                    }
                    else
                    {
                        try
                        {
                            string token = await _pazaramaService.GetAccessToken(girisBilgisi.ApiKey, girisBilgisi.SecretKey);
                            if (string.IsNullOrEmpty(token))
                            {
                                entity.Pazarama_Gonderildi = 0;
                                entity.Pazarama_Success = false;
                                entity.Pazarama_Message = "Token alınamadı.";
                                entity.Pazarama_ReportId = string.Empty;
                                entity.Pazarama_GuncellenmeTarihi = DateTime.Now;
                                _repository.Guncelle(entity);
                                results.Add(new { marketplace = "Pazarama", success = false, message = "Token alınamadı." });
                            }
                            else
                            {
                                var productData = new
                                {
                                    categoryId = entity.Pazarama_CategoryId,
                                    brandId = entity.Pazarama_BrandId,
                                    barcode = entity.Barcode,
                                    title = entity.Title,
                                    description = entity.Description,
                                    salePrice = entity.Pazarama_SalePrice,
                                    listPrice = entity.ListPrice,
                                    stockQuantity = entity.Pazarama_StockQuantity,
                                    vatRate = entity.VatRate,
                                    images = imageUrls.Select((url, index) => new { url, order = index + 1 }).ToArray()
                                };

                                var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.pazarama.com/products")
                                {
                                    Content = new StringContent(JsonConvert.SerializeObject(productData), Encoding.UTF8, "application/json")
                                };
                                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                                var response = await _httpClient.SendAsync(requestMessage);
                                var responseContent = await response.Content.ReadAsStringAsync();

                                if (response.IsSuccessStatusCode)
                                {
                                    var result = JsonConvert.DeserializeObject<dynamic>(responseContent);
                                    entity.Pazarama_Gonderildi = 1;
                                    entity.Pazarama_Success = true;
                                    entity.Pazarama_Message = "Ürün başarıyla eklendi/güncellendi";
                                    entity.Pazarama_ReportId = result?.id?.ToString() ?? string.Empty;
                                    entity.Pazarama_GuncellenmeTarihi = DateTime.Now;
                                    _repository.Guncelle(entity);

                                    results.Add(new
                                    {
                                        marketplace = "Pazarama",
                                        success = true,
                                        message = entity.Pazarama_Message,
                                        reportId = entity.Pazarama_ReportId
                                    });
                                }
                                else
                                {
                                    entity.Pazarama_Gonderildi = 0;
                                    entity.Pazarama_Success = false;
                                    entity.Pazarama_Message = $"Hata: {(int)response.StatusCode} - {responseContent}";
                                    entity.Pazarama_ReportId = string.Empty;
                                    entity.Pazarama_GuncellenmeTarihi = DateTime.Now;
                                    _repository.Guncelle(entity);

                                    results.Add(new
                                    {
                                        marketplace = "Pazarama",
                                        success = false,
                                        message = entity.Pazarama_Message
                                    });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            entity.Pazarama_Gonderildi = 0;
                            entity.Pazarama_Success = false;
                            entity.Pazarama_Message = $"Hata: {ex.Message}";
                            entity.Pazarama_ReportId = string.Empty;
                            entity.Pazarama_GuncellenmeTarihi = DateTime.Now;
                            _repository.Guncelle(entity);

                            results.Add(new
                            {
                                marketplace = "Pazarama",
                                success = false,
                                message = entity.Pazarama_Message
                            });
                        }
                    }
                }
                else
                {
                    entity.Pazarama_Gonderildi = null;
                    entity.Pazarama_Success = null;
                    entity.Pazarama_Message = null;
                    entity.Pazarama_ReportId = null;
                    entity.Pazarama_GuncellenmeTarihi = DateTime.Now;
                    _repository.Guncelle(entity);
                }

                // ÇiçekSepeti ürün ekleme
                if (request.CicekSepeti?.CategoryId != null)
                {
                    var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == 5).FirstOrDefault();
                    if (girisBilgisi == null)
                    {
                        entity.CicekSepeti_Gonderildi = 0;
                        entity.CicekSepeti_Success = false;
                        entity.CicekSepeti_Message = "API kimlik bilgileri bulunamadı.";
                        entity.CicekSepeti_ReportId = string.Empty;
                        entity.CicekSepeti_GuncellenmeTarihi = DateTime.Now;
                        _repository.Guncelle(entity);
                        results.Add(new { marketplace = "CicekSepeti", success = false, message = "API kimlik bilgileri bulunamadı." });
                    }
                    else
                    {
                        string apiUrl = "https://api.ciceksepeti.com/api/v1/products";
                        using var client = new HttpClient();
                        client.DefaultRequestHeaders.Add("x-api-key", girisBilgisi.ApiKey);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        try
                        {
                            var productData = new
                            {
                                ProductName = entity.Title,
                                ProductCode = entity.ProductCode,
                                StockCode = entity.CicekSepeti_StockCode,
                                IsActive = true,
                                CategoryId = entity.CicekSepeti_CategoryId ?? 0,
                                ProductStatusType = entity.CicekSepeti_ProductStatusType,
                                IsUseStockQuantity = true,
                                StockQuantity = entity.CicekSepeti_StockQuantity ?? 0,
                                SalesPrice = Convert.ToDouble(entity.CicekSepeti_SalePrice),
                                ListPrice = Convert.ToDouble(entity.ListPrice),
                                Barcode = entity.Barcode,
                                Images = imageUrls
                            };

                            var json = JsonConvert.SerializeObject(productData);
                            var content = new StringContent(json, Encoding.UTF8, "application/json");
                            var response = await client.PostAsync(apiUrl, content);
                            var responseContent = await response.Content.ReadAsStringAsync();

                            if (response.IsSuccessStatusCode)
                            {
                                var result = JsonConvert.DeserializeObject<dynamic>(responseContent);
                                entity.CicekSepeti_Gonderildi = 1;
                                entity.CicekSepeti_Success = true;
                                entity.CicekSepeti_Message = "Ürün başarıyla eklendi/güncellendi";
                                entity.CicekSepeti_ReportId = result?.id?.ToString() ?? string.Empty;
                                entity.CicekSepeti_GuncellenmeTarihi = DateTime.Now;
                                _repository.Guncelle(entity);

                                results.Add(new
                                {
                                    marketplace = "CicekSepeti",
                                    success = true,
                                    message = entity.CicekSepeti_Message,
                                    reportId = entity.CicekSepeti_ReportId
                                });
                            }
                            else
                            {
                                entity.CicekSepeti_Gonderildi = 0;
                                entity.CicekSepeti_Success = false;
                                entity.CicekSepeti_Message = $"Hata: {(int)response.StatusCode} - {responseContent}";
                                entity.CicekSepeti_ReportId = string.Empty;
                                entity.CicekSepeti_GuncellenmeTarihi = DateTime.Now;
                                _repository.Guncelle(entity);

                                results.Add(new
                                {
                                    marketplace = "CicekSepeti",
                                    success = false,
                                    message = entity.CicekSepeti_Message
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            entity.CicekSepeti_Gonderildi = 0;
                            entity.CicekSepeti_Success = false;
                            entity.CicekSepeti_Message = $"Hata: {ex.Message}";
                            entity.CicekSepeti_ReportId = string.Empty;
                            entity.CicekSepeti_GuncellenmeTarihi = DateTime.Now;
                            _repository.Guncelle(entity);

                            results.Add(new
                            {
                                marketplace = "CicekSepeti",
                                success = false,
                                message = entity.CicekSepeti_Message
                            });
                        }
                    }
                }
                else
                {
                    entity.CicekSepeti_Gonderildi = null;
                    entity.CicekSepeti_Success = null;
                    entity.CicekSepeti_Message = null;
                    entity.CicekSepeti_ReportId = null;
                    entity.CicekSepeti_GuncellenmeTarihi = DateTime.Now;
                    _repository.Guncelle(entity);
                }

                // Farmazon ürün ekleme
                if (request.Farmazon?.Stock != null)
                {
                    var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == 10).FirstOrDefault();
                    if (girisBilgisi == null)
                    {
                        entity.Farmazon_Gonderildi = 0;
                        entity.Farmazon_Success = false;
                        entity.Farmazon_Message = "API kimlik bilgileri bulunamadı.";
                        entity.Farmazon_ReportId = string.Empty;
                        entity.Farmazon_GuncellenmeTarihi = DateTime.Now;
                        _repository.Guncelle(entity);
                        results.Add(new { marketplace = "Farmazon", success = false, message = "API kimlik bilgileri bulunamadı." });
                    }
                    else
                    {
                        string baseUrl = "https://staging.lab.farmazon.com.tr/api";
                        using var client = new HttpClient();
                        client.DefaultRequestHeaders.Add("User-Agent", "API_alvit");

                        try
                        {
                            var loginData = new Dictionary<string, string>
                    {
                        { "username", girisBilgisi.KullaniciAdi },
                        { "password", girisBilgisi.Sifre },
                        { "clientName", girisBilgisi.ApiKey },
                        { "clientSecretKey", girisBilgisi.SecretKey }
                    };

                            var content = new FormUrlEncodedContent(loginData);
                            var loginResponse = await client.PostAsync($"{baseUrl}/v1/account/signin", content);
                            var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();

                            if (!loginResponse.IsSuccessStatusCode)
                            {
                                entity.Farmazon_Gonderildi = 0;
                                entity.Farmazon_Success = false;
                                entity.Farmazon_Message = $"Token alınamadı: {loginResponse.StatusCode} - {loginResponseContent}";
                                entity.Farmazon_ReportId = string.Empty;
                                entity.Farmazon_GuncellenmeTarihi = DateTime.Now;
                                _repository.Guncelle(entity);
                                results.Add(new { marketplace = "Farmazon", success = false, message = entity.Farmazon_Message });
                            }
                            else
                            {
                                var loginResult = JsonConvert.DeserializeObject<FarmazonLoginResponse>(loginResponseContent);
                                if (loginResult.Result?.Token == null)
                                {
                                    entity.Farmazon_Gonderildi = 0;
                                    entity.Farmazon_Success = false;
                                    entity.Farmazon_Message = "Token alınamadı.";
                                    entity.Farmazon_ReportId = string.Empty;
                                    entity.Farmazon_GuncellenmeTarihi = DateTime.Now;
                                    _repository.Guncelle(entity);
                                    results.Add(new { marketplace = "Farmazon", success = false, message = "Token alınamadı." });
                                }
                                else
                                {
                                    var productData = new FarmazonProductRequest
                                    {
                                        Id = entity.Farmazon_Id,
                                        Price = entity.Farmazon_Price,
                                        Stock = entity.Farmazon_Stock,
                                        State = 1,
                                        Expiration = entity.FarmaBorsa_ExpirationDate,
                                        MaxCount = entity.Farmazon_MaxCount ?? 0,
                                        Description = entity.Description,
                                        IsFeatured = entity.Farmazon_IsFeatured,
                                        Vat = entity.VatRate,
                                        Sku = entity.Farmazon_StockCode
                                    };

                                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Result.Token);
                                    var requestList = new List<FarmazonProductRequest> { productData };
                                    var json = JsonConvert.SerializeObject(requestList);
                                    var productContent = new StringContent(json, Encoding.UTF8, "application/json");
                                    var response = await client.PostAsync($"{baseUrl}/v2/listings/createlistings", productContent);
                                    var responseContent = await response.Content.ReadAsStringAsync();

                                    if (response.IsSuccessStatusCode)
                                    {
                                        var result = JsonConvert.DeserializeObject<FarmazonProductResponse>(responseContent);
                                        var firstResult = result.Result?.FirstOrDefault();

                                        if (firstResult?.Success == true)
                                        {
                                            entity.Farmazon_Gonderildi = 1;
                                            entity.Farmazon_Success = true;
                                            entity.Farmazon_Message = "Ürün başarıyla eklendi/güncellendi";
                                            entity.Farmazon_ReportId = firstResult.RequestItem.ToString();
                                            entity.Farmazon_GuncellenmeTarihi = DateTime.Now;
                                            _repository.Guncelle(entity);

                                            results.Add(new
                                            {
                                                marketplace = "Farmazon",
                                                success = true,
                                                message = entity.Farmazon_Message,
                                                reportId = entity.Farmazon_ReportId
                                            });
                                        }
                                        else
                                        {
                                            var error = firstResult?.Errors?.FirstOrDefault();
                                            entity.Farmazon_Gonderildi = 0;
                                            entity.Farmazon_Success = false;
                                            entity.Farmazon_Message = error?.Message ?? "Ürün eklenemedi";
                                            entity.Farmazon_ReportId = string.Empty;
                                            entity.Farmazon_GuncellenmeTarihi = DateTime.Now;
                                            _repository.Guncelle(entity);

                                            results.Add(new
                                            {
                                                marketplace = "Farmazon",
                                                success = false,
                                                message = entity.Farmazon_Message
                                            });
                                        }
                                    }
                                    else
                                    {
                                        entity.Farmazon_Gonderildi = 0;
                                        entity.Farmazon_Success = false;
                                        entity.Farmazon_Message = $"Hata: {(int)response.StatusCode} - {responseContent}";
                                        entity.Farmazon_ReportId = string.Empty;
                                        entity.Farmazon_GuncellenmeTarihi = DateTime.Now;
                                        _repository.Guncelle(entity);

                                        results.Add(new
                                        {
                                            marketplace = "Farmazon",
                                            success = false,
                                            message = entity.Farmazon_Message
                                        });
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            entity.Farmazon_Gonderildi = 0;
                            entity.Farmazon_Success = false;
                            entity.Farmazon_Message = $"Hata: {ex.Message}";
                            entity.Farmazon_ReportId = string.Empty;
                            entity.Farmazon_GuncellenmeTarihi = DateTime.Now;
                            _repository.Guncelle(entity);

                            results.Add(new
                            {
                                marketplace = "Farmazon",
                                success = false,
                                message = entity.Farmazon_Message
                            });
                        }
                    }
                }
                else
                {
                    entity.Farmazon_Gonderildi = null;
                    entity.Farmazon_Success = null;
                    entity.Farmazon_Message = null;
                    entity.Farmazon_ReportId = null;
                    entity.Farmazon_GuncellenmeTarihi = DateTime.Now;
                    _repository.Guncelle(entity);
                }

                // FarmaBorsa ürün ekleme
                if (request.FarmaBorsa?.StockQuantity != null)
                {
                    var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == 1).FirstOrDefault();
                    if (girisBilgisi == null)
                    {
                        entity.FarmaBorsa_Gonderildi = 0;
                        entity.FarmaBorsa_Success = false;
                        entity.FarmaBorsa_Message = "API kimlik bilgileri bulunamadı.";
                        entity.FarmaBorsa_GuncellenmeTarihi = DateTime.Now;
                        _repository.Guncelle(entity);
                        results.Add(new { marketplace = "FarmaBorsa", success = false, message = "API kimlik bilgileri bulunamadı." });
                    }
                    else
                    {
                        string apiUrl = "https://wapi.farmaborsa.com/api/Entegre/IlanKaydet";
                        using var client = new HttpClient();

                        try
                        {
                            var input = new
                            {
                                nick = girisBilgisi.KullaniciAdi ?? "",
                                parola = girisBilgisi.Sifre ?? "",
                                apiKey = girisBilgisi.ApiKey ?? "",
                                urunAd = entity.Title ?? "",
                                adet = entity.FarmaBorsa_StockQuantity ?? 1,
                                maxAdet = entity.FarmaBorsa_MaxQuantity ?? 1,
                                op = 0.0,
                                tutar = entity.FarmaBorsa_SalePrice,
                                borsadaGoster = true,
                                miad = entity.FarmaBorsa_ExpirationDate ?? "",
                                miadTip = entity.FarmaBorsa_MiadTip ?? 0,
                                barkod = entity.Barcode ?? "",
                                resimUrl = imageUrls.FirstOrDefault() ?? ""
                            };

                            var json = JsonConvert.SerializeObject(input);
                            var content = new StringContent(json, Encoding.UTF8, "application/json");
                            var response = await client.PostAsync(apiUrl, content);
                            var responseContent = await response.Content.ReadAsStringAsync();

                            if (response.IsSuccessStatusCode)
                            {
                                var result = JsonConvert.DeserializeObject<FarmaBorsaProductResponse>(responseContent);
                                entity.FarmaBorsa_Gonderildi = !result.hata ? 1 : 0;
                                entity.FarmaBorsa_Success = !result.hata;
                                entity.FarmaBorsa_Message = result.mesaj;
                                entity.FarmaBorsa_GuncellenmeTarihi = DateTime.Now;
                                _repository.Guncelle(entity);

                                results.Add(new
                                {
                                    marketplace = "FarmaBorsa",
                                    success = !result.hata,
                                    message = entity.FarmaBorsa_Message
                                });
                            }
                            else
                            {
                                entity.FarmaBorsa_Gonderildi = 0;
                                entity.FarmaBorsa_Success = false;
                                entity.FarmaBorsa_Message = $"Hata: {(int)response.StatusCode} - {responseContent}";
                                entity.FarmaBorsa_GuncellenmeTarihi = DateTime.Now;
                                _repository.Guncelle(entity);

                                results.Add(new
                                {
                                    marketplace = "FarmaBorsa",
                                    success = false,
                                    message = entity.FarmaBorsa_Message
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            entity.FarmaBorsa_Gonderildi = 0;
                            entity.FarmaBorsa_Success = false;
                            entity.FarmaBorsa_Message = $"Hata: {ex.Message}";
                            entity.FarmaBorsa_GuncellenmeTarihi = DateTime.Now;
                            _repository.Guncelle(entity);

                            results.Add(new
                            {
                                marketplace = "FarmaBorsa",
                                success = false,
                                message = entity.FarmaBorsa_Message
                            });
                        }
                    }
                }
                else
                {
                    entity.FarmaBorsa_Gonderildi = null;
                    entity.FarmaBorsa_Success = null;
                    entity.FarmaBorsa_Message = null;
                    entity.FarmaBorsa_GuncellenmeTarihi = DateTime.Now;
                    _repository.Guncelle(entity);
                }

                // Trendyol ürün ekleme
                if (request.Trendyol?.CategoryId != null)
                {
                    entity.Trendyol_Gonderildi = 0;
                    entity.Trendyol_Success = false;
                    entity.Trendyol_Message = "Trendyol entegrasyonu henüz uygulanmadı.";
                    entity.Trendyol_GuncellenmeTarihi = DateTime.Now;
                    _repository.Guncelle(entity);
                    results.Add(new { marketplace = "Trendyol", success = false, message = "Trendyol entegrasyonu henüz uygulanmadı." });
                }
                else
                {
                    entity.Trendyol_Gonderildi = null;
                    entity.Trendyol_Success = null;
                    entity.Trendyol_Message = null;
                    entity.Trendyol_GuncellenmeTarihi = DateTime.Now;
                    _repository.Guncelle(entity);
                }

                // Genel Gonderildi durumunu kontrol et
                entity.Gonderildi = new[]
                {
            entity.PTT_Gonderildi,
            entity.N11_Gonderildi,
            entity.GoTurc_Gonderildi,
            entity.Idefix_Gonderildi,
            entity.Pazarama_Gonderildi,
            entity.CicekSepeti_Gonderildi,
            entity.Farmazon_Gonderildi,
            entity.FarmaBorsa_Gonderildi,
            entity.Trendyol_Gonderildi
        }.Any(g => g == 1) ? 1 : 0;

                _repository.Guncelle(entity);

                if (!results.Any())
                {
                    return Json(new { success = false, message = "Hiçbir pazar yeri işlenmedi." });
                }

                return Json(new { success = true, results });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ürün kaydedilirken bir hata oluştu: {ex.Message}");
                return View("Index", request);
            }
        }
        [HttpGet]
        public IActionResult DownloadExcelTemplate()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Ürün Şablonu");

            // Başlık satırını ekle
            string[] headers = new[]
            {
                "Başlık", "ÜrünKodu", "Barkod", "Açıklama", "KısaAçıklama", "ListeFiyatı", "KDVOranı",
                "PTT_KategoriId", "PTT_SatışFiyatı", "PTT_KDVDahilFiyat", "PTT_StokMiktarı", "PTT_Desi",
                "PTT_İndirim", "PTT_TahminiKargoTeslimSüresi", "PTT_GarantiSüresi", "PTT_GarantiSağlayıcı",
                "N11_KategoriId", "N11_SatışFiyatı", "N11_StokMiktarı", "N11_İndirimYüzdesi",
                "Idefix_KategoriId", "Idefix_MarkaId", "Idefix_SatışFiyatı", "Idefix_StokMiktarı",
                "Idefix_TedarikçiStokKodu", "Idefix_Ağırlık", "Idefix_KarşılaştırmaFiyatı",
                "Idefix_TeslimatSüresi", "Idefix_TeslimatTipi", "Idefix_KargoŞirketiId",
                "Idefix_GönderimAdresiId", "Idefix_İadeAdresiId",
                "FarmaBorsa_Kod", "FarmaBorsa_SatışFiyatı", "FarmaBorsa_StokMiktarı",
                "FarmaBorsa_AzamiMiktar", "FarmaBorsa_SonKullanmaTarihi", "FarmaBorsa_MiadTip",
                "FarmaBorsa_BorsadaGöster",
                "Farmazon_Id", "Farmazon_SatışFiyatı", "Farmazon_StokMiktarı", "Farmazon_StokKodu",
                "Farmazon_SonKullanmaTarihi", "Farmazon_AzamiMiktar", "Farmazon_ÖneÇıkanÜrün",
                "Pazarama_KategoriId", "Pazarama_MarkaId", "Pazarama_SatışFiyatı", "Pazarama_StokMiktarı",
                "ÇiçekSepeti_KategoriId", "ÇiçekSepeti_SatışFiyatı", "ÇiçekSepeti_StokMiktarı",
                "ÇiçekSepeti_StokKodu", "ÇiçekSepeti_KategoriAdı", "ÇiçekSepeti_ÜrünDurumTipi",
                "ÇiçekSepeti_VaryantAdı",
                "GoTurc_KategoriId", "GoTurc_MarkaId", "GoTurc_TeslimatYöntemi",
                "GoTurc_SatışFiyatı", "GoTurc_StokMiktarı",
                "Trendyol_KategoriId", "Trendyol_MarkaId", "Trendyol_SatışFiyatı", "Trendyol_StokMiktarı"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
            }

            // Örnek veri satırı
            worksheet.Cells[2, 1].Value = "Örnek Ürün";
            worksheet.Cells[2, 2].Value = "PRD123";
            worksheet.Cells[2, 3].Value = "123456789";
            worksheet.Cells[2, 4].Value = "Bu bir örnek ürün açıklamasıdır.";
            worksheet.Cells[2, 5].Value = "Kısa açıklama.";
            worksheet.Cells[2, 6].Value = 100.50;
            worksheet.Cells[2, 7].Value = 18;
            worksheet.Cells[2, 8].Value = 123;
            worksheet.Cells[2, 9].Value = "987654321";
            worksheet.Cells[2, 10].Value = 90.00;
            worksheet.Cells[2, 11].Value = 108.00;
            worksheet.Cells[2, 12].Value = 50;
            worksheet.Cells[2, 13].Value = 1.5;
            worksheet.Cells[2, 14].Value = 10.00;
            worksheet.Cells[2, 15].Value = 3;
            worksheet.Cells[2, 16].Value = 24;
            worksheet.Cells[2, 17].Value = "Marka X";
            worksheet.Cells[2, 18].Value = 456;
            worksheet.Cells[2, 19].Value = 95.00;
            worksheet.Cells[2, 20].Value = 60;
            worksheet.Cells[2, 21].Value = 15.00;
            worksheet.Cells[2, 22].Value = 789;
            worksheet.Cells[2, 23].Value = 101;
            worksheet.Cells[2, 24].Value = 92.00;
            worksheet.Cells[2, 25].Value = 70;
            worksheet.Cells[2, 26].Value = "VSC123";
            worksheet.Cells[2, 27].Value = 1.2;
            worksheet.Cells[2, 28].Value = 110.00;
            worksheet.Cells[2, 29].Value = 5;
            worksheet.Cells[2, 30].Value = "regular";
            worksheet.Cells[2, 31].Value = 1;
            worksheet.Cells[2, 32].Value = 1;
            worksheet.Cells[2, 33].Value = 1;
            worksheet.Cells[2, 34].Value = "FB123";
            worksheet.Cells[2, 35].Value = 85.00;
            worksheet.Cells[2, 36].Value = 80;
            worksheet.Cells[2, 37].Value = 100;
            worksheet.Cells[2, 38].Value = "2025-12-31";
            worksheet.Cells[2, 39].Value = "0";
            worksheet.Cells[2, 40].Value = true;
            worksheet.Cells[2, 41].Value = "FZ123";
            worksheet.Cells[2, 42].Value = 88.00;
            worksheet.Cells[2, 43].Value = 75;
            worksheet.Cells[2, 44].Value = "FZS123";
            worksheet.Cells[2, 45].Value = "2025-12-31";
            worksheet.Cells[2, 46].Value = 100;
            worksheet.Cells[2, 47].Value = true;
            worksheet.Cells[2, 48].Value = 234;
            worksheet.Cells[2, 49].Value = 102;
            worksheet.Cells[2, 50].Value = 92.00;
            worksheet.Cells[2, 51].Value = 55;
            worksheet.Cells[2, 52].Value = 567;
            worksheet.Cells[2, 53].Value = 80.00;
            worksheet.Cells[2, 54].Value = 65;
            worksheet.Cells[2, 55].Value = "CS123";
            worksheet.Cells[2, 56].Value = "Kategori Adı";
            worksheet.Cells[2, 57].Value = "Yeni";
            worksheet.Cells[2, 58].Value = "Varyant 1";
            worksheet.Cells[2, 59].Value = 890;
            worksheet.Cells[2, 60].Value = 103;
            worksheet.Cells[2, 61].Value = "Teslimat 1";
            worksheet.Cells[2, 62].Value = 87.00;
            worksheet.Cells[2, 63].Value = 70;
            worksheet.Cells[2, 64].Value = 345;
            worksheet.Cells[2, 65].Value = 104;
            worksheet.Cells[2, 66].Value = 90.00;
            worksheet.Cells[2, 67].Value = 60;

            // Sütun genişliklerini ayarla
            worksheet.Cells.AutoFitColumns();

            var stream = new MemoryStream(package.GetAsByteArray());
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UrunSablonu.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile excelFile, int pazarYeriId)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                return Json(new { success = false, message = "Lütfen bir Excel dosyası yükleyin." });
            }

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var stream = new MemoryStream();
                await excelFile.CopyToAsync(stream);
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;
                var results = new List<dynamic>();

                for (int row = 2; row <= rowCount; row++) // Başlık satırını atla
                {
                    try
                    {
                        var request = new UnifiedMarketplaceModel
                        {
                            Title = worksheet.Cells[row, 1].Text,
                            ProductCode = worksheet.Cells[row, 2].Text,
                            Barcode = worksheet.Cells[row, 3].Text,
                            Description = worksheet.Cells[row, 4].Text,
                            ListPrice = string.IsNullOrEmpty(worksheet.Cells[row, 6].Text) ? 0 : Convert.ToDecimal(worksheet.Cells[row, 6].Text.Replace(",", ".")),
                            VatRate = string.IsNullOrEmpty(worksheet.Cells[row, 7].Text) ? 0 : Convert.ToInt32(worksheet.Cells[row, 7].Text),
                            PTT = new PTTUrunModel
                            {
                                CategoryId = string.IsNullOrEmpty(worksheet.Cells[row, 8].Text) ? null : Convert.ToInt32(worksheet.Cells[row, 8].Text),
                                SalePrice = string.IsNullOrEmpty(worksheet.Cells[row, 10].Text) ? 0 : Convert.ToDecimal(worksheet.Cells[row, 10].Text.Replace(",", ".")),
                                PriceWithVat = string.IsNullOrEmpty(worksheet.Cells[row, 11].Text) ? 0 : Convert.ToDecimal(worksheet.Cells[row, 11].Text.Replace(",", ".")),
                                StockQuantity = string.IsNullOrEmpty(worksheet.Cells[row, 12].Text) ? 0 : Convert.ToInt32(worksheet.Cells[row, 12].Text),
                                Desi = string.IsNullOrEmpty(worksheet.Cells[row, 13].Text) ? null : Convert.ToDecimal(worksheet.Cells[row, 13].Text.Replace(",", ".")),
                                Discount = string.IsNullOrEmpty(worksheet.Cells[row, 14].Text) ? null : Convert.ToDecimal(worksheet.Cells[row, 14].Text.Replace(",", ".")),
                                EstimatedCourierDelivery = string.IsNullOrEmpty(worksheet.Cells[row, 15].Text) ? null : Convert.ToInt32(worksheet.Cells[row, 15].Text),
                                WarrantyDuration = string.IsNullOrEmpty(worksheet.Cells[row, 16].Text) ? null : Convert.ToInt32(worksheet.Cells[row, 16].Text),
                            },
                            N11 = new N11UrunModel
                            {
                                CategoryId = string.IsNullOrEmpty(worksheet.Cells[row, 18].Text) ? null : Convert.ToInt32(worksheet.Cells[row, 18].Text),
                                SalePrice = string.IsNullOrEmpty(worksheet.Cells[row, 19].Text) ? 0 : Convert.ToDecimal(worksheet.Cells[row, 19].Text.Replace(",", ".")),
                                StockQuantity = string.IsNullOrEmpty(worksheet.Cells[row, 20].Text) ? 0 : Convert.ToInt32(worksheet.Cells[row, 20].Text),
                            },
                            Idefix = new IdefixUrunModel
                            {
                                CategoryId = string.IsNullOrEmpty(worksheet.Cells[row, 22].Text) ? null : Convert.ToInt32(worksheet.Cells[row, 22].Text),
                                BrandId = string.IsNullOrEmpty(worksheet.Cells[row, 23].Text) ? null : Convert.ToInt32(worksheet.Cells[row, 23].Text),
                                SalePrice = string.IsNullOrEmpty(worksheet.Cells[row, 24].Text) ? 0 : Convert.ToDecimal(worksheet.Cells[row, 24].Text.Replace(",", ".")),
                                StockQuantity = string.IsNullOrEmpty(worksheet.Cells[row, 25].Text) ? 0 : Convert.ToInt32(worksheet.Cells[row, 25].Text),
                                VendorStockCode = worksheet.Cells[row, 26].Text,
                                Weight = string.IsNullOrEmpty(worksheet.Cells[row, 27].Text) ? null : Convert.ToDecimal(worksheet.Cells[row, 27].Text.Replace(",", ".")),
                                ComparePrice = string.IsNullOrEmpty(worksheet.Cells[row, 28].Text) ? null : Convert.ToDecimal(worksheet.Cells[row, 28].Text.Replace(",", ".")),
                                DeliveryDuration = string.IsNullOrEmpty(worksheet.Cells[row, 29].Text) ? null : Convert.ToInt32(worksheet.Cells[row, 29].Text),
                                CargoCompanyId = string.IsNullOrEmpty(worksheet.Cells[row, 31].Text) ? null : Convert.ToInt32(worksheet.Cells[row, 31].Text),
                                ShipmentAddressId = string.IsNullOrEmpty(worksheet.Cells[row, 32].Text) ? null : Convert.ToInt32(worksheet.Cells[row, 32].Text),
                                ReturnAddressId = string.IsNullOrEmpty(worksheet.Cells[row, 33].Text) ? null : Convert.ToInt32(worksheet.Cells[row, 33].Text)
                            },
                            FarmaBorsa = new FarmaBorsaUrunModel
                            {
                                SalePrice = string.IsNullOrEmpty(worksheet.Cells[row, 35].Text) ? 0 : Convert.ToDecimal(worksheet.Cells[row, 35].Text.Replace(",", ".")),
                                StockQuantity = string.IsNullOrEmpty(worksheet.Cells[row, 36].Text) ? 0 : Convert.ToInt32(worksheet.Cells[row, 36].Text),
                                MaxQuantity = string.IsNullOrEmpty(worksheet.Cells[row, 37].Text) ? 0 : Convert.ToInt32(worksheet.Cells[row, 37].Text),
                                ExpirationDate = worksheet.Cells[row, 38].Text,
                                MiadTip = string.IsNullOrEmpty(worksheet.Cells[row, 39].Text) ? 0 : Convert.ToInt32(worksheet.Cells[row, 39].Text),
                            },
                            Farmazon = new FarmazonUrunModel
                            {
                                Id = string.IsNullOrEmpty(worksheet.Cells[row, 41].Text) ? 0 : Convert.ToUInt32(worksheet.Cells[row, 41].Text),
                                Price = string.IsNullOrEmpty(worksheet.Cells[row, 42].Text) ? 0 : Convert.ToDecimal(worksheet.Cells[row, 42].Text.Replace(",", ".")),
                                Stock = string.IsNullOrEmpty(worksheet.Cells[row, 43].Text) ? 0 : Convert.ToInt32(worksheet.Cells[row, 43].Text),
                                StockCode = worksheet.Cells[row, 44].Text,
                                Expiration = worksheet.Cells[row, 45].Text,
                                MaxCount = string.IsNullOrEmpty(worksheet.Cells[row, 46].Text) ? 0 : Convert.ToInt32(worksheet.Cells[row, 46].Text),
                                IsFeatured = string.IsNullOrEmpty(worksheet.Cells[row, 47].Text) ? false : Convert.ToBoolean(worksheet.Cells[row, 47].Text)
                            },
                            Pazarama = new PazaramaUrunModel
                            {
                                CategoryId = string.IsNullOrEmpty(worksheet.Cells[row, 48].Text) ? null : Convert.ToInt32(worksheet.Cells[row, 48].Text),
                                BrandId = string.IsNullOrEmpty(worksheet.Cells[row, 49].Text) ? null : Convert.ToInt32(worksheet.Cells[row, 49].Text),
                                SalePrice = string.IsNullOrEmpty(worksheet.Cells[row, 50].Text) ? 0 : Convert.ToDecimal(worksheet.Cells[row, 50].Text.Replace(",", ".")),
                                StockQuantity = string.IsNullOrEmpty(worksheet.Cells[row, 51].Text) ? 0 : Convert.ToInt32(worksheet.Cells[row, 51].Text)
                            },
                            CicekSepeti = new CicekSepetiUrunModel
                            {
                                CategoryId = string.IsNullOrEmpty(worksheet.Cells[row, 52].Text) ? null : Convert.ToInt32(worksheet.Cells[row, 52].Text),
                                SalePrice = string.IsNullOrEmpty(worksheet.Cells[row, 53].Text) ? 0 : Convert.ToDecimal(worksheet.Cells[row, 53].Text.Replace(",", ".")),
                                StockQuantity = string.IsNullOrEmpty(worksheet.Cells[row, 54].Text) ? 0 : Convert.ToInt32(worksheet.Cells[row, 54].Text),
                                StockCode = worksheet.Cells[row, 55].Text,
                                CategoryName = worksheet.Cells[row, 56].Text,
                                ProductStatusType = worksheet.Cells[row, 57].Text,
                                VariantName = worksheet.Cells[row, 58].Text
                            },
                            GoTurc = new GoTurcMarketplaceModel
                            {
                                CategoryId = string.IsNullOrEmpty(worksheet.Cells[row, 59].Text) ? null : Convert.ToInt32(worksheet.Cells[row, 59].Text),
                                BrandId = string.IsNullOrEmpty(worksheet.Cells[row, 60].Text) ? null : Convert.ToInt32(worksheet.Cells[row, 60].Text),
                                DeliveryNo = string.IsNullOrEmpty(worksheet.Cells[row, 61].Text) ? 0 : Convert.ToInt64(worksheet.Cells[row, 61].Text),
                                SalePrice = string.IsNullOrEmpty(worksheet.Cells[row, 62].Text) ? 0 : Convert.ToDecimal(worksheet.Cells[row, 62].Text.Replace(",", ".")),
                                StockQuantity = string.IsNullOrEmpty(worksheet.Cells[row, 63].Text) ? 0 : Convert.ToInt32(worksheet.Cells[row, 63].Text)
                            },
                            Trendyol = new TrendyolUrunModel
                            {
                                CategoryId = string.IsNullOrEmpty(worksheet.Cells[row, 64].Text) ? null : Convert.ToInt32(worksheet.Cells[row, 64].Text),
                                BrandId = string.IsNullOrEmpty(worksheet.Cells[row, 65].Text) ? null : Convert.ToInt32(worksheet.Cells[row, 65].Text),
                                SalePrice = string.IsNullOrEmpty(worksheet.Cells[row, 66].Text) ? 0 : Convert.ToDecimal(worksheet.Cells[row, 66].Text.Replace(",", ".")),
                                StockQuantity = string.IsNullOrEmpty(worksheet.Cells[row, 67].Text) ? 0 : Convert.ToInt32(worksheet.Cells[row, 67].Text)
                            }
                        };

                        // Mevcut Kaydet metodunu çağır
                        var result = await Kaydet(request);
                        results.Add(new { Row = row, Result = result });
                    }
                    catch (Exception ex)
                    {
                        results.Add(new { Row = row, Result = new { success = false, message = $"Satır {row}: Hata - {ex.Message}" } });
                    }
                }

                return Json(new { success = true, message = "Ürünler işleniyor.", results });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

     
        public async Task<IActionResult> ImportExcel(IFormFile excelFile, bool sendToMarketplaces = false)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                return Json(new { success = false, message = "Lütfen geçerli bir Excel dosyası yükleyin." });
            }

            try
            {
                using var stream = new MemoryStream();
                await excelFile.CopyToAsync(stream);
                using var document = SpreadsheetDocument.Open(stream, false);
                var workbookPart = document.WorkbookPart;
                var worksheetPart = workbookPart.WorksheetParts.First();
                var sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
                var rows = sheetData.Elements<Row>().ToList();

                if (!rows.Any())
                    return Json(new { success = false, message = "Excel dosyası boş." });

                // Kolon eşleştirme
                var headerRow = rows.First();
                var mappings = GetColumnMappings(headerRow, workbookPart);

                var results = new List<dynamic>();
                var dataRows = rows.Skip(1).ToList();

                foreach (var row in dataRows)
                {
                    var errors = new List<string>();
                    var request = new UnifiedMarketplaceModel
                    {
                        PTT = new PTTUrunModel(),
                        N11 = new N11UrunModel(),
                        Idefix = new IdefixUrunModel(),
                        FarmaBorsa = new FarmaBorsaUrunModel(),
                        Farmazon = new FarmazonUrunModel(),
                        Pazarama = new PazaramaUrunModel(),
                        CicekSepeti = new CicekSepetiUrunModel(),
                        GoTurc = new GoTurcMarketplaceModel(),
                        Trendyol = new TrendyolUrunModel()
                    };

                    var unifiedMarketplace = new UnifiedMarketplace(); // Yeni model nesnesi

                    try
                    {
                        // Genel alanlar
                        request.Title = GetCellValue(row, mappings, "Title", workbookPart, errors);
                        request.ProductCode = GetCellValue(row, mappings, "ProductCode", workbookPart, errors);
                        request.Barcode = GetCellValue(row, mappings, "Barcode", workbookPart, errors);
                        request.ListPrice = ParseDecimal(GetCellValue(row, mappings, "ListPrice", workbookPart, errors), errors);
                        request.VatRate = ParseInt(GetCellValue(row, mappings, "VatRate", workbookPart, errors), errors) ?? 0;
                        request.Description = GetCellValue(row, mappings, "Description", workbookPart, errors);

                        // UnifiedMarketplace modeline eşitleme (genel alanlar)
                        unifiedMarketplace.Title = request.Title;
                        unifiedMarketplace.ProductCode = request.ProductCode;
                        unifiedMarketplace.Barcode = request.Barcode;
                        unifiedMarketplace.ListPrice = request.ListPrice;
                        unifiedMarketplace.VatRate = request.VatRate;
                        unifiedMarketplace.Description = request.Description;

                        // Zorunlu alan doğrulama
                        if (string.IsNullOrEmpty(request.Title))
                            errors.Add("Başlık zorunludur.");
                        if (string.IsNullOrEmpty(request.Barcode))
                            errors.Add("Barkod zorunludur.");

                        // PTT alanları
                        if (mappings.ContainsKey("PTT_CategoryId") && !string.IsNullOrEmpty(GetCellValue(row, mappings, "PTT_CategoryId", workbookPart, errors)))
                        {
                            request.PTT.CategoryId = ParseInt(GetCellValue(row, mappings, "PTT_CategoryId", workbookPart, errors), errors);
                            request.PTT.SalePrice = ParseDecimal(GetCellValue(row, mappings, "PTT_SalePrice", workbookPart, errors), errors);
                            request.PTT.PriceWithVat = ParseDecimal(GetCellValue(row, mappings, "PTT_PriceWithVat", workbookPart, errors), errors);
                            request.PTT.StockQuantity = ParseInt(GetCellValue(row, mappings, "PTT_StockQuantity", workbookPart, errors), errors) ?? 0;
                            request.PTT.Desi = ParseDecimal(GetCellValue(row, mappings, "PTT_Desi", workbookPart, errors), errors);
                            request.PTT.Discount = ParseDecimal(GetCellValue(row, mappings, "PTT_Discount", workbookPart, errors), errors);
                            request.PTT.EstimatedCourierDelivery = ParseInt(GetCellValue(row, mappings, "PTT_EstimatedCourierDelivery", workbookPart, errors), errors);
                            request.PTT.WarrantyDuration = ParseInt(GetCellValue(row, mappings, "PTT_WarrantyDuration", workbookPart, errors), errors);

                            // UnifiedMarketplace modeline eşitleme (PTT)
                            unifiedMarketplace.PTT_CategoryId = request.PTT.CategoryId;
                            unifiedMarketplace.PTT_SalePrice = request.PTT.SalePrice;
                            unifiedMarketplace.PTT_PriceWithVat = request.PTT.PriceWithVat;
                            unifiedMarketplace.PTT_StockQuantity = request.PTT.StockQuantity;
                            unifiedMarketplace.PTT_Desi = request.PTT.Desi;
                            unifiedMarketplace.PTT_Discount = request.PTT.Discount;
                            unifiedMarketplace.PTT_EstimatedCourierDelivery = request.PTT.EstimatedCourierDelivery;
                            unifiedMarketplace.PTT_WarrantyDuration = request.PTT.WarrantyDuration;
                        }

                        // N11 alanları
                        if (mappings.ContainsKey("N11_CategoryId") && !string.IsNullOrEmpty(GetCellValue(row, mappings, "N11_CategoryId", workbookPart, errors)))
                        {
                            request.N11.CategoryId = ParseInt(GetCellValue(row, mappings, "N11_CategoryId", workbookPart, errors), errors);
                            request.N11.SalePrice = ParseDecimal(GetCellValue(row, mappings, "N11_SalePrice", workbookPart, errors), errors) ?? 0m;
                            request.N11.StockQuantity = ParseInt(GetCellValue(row, mappings, "N11_StockQuantity", workbookPart, errors), errors) ?? 0;

                            unifiedMarketplace.N11_CategoryId = request.N11.CategoryId;
                            unifiedMarketplace.N11_SalePrice = request.N11.SalePrice;
                            unifiedMarketplace.N11_StockQuantity = request.N11.StockQuantity;
                        }

                        // Idefix alanları
                        if (mappings.ContainsKey("Idefix_CategoryId") && !string.IsNullOrEmpty(GetCellValue(row, mappings, "Idefix_CategoryId", workbookPart, errors)))
                        {
                            request.Idefix.CategoryId = ParseInt(GetCellValue(row, mappings, "Idefix_CategoryId", workbookPart, errors), errors);
                            request.Idefix.BrandId = ParseInt(GetCellValue(row, mappings, "Idefix_BrandId", workbookPart, errors), errors);
                            request.Idefix.SalePrice = ParseDecimal(GetCellValue(row, mappings, "Idefix_SalePrice", workbookPart, errors), errors);
                            request.Idefix.StockQuantity = ParseInt(GetCellValue(row, mappings, "Idefix_StockQuantity", workbookPart, errors), errors);
                            request.Idefix.VendorStockCode = GetCellValue(row, mappings, "Idefix_VendorStockCode", workbookPart, errors);
                            request.Idefix.Weight = ParseDecimal(GetCellValue(row, mappings, "Idefix_Weight", workbookPart, errors), errors);
                            request.Idefix.ComparePrice = ParseDecimal(GetCellValue(row, mappings, "Idefix_ComparePrice", workbookPart, errors), errors);
                            request.Idefix.DeliveryDuration = ParseInt(GetCellValue(row, mappings, "Idefix_DeliveryDuration", workbookPart, errors), errors);
                            request.Idefix.CargoCompanyId = ParseInt(GetCellValue(row, mappings, "Idefix_CargoCompanyId", workbookPart, errors), errors);
                            request.Idefix.ShipmentAddressId = ParseInt(GetCellValue(row, mappings, "Idefix_ShipmentAddressId", workbookPart, errors), errors);
                            request.Idefix.ReturnAddressId = ParseInt(GetCellValue(row, mappings, "Idefix_ReturnAddressId", workbookPart, errors), errors);

                            unifiedMarketplace.Idefix_CategoryId = request.Idefix.CategoryId;
                            unifiedMarketplace.Idefix_BrandId = request.Idefix.BrandId;
                            unifiedMarketplace.Idefix_SalePrice = request.Idefix.SalePrice;
                            unifiedMarketplace.Idefix_StockQuantity = request.Idefix.StockQuantity;
                            unifiedMarketplace.Idefix_VendorStockCode = request.Idefix.VendorStockCode;
                            unifiedMarketplace.Idefix_Weight = request.Idefix.Weight;
                            unifiedMarketplace.Idefix_ComparePrice = request.Idefix.ComparePrice;
                            unifiedMarketplace.Idefix_DeliveryDuration = request.Idefix.DeliveryDuration;
                            unifiedMarketplace.Idefix_CargoCompanyId = request.Idefix.CargoCompanyId;
                            unifiedMarketplace.Idefix_ShipmentAddressId = request.Idefix.ShipmentAddressId;
                            unifiedMarketplace.Idefix_ReturnAddressId = request.Idefix.ReturnAddressId;
                        }

                     request.FarmaBorsa.SalePrice = ParseDecimal(GetCellValue(row, mappings, "FarmaBorsa_SalePrice", workbookPart, errors), errors);
                            request.FarmaBorsa.StockQuantity = ParseInt(GetCellValue(row, mappings, "FarmaBorsa_StockQuantity", workbookPart, errors), errors);
                            request.FarmaBorsa.MaxQuantity = ParseInt(GetCellValue(row, mappings, "FarmaBorsa_MaxQuantity", workbookPart, errors), errors);
                            request.FarmaBorsa.ExpirationDate = GetCellValue(row, mappings, "FarmaBorsa_ExpirationDate", workbookPart, errors);
                            request.FarmaBorsa.MiadTip = ParseInt(GetCellValue(row, mappings, "FarmaBorsa_MiadTip", workbookPart, errors), errors) ?? 0;

                            unifiedMarketplace.FarmaBorsa_SalePrice = request.FarmaBorsa.SalePrice;
                            unifiedMarketplace.FarmaBorsa_StockQuantity = request.FarmaBorsa.StockQuantity;
                            unifiedMarketplace.FarmaBorsa_MaxQuantity = request.FarmaBorsa.MaxQuantity;
                            unifiedMarketplace.FarmaBorsa_ExpirationDate = request.FarmaBorsa.ExpirationDate;
                            unifiedMarketplace.FarmaBorsa_MiadTip = request.FarmaBorsa.MiadTip;
                        

                        // Farmazon alanları
                        if (mappings.ContainsKey("Farmazon_Id") && !string.IsNullOrEmpty(GetCellValue(row, mappings, "Farmazon_Id", workbookPart, errors)))
                        {
                            request.Farmazon.Id = ParseInt(GetCellValue(row, mappings, "Farmazon_Id", workbookPart, errors), errors) ?? 0;
                            request.Farmazon.Price = ParseDecimal(GetCellValue(row, mappings, "Farmazon_Price", workbookPart, errors), errors) ?? 0m;
                            request.Farmazon.Stock = ParseInt(GetCellValue(row, mappings, "Farmazon_Stock", workbookPart, errors), errors) ?? 0;
                            request.Farmazon.StockCode = GetCellValue(row, mappings, "Farmazon_StockCode", workbookPart, errors);
                            request.Farmazon.Expiration = GetCellValue(row, mappings, "Farmazon_Expiration", workbookPart, errors);
                            request.Farmazon.MaxCount = ParseInt(GetCellValue(row, mappings, "Farmazon_MaxCount", workbookPart, errors), errors);
                            request.Farmazon.IsFeatured = GetCellValue(row, mappings, "Farmazon_IsFeatured", workbookPart, errors) == "Evet";

                            unifiedMarketplace.Farmazon_Id = request.Farmazon.Id;
                            unifiedMarketplace.Farmazon_Price = request.Farmazon.Price;
                            unifiedMarketplace.Farmazon_Stock = request.Farmazon.Stock;
                            unifiedMarketplace.Farmazon_StockCode = request.Farmazon.StockCode;
                            unifiedMarketplace.Farmazon_Expiration = request.Farmazon.Expiration;
                            unifiedMarketplace.Farmazon_MaxCount = request.Farmazon.MaxCount;
                            unifiedMarketplace.Farmazon_IsFeatured = request.Farmazon.IsFeatured;
                        }

                        // Pazarama alanları
                        if (mappings.ContainsKey("Pazarama_CategoryId") && !string.IsNullOrEmpty(GetCellValue(row, mappings, "Pazarama_CategoryId", workbookPart, errors)))
                        {
                            request.Pazarama.CategoryId = ParseInt(GetCellValue(row, mappings, "Pazarama_CategoryId", workbookPart, errors), errors);
                            request.Pazarama.BrandId = ParseInt(GetCellValue(row, mappings, "Pazarama_BrandId", workbookPart, errors), errors);
                            request.Pazarama.SalePrice = ParseDecimal(GetCellValue(row, mappings, "Pazarama_SalePrice", workbookPart, errors), errors);
                            request.Pazarama.StockQuantity = ParseInt(GetCellValue(row, mappings, "Pazarama_StockQuantity", workbookPart, errors), errors);

                            unifiedMarketplace.Pazarama_CategoryId = request.Pazarama.CategoryId;
                            unifiedMarketplace.Pazarama_BrandId = request.Pazarama.BrandId;
                            unifiedMarketplace.Pazarama_SalePrice = request.Pazarama.SalePrice;
                            unifiedMarketplace.Pazarama_StockQuantity = request.Pazarama.StockQuantity;
                        }

                        // ÇiçekSepeti alanları
                        if (mappings.ContainsKey("CicekSepeti_CategoryId") && !string.IsNullOrEmpty(GetCellValue(row, mappings, "CicekSepeti_CategoryId", workbookPart, errors)))
                        {
                            request.CicekSepeti.CategoryId = ParseInt(GetCellValue(row, mappings, "CicekSepeti_CategoryId", workbookPart, errors), errors);
                            request.CicekSepeti.SalePrice = ParseDecimal(GetCellValue(row, mappings, "CicekSepeti_SalePrice", workbookPart, errors), errors);
                            request.CicekSepeti.StockQuantity = ParseInt(GetCellValue(row, mappings, "CicekSepeti_StockQuantity", workbookPart, errors), errors);
                            request.CicekSepeti.StockCode = GetCellValue(row, mappings, "CicekSepeti_StockCode", workbookPart, errors);
                            request.CicekSepeti.CategoryName = GetCellValue(row, mappings, "CicekSepeti_CategoryName", workbookPart, errors);
                            request.CicekSepeti.ProductStatusType = GetCellValue(row, mappings, "CicekSepeti_ProductStatusType", workbookPart, errors);
                            request.CicekSepeti.VariantName = GetCellValue(row, mappings, "CicekSepeti_VariantName", workbookPart, errors);

                            unifiedMarketplace.CicekSepeti_CategoryId = request.CicekSepeti.CategoryId;
                            unifiedMarketplace.CicekSepeti_SalePrice = request.CicekSepeti.SalePrice;
                            unifiedMarketplace.CicekSepeti_StockQuantity = request.CicekSepeti.StockQuantity;
                            unifiedMarketplace.CicekSepeti_StockCode = request.CicekSepeti.StockCode;
                            unifiedMarketplace.CicekSepeti_CategoryName = request.CicekSepeti.CategoryName;
                            unifiedMarketplace.CicekSepeti_ProductStatusType = request.CicekSepeti.ProductStatusType;
                            unifiedMarketplace.CicekSepeti_VariantName = request.CicekSepeti.VariantName;
                        }

                        // GoTurc alanları
                        if (mappings.ContainsKey("GoTurc_CategoryId") && !string.IsNullOrEmpty(GetCellValue(row, mappings, "GoTurc_CategoryId", workbookPart, errors)))
                        {
                            request.GoTurc.CategoryId = ParseInt(GetCellValue(row, mappings, "GoTurc_CategoryId", workbookPart, errors), errors);
                            request.GoTurc.BrandId = ParseInt(GetCellValue(row, mappings, "GoTurc_BrandId", workbookPart, errors), errors);
                            request.GoTurc.DeliveryNo = ParseInt(GetCellValue(row, mappings, "GoTurc_DeliveryNo", workbookPart, errors), errors);
                            request.GoTurc.SalePrice = ParseDecimal(GetCellValue(row, mappings, "GoTurc_SalePrice", workbookPart, errors), errors);
                            request.GoTurc.StockQuantity = ParseInt(GetCellValue(row, mappings, "GoTurc_StockQuantity", workbookPart, errors), errors);

                            unifiedMarketplace.GoTurc_CategoryId = request.GoTurc.CategoryId;
                            unifiedMarketplace.GoTurc_BrandId = request.GoTurc.BrandId;
                            unifiedMarketplace.GoTurc_DeliveryNo = request.GoTurc.DeliveryNo;
                            unifiedMarketplace.GoTurc_SalePrice = request.GoTurc.SalePrice;
                            unifiedMarketplace.GoTurc_StockQuantity = request.GoTurc.StockQuantity;
                        }

                        // Trendyol alanları
                        if (mappings.ContainsKey("Trendyol_CategoryId") && !string.IsNullOrEmpty(GetCellValue(row, mappings, "Trendyol_CategoryId", workbookPart, errors)))
                        {
                            request.Trendyol.CategoryId = ParseInt(GetCellValue(row, mappings, "Trendyol_CategoryId", workbookPart, errors), errors);
                            request.Trendyol.BrandId = ParseInt(GetCellValue(row, mappings, "Trendyol_BrandId", workbookPart, errors), errors);
                            request.Trendyol.SalePrice = ParseDecimal(GetCellValue(row, mappings, "Trendyol_SalePrice", workbookPart, errors), errors);
                            request.Trendyol.StockQuantity = ParseInt(GetCellValue(row, mappings, "Trendyol_StockQuantity", workbookPart, errors), errors);

                            unifiedMarketplace.Trendyol_CategoryId = request.Trendyol.CategoryId;
                            unifiedMarketplace.Trendyol_BrandId = request.Trendyol.BrandId;
                            unifiedMarketplace.Trendyol_SalePrice = request.Trendyol.SalePrice;
                            unifiedMarketplace.Trendyol_StockQuantity = request.Trendyol.StockQuantity;
                        }

                        // PTT Varyantları
                        if (mappings.ContainsKey("PTT_Variant_Barcode") && !string.IsNullOrEmpty(GetCellValue(row, mappings, "PTT_Variant_Barcode", workbookPart, errors)))
                        {
                            var variant = new PTTVariantModel
                            {
                                Barcode = GetCellValue(row, mappings, "PTT_Variant_Barcode", workbookPart, errors),
                                Price = ParseDecimal(GetCellValue(row, mappings, "PTT_Variant_Price", workbookPart, errors), errors),
                                Quantity = ParseInt(GetCellValue(row, mappings, "PTT_Variant_Quantity", workbookPart, errors), errors),
                                Attributes = new List<PTTVariantAttributeModel>
                        {
                            new PTTVariantAttributeModel
                            {
                                Definition = GetCellValue(row, mappings, "PTT_Variant_AttributeDefinition", workbookPart, errors),
                                Value = GetCellValue(row, mappings, "PTT_Variant_AttributeValue", workbookPart, errors)
                            }
                        }
                            };
                            request.PTT.Variants.Add(variant);
                            unifiedMarketplace.PTT_VariantsJson = JsonConvert.SerializeObject(request.PTT.Variants);
                        }

                        // Idefix Özellikleri
                        if (mappings.ContainsKey("Idefix_AttributeId") && !string.IsNullOrEmpty(GetCellValue(row, mappings, "Idefix_AttributeId", workbookPart, errors)))
                        {
                            var attribute = new IdefixAttribute
                            {
                                AttributeId = ParseLong(GetCellValue(row, mappings, "Idefix_AttributeId", workbookPart, errors), errors) ?? 0,
                                AttributeValueId = ParseLong(GetCellValue(row, mappings, "Idefix_AttributeValueId", workbookPart, errors), errors) ?? 0
                            };
                            request.Idefix.Attributes.Add(attribute);
                            unifiedMarketplace.Idefix_AttributesJson = JsonConvert.SerializeObject(request.Idefix.Attributes);
                        }

                        // Hatalar varsa, sonucu ekle ve devam et
                        if (errors.Any())
                        {
                            results.Add(new { success = false, rowIndex = row.RowIndex?.Value, message = $"Satır {row.RowIndex?.Value}: {string.Join("; ", errors)}" });
                            
                        }

                        // Eğer sendToMarketplaces true ise, Gonder metodunu çağır
                        if (sendToMarketplaces)
                        {
                            IActionResult gonderResult;
                            using (var context = new Context()) // Veritabanı bağlamı
                            {
                                // Önce veritabanına kaydet
                                unifiedMarketplace.Durumu = 1;
                                unifiedMarketplace.PTT_EklenmeTarihi = DateTime.Now;
                                unifiedMarketplace.N11_EklenmeTarihi = DateTime.Now;
                                unifiedMarketplace.Idefix_EklenmeTarihi = DateTime.Now;
                                unifiedMarketplace.FarmaBorsa_EklenmeTarihi = DateTime.Now;
                                unifiedMarketplace.Farmazon_EklenmeTarihi = DateTime.Now;
                                unifiedMarketplace.Pazarama_EklenmeTarihi = DateTime.Now;
                                unifiedMarketplace.CicekSepeti_EklenmeTarihi = DateTime.Now;
                                unifiedMarketplace.GoTurc_EklenmeTarihi = DateTime.Now;
                                unifiedMarketplace.Trendyol_EklenmeTarihi = DateTime.Now;

                                context.UnifiedMarketplace.Add(unifiedMarketplace);
                                await context.SaveChangesAsync();

                                // Gonder metodunu çağır
                                gonderResult = await Gonder(request, unifiedMarketplace.Id);
                            }

                            // Gonder sonucunu işle
                            if (gonderResult is JsonResult jsonResult)
                            {
                                var jsonValue = jsonResult.Value;
                                results.Add(new
                                {
                                    success = jsonValue.GetType().GetProperty("success")?.GetValue(jsonValue) ?? false,
                                    rowIndex = row.RowIndex?.Value,
                                    message = jsonValue.GetType().GetProperty("message")?.GetValue(jsonValue) ?? "Pazar yerlerine gönderim tamamlandı.",
                                    marketplaceResults = jsonValue.GetType().GetProperty("results")?.GetValue(jsonValue)
                                });
                            }
                            else
                            {
                                results.Add(new
                                {
                                    success = false,
                                    rowIndex = row.RowIndex?.Value,
                                    message = $"Satır {row.RowIndex?.Value}: Pazar yerlerine gönderim başarısız."
                                });
                            }
                        }
                        else
                        {
                            // Sadece veritabanına kaydet
                            using (var context = new Context())
                            {
                                unifiedMarketplace.Durumu = 1;
                                unifiedMarketplace.PTT_EklenmeTarihi = DateTime.Now;
                                unifiedMarketplace.N11_EklenmeTarihi = DateTime.Now;
                                unifiedMarketplace.Idefix_EklenmeTarihi = DateTime.Now;
                                unifiedMarketplace.FarmaBorsa_EklenmeTarihi = DateTime.Now;
                                unifiedMarketplace.Farmazon_EklenmeTarihi = DateTime.Now;
                                unifiedMarketplace.Pazarama_EklenmeTarihi = DateTime.Now;
                                unifiedMarketplace.CicekSepeti_EklenmeTarihi = DateTime.Now;
                                unifiedMarketplace.GoTurc_EklenmeTarihi = DateTime.Now;
                                unifiedMarketplace.Trendyol_EklenmeTarihi = DateTime.Now;

                                context.UnifiedMarketplace.Add(unifiedMarketplace);
                                await context.SaveChangesAsync();

                                results.Add(new
                                {
                                    success = true,
                                    rowIndex = row.RowIndex?.Value,
                                    message = $"Satır {row.RowIndex?.Value}: Veritabanına başarıyla kaydedildi."
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        results.Add(new
                        {
                            success = false,
                            rowIndex = row.RowIndex?.Value,
                            message = $"Satır {row.RowIndex?.Value} işlenirken hata: {ex.Message}"
                        });
                    }
                }

                return Json(new
                {
                    success = true,
                    totalRows = dataRows.Count,
                    processedRows = results.Count(r =>
                    {
                        try
                        {
                            return r.success == true;
                        }
                        catch
                        {
                            return false;
                        }
                    }),
                    results
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Excel işlenirken hata oluştu: {ex.Message}" });
            }
        }
        private Dictionary<string, int> GetColumnMappings(Row headerRow, WorkbookPart workbookPart)
        {
            var mappings = new Dictionary<string, int>();
            var cells = headerRow.Elements<Cell>().ToList();

            // Define the mapping from Turkish column names to English field names
            var columnNameMap = new Dictionary<string, string>
    {
        // Genel alanlar
        { "Ürün Adı", "Title" },
        { "Ürün Kodu", "ProductCode" },
        { "Barkod", "Barcode" },
        { "Liste Fiyatı", "ListPrice" },
        { "KDV Oranı", "VatRate" },
        { "Açıklama", "Description" },
        // PTT alanları
        { "PTT Kategori", "PTT_CategoryId" },
        { "PTT KDV Hariç Fiyat", "PTT_SalePrice" },
        { "PTT KDV Dahil Fiyat", "PTT_PriceWithVat" },
        { "PTT Stok Miktarı", "PTT_StockQuantity" },
        { "PTT Desi", "PTT_Desi" },
        { "PTT İndirim", "PTT_Discount" },
        { "PTT Tahmini Kargo Teslim Süresi", "PTT_EstimatedCourierDelivery" },
        { "PTT Garanti Süresi", "PTT_WarrantyDuration" },
        // N11 alanları
        { "N11 Kategori", "N11_CategoryId" },
        { "N11 Satış Fiyatı", "N11_SalePrice" },
        { "N11 Stok Miktarı", "N11_StockQuantity" },
        // Idefix alanları
        { "Idefix Kategori", "Idefix_CategoryId" },
        { "Idefix Marka", "Idefix_BrandId" },
        { "Idefix Satış Fiyatı", "Idefix_SalePrice" },
        { "Idefix Stok Miktarı", "Idefix_StockQuantity" },
        { "Idefix Tedarikçi Stok Kodu", "Idefix_VendorStockCode" },
        { "Idefix Ağırlık", "Idefix_Weight" },
        { "Idefix Karşılaştırma Fiyatı", "Idefix_ComparePrice" },
        { "Idefix Teslimat Süresi", "Idefix_DeliveryDuration" },
        { "Idefix Kargo Şirketi", "Idefix_CargoCompanyId" },
        { "Idefix Gönderim Adresi", "Idefix_ShipmentAddressId" },
        { "Idefix İade Adresi", "Idefix_ReturnAddressId" },
        // FarmaBorsa alanları
        { "FarmaBorsa Satış Fiyatı", "FarmaBorsa_SalePrice" },
        { "FarmaBorsa Stok Miktarı", "FarmaBorsa_StockQuantity" },
        { "FarmaBorsa Maksimum Satış Miktarı", "FarmaBorsa_MaxQuantity" },
        { "FarmaBorsa Son Kullanım Tarihi", "FarmaBorsa_ExpirationDate" },
        { "FarmaBorsa Son Kullanım Tipi", "FarmaBorsa_MiadTip" },
        // Farmazon alanları
        { "Farmazon Ürün ID", "Farmazon_Id" },
        { "Farmazon Satış Fiyatı", "Farmazon_Price" },
        { "Farmazon Stok Miktarı", "Farmazon_Stock" },
        { "Farmazon Stok Kodu", "Farmazon_StockCode" },
        { "Farmazon Son Kullanım Tarihi", "Farmazon_Expiration" },
        { "Farmazon Maksimum Satış Miktarı", "Farmazon_MaxCount" },
        { "Farmazon Öne Çıkan Ürün", "Farmazon_IsFeatured" },
        // Pazarama alanları
        { "Pazarama Kategori", "Pazarama_CategoryId" },
        { "Pazarama Marka", "Pazarama_BrandId" },
        { "Pazarama Satış Fiyatı", "Pazarama_SalePrice" },
        { "Pazarama Stok Miktarı", "Pazarama_StockQuantity" },
        // ÇiçekSepeti alanları
        { "ÇiçekSepeti Kategori", "CicekSepeti_CategoryId" },
        { "ÇiçekSepeti Satış Fiyatı", "CicekSepeti_SalePrice" },
        { "ÇiçekSepeti Stok Miktarı", "CicekSepeti_StockQuantity" },
        { "ÇiçekSepeti Stok Kodu", "CicekSepeti_StockCode" },
        { "ÇiçekSepeti Kategori Adı", "CicekSepeti_CategoryName" },
        { "ÇiçekSepeti Ürün Durum Tipi", "CicekSepeti_ProductStatusType" },
        { "ÇiçekSepeti Varyant Adı", "CicekSepeti_VariantName" },
        // GoTurc alanları
        { "GoTurc Kategori", "GoTurc_CategoryId" },
        { "GoTurc Marka", "GoTurc_BrandId" },
        { "GoTurc Teslimat Yöntemi", "GoTurc_DeliveryNo" },
        { "GoTurc Satış Fiyatı", "GoTurc_SalePrice" },
        { "GoTurc Stok Miktarı", "GoTurc_StockQuantity" },
        // Trendyol alanları
        { "Trendyol Kategori", "Trendyol_CategoryId" },
        { "Trendyol Marka", "Trendyol_BrandId" },
        { "Trendyol Satış Fiyatı", "Trendyol_SalePrice" },
        { "Trendyol Stok Miktarı", "Trendyol_StockQuantity" },
        // PTT Varyant alanları
        { "PTT Varyant Barkod", "PTT_Variant_Barcode" },
        { "PTT Varyant Fiyat", "PTT_Variant_Price" },
        { "PTT Varyant Stok", "PTT_Variant_Quantity" },
        { "PTT Varyant Nitelik Tanımı", "PTT_Variant_AttributeDefinition" },
        { "PTT Varyant Nitelik Değeri", "PTT_Variant_AttributeValue" }
    };

            for (int i = 0; i < cells.Count; i++)
            {
                var value = GetCellValue(cells, i, workbookPart)?.Trim();
                if (!string.IsNullOrEmpty(value) && columnNameMap.ContainsKey(value))
                {
                    mappings[columnNameMap[value]] = i;
                }
            }
            return mappings;
        }

        private string GetCellValue(Row row, Dictionary<string, int> mappings, string columnName, WorkbookPart workbookPart, List<string> errors)
        {
            if (!mappings.ContainsKey(columnName))
            {
                errors.Add($"{columnName} kolonu bulunamadı.");
                return string.Empty;
            }
            var cells = row.Elements<Cell>().ToList();
            var index = mappings[columnName];
            if (index >= cells.Count)
            {
                errors.Add($"{columnName} kolonunda veri eksik.");
                return string.Empty;
            }
            var cell = cells[index];
            var value = cell.InnerText;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                var sharedStringTable = workbookPart.SharedStringTablePart?.SharedStringTable;
                if (sharedStringTable != null && int.TryParse(value, out var stringIndex))
                {
                    value = sharedStringTable.ElementAtOrDefault(stringIndex)?.InnerText ?? string.Empty;
                }
            }
            return value?.Trim() ?? string.Empty;
        }

        private string GetCellValue(List<Cell> cells, int index, WorkbookPart workbookPart)
        {
            if (index >= cells.Count) return string.Empty;
            var cell = cells[index];
            var value = cell.InnerText;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                var sharedStringTable = workbookPart.SharedStringTablePart?.SharedStringTable;
                if (sharedStringTable != null && int.TryParse(value, out var stringIndex))
                {
                    value = sharedStringTable.ElementAtOrDefault(stringIndex)?.InnerText ?? string.Empty;
                }
            }
            return value?.Trim() ?? string.Empty;
        }

        private decimal? ParseDecimal(string value, List<string> errors = null)
        {
            if (string.IsNullOrEmpty(value)) return null;
            // Replace comma with dot for decimal parsing (common in Turkish number formats)
            value = value.Replace(',', '.');
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;
            errors?.Add($"Geçersiz ondalık sayı: {value}");
            return null;
        }

        private int? ParseInt(string value, List<string> errors = null)
        {
            if (string.IsNullOrEmpty(value)) return null;
            if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;
            errors?.Add($"Geçersiz tamsayı: {value}");
            return null;
        }

        private long? ParseLong(string value, List<string> errors = null)
        {
            if (string.IsNullOrEmpty(value)) return null;
            if (long.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;
            errors?.Add($"Geçersiz uzun tamsayı: {value}");
            return null;
        }
    }
}