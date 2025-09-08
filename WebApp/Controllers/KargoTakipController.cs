using Microsoft.AspNetCore.Mvc;
using SuratServiceReference;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class KargoTakipController : BaseController
    {
        public async Task<IActionResult> Index()
        {
            await LoadCommonData();
			return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(string kargoTakipNo)
        {
            await LoadCommonData();

            if (string.IsNullOrEmpty(kargoTakipNo))
            {
                ViewBag.ErrorMessage = "Kargo takip numarası boş olamaz.";
                return View("Index");
            }
            try
            {
                Uyeler uyeler = new Uyeler();
                uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");

                // Servis istemcisi oluşturuluyor
                var client = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap);

                Gonderi gonderi = new Gonderi
                {
                    OzelKargoTakipNo = kargoTakipNo,
                    Email = uyeler.EMail
                };


                // Servisten bilgi alınıyor
                var kargoBilgileri = await client.GonderiyiKargoyaGonderAsync(uyeler.EMail, uyeler.Sifre, gonderi);

                if (!string.IsNullOrEmpty(kargoBilgileri))
                {
                    ViewBag.KargoSonuc = kargoBilgileri;
                    // return View("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Kargo bilgisi bulunamadı.";
                    //return View("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Bir hata oluştu: " + ex.Message;
                return View("Index");
            }
            return RedirectToAction("Index", "KargoTakip");
        }
    }
}
