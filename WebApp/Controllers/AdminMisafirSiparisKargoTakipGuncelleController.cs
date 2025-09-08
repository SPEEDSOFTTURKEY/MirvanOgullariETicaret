using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminMisafirSiparisKargoTakipGuncelleController : AdminBaseController
    {
        public IActionResult Index()
        {
            MisafirSiparisKargoTakip siparisKargoTakip =new MisafirSiparisKargoTakip();
            siparisKargoTakip = HttpContext.Session.GetObjectFromJson<MisafirSiparisKargoTakip>("MisafirSiparisKargoTakip");
            ViewBag.SiparisKargo= siparisKargoTakip;
            return View();
        }
            
        public IActionResult Kaydet(MisafirSiparisKargoTakip siparisKargoTakip)
        {
            if (siparisKargoTakip != null)
            {
                MisafirSiparisKargoTakip takip = new MisafirSiparisKargoTakip();
                MisafirSiparisKargoTakipRepository siparisKargoTakipRepository = new MisafirSiparisKargoTakipRepository();
                takip = siparisKargoTakipRepository.Getir(siparisKargoTakip.Id);
                takip.GuncellenmeTarihi = DateTime.Now;
                takip.EklenmeTarihi = siparisKargoTakip.EklenmeTarihi;
                takip.Durumu = 1;
                takip.SiparisGuestId = siparisKargoTakip.SiparisGuestId;
                takip.Aciklama = siparisKargoTakip.Aciklama;
                takip.KargoTakipNumarasi = siparisKargoTakip.KargoTakipNumarasi;
                takip.KargoFirmasi = siparisKargoTakip.KargoFirmasi;
                siparisKargoTakipRepository.Guncelle(takip);
                HttpContext.Session.Remove("SiparisId");
            }

            return RedirectToAction("Index", "AdminMisafirSiparisKargoTakip");
        }
    }
}
