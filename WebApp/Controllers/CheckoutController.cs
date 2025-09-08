using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authorization;
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
    public class CheckoutController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(IConfiguration configuration, ILogger<CheckoutController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index()
        {
            LoadCommonViewBagData();
            var sepetList = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();

            if (!sepetList.Any())
            {
                TempData["ErrorMessage"] = "Sepetinizde ürün bulunmamaktadır.";
                return RedirectToAction("Index", "ShoppingCart");
            }

            var cartSummary = CalculateCartSummary(sepetList);
            var model = new SepetViewModel
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

            ViewBag.SepetSayi = sepetList.Count;
            _logger.LogInformation("Checkout loaded with {Count} items", sepetList.Count);
            return View(model);
        }

        [HttpPost]
        public IActionResult ApplyCoupon(string code)
        {
            try
            {
                var sepetList = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();
                var genelToplam = sepetList.Sum(x => x.Toplam ?? 0);

                var indirimKodu = new IndirimKoduRepository()
                    .GetirList(x => x.Kodu == code && x.Durumu == 1 && x.BaslangicTarihi <= DateTime.Now && x.BitisTarihi >= DateTime.Now)
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

                var indirimMiktari = (genelToplam * indirimKodu.Orani) / 100;
                HttpContext.Session.SetInt32("IndirimId", indirimKodu.Id);
                HttpContext.Session.SetString("IndirimKodu", indirimKodu.Kodu);
                HttpContext.Session.SetString("IndirimOrani", indirimKodu.Orani.ToString(CultureInfo.InvariantCulture));
                HttpContext.Session.SetString("IndirimMiktari", indirimMiktari.ToString("F2", CultureInfo.InvariantCulture));

                var cartSummary = CalculateCartSummary(sepetList, indirimMiktari);
                _logger.LogInformation("Applied discount code {Code}, discount: {IndirimMiktari}", code, indirimMiktari);

                return Json(new
                {
                    success = true,
                    message = $"%{indirimKodu.Orani} indirim uygulandı.",
                    discountAmount = indirimMiktari.ToString("N2", CultureInfo.GetCultureInfo("tr-TR")),
                    genelToplam = cartSummary.GenelToplam.ToString("N2", CultureInfo.GetCultureInfo("tr-TR")),
                    indirimMiktari = cartSummary.IndirimMiktari.ToString("N2", CultureInfo.GetCultureInfo("tr-TR")),
                    kargoUcreti = cartSummary.KargoUcreti.ToString("N2", CultureInfo.GetCultureInfo("tr-TR")),
                    odenecekToplam = cartSummary.OdenecekToplam.ToString("N2", CultureInfo.GetCultureInfo("tr-TR")),
                    kdvToplam = cartSummary.KDVToplam.ToString("N2", CultureInfo.GetCultureInfo("tr-TR"))
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ApplyCoupon action");
                return Json(new { success = false, message = "İşlem sırasında bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ProcessCheckout(int UyeKargoAdresId, CheckoutFormModel formData = null, string MusteriAdiSoyadi = null, string MusteriEmail = null, string MusteriTelefon = null, string MusteriVergiDairesi = null, string MusteriAdres = null, string Notlar = null, string PaymentMethod = "CreditCard")
        {
            try
            {
                var sepetList = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();
                if (!sepetList.Any())
                {
                    return Json(new { success = false, message = "Sepetiniz boş. Lütfen sepetinize ürün ekleyin." });
                }

                Uyeler uyeler;
                UyeAdres uyeAdres;
                var isGuest = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler") == null;

                if (isGuest)
                {
                    if (string.IsNullOrEmpty(MusteriAdiSoyadi))
                        return Json(new { success = false, message = "Ad soyad zorunludur." });

                    var adSoyad = MusteriAdiSoyadi.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    uyeler = new Uyeler
                    {
                        Adi = adSoyad.Length > 0 ? adSoyad[0] : "",
                        Soyadi = adSoyad.Length > 1 ? adSoyad[1] : "",
                        EMail = MusteriEmail,
                        Telefon = MusteriTelefon,
                        Durumu = PaymentMethod == "KapidaOdeme" ? 0 : 1,
                        EklenmeTarihi = DateTime.Now
                    };

                    uyeAdres = new UyeAdres
                    {
                        Adi = uyeler.Adi,
                        Soyadi = uyeler.Soyadi,
                        Telefon = uyeler.Telefon,
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
                    HttpContext.Session.SetInt32("UyeKargoAdresId", 0);
                }
                else
                {
                    uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");
                    if (formData.address_type == "new")
                    {
                        uyeAdres = new UyeAdres
                        {
                            UyeId = uyeler.Id,
                            Adi = formData.Adi,
                            Soyadi = formData.Soyadi,
                            Telefon = uyeler.Telefon,
                            il = formData.Il,
                            Adres = formData.Adres,
                            Durumu = 1,
                            EklenmeTarihi = DateTime.Now,
                            GuncellenmeTarihi = DateTime.Now,
                            AdresBasligi = "Yeni Adres"
                        };
                        new UyeAdresRepository().Ekle(uyeAdres);
                    }
                    else
                    {
                        uyeAdres = new UyeAdresRepository().Getir(x => x.Id == formData.UyeKargoAdresId && x.UyeId == uyeler.Id && x.Durumu == 1);
                        if (uyeAdres == null)
                        {
                            return Json(new { success = false, message = "Geçerli bir kargo adresi seçiniz." });
                        }
                    }

                    HttpContext.Session.SetObjectAsJson("UyeKargoAdresi", uyeAdres);
                    HttpContext.Session.SetString("SiparisNotu", formData.SiparisNotu ?? "");
                    HttpContext.Session.SetInt32("UyeKargoAdresId", uyeAdres.Id);
                }

                var cartSummary = CalculateCartSummary(sepetList, paymentMethod: PaymentMethod);
                HttpContext.Session.SetString("OdenecekTutar", cartSummary.OdenecekToplam.ToString("F2", CultureInfo.InvariantCulture));
                HttpContext.Session.SetString("IndirimsizTutar", cartSummary.GenelToplam.ToString("F2", CultureInfo.InvariantCulture));
                HttpContext.Session.SetString("IndirimTutari", cartSummary.IndirimMiktari.ToString("F2", CultureInfo.InvariantCulture));
                _logger.LogInformation("Checkout processed for {Email}, PaymentMethod: {PaymentMethod}, AddressId: {AddressId}", uyeler.EMail, PaymentMethod, uyeAdres?.Id ?? 0);

                return Json(new
                {
                    success = true,
                    message = "Bilgiler kaydedildi.",
                    redirectUrl = PaymentMethod == "CreditCard" ? Url.Action("Odeme") : Url.Action("KapidaOdeme")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProcessCheckout action");
                return Json(new { success = false, message = "İşlem sırasında bir hata oluştu: " + ex.Message });
            }
        }

        public async Task<IActionResult> Odeme()
        {
            var indirimsizTutar = HttpContext.Session.GetString("IndirimsizTutar");
            var indirimTutari = HttpContext.Session.GetString("IndirimTutari");
            var odenecekTutar = HttpContext.Session.GetString("OdenecekTutar");
            var uyeKargoAdresId = HttpContext.Session.GetInt32("UyeKargoAdresId") ?? 0;

            return await ProcessPayment(indirimsizTutar, indirimTutari, odenecekTutar, uyeKargoAdresId, "CreditCard");
        }

        public async Task<IActionResult> KapidaOdeme()
        {
            var indirimsizTutar = HttpContext.Session.GetString("IndirimsizTutar");
            var indirimTutari = HttpContext.Session.GetString("IndirimTutari");
            var odenecekTutar = HttpContext.Session.GetString("OdenecekTutar");
            var uyeKargoAdresId = HttpContext.Session.GetInt32("UyeKargoAdresId") ?? 0;

            return await ProcessPayment(indirimsizTutar, indirimTutari, odenecekTutar, uyeKargoAdresId, "KapidaOdeme");
        }

        private async Task<IActionResult> ProcessPayment(string indirimsizTutar, string indirimTutari, string odenecekTutar, int uyeKargoAdresId, string paymentMethod)
        {
            var hata = new StringBuilder();
            try
            {
                if (!decimal.TryParse(odenecekTutar, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal fiyat) ||
                    !decimal.TryParse(indirimsizTutar, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal tutar))
                {
                    TempData["ErrorMessage"] = "Geçersiz ödeme tutarı.";
                    _logger.LogWarning("Invalid payment amount: OdenecekTutar={OdenecekTutar}, indirimsizTutar={indirimsizTutar}", odenecekTutar, indirimsizTutar);
                    return RedirectToAction("Index");
                }

                decimal indirimTutar = string.IsNullOrEmpty(indirimTutari) ? 0 : decimal.Parse(indirimTutari, NumberStyles.Any, CultureInfo.InvariantCulture);
                fiyat = Math.Round(fiyat, 2, MidpointRounding.AwayFromZero);
                tutar = Math.Round(tutar, 2, MidpointRounding.AwayFromZero);

                LoadCommonViewBagData();
                var sepet = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();
                if (!sepet.Any())
                {
                    TempData["ErrorMessage"] = "Sepet boş, ödeme yapılamaz.";
                    _logger.LogWarning("Empty cart during payment attempt");
                    return RedirectToAction("Index");
                }

                bool isGuest = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler") == null;
                Uyeler uyeler = isGuest ? HttpContext.Session.GetObjectFromJson<Uyeler>("UyeliksizUyeler") : HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");
                UyeAdres uyeKargoAdresi;

                if (isGuest)
                {
                    uyeKargoAdresi = HttpContext.Session.GetObjectFromJson<UyeAdres>("UyeliksizUyeAdres");
                }
                else
                {
                    uyeKargoAdresi = uyeKargoAdresId > 0
                        ? new UyeAdresRepository().Getir(x => x.Id == uyeKargoAdresId && x.UyeId == uyeler.Id && x.Durumu == 1)
                        : HttpContext.Session.GetObjectFromJson<UyeAdres>("UyeKargoAdresi");
                }

                if (uyeler == null || uyeKargoAdresi == null)
                {
                    TempData["ErrorMessage"] = isGuest ? "Misafir kullanıcı bilgileri eksik." : "Geçerli bir kargo adresi seçiniz.";
                    _logger.LogWarning("Missing user/address data: isGuest={IsGuest}, UyeKargoAdresId={UyeKargoAdresId}", isGuest, uyeKargoAdresId);
                    return RedirectToAction("Index");
                }

                if (isGuest) uyeler.Id = 0;

                var cartSummary = CalculateCartSummary(sepet, paymentMethod: paymentMethod);
                var kargoUcret = cartSummary.KargoUcreti;
                var odenecekToplam = cartSummary.OdenecekToplam;

                int? indirimId = HttpContext.Session.GetInt32("IndirimId");
                HttpContext.Session.SetString("OdenecekTutar", odenecekToplam.ToString("F2", CultureInfo.InvariantCulture));
                HttpContext.Session.SetObjectAsJson("UyeKargoAdresi", uyeKargoAdresi);
                HttpContext.Session.SetObjectAsJson(isGuest ? "UyeliksizUyeler" : "Uyeler", uyeler);
                HttpContext.Session.SetInt32("IndirimId", indirimId ?? 0);
                HttpContext.Session.SetInt32("UyeId", uyeler.Id);

                var existingOrderId = HttpContext.Session.GetInt32("CurrentOrderId");
                if (existingOrderId.HasValue)
                {
                    _logger.LogInformation("Order already created for this session, ID: {OrderId}", existingOrderId);
                    TempData["OrderId"] = existingOrderId;
                    ClearSessionData(isGuest);
                    return RedirectToAction("Index", "SiparisOnay");
                }

                int orderId = CreateOrder(uyeler, uyeKargoAdresi, sepet, tutar, indirimTutar, kargoUcret, odenecekToplam, isGuest, indirimId, paymentMethod);
                HttpContext.Session.SetInt32("CurrentOrderId", orderId);

                if (paymentMethod == "KapidaOdeme")
                {
                    new OdemeRepository().Ekle(new Odeme
                    {
                        PaymentId = Guid.NewGuid().ToString(),
                        OdemeTutari = odenecekToplam.ToString("F2", CultureInfo.InvariantCulture),
                        EklenmeTarihi = DateTime.Now,
                        BuyerName = uyeler.Adi,
                        BuyerSurname = uyeler.Soyadi,
                        BuyerGsmNumber = uyeler.Telefon
                    });

                    ClearSessionData(isGuest);
                    TempData["OrderId"] = orderId;
                    _logger.LogInformation("Cash on Delivery order created, ID: {OrderId}", orderId);
                    return RedirectToAction("Index", "SiparisOnay");
                }

                var request = CreateIyzicoRequest(uyeler, uyeKargoAdresi, sepet, odenecekToplam, indirimTutar, kargoUcret, isGuest);
                request.CallbackUrl = uyeKargoAdresId == 0
                    ? Url.Action("Index", "SiparisOnayKartUyesiz", new { sipariskodu = $"S{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4)}" }, Request.Scheme)
                    : Url.Action("Index", "SiparisOnayKart", null, Request.Scheme);

                var options = new Options
                {
                    ApiKey = _configuration["Iyzico:ApiKey"],
                    SecretKey = _configuration["Iyzico:SecretKey"],
                    BaseUrl = _configuration["Iyzico:BaseUrl"]
                };

                if (string.IsNullOrEmpty(options.ApiKey) || string.IsNullOrEmpty(options.SecretKey) || string.IsNullOrEmpty(options.BaseUrl))
                {
                    _logger.LogError("Iyzico configuration is missing or invalid.");
                    TempData["ErrorMessage"] = "Ödeme sistemi yapılandırması eksik.";
                    return RedirectToAction("Index");
                }

                var threedsInitialize = await CheckoutFormInitialize.Create(request, options);

                if (threedsInitialize?.Status != "success")
                {
                    hata.AppendLine($"Hata Kodu: {threedsInitialize?.ErrorCode}");
                    hata.AppendLine($"Hata Mesajı: {threedsInitialize?.ErrorMessage}");
                    HttpContext.Session.SetString("OdemeHataMesaj", hata.ToString());
                    _logger.LogError("Iyzico error: {ErrorCode}, {ErrorMessage}", threedsInitialize?.ErrorCode, threedsInitialize?.ErrorMessage);
                    return RedirectToAction("Index", "SiparisOnaylanmadi");
                }

                if (string.IsNullOrEmpty(threedsInitialize.PaymentPageUrl))
                {
                    hata.AppendLine("Iyzico ödeme sayfası URL'si alınamadı.");
                    HttpContext.Session.SetString("OdemeHataMesaj", hata.ToString());
                    _logger.LogError("Iyzico PaymentPageUrl is null or empty.");
                    return RedirectToAction("Index", "SiparisOnaylanmadi");
                }

                ClearSessionData(isGuest);
                _logger.LogInformation("Payment initiated successfully, redirecting to {PaymentPageUrl}", threedsInitialize.PaymentPageUrl);
                return Redirect(threedsInitialize.PaymentPageUrl);
            }
            catch (Exception ex)
            {
                hata.AppendLine($"Hata: {ex.Message}");
                HttpContext.Session.SetObjectAsJson("Mesaj", new Mesaj
                {
                    Baslik = "Hata",
                    Metin = hata.ToString(),
                    ResimDosya = string.Empty
                });
                _logger.LogError(ex, "Error in ProcessPayment action, PaymentMethod: {PaymentMethod}", paymentMethod);
                return RedirectToAction("Index", "Message");
            }
        }

        #region Helper Methods
        private void LoadCommonViewBagData()
        {
            var sepetList = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();
            ViewBag.SepetSayi = sepetList.Count;
            ViewBag.Sepet = sepetList;

            ViewBag.AnaKategori = new UrunAnaKategoriRepository()
                .GetirList(x => x.Durumu == 1, new List<string> { "UrunAnaKategoriFotograf" });
            ViewBag.AltKategori = new UrunAltKategoriRepository().GetirList(x => x.Durumu == 1);
            ViewBag.AnaSayfaBannerMetin = new AnaSayfaBannerMetinRepository().GetirList(x => x.Durumu == 1);
            ViewBag.Iletisim = new IletisimBilgileriRepository().Getir(x => x.Durumu == 1);

            var uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");
            ViewBag.Uyeler = uyeler;

            if (uyeler != null)
            {
                var uyeAdresList = new UyeAdresRepository().GetirList(x => x.UyeId == uyeler.Id && x.Durumu == 1) ?? new List<UyeAdres>();
                ViewBag.UyeAdres = uyeAdresList;
                HttpContext.Session.SetObjectAsJson("Adres", uyeAdresList);
                ViewBag.IndirimKodlari = new IndirimKoduRepository()
                    .GetirList(x => x.Durumu == 1 && x.BaslangicTarihi <= DateTime.Now && x.BitisTarihi >= DateTime.Now);
            }
            else
            {
                var guestUyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("UyeliksizUyeler");
                var guestAddress = HttpContext.Session.GetObjectFromJson<UyeAdres>("UyeliksizUyeAdres");
                ViewBag.UyeAdres = guestAddress != null ? new List<UyeAdres> { guestAddress } : new List<UyeAdres>();
                ViewBag.GuestUyeler = guestUyeler ?? new Uyeler();
            }
        }

        private decimal CalculateShippingFee(decimal tutar)
        {
            var kargoUcretRepository = new KargoUcretRepository();
            var kargoUcretiBilgisi = kargoUcretRepository.Getir(x => x.Durumu == 1);
            var kargoUcretKontrol = kargoUcretRepository.Listele()
                .Where(x => tutar < x.SepetTutari && x.Durumu == 1)
                .OrderByDescending(x => x.SepetTutari)
                .FirstOrDefault();

            return (kargoUcretKontrol != null && kargoUcretiBilgisi != null)
                ? Math.Round(kargoUcretiBilgisi.KargoUcreti, 2, MidpointRounding.AwayFromZero)
                : 0;
        }

        private (decimal GenelToplam, decimal IndirimMiktari, decimal IndirimliToplam, decimal KargoUcreti, decimal OdenecekToplam, decimal KDVToplam)
            CalculateCartSummary(List<Sepet> sepetList, decimal? manualIndirimMiktari = null, string paymentMethod = null)
        {
            var genelToplam = sepetList.Sum(x => x.Toplam ?? 0);
            decimal indirimMiktari;

            if (manualIndirimMiktari.HasValue)
            {
                indirimMiktari = manualIndirimMiktari.Value;
            }
            else if (HttpContext.Session.GetString("IndirimMiktari") is string indirimStr && decimal.TryParse(indirimStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedIndirim))
            {
                indirimMiktari = parsedIndirim;
            }
            else
            {
                indirimMiktari = new SepetIndirimRepository()
                    .GetirList(x => x.SepetTutari <= genelToplam && x.Durumu == 1)
                    .OrderByDescending(x => x.SepetTutari)
                    .FirstOrDefault()?.IndirimMiktari ?? 0;
            }

            var indirimliToplam = genelToplam - indirimMiktari;
            var kargoUcreti = indirimliToplam > 0 ? CalculateShippingFee(indirimliToplam) : 0;
            if (paymentMethod == "KapidaOdeme") kargoUcreti += 70.00m;

            var odenecekToplam = indirimliToplam + kargoUcreti;
            var kdvToplam = Math.Round(odenecekToplam / 11, 2, MidpointRounding.AwayFromZero);

            return (genelToplam, indirimMiktari, indirimliToplam, kargoUcreti, odenecekToplam, kdvToplam);
        }

        private int CreateOrder(Uyeler uyeler, UyeAdres uyeKargoAdresi, List<Sepet> sepet, decimal tutar, decimal indirimTutar, decimal kargoUcret, decimal fiyat, bool isGuest, int? indirimId, string paymentMethod)
        {
            var siparisKodu = $"S{(isGuest ? "G" : "")}{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4)}";
            var sepetRepository = new SepetRepository();
            var guestSepetRepository = new GuestSepetRepository(); // New repository for GuestSepet
            var birimRep = new BirimlerRepository();

            if (isGuest)
            {
                var siparis = new SiparisGuest
                {
                    SiparisKodu = siparisKodu,
                    MusteriAdiSoyadi = $"{uyeler.Adi} {uyeler.Soyadi}",
                    MusteriAdres = uyeKargoAdresi.Adres,
                    MusteriTelefon = uyeler.Telefon,
                    MusteriEmail = uyeler.EMail,
                    MusteriVergiDairesi = uyeKargoAdresi.il,
                    MusteriVergiNumarasi = uyeler.TCKimlikNo?.ToString(),
                    DurumId = 1,
                    EklenmeTarihi = DateTime.Now,
                    IPAdresi = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1",
                    Notlar = HttpContext.Session.GetString("SiparisNotu"),
                    ToplamTutar = tutar,
                    IndirimMiktari = indirimTutar,
                    KargoUcreti = kargoUcret,
                    OdenecekTutar = fiyat,
                    OdemeTipi = paymentMethod,
                    Durumu = 1
                };
                new SiparisGuestRepository().Ekle(siparis);

                foreach (var sep in sepet)
                {
                    var mevcutBirim = birimRep.Getir(x => x.Id == sep.BirimlerId);
                    var urun = new UrunRepository().Getir(x => x.Id == sep.UrunId);

                    var guestSepet = new GuestSepet
                    {
                        UrunId = sep.UrunId,
                        Fiyat = sep.Fiyat,
                        UrunAdi = urun?.Adi ?? sep.UrunAdi,
                        Birim = sep.Birim,
                        Miktar = sep.Miktar,
                        UrunResmi = sep.UrunResmi,
                        Toplam = sep.Toplam,
                        SiparisGuestId = siparis.Id,
                        Durumu = 1,
                        EklenmeTarihi = DateTime.Now,
                        GuncellenmeTarihi = DateTime.Now,
                        SiparisKodu = siparisKodu,
                        BirimlerId = mevcutBirim?.Id ?? sep.BirimlerId
                    };
                    guestSepetRepository.Ekle(guestSepet);
                }
                return siparis.Id;
            }
            else
            {
                var siparis = new Siparis
                {
                    SiparisKodu = siparisKodu,
                    KargoAdresiId = uyeKargoAdresi?.Id ?? 0,
                    KargoId = 1,
                    ToplamTutar = tutar,
                    IndirimMiktari = indirimTutar,
                    KargoUcreti = kargoUcret,
                    OdenecekTutar = fiyat,
                    MusteriAdiSoyadi = $"{uyeler.Adi} {uyeler.Soyadi}",
                    MusteriAdres = uyeKargoAdresi?.Adres ?? "",
                    MusteriFaturaAdres = uyeKargoAdresi?.Adres ?? "",
                    MusteriTelefon = uyeler.Telefon,
                    MusteriEMail = uyeler.EMail,
                    MusteriVergiDairesi = uyeKargoAdresi?.il ?? "",
                    MusteriVergiNumarasi = uyeler.TCKimlikNo?.ToString(),
                    SiparisNotu = HttpContext.Session.GetString("SiparisNotu"),
                    DurumId = 1,
                    Durumu = 1,
                    EklenmeTarihi = DateTime.Now,
                    GuncellenmeTarihi = DateTime.Now,
                    UyelerId = uyeler.Id,
                    IndirimId = indirimId,
                    OdemeTipi = paymentMethod
                };
                new SiparisRepository().Ekle(siparis);

                if (indirimId.HasValue && indirimId > 0)
                {
                    new IndirimUyeRepository().Ekle(new IndirimUye
                    {
                        Durumu = 1,
                        EklenmeTarihi = DateTime.Now,
                        UyeId = uyeler.Id,
                        IndirimId = indirimId,
                        GuncellenmeTarihi = DateTime.Now
                    });
                }

                foreach (var sep in sepet)
                {
                    var mevcutBirim = birimRep.Getir(x => x.Id == sep.BirimlerId);
                    sep.Uye = null;
                    sep.Birim = null;
                    sep.BirimlerId = mevcutBirim?.Id ?? sep.BirimlerId;
                    sep.UyeId = uyeler.Id;
                    sep.SiparisId = siparis.Id;
                    sep.Durumu = 1;
                    sep.EklenmeTarihi = DateTime.Now;
                    sepetRepository.Ekle(sep);
                }
                return siparis.Id;
            }
        }

        private CreateCheckoutFormInitializeRequest CreateIyzicoRequest(Uyeler uyeler, UyeAdres uyeKargoAdresi, List<Sepet> sepet, decimal fiyat, decimal indirimTutar, decimal kargoUcret, bool isGuest)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            if (ipAddress == "::1")
                ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                    .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString() ?? "127.0.0.1";

            var request = new CreateCheckoutFormInitializeRequest
            {
                Locale = Locale.TR.ToString(),
                ConversationId = Guid.NewGuid().ToString(),
                Currency = Currency.TRY.ToString(),
                BasketId = Guid.NewGuid().ToString(),
                PaymentGroup = PaymentGroup.PRODUCT.ToString(),
                Price = fiyat.ToString("F2", CultureInfo.InvariantCulture), // Toplam ödeme tutarı
                PaidPrice = fiyat.ToString("F2", CultureInfo.InvariantCulture), // Ödenecek tutar (aynı olmalı)
                Buyer = new Buyer
                {
                    Id = isGuest ? "guest_" + Guid.NewGuid().ToString() : uyeler.Id.ToString(),
                    Name = uyeler.Adi,
                    Surname = uyeler.Soyadi,
                    GsmNumber = uyeler.Telefon,
                    Email = uyeler.EMail ?? "example@gmail.com",
                    IdentityNumber = "74300864791",
                    LastLoginDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    RegistrationDate = uyeler.EklenmeTarihi?.ToString("yyyy-MM-dd HH:mm:ss") ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    RegistrationAddress = uyeKargoAdresi?.Adres ?? "",
                    Ip = ipAddress,
                    City = uyeKargoAdresi?.il ?? "",
                    Country = "Turkey",
                    ZipCode = ""
                },
                ShippingAddress = new Address
                {
                    ContactName = $"{uyeKargoAdresi?.Adi ?? uyeler.Adi} {uyeKargoAdresi?.Soyadi ?? uyeler.Soyadi}",
                    City = uyeKargoAdresi?.il ?? "",
                    Country = "Turkey",
                    Description = uyeKargoAdresi?.Adres ?? "",
                    ZipCode = ""
                },
                BillingAddress = new Address
                {
                    ContactName = $"{uyeKargoAdresi?.Adi ?? uyeler.Adi} {uyeKargoAdresi?.Soyadi ?? uyeler.Soyadi}",
                    City = uyeKargoAdresi?.il ?? "",
                    Country = "Turkey",
                    Description = uyeKargoAdresi?.Adres ?? "",
                    ZipCode = ""
                }
            };

            var urunRepository = new UrunRepository();
            var basketItems = new List<BasketItem>();

            // Kargo ücretini ayrı bir öğe olarak ekleyelim
            if (kargoUcret > 0)
            {
                basketItems.Add(new BasketItem
                {
                    Id = "SHIPPING",
                    Name = "Kargo Ücreti",
                    Category1 = "Kargo",
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Price = kargoUcret.ToString("F2", CultureInfo.InvariantCulture)
                });
            }

            // Sepet öğelerini ekleyelim
            foreach (var sepetItem in sepet)
            {
                var urun = urunRepository.Getir(x => x.Durumu == 1 && x.Id == sepetItem.UrunId);
                if (urun == null) continue;

                var urunFiyat = Convert.ToDecimal(sepetItem.Fiyat, CultureInfo.InvariantCulture);
                var toplamUrunFiyati = urunFiyat * sepetItem.Miktar;

                // İndirimi bu ürüne orantılı olarak dağıtalım
                var urunIndirim = (toplamUrunFiyati / sepet.Sum(x => x.Fiyat * x.Miktar)) * indirimTutar;
                var indirimliFiyat = toplamUrunFiyati - urunIndirim;

                basketItems.Add(new BasketItem
                {
                    Id = urun.Id.ToString(),
                    Name = urun.Adi,
                    Category1 = urun.UrunAnaKategori?.Adi ?? "Kategori Yok",
                    Category2 = urun.UrunAltKategori?.Adi ?? "Alt Kategori Yok",
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Price =Convert.ToString(indirimliFiyat)
                });
            }

            // Toplam kontrolü yapalım
            decimal basketTotal = basketItems.Sum(x => decimal.Parse(x.Price, CultureInfo.InvariantCulture));
            if (Math.Abs(basketTotal - fiyat) > 0.01m)
            {
                // Küçük bir fark varsa, bunu son ürüne ekleyelim/düşürelim
                var difference = fiyat - basketTotal;
                if (basketItems.Any())
                {
                    var lastItem = basketItems.Last();
                    var lastItemPrice = decimal.Parse(lastItem.Price, CultureInfo.InvariantCulture);
                    lastItem.Price = (lastItemPrice + difference).ToString("F2", CultureInfo.InvariantCulture);
                }
            }

            request.BasketItems = basketItems;
            return request;
        }

        private void ClearSessionData(bool isGuest)
        {
            HttpContext.Session.SetObjectAsJson("OdemeBilgileri", null);
            HttpContext.Session.SetObjectAsJson("SepetBilgileri", null);
            HttpContext.Session.SetObjectAsJson("Sepet", null);
            if (isGuest)
            {
                HttpContext.Session.Remove("UyeliksizUyeler");
                HttpContext.Session.Remove("UyeliksizUyeAdres");
                HttpContext.Session.Remove("SiparisNotu");
                HttpContext.Session.Remove("UyeKargoAdresId");
            }
            HttpContext.Session.Remove("CurrentOrderId");
        }
        #endregion
    }

    public class CheckoutFormModel
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string EMail { get; set; }
        public string Telefon { get; set; }
        public string Adres { get; set; }
        public string Il { get; set; }
        public string PaymentMethod { get; set; }
        public string SiparisNotu { get; set; }
        public int UyeKargoAdresId { get; set; }
        public string address_type { get; set; }
    }
}
