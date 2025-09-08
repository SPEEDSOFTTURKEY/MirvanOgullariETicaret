using Microsoft.AspNetCore.Mvc;
using System.Text;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class SiparisOnayKartController : BaseController
    {
        private readonly ILogger<SiparisOnayKartController> _logger;

        public SiparisOnayKartController(ILogger<SiparisOnayKartController> logger)
        {
            _logger = logger;
        }


        [HttpPost]
        public async Task<IActionResult>  Index(SiparisVeSepet siparisVeSepet)
        {
            await LoadCommonData();

            try
            {

                // Get cart and addresses
                var sepets = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();
                var kargoAdresi = HttpContext.Session.GetObjectFromJson<UyeAdres>("UyeKargoAdresi");
                var faturaAdresi = HttpContext.Session.GetObjectFromJson<UyeAdres>("UyeFaturaAdresi");
                ViewBag.SepetSayi = sepets.Count;

                if (Convert.ToInt32(siparisVeSepet.indirimId) > 0 && Convert.ToInt32(siparisVeSepet.uyeId) > 0)
                {
                    var indirimUye = new IndirimUye
                    {
                        Durumu = 1,
                        EklenmeTarihi = DateTime.Now,
                        UyeId = Convert.ToInt32(siparisVeSepet.uyeId),
                        IndirimId = Convert.ToInt32(siparisVeSepet.indirimId),
                        GuncellenmeTarihi = DateTime.Now
                    };
                    var indirimUyeRepository = new IndirimUyeRepository();
                    indirimUyeRepository.Ekle(indirimUye);
                }


                // Send email
                var 
                    mail = new EMail
                {
                    EmailFrom = "2kids2momsiparis@gmail.com",
                    EmailTo = "2kids2momsiparis@gmail.com",
                    EmailHeader = "2KİDS2MOM SİPARİŞ BİLGİLENDİRME",
                    EmailSubject = "2KİDS2MOM SİPARİŞ BİLGİLENDİRME",
                    Html = true,
                    Port = 587,
                    Host = "smtp.gmail.com",
                    Password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? "uraenfbwynetexwe"
                };

                var emailBody = new StringBuilder();
                emailBody.Append("<p style='font-weight:bold'>2KİDS2MOM SİPARİŞ BİLGİLENDİRME</p>");
                emailBody.Append("<p>Sipariş Ödeme Tipi: Kart Ödeme</p>");
                emailBody.Append("<p>Sipariş Tarihi: " + DateTime.Now + "</p>");
                emailBody.Append("<hr>");

                if (sepets.Any())
                {
                    foreach (var sepet in sepets)
                    {
                        emailBody.Append($"<p><strong>Müşteri Adı Soyadı:</strong> {sepet.Uye?.Adi} {sepet.Uye?.Soyadi}</p>");
                        emailBody.Append($"<p><strong>Müşteri Telefon Numarası:</strong> {sepet.Uye?.Telefon}</p>");
                        emailBody.Append($"<p><strong>Ürün Adı:</strong> {sepet.UrunAdi}</p>");
                        emailBody.Append($"<p><strong>Fiyat:</strong> {sepet.Fiyat} TL</p>");
                        emailBody.Append($"<p><strong>Beden:</strong> {sepet.Birimler?.Adi}</p>");
                        emailBody.Append($"<p><strong>Adet:</strong> {sepet.Miktar}</p>");
                        emailBody.Append($"<p><strong>Toplam Fiyat:</strong> {sepet.Toplam}</p>");
                        emailBody.Append("<p><strong>Kargo Adresi Bilgileri:</strong><br>" +
                            $"<strong>Alıcı Adı:</strong> {kargoAdresi?.Adi} {kargoAdresi?.Soyadi}<br>" +
                            $"<strong>Telefon Numarası:</strong> {kargoAdresi?.Telefon}<br>" +
                            $"<strong>Mahalle:</strong> {kargoAdresi?.Mahalle}<br>" +
                            $"<strong>İlçe:</strong> {kargoAdresi?.ilce}<br>" +
                            $"<strong>İl:</strong> {kargoAdresi?.il}<br>" +
                            $"<strong>Adres:</strong> {kargoAdresi?.Adres}</p>");
                        emailBody.Append("<p><strong>Fatura Adresi Bilgileri:</strong><br>" +
                            $"<strong>Alıcı Adı:</strong> {faturaAdresi?.Adi} {faturaAdresi?.Soyadi}<br>" +
                            $"<strong>Telefon Numarası:</strong> {faturaAdresi?.Telefon}<br>" +
                            $"<strong>Mahalle:</strong> {faturaAdresi?.Mahalle}<br>" +
                            $"<strong>İlçe:</strong> {faturaAdresi?.ilce}<br>" +
                            $"<strong>İl:</strong> {faturaAdresi?.il}<br>" +
                            $"<strong>Adres:</strong> {faturaAdresi?.Adres}</p>");
                        emailBody.Append("<hr>");
                    }
                }
                else
                {
                    emailBody.Append("<p>Sepetinizde ürün bulunmamaktadır.</p>");
                }

                mail.EmailBody = emailBody;
                mail.EmailGonder(mail);

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Index action");
                TempData["ErrorMessage"] = "Sipariş onaylama sırasında bir hata oluştu.";
                return RedirectToAction("Index", "SiparisOnaylanmadi");
            }
        }
    }
}