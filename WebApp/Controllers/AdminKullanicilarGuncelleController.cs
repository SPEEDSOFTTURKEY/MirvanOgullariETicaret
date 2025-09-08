using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminKullanicilarGuncelleController : AdminBaseController
    {
        public IActionResult Index()
        {
            Kullanicilar guncellemeBilgisi = HttpContext.Session.GetObjectFromJson<Kullanicilar>("Kullanici");
            ViewBag.Kullanicilar = guncellemeBilgisi;
            return View();
        }
        public IActionResult Kaydet(Kullanicilar kullanicilar)
        {
           
                KullanicilarRepository repository = new KullanicilarRepository();
                Kullanicilar existingEntity = repository.Getir(kullanicilar.Id);
                if (existingEntity != null)
                {
                    existingEntity.Adi = kullanicilar.Adi;
                    existingEntity.Sifre = kullanicilar.Sifre;
                existingEntity.LisansAnahtari = kullanicilar.LisansAnahtari;
                    existingEntity.GuncellenmeTarihi = DateTime.Now;
                    repository.Guncelle(existingEntity);
                }
                return RedirectToAction("Index", "AdminKullanicilar");
        
        }
    }
}
