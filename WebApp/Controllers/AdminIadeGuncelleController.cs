using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminIadeGuncelleController : AdminBaseController
    {
        public IActionResult Index()
        {
            Iade guncellemeBilgisi = HttpContext.Session.GetObjectFromJson<Iade>("Iade");
            ViewBag.Hakkimizda = guncellemeBilgisi;
            return View();
        }
        public IActionResult Kaydet(Iade hakkimizdaBilgileri)
        {

            IadeRepository repository = new IadeRepository();
            Iade existingEntity = repository.Getir(hakkimizdaBilgileri.Id);
            if (existingEntity != null)
            {
                existingEntity.Metin = hakkimizdaBilgileri.Metin;
                existingEntity.Baslik = hakkimizdaBilgileri.Baslik;
                existingEntity.AltBaslik = hakkimizdaBilgileri.AltBaslik;
                existingEntity.GuncellenmeTarihi = DateTime.Now;
                repository.Guncelle(existingEntity);
            }
            return RedirectToAction("Index", "AdminIade");

        }
    }
}
