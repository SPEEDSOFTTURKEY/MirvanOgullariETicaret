using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminIndirimKoduGuncelleController : AdminBaseController
    {
        public IActionResult Index()
        {
            IndirimKodu guncellemeBilgisi = HttpContext.Session.GetObjectFromJson<IndirimKodu>("IndirimKodu");
            ViewBag.IndirimKodu = guncellemeBilgisi;
            return View();
          
        }

        public IActionResult Kaydet(IndirimKodu ındirimKodu)
        {

            IndirimKoduRepository ındirimKoduRepository = new IndirimKoduRepository(); IndirimKodu existingEntity = ındirimKoduRepository.Getir(ındirimKodu.Id);
            if (existingEntity != null)
            {
                existingEntity.Kodu = ındirimKodu.Kodu;
                existingEntity.Orani = ındirimKodu.Orani;
                existingEntity.AltLimit = ındirimKodu.AltLimit;
                existingEntity.BitisTarihi = ındirimKodu.BitisTarihi;
                existingEntity.BaslangicTarihi = ındirimKodu.BaslangicTarihi;
                existingEntity.GuncellenmeTarihi = DateTime.Now;
                ındirimKoduRepository.Guncelle(existingEntity);

            }
            return RedirectToAction("Index", "AdminIndırımKodu");


        }
    }
}
