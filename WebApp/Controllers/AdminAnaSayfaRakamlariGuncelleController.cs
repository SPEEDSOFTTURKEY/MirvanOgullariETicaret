using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminAnaSayfaRakamlariGuncelleController : AdminBaseController
    {
        public IActionResult Index()
        {
            AnaSayfaRakamlari guncellemeBilgisi = HttpContext.Session.GetObjectFromJson<AnaSayfaRakamlari>("AnaSayfaRakamlari");
            ViewBag.AnaSayfaRakam = guncellemeBilgisi;
            return View();
        }
        public IActionResult Kaydet(AnaSayfaRakamlari bilgiler)
        {
             AnaSayfaRakamlariRepository repository = new AnaSayfaRakamlariRepository();
                AnaSayfaRakamlari existingEntity = repository.Getir(bilgiler.Id);
                if (existingEntity != null)
                {
                    existingEntity.Baslik = bilgiler.Baslik;
                    existingEntity.Sayi = bilgiler.Sayi;
                    existingEntity.GuncellenmeTarihi = DateTime.Now;
                    repository.Guncelle(existingEntity);
                }
                return RedirectToAction("Index", "AdminAnaSayfaRakamlari");
       
        }
    }
}
