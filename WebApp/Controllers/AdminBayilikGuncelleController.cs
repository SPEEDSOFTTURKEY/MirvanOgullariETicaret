using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminBayilikGuncelleController : AdminBaseController
    {
        public IActionResult Index()
        {
            Bayilik guncellemeBilgisi = HttpContext.Session.GetObjectFromJson<Bayilik>("Bayilik");
            ViewBag.Hakkimizda = guncellemeBilgisi;
            return View();
        }
        public IActionResult Kaydet(Bayilik hakkimizdaBilgileri)
        {

            BayilikRepository repository = new BayilikRepository();
            Bayilik existingEntity = repository.Getir(hakkimizdaBilgileri.Id);
            if (existingEntity != null)
            {
                existingEntity.Metin = hakkimizdaBilgileri.Metin;
                existingEntity.Baslik = hakkimizdaBilgileri.Baslik;
                existingEntity.AltBaslik = hakkimizdaBilgileri.AltBaslik;
                existingEntity.GuncellenmeTarihi = DateTime.Now;
                repository.Guncelle(existingEntity);
            }
            return RedirectToAction("Index", "AdminBayilik");

        }
    }
}
