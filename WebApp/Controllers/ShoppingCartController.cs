using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class ShoppingCartController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ShoppingCartController> _logger;

        public ShoppingCartController(IConfiguration configuration, ILogger<ShoppingCartController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            await LoadCommonData();

            HttpContext.Session.Remove("UrunListesi");
            LoadCommonViewBagData();

            List<Sepet> sepetList = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();
            ViewBag.Sepet = sepetList;

            var cartSummary = CalculateCartSummary(sepetList);

            SepetViewModel model = new()
            {
                SepetListesi = sepetList,
                GenelToplam = cartSummary.GenelToplam,
                IndirimMiktari = cartSummary.IndirimMiktari,
                IndirimliToplam = cartSummary.IndirimliToplam,
                KargoUcreti = cartSummary.KargoUcreti,
                OdenecekToplam = cartSummary.OdenecekToplam,
                KDVToplam = cartSummary.KDVToplam,
                KDVOrani = 10
            };

            KargoUcretRepository kargoUcretRepository = new KargoUcretRepository();
            KargoUcret kargoUcret = kargoUcretRepository.Getir(x => x.Durumu == 1);
            ViewBag.KargoUcret = kargoUcret;
            ViewBag.SepetSayi = sepetList.Count;
            _logger.LogInformation("Cart loaded with {Count} items", sepetList.Count);

            var uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");
            ViewBag.Uyeler = uyeler;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Add(int id, string Miktar, string Birim, int BirimId, int? UyeId = null, string UrunAdi = null, decimal? Fiyat = null, decimal? IndirimliFiyat = null)
        {
            HttpContext.Session.Remove("UrunListesi");
            LoadCommonViewBagData();

            try
            {
                if (BirimId <= 0)
                    return Json(new { success = false, message = "Lütfen bir birim seçiniz." });

                UrunRepository urunRepository = new UrunRepository();
                List<string> join = new List<string> { "UrunFotograf" };
                var urun = urunRepository.Getir(x => x.Id == id, join);

                if (urun == null)
                    return Json(new { success = false, message = "Ürün bulunamadı!" });

                if (IndirimliFiyat.HasValue && IndirimliFiyat > 0)
                    urun.Fiyat = Math.Round(IndirimliFiyat.Value, 2);
                else if (Fiyat.HasValue && Fiyat > 0)
                    urun.Fiyat = Fiyat.Value;

                Uyeler uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");

                UrunStokRepository urunStokRepository = new UrunStokRepository();
                int stokMiktari = urunStokRepository.GetirList(x =>
                    x.UrunId == urun.Id &&
                    x.BirimlerId == BirimId &&
                    x.Durumu == 1)
                    .Sum(x => x.Stok);

                if (!int.TryParse(Miktar, out int miktar) || miktar <= 0)
                    miktar = 1;

                if (miktar > stokMiktari)
                    return Json(new { success = false, message = $"Yetersiz stok! Mevcut stok: {stokMiktari}" });

                List<Sepet> sepetList = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();

                BirimlerRepository birimlerRepository = new BirimlerRepository();
                var birimler = birimlerRepository.Getir(BirimId);

                if (birimler == null)
                    return Json(new { success = false, message = "Birim bilgisi bulunamadı!" });

                var existingItem = sepetList.FirstOrDefault(s => s.UrunId == id && s.BirimlerId == BirimId);

                if (existingItem != null)
                {
                    existingItem.Miktar += miktar;
                    existingItem.GuncellenmeTarihi = DateTime.Now;
                    existingItem.Toplam = existingItem.Fiyat * existingItem.Miktar;
                }
                else
                {
                    sepetList.Add(new Sepet(urun, miktar, Birim ?? "Adet", BirimId, uyeler, birimler));
                }

                HttpContext.Session.SetObjectAsJson("Sepet", sepetList);
                var cartSummary = CalculateCartSummary(sepetList);

                return Json(new
                {
                    success = true,
                    message = $"{miktar} adet {urun.Adi} sepete eklendi.",
                    cartCount = sepetList.Count,
                    cartTotal = cartSummary.OdenecekToplam.ToString("N2") + " TL",
                    itemsHtml = RenderCartItemsPartial(sepetList)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Add action");
                return Json(new { success = false, message = "İşlem sırasında bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Remove(int id)
        {
            HttpContext.Session.Remove("UrunListesi");
            LoadCommonViewBagData();

            try
            {
                List<Sepet> sepetList = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();
                sepetList.RemoveAll(x => x.UrunId == id);
                HttpContext.Session.SetObjectAsJson("Sepet", sepetList);
                var cartSummary = CalculateCartSummary(sepetList);

                _logger.LogInformation("Removed item {UrunId} from cart", id);

                return Json(new
                {
                    success = true,
                    message = "Ürün sepetten kaldırıldı.",
                    cartCount = sepetList.Count,
                    cartTotal = cartSummary.OdenecekToplam.ToString("N2") + " TL",
                    genelToplam = cartSummary.GenelToplam.ToString("N2"),
                    indirimMiktari = cartSummary.IndirimMiktari.ToString("N2"),
                    kargoUcreti = cartSummary.KargoUcreti.ToString("N2"),
                    odenecekToplam = cartSummary.OdenecekToplam.ToString("N2"),
                    kdvToplam = cartSummary.KDVToplam.ToString("N2"),
                    itemsHtml = RenderCartItemsPartial(sepetList)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Remove action");
                return Json(new { success = false, message = "İşlem sırasında bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int id, int birimId, int newQuantity)
        {
            HttpContext.Session.Remove("UrunListesi");
            LoadCommonViewBagData();

            try
            {
                if (newQuantity <= 0)
                    return Json(new { success = false, message = "Geçersiz miktar." });

                List<Sepet> sepetList = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();
                var sepetItem = sepetList.FirstOrDefault(s => s.UrunId == id && s.BirimlerId == birimId);

                if (sepetItem == null)
                {
                    _logger.LogWarning("Item {UrunId} not found in cart", id);
                    return Json(new { success = false, message = "Ürün sepette bulunamadı." });
                }

                UrunStokRepository urunStokRepository = new UrunStokRepository();
                int stokMiktari = urunStokRepository.GetirList(x =>
                    x.UrunId == id &&
                    x.BirimlerId == birimId &&
                    x.Durumu == 1)
                    .Sum(x => x.Stok);

                if (newQuantity > stokMiktari)
                {
                    _logger.LogWarning("Insufficient stock for item {UrunId}, requested: {NewQuantity}, available: {Stok}", id, newQuantity, stokMiktari);
                    return Json(new { success = false, message = $"Yetersiz stok! Mevcut stok: {stokMiktari}" });
                }

                sepetItem.Miktar = newQuantity;
                sepetItem.GuncellenmeTarihi = DateTime.Now;
                sepetItem.Toplam = sepetItem.Fiyat * newQuantity;

                HttpContext.Session.SetObjectAsJson("Sepet", sepetList);
                var cartSummary = CalculateCartSummary(sepetList);

                _logger.LogInformation("Updated quantity for item {UrunId} to {NewQuantity}, total: {Toplam}", id, newQuantity, sepetItem.Toplam);

                return Json(new
                {
                    success = true,
                    message = "Miktar güncellendi.",
                    itemTotal = sepetItem.Toplam,
                    cartCount = sepetList.Count,
                    cartTotal = cartSummary.OdenecekToplam.ToString("N2") + " TL",
                    genelToplam = cartSummary.GenelToplam.ToString("N2"),
                    indirimMiktari = cartSummary.IndirimMiktari.ToString("N2"),
                    kargoUcreti = cartSummary.KargoUcreti.ToString("N2"),
                    odenecekToplam = cartSummary.OdenecekToplam.ToString("N2"),
                    kdvToplam = cartSummary.KDVToplam.ToString("N2")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateQuantity action");
                return Json(new { success = false, message = "İşlem sırasında bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult ApplyDiscountCode(string code)
        {
            HttpContext.Session.Remove("UrunListesi");
            LoadCommonViewBagData();

            try
            {
                List<Sepet> sepetList = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();
                decimal genelToplam = sepetList.Sum(x => x.Toplam ?? 0);

                IndirimKoduRepository indirimKoduRepository = new IndirimKoduRepository();
                var indirimKodu = indirimKoduRepository.GetirList(x =>
                    x.Kodu == code &&
                    x.Durumu == 1 &&
                    x.BaslangicTarihi <= DateTime.Now &&
                    x.BitisTarihi >= DateTime.Now)
                    .FirstOrDefault();

                if (indirimKodu == null)
                {
                    _logger.LogWarning("Invalid discount code: {Code}", code);
                    return Json(new { success = false, message = "Geçersiz indirim kodu veya süresi dolmuş." });
                }

                if (genelToplam < indirimKodu.AltLimit)
                {
                    _logger.LogWarning("Cart total {GenelToplam} below minimum {AltLimit} for discount code {Code}", genelToplam, indirimKodu.AltLimit, code);
                    return Json(new { success = false, message = $"Bu kodu kullanmak için minimum sepet tutarı {indirimKodu.AltLimit} TL olmalıdır." });
                }

                decimal indirimMiktari = (genelToplam * indirimKodu.Orani) / 100;
                HttpContext.Session.SetInt32("IndirimId", indirimKodu.Id);
                HttpContext.Session.SetString("IndirimKodu", indirimKodu.Kodu);
                HttpContext.Session.SetString("IndirimOrani", indirimKodu.Orani.ToString());
                HttpContext.Session.SetString("IndirimMiktari", indirimMiktari.ToString("N2"));

                var cartSummary = CalculateCartSummary(sepetList, indirimMiktari);

                _logger.LogInformation("Applied discount code {Code}, discount: {IndirimMiktari}", code, indirimMiktari);

                return Json(new
                {
                    success = true,
                    message = $"%{indirimKodu.Orani} indirim uygulandı.",
                    discountAmount = indirimMiktari.ToString("N2"),
                    genelToplam = cartSummary.GenelToplam.ToString("N2"),
                    indirimMiktari = cartSummary.IndirimMiktari.ToString("N2"),
                    kargoUcreti = cartSummary.KargoUcreti.ToString("N2"),
                    odenecekToplam = cartSummary.OdenecekToplam.ToString("N2"),
                    kdvToplam = cartSummary.KDVToplam.ToString("N2")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ApplyDiscountCode action");
                return Json(new { success = false, message = "İşlem sırasında bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult GuestCheckout(string MusteriAdiSoyadi, string MusteriEmail, string MusteriTelefon, string MusteriVergiDairesi, string MusteriAdres, string Notlar)
        {
            HttpContext.Session.Remove("UrunListesi");
            LoadCommonViewBagData();

            List<Sepet> sepetList = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();
            ViewBag.Sepet = sepetList;

            try
            {
                if (string.IsNullOrEmpty(MusteriAdiSoyadi))
                    return Json(new { success = false, message = "Ad soyad zorunludur." });

                string[] adSoyad = MusteriAdiSoyadi.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string adi = adSoyad.Length > 0 ? adSoyad[0] : "";
                string soyadi = adSoyad.Length > 1 ? adSoyad[1] : "";

                var uyeler = new Uyeler
                {
                    Adi = adi,
                    Soyadi = soyadi,
                    EMail = MusteriEmail,
                    Telefon = MusteriTelefon,
                    Durumu = 1,
                    EklenmeTarihi = DateTime.Now
                };

                var uyeAdres = new UyeAdres
                {
                    Adi = adi,
                    Soyadi = soyadi,
                    Telefon = MusteriTelefon,
                    il = MusteriVergiDairesi,
                    Adres = MusteriAdres,
                    Durumu = 1,
                    EklenmeTarihi = DateTime.Now,
                    GuncellenmeTarihi = DateTime.Now,
                    AdresBasligi = "Misafir Adresi"
                };

                HttpContext.Session.SetObjectAsJson("UyeliksizUyeler", uyeler);
                HttpContext.Session.SetObjectAsJson("UyeliksizUyeAdres", uyeAdres);
                HttpContext.Session.SetString("SiparisNotu", Notlar ?? "");

                var savedUyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("UyeliksizUyeler");
                var savedAdres = HttpContext.Session.GetObjectFromJson<UyeAdres>("UyeliksizUyeAdres");

                if (savedUyeler == null || savedAdres == null)
                {
                    _logger.LogError("Failed to save guest checkout data to session.");
                    return Json(new { success = false, message = "Bilgiler kaydedilemedi. Lütfen tekrar deneyin." });
                }

                _logger.LogInformation("Guest checkout data saved for {Email}", MusteriEmail);
                return Json(new { success = true, message = "Bilgiler kaydedildi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GuestCheckout action");
                return Json(new { success = false, message = "İşlem sırasında bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Odeme(string indirimsizTutar, string indirimTutari, string OdenecekTutar, int UyeKargoAdresId)
        {
            StringBuilder Hata = new StringBuilder();
            Uyeler uyeler = null;
            UyeAdres uyeKargoAdresi = null;
            bool isGuest = false;

            try
            {
                decimal indirimTutar = string.IsNullOrEmpty(indirimTutari) ? 0 : Convert.ToDecimal(indirimTutari.Replace(".", ","), new CultureInfo("tr-TR"));
                if (!decimal.TryParse(OdenecekTutar, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal fiyat))
                {
                    TempData["ErrorMessage"] = "Geçersiz ödeme tutarı.";
                    _logger.LogWarning("Invalid payment amount: {OdenecekTutar}", OdenecekTutar);
                    return RedirectToAction("Index");
                }
                if (!decimal.TryParse(indirimsizTutar, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal tutar))
                {
                    TempData["ErrorMessage"] = "Geçersiz ödeme tutarı.";
                    _logger.LogWarning("Invalid payment amount: {indirimsizTutar}", indirimsizTutar);
                    return RedirectToAction("Index");
                }
                fiyat = Math.Round(fiyat, 2, MidpointRounding.AwayFromZero);
                tutar = Math.Round(tutar, 2, MidpointRounding.AwayFromZero);

                LoadCommonViewBagData();

                KargoUcretRepository kargoUcretRepository = new KargoUcretRepository();
                KargoUcret kargoUcretiBilgisi = kargoUcretRepository.Getir(x => x.Durumu == 1);
                decimal kargoUcret = 0;
                var kargoUcretKontrol = kargoUcretRepository.Listele()
                    .Where(x => tutar < x.SepetTutari && x.Durumu == 1)
                    .OrderByDescending(x => x.SepetTutari)
                    .FirstOrDefault();
                if (kargoUcretKontrol != null && kargoUcretiBilgisi != null)
                {
                    kargoUcret = kargoUcretiBilgisi.KargoUcreti;
                    kargoUcret = Math.Round(kargoUcret, 2, MidpointRounding.AwayFromZero);
                }

                if (HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler") != null)
                {
                    UyeAdresRepository uyeAdresRepository = new UyeAdresRepository();
                    uyeKargoAdresi = uyeAdresRepository.Getir(x => x.Id == UyeKargoAdresId);
                    if (uyeKargoAdresi == null)
                    {
                        TempData["ErrorMessage"] = "Lütfen geçerli bir kargo adresi seçiniz.";
                        _logger.LogWarning("Invalid shipping address ID: {UyeKargoAdresId}", UyeKargoAdresId);
                        return RedirectToAction("Index");
                    }

                    UyelerRepository uyelerRepository = new UyelerRepository();
                    uyeler = uyelerRepository.Getir(x => x.Durumu == 1 && x.Id == uyeKargoAdresi.UyeId);
                }
                else
                {
                    isGuest = true;
                    uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("UyeliksizUyeler");
                    uyeKargoAdresi = HttpContext.Session.GetObjectFromJson<UyeAdres>("UyeliksizUyeAdres");
                    if (uyeler == null || uyeKargoAdresi == null)
                    {
                        TempData["ErrorMessage"] = "Misafir kullanıcı bilgileri eksik. Lütfen formu doldurun.";
                        _logger.LogWarning("Missing guest user data");
                        return RedirectToAction("Index");
                    }
                    uyeler.Id = 0;
                }

                int? indirimId = HttpContext.Session.GetInt32("IndirimId");
                HttpContext.Session.SetString("OdenecekTutar", OdenecekTutar);
                HttpContext.Session.SetObjectAsJson("UyeKargoAdresi", uyeKargoAdresi);
                HttpContext.Session.SetObjectAsJson(isGuest ? "UyeliksizUyeler" : "Uyeler", uyeler);
                HttpContext.Session.SetInt32("IndirimId", indirimId ?? 0);
                HttpContext.Session.SetInt32("UyeId", uyeler.Id);

                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
                if (ipAddress == "::1")
                {
                    ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                        .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString() ?? "127.0.0.1";
                }

                Options options = new Options
                {
                    ApiKey = "sandbox-MLxeI6CAMpZLROPZzpdkH6GaP4LPYEwk",
                    SecretKey = "sandbox-QFzX4WLY5sdt2i1AsRpPGuHsHxel3TUf",
                    BaseUrl = "https://sandbox-api.iyzipay.com"
                };

                CreateCheckoutFormInitializeRequest request = new CreateCheckoutFormInitializeRequest
                {
                    Locale = Locale.TR.ToString(),
                    ConversationId = Guid.NewGuid().ToString(),
                    Currency = Currency.TRY.ToString(),
                    BasketId = Guid.NewGuid().ToString(),
                    PaymentGroup = PaymentGroup.PRODUCT.ToString(),
                    Price = fiyat.ToString("F2", CultureInfo.InvariantCulture),
                    PaidPrice = fiyat.ToString("F2", CultureInfo.InvariantCulture),
                    Buyer = new Buyer
                    {
                        Id = isGuest ? "guest_" + Guid.NewGuid().ToString() : uyeler.Id.ToString(),
                        Name = uyeler.Adi,
                        Surname = uyeler.Soyadi,
                        GsmNumber = uyeler.Telefon,
                        Email = uyeler.EMail,
                        IdentityNumber = uyeler.TCKimlikNo?.ToString() ?? "74300864791",
                        LastLoginDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        RegistrationDate = uyeler.EklenmeTarihi?.ToString("yyyy-MM-dd HH:mm:ss") ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        RegistrationAddress = uyeKargoAdresi.Adres,
                        Ip = ipAddress,
                        City = uyeKargoAdresi.il,
                        Country = "Turkey",
                        ZipCode = ""
                    },
                    ShippingAddress = new Address
                    {
                        ContactName = $"{uyeKargoAdresi.Adi} {uyeKargoAdresi.Soyadi}",
                        City = uyeKargoAdresi.il,
                        Country = "Turkey",
                        Description = uyeKargoAdresi.Adres,
                        ZipCode = ""
                    },
                    BillingAddress = new Address
                    {
                        ContactName = $"{uyeKargoAdresi.Adi} {uyeKargoAdresi.Soyadi}",
                        City = uyeKargoAdresi.il,
                        Country = "Turkey",
                        Description = uyeKargoAdresi.Adres,
                        ZipCode = ""
                    }
                };

                List<Sepet> sepet = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();
                if (!sepet.Any())
                {
                    TempData["ErrorMessage"] = "Sepet boş, ödeme yapılamaz.";
                    _logger.LogWarning("Empty cart during payment attempt");
                    return RedirectToAction("Index");
                }

                UrunRepository urunRepository = new UrunRepository();
                List<BasketItem> basketItems = new List<BasketItem>();
                bool indirimUygulandi = false;
                bool kargoUygulandi = false;

                foreach (var sepetItem in sepet)
                {
                    var urun = urunRepository.Getir(x => x.Durumu == 1 && x.Id == sepetItem.UrunId);
                    if (urun == null) continue;

                    decimal urunFiyat = sepetItem.Fiyat ?? 0;

                    for (int i = 0; i < sepetItem.Miktar; i++)
                    {
                        decimal itemFiyat = urunFiyat;

                        if (!indirimUygulandi && indirimTutar > 0)
                        {
                            itemFiyat -= indirimTutar / sepet.Sum(s => s.Miktar); // Distribute discount
                            indirimUygulandi = true;
                        }

                        if (!kargoUygulandi && kargoUcret > 0)
                        {
                            itemFiyat += kargoUcret / sepet.Sum(s => s.Miktar); // Distribute shipping
                            kargoUygulandi = true;
                        }

                        basketItems.Add(new BasketItem
                        {
                            Id = urun.Id.ToString(),
                            Name = urun.Adi,
                            Category1 = urun.UrunAnaKategori?.Adi ?? "Kategori Yok",
                            Category2 = urun.UrunAltKategori?.Adi ?? "Alt Kategori Yok",
                            ItemType = BasketItemType.PHYSICAL.ToString(),
                            Price = itemFiyat.ToString("F2", CultureInfo.InvariantCulture)
                        });
                    }
                }
                request.BasketItems = basketItems;

                Siparis siparis = new Siparis
                {
                    SiparisKodu = $"S{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4)}",
                    KargoAdresiId = UyeKargoAdresId,
                    KargoId = 1, // Default shipping method
                    ToplamTutar = tutar,
                    IndirimMiktari = indirimTutar,
                    KargoUcreti = kargoUcret,
                    OdenecekTutar = fiyat,
                    MusteriAdiSoyadi = $"{uyeler.Adi} {uyeler.Soyadi}",
                    MusteriAdres = uyeKargoAdresi.Adres,
                    MusteriFaturaAdres = uyeKargoAdresi.Adres,
                    MusteriTelefon = uyeler.Telefon,
                    MusteriEMail = uyeler.EMail,
                    MusteriVergiDairesi = uyeKargoAdresi.il,
                    MusteriVergiNumarasi = uyeler.TCKimlikNo?.ToString(),
                    SiparisNotu = HttpContext.Session.GetString("SiparisNotu"),
                    DurumId = 1, // "Bekliyor"
                    Durumu = 1,
                    EklenmeTarihi = DateTime.Now,
                    GuncellenmeTarihi = DateTime.Now,
                    UyelerId = uyeler.Id,
                    IndirimId = indirimId,
                    OdemeTipi = "Kart"
                };

                SiparisRepository siparisRepository = new SiparisRepository();
                siparisRepository.Ekle(siparis);

                Odeme odeme = new Odeme
                {
                    PaymentId = Guid.NewGuid().ToString(),
                    OdemeTutari = OdenecekTutar,
                    EklenmeTarihi = DateTime.Now,
                    BuyerName = uyeler.Adi,
                    BuyerSurname = uyeler.Soyadi,
                    BuyerGsmNumber = uyeler.Telefon
                };
                OdemeRepository odemeRep = new OdemeRepository();
                odemeRep.Ekle(odeme);

                SepetRepository sepetRepository = new SepetRepository();
                BirimlerRepository birimRep = new BirimlerRepository();
                foreach (var sep in sepet)
                {
                    var mevcutBirim = birimRep.Getir(x => x.Id == sep.BirimlerId);
                    sep.Uye = null;
                    sep.Birim = null;
                    sep.BirimlerId = mevcutBirim?.Id ?? sep.BirimlerId;
                    sep.UyeId = uyeler.Id;
                    sep.SiparisId = siparis.Id;
                    sepetRepository.Ekle(sep);
                }

                request.CallbackUrl = UyeKargoAdresId == 0
                    ? Url.Action("Index", "SiparisOnayKartUyesiz", new { sipariskodu = siparis.SiparisKodu }, Request.Scheme)
                    : Url.Action("Index", "SiparisOnayKart", null, Request.Scheme);

                CheckoutFormInitialize threedsInitialize = await CheckoutFormInitialize.Create(request, options);

                if (threedsInitialize?.ErrorCode != null)
                {
                    Hata.AppendLine($"Hata Kodu: {threedsInitialize.ErrorCode}");
                    Hata.AppendLine($"Hata Mesajı: {threedsInitialize.ErrorMessage}");
                    Hata.AppendLine($"Durum Kodu: {threedsInitialize.StatusCode}");
                    HttpContext.Session.SetString("OdemeHataMesaj", Hata.ToString());
                    _logger.LogError("Iyzipay error: {ErrorCode}, {ErrorMessage}", threedsInitialize.ErrorCode, threedsInitialize.ErrorMessage);
                    return RedirectToAction("Index", "SiparisOnaylanmadi");
                }

                if (threedsInitialize?.Status == "success")
                {
                    HttpContext.Session.SetObjectAsJson("Uyeler", uyeler);
                    HttpContext.Session.SetObjectAsJson("OdemeBilgileri", null);
                    HttpContext.Session.SetObjectAsJson("SepetBilgileri", null);
                    HttpContext.Session.SetObjectAsJson("Sepet", null);
                    _logger.LogInformation("Payment initiated successfully, redirecting to {PaymentPageUrl}", threedsInitialize.PaymentPageUrl);
                    if (UyeKargoAdresId == 0)
                    {
                        HttpContext.Session.Clear();
                    }
                    return Redirect(threedsInitialize.PaymentPageUrl);
                }

                Hata.AppendLine("Ödeme işlemi başarısız.");
                HttpContext.Session.SetString("OdemeHataMesaj", Hata.ToString());
                _logger.LogWarning("Payment failed without specific error");
                return RedirectToAction("Index", "SiparisOnaylanmadi");
            }
            catch (Exception ex)
            {
                Hata.AppendLine($"Hata: {ex.Message}");
                HttpContext.Session.SetObjectAsJson("Mesaj", new Mesaj
                {
                    Baslik = "Hata",
                    Metin = Hata.ToString(),
                    ResimDosya = string.Empty
                });
                _logger.LogError(ex, "Error in Odeme action");
                return RedirectToAction("Index", "Message");
            }
        }

        [HttpPost]
        public IActionResult KapidaOdeme(string indirimsizTutar, string indirimTutari, string OdenecekTutar, int UyeKargoAdresId)
        {
            StringBuilder Hata = new StringBuilder();
            try
            {
                decimal indirimTutar = string.IsNullOrEmpty(indirimTutari) ? 0 : Convert.ToDecimal(indirimTutari.Replace(".", ","), new CultureInfo("tr-TR"));
                if (!decimal.TryParse(OdenecekTutar, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal fiyat))
                {
                    TempData["ErrorMessage"] = "Geçersiz ödeme tutarı.";
                    _logger.LogWarning("Invalid payment amount for KapidaOdeme: {OdenecekTutar}", OdenecekTutar);
                    return RedirectToAction("Index");
                }
                fiyat = Math.Round(fiyat, 2, MidpointRounding.AwayFromZero);

                if (!decimal.TryParse(indirimsizTutar, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal tutar))
                {
                    TempData["ErrorMessage"] = "Geçersiz ödeme tutarı.";
                    _logger.LogWarning("Invalid payment amount: {indirimsizTutar}", indirimsizTutar);
                    return RedirectToAction("Index");
                }
                tutar = Math.Round(tutar, 2, MidpointRounding.AwayFromZero);

                LoadCommonViewBagData();

                KargoUcretRepository kargoUcretRepository = new KargoUcretRepository();
                KargoUcret kargoUcretiBilgisi = kargoUcretRepository.Getir(x => x.Durumu == 1);
                decimal kargoUcret = 0;
                var kargoUcretKontrol = kargoUcretRepository.Listele()
                    .Where(x => tutar < x.SepetTutari && x.Durumu == 1)
                    .OrderByDescending(x => x.SepetTutari)
                    .FirstOrDefault();
                if (kargoUcretKontrol != null && kargoUcretiBilgisi != null)
                {
                    kargoUcret = kargoUcretiBilgisi.KargoUcreti;
                    kargoUcret = Math.Round(kargoUcret, 2, MidpointRounding.AwayFromZero);
                }

                Uyeler uyeler = null;
                UyeAdres uyeKargoAdresi = null;
                bool isGuest = false;

                if (HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler") != null)
                {
                    UyeAdresRepository uyeAdresRepository = new UyeAdresRepository();
                    uyeKargoAdresi = uyeAdresRepository.Getir(x => x.Id == UyeKargoAdresId);
                    if (uyeKargoAdresi == null)
                    {
                        TempData["ErrorMessage"] = "Lütfen geçerli bir kargo adresi seçiniz.";
                        _logger.LogWarning("Invalid shipping address ID for KapidaOdeme: {UyeKargoAdresId}", UyeKargoAdresId);
                        return RedirectToAction("Index");
                    }

                    UyelerRepository uyelerRepository = new UyelerRepository();
                    uyeler = uyelerRepository.Getir(x => x.Durumu == 1 && x.Id == uyeKargoAdresi.UyeId);
                }
                else
                {
                    isGuest = true;
                    uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("UyeliksizUyeler");
                    uyeKargoAdresi = HttpContext.Session.GetObjectFromJson<UyeAdres>("UyeliksizUyeAdres");
                    if (uyeler == null || uyeKargoAdresi == null)
                    {
                        TempData["ErrorMessage"] = "Misafir kullanıcı bilgileri eksik. Lütfen formu doldurun.";
                        _logger.LogWarning("Missing guest user data for KapidaOdeme");
                        return RedirectToAction("Index");
                    }
                    uyeler.Id = 0;
                }

                HttpContext.Session.SetString("OdenecekTutar", OdenecekTutar);
                HttpContext.Session.SetObjectAsJson("UyeKargoAdresi", uyeKargoAdresi);
                HttpContext.Session.SetObjectAsJson(isGuest ? "UyeliksizUyeler" : "Uyeler", uyeler);

                List<Sepet> sepet = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();
                if (!sepet.Any())
                {
                    TempData["ErrorMessage"] = "Sepet boş, ödeme yapılamaz.";
                    _logger.LogWarning("Empty cart during KapidaOdeme attempt");
                    return RedirectToAction("Index");
                }

                int? indirimId = HttpContext.Session.GetInt32("IndirimId");
                int orderId;

                Siparis siparis = new Siparis
                {
                    SiparisKodu = $"S{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4)}",
                    KargoAdresiId = UyeKargoAdresId,
                    KargoId = 1, // Default shipping method
                    ToplamTutar = tutar,
                    IndirimMiktari = indirimTutar,
                    KargoUcreti = kargoUcret,
                    OdenecekTutar = fiyat,
                    MusteriAdiSoyadi = $"{uyeler.Adi} {uyeler.Soyadi}",
                    MusteriAdres = uyeKargoAdresi.Adres,
                    MusteriFaturaAdres = uyeKargoAdresi.Adres,
                    MusteriTelefon = uyeler.Telefon,
                    MusteriEMail = uyeler.EMail,
                    MusteriVergiDairesi = uyeKargoAdresi.il,
                    MusteriVergiNumarasi = uyeler.TCKimlikNo?.ToString(),
                    SiparisNotu = HttpContext.Session.GetString("SiparisNotu"),
                    DurumId = 1, // "Bekliyor"
                    Durumu = 1,
                    EklenmeTarihi = DateTime.Now,
                    GuncellenmeTarihi = DateTime.Now,
                    UyelerId = uyeler.Id,
                    IndirimId = indirimId,
                    OdemeTipi = "KapidaOdeme"
                };

                SiparisRepository siparisRepository = new SiparisRepository();
                siparisRepository.Ekle(siparis);
                orderId = siparis.Id;

                if (indirimId.HasValue && indirimId > 0)
                {
                    IndirimUye indirimUye = new IndirimUye
                    {
                        Durumu = 1,
                        EklenmeTarihi = DateTime.Now,
                        UyeId = uyeler.Id,
                        IndirimId = indirimId,
                        GuncellenmeTarihi = DateTime.Now
                    };
                    IndirimUyeRepository indirimUyeRepository = new IndirimUyeRepository();
                    indirimUyeRepository.Ekle(indirimUye);
                }

                SepetRepository sepetRepository = new SepetRepository();
                BirimlerRepository birimRep = new BirimlerRepository();
                foreach (var sep in sepet)
                {
                    var mevcutBirim = birimRep.Getir(x => x.Id == sep.BirimlerId);
                    sep.Uye = null;
                    sep.Birim = null;
                    sep.BirimlerId = mevcutBirim?.Id ?? sep.BirimlerId;
                    sep.UyeId = uyeler.Id;
                    sep.SiparisId = orderId;
                    sepetRepository.Ekle(sep);
                }

                Odeme odeme = new Odeme
                {
                    PaymentId = Guid.NewGuid().ToString(),
                    OdemeTutari = OdenecekTutar,
                    EklenmeTarihi = DateTime.Now,
                    BuyerName = uyeler.Adi,
                    BuyerSurname = uyeler.Soyadi,
                    BuyerGsmNumber = uyeler.Telefon
                };

                OdemeRepository odemeRep = new OdemeRepository();
                odemeRep.Ekle(odeme);

                HttpContext.Session.SetObjectAsJson("OdemeBilgileri", null);
                HttpContext.Session.SetObjectAsJson("SepetBilgileri", null);
                HttpContext.Session.SetObjectAsJson("Sepet", null);
                _logger.LogInformation("KapidaOdeme order created, order ID: {OrderId}", orderId);

                TempData["OrderId"] = orderId;
                return RedirectToAction("Index", "SiparisOnay");
            }
            catch (Exception ex)
            {
                Hata.AppendLine($"Hata: {ex.Message}");
                HttpContext.Session.SetObjectAsJson("Mesaj", new Mesaj
                {
                    Baslik = "Hata",
                    Metin = Hata.ToString(),
                    ResimDosya = string.Empty
                });
                _logger.LogError(ex, "Error in KapidaOdeme action");
                return RedirectToAction("Index", "Message");
            }
        }

        #region Helper Methods
        private void LoadCommonViewBagData()
        {
            UrunAnaKategoriRepository urunAnaKategoriRepository = new UrunAnaKategoriRepository();
            List<string> join = new List<string> { "UrunAnaKategoriFotograf" };
            ViewBag.AnaKategori = urunAnaKategoriRepository.GetirList(x => x.Durumu == 1, join);

            UrunAltKategoriRepository urunAltKategoriRepository = new UrunAltKategoriRepository();
            ViewBag.AltKategori = urunAltKategoriRepository.GetirList(x => x.Durumu == 1);

            AnaSayfaBannerMetinRepository anaSayfaBannerMetinRepository = new AnaSayfaBannerMetinRepository();
            ViewBag.AnaSayfaBannerMetin = anaSayfaBannerMetinRepository.GetirList(x => x.Durumu == 1);

            IletisimBilgileriRepository iletisimBilgileriRepository = new IletisimBilgileriRepository();
            ViewBag.Iletisim = iletisimBilgileriRepository.Getir(x => x.Durumu == 1);

            Uyeler uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");
            ViewBag.Uyeler = uyeler;

            if (uyeler != null)
            {
                UyeAdresRepository uyeAdresRepository = new UyeAdresRepository();
                List<UyeAdres> uyeadreslist = uyeAdresRepository.GetirList(x => x.UyeId == uyeler.Id && x.Durumu == 1) ?? new List<UyeAdres>();
                ViewBag.UyeAdres = uyeadreslist;
                HttpContext.Session.SetObjectAsJson("Adres", uyeadreslist);

                IndirimKoduRepository indirimKoduRepository = new IndirimKoduRepository();
                ViewBag.IndirimKodlari = indirimKoduRepository.GetirList(x =>
                    x.Durumu == 1 &&
                    x.BaslangicTarihi <= DateTime.Now &&
                    x.BitisTarihi >= DateTime.Now);
            }
            else
            {
                var guestUyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("UyeliksizUyeler");
                var guestAddress = HttpContext.Session.GetObjectFromJson<UyeAdres>("UyeliksizUyeAdres");
                ViewBag.UyeAdres = guestAddress != null ? new List<UyeAdres> { guestAddress } : new List<UyeAdres>();
                ViewBag.GuestUyeler = guestUyeler ?? new Uyeler();
            }
        }

        private (decimal GenelToplam, decimal IndirimMiktari, decimal IndirimliToplam, decimal KargoUcreti, decimal OdenecekToplam, decimal KDVToplam)
            CalculateCartSummary(List<Sepet> sepetList, decimal? manualIndirimMiktari = null)
        {
            decimal genelToplam = sepetList.Sum(x => x.Toplam ?? 0);
            decimal indirimMiktari = manualIndirimMiktari ?? 0;

            if (!manualIndirimMiktari.HasValue)
            {
                SepetIndirimRepository sepetIndirimRepository = new SepetIndirimRepository();
                var sepetIndirim = sepetIndirimRepository.GetirList(x =>
                    x.SepetTutari <= genelToplam &&
                    x.Durumu == 1)
                    .OrderByDescending(x => x.SepetTutari)
                    .FirstOrDefault();

                if (sepetIndirim != null)
                {
                    indirimMiktari = sepetIndirim.IndirimMiktari;
                }
                else
                {
                    string indirimMiktariStr = HttpContext.Session.GetString("IndirimMiktari");
                    if (!string.IsNullOrEmpty(indirimMiktariStr) && decimal.TryParse(indirimMiktariStr, out decimal sessionIndirim))
                    {
                        indirimMiktari = sessionIndirim;
                    }
                }
            }

            decimal indirimliToplam = genelToplam - indirimMiktari;
            decimal kargoUcreti = 0;

            if (indirimliToplam > 0)
            {
                KargoUcretRepository kargoUcretRepository = new KargoUcretRepository();
                var kargoUcretObj = kargoUcretRepository.Getir(x => x.Durumu == 1);

                if (kargoUcretObj != null && indirimliToplam < kargoUcretObj.SepetTutari)
                {
                    kargoUcreti = kargoUcretObj.KargoUcreti;
                }
            }

            decimal odenecekToplam = indirimliToplam + kargoUcreti;
            decimal kdvToplam = Math.Round(odenecekToplam / 11, 2);

            return (genelToplam, indirimMiktari, indirimliToplam, kargoUcreti, odenecekToplam, kdvToplam);
        }

        private string RenderCartItemsPartial(List<Sepet> sepetList)
        {
            return "";
        }
        #endregion
    }
}