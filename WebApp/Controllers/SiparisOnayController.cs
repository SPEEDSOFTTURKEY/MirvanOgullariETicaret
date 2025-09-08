using Iyzipay.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class SiparisOnayController : BaseController
    {
        private readonly ILogger<SiparisOnayController> _logger;

        public SiparisOnayController(ILogger<SiparisOnayController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            await LoadCommonData();

            if (TempData["OrderId"] == null)
                return RedirectToAction("Error", "Home");

            int orderId = Convert.ToInt32(TempData["OrderId"]);
            try
            {
                SiparisGuest siparisGuest = new SiparisGuest();
                SiparisGuestRepository siparisGuestRepository = new SiparisGuestRepository();
                siparisGuest = siparisGuestRepository.Getir(orderId);
                if (siparisGuest != null)
                {
                    ViewBag.SiparisNo = siparisGuest.SiparisKodu;
                }

                // Retrieve user data
                Uyeler uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");
                bool isGuest = uyeler == null;
                if (isGuest)
                {
                    uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("UyeliksizUyeler");
                }
                ViewBag.Uyeler = uyeler;
                ViewBag.IsGuest = isGuest;

                // Retrieve address data
                UyeAdres kargoAdresi = HttpContext.Session.GetObjectFromJson<UyeAdres>("UyeKargoAdresi");
                UyeAdres faturaAdresi = HttpContext.Session.GetObjectFromJson<UyeAdres>("UyeFaturaAdresi") ?? kargoAdresi; // Fallback to kargoAdresi if faturaAdresi is null
                ViewBag.KargoAdresi = kargoAdresi;
                ViewBag.FaturaAdresi = faturaAdresi;

                // Retrieve cart data
                List<Sepet> sepets = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("SepetBilgileri") ?? new List<Sepet>();
                ViewBag.SepetSayi = sepets.Count;
                ViewBag.SepetBilgileri = sepets;

                // Retrieve payment data
                Odeme odeme = HttpContext.Session.GetObjectFromJson<Odeme>("OdemeBilgileri");
                ViewBag.OdemeBilgileri = odeme;
                ViewBag.OdenecekTutar = HttpContext.Session.GetString("OdenecekTutar");

                if (sepets.Any() && kargoAdresi != null && odeme != null)
                {
                    // Send confirmation email
                    EMail mail = new EMail
                    {
                        EmailFrom = "2kids2momsiparis@gmail.com",
                        EmailTo = "2kids2momsiparis@gmail.com",
                        EmailHeader = "2KİDS2MOM SİPARİŞ BİLGİLENDİRME",
                        EmailSubject = "2KİDS2MOM SİPARİŞ BİLGİLENDİRME",
                        Html = true,
                        Port = 587,
                        Host = "smtp.gmail.com",
                        Password = "uraenfbwynetexwe"
                    };

                    StringBuilder emailBody = new StringBuilder();
                    emailBody.Append("<p style='font-weight:bold'>2KİDS2MOM SİPARİŞ BİLGİLENDİRME</p>");
                    emailBody.Append("<p>Sipariş Ödeme Tipi: Kapıda Ödeme</p>");
                    emailBody.Append("<p>Sipariş Tarihi: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "</p>");
                    emailBody.Append("<hr>");

                    foreach (var sepet in sepets)
                    {
                        emailBody.Append("<p><strong>Müşteri Adı Soyadı:</strong> " + (uyeler?.Adi + " " + uyeler?.Soyadi ?? "Misafir") + "</p>");
                        emailBody.Append("<p><strong>Müşteri Telefon Numarası:</strong> " + (uyeler?.Telefon ?? "Bilinmiyor") + "</p>");
                        emailBody.Append("<p><strong>Ürün Adı:</strong> " + sepet.UrunAdi + "</p>");
                        emailBody.Append("<p><strong>Fiyat:</strong> " + sepet.Fiyat + " TL</p>");
                        emailBody.Append("<p><strong>Beden:</strong> " + (sepet.Birimler?.Adi ?? "Belirtilmemiş") + "</p>");
                        emailBody.Append("<p><strong>Adet:</strong> " + sepet.Miktar + "</p>");
                        emailBody.Append("<p><strong>Toplam Fiyat:</strong> " + sepet.Toplam + " TL</p>");
                        emailBody.Append("<hr>");
                    }

                    emailBody.Append("<p><strong>Kargo Adresi Bilgileri:</strong><br>" +
                                     "<strong>Alıcı Adı:</strong> " + (kargoAdresi.Adi + " " + kargoAdresi.Soyadi) + "<br>" +
                                     "<strong>Telefon Numarası:</strong> " + kargoAdresi.Telefon + "<br>" +
                                     "<strong>Mahalle:</strong> " + kargoAdresi.Mahalle + "<br>" +
                                     "<strong>İlçe:</strong> " + kargoAdresi.ilce + "<br>" +
                                     "<strong>İl:</strong> " + kargoAdresi.il + "<br>" +
                                     "<strong>Adres:</strong> " + kargoAdresi.Adres + "</p>");

                    emailBody.Append("<p><strong>Fatura Adresi Bilgileri:</strong><br>" +
                                     "<strong>Alıcı Adı:</strong> " + (faturaAdresi.Adi + " " + faturaAdresi.Soyadi) + "<br>" +
                                     "<strong>Telefon Numarası:</strong> " + faturaAdresi.Telefon + "<br>" +
                                     "<strong>Mahalle:</strong> " + faturaAdresi.Mahalle + "<br>" +
                                     "<strong>İlçe:</strong> " + faturaAdresi.ilce + "<br>" +
                                     "<strong>İl:</strong> " + faturaAdresi.il + "<br>" +
                                     "<strong>Adres:</strong> " + faturaAdresi.Adres + "</p>");

                    emailBody.Append("<p><strong>Toplam Ödeme Tutarı:</strong> " + odeme.OdemeTutari + " TL</p>");

                    mail.EmailBody = emailBody;
                    mail.EmailGonder(mail);
                }
                else
                {
                    _logger.LogWarning("Missing data for order confirmation email: Sepet={SepetCount}, KargoAdresi={KargoAdresi}, Odeme={Odeme}", sepets.Count, kargoAdresi != null, odeme != null);
                }

                // Clear session data after displaying confirmation
                HttpContext.Session.SetObjectAsJson("OdemeBilgileri", null);
                HttpContext.Session.SetObjectAsJson("SepetBilgileri", null);
                HttpContext.Session.SetObjectAsJson("Sepet", null);
                HttpContext.Session.SetObjectAsJson("UyeKargoAdresi", null);
                HttpContext.Session.SetObjectAsJson("UyeFaturaAdresi", null);
                HttpContext.Session.SetString("OdenecekTutar", "");
                HttpContext.Session.SetObjectAsJson(isGuest ? "UyeliksizUyeler" : "Uyeler", null);

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SiparisOnay Index action");
                TempData["ErrorMessage"] = "Sipariş onayı görüntülenirken bir hata oluştu.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}