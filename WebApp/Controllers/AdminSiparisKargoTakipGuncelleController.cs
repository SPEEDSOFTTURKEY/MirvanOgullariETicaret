using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminSiparisKargoTakipGuncelleController : AdminBaseController
    {
        public IActionResult Index()
        {
            SiparisKargoTakip siparisKargoTakip =new SiparisKargoTakip();
            siparisKargoTakip = HttpContext.Session.GetObjectFromJson<SiparisKargoTakip>("SiparisKargoTakip");
            ViewBag.SiparisKargo= siparisKargoTakip;
            return View();
        }
            
        public IActionResult Kaydet(SiparisKargoTakip siparisKargoTakip)
        {
            if (siparisKargoTakip != null)
            {
                SiparisKargoTakip takip = new SiparisKargoTakip();
                SiparisKargoTakipRepository siparisKargoTakipRepository = new SiparisKargoTakipRepository();
                takip = siparisKargoTakipRepository.Getir(siparisKargoTakip.Id);
                takip.GuncellenmeTarihi = DateTime.Now;
                takip.EklenmeTarihi = siparisKargoTakip.EklenmeTarihi;
                takip.Durumu = 1;
                takip.UyelerId = siparisKargoTakip.UyelerId;
                takip.SiparisId = siparisKargoTakip.SiparisId;
                takip.Aciklama = siparisKargoTakip.Aciklama;
                takip.KargoTakipNumarasi = siparisKargoTakip.KargoTakipNumarasi;
                takip.KargoFirmasi = siparisKargoTakip.KargoFirmasi;
                siparisKargoTakipRepository.Guncelle(takip);
                HttpContext.Session.Remove("SiparisId");
            }

            return RedirectToAction("Index", "AdminSiparisKargoTakip");
        }
    }
}
