using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminKullanimKosullariGuncelleController : AdminBaseController
    {
        public IActionResult Index()
        {
            KullanimKosullari guncellemeBilgisi = HttpContext.Session.GetObjectFromJson<KullanimKosullari>("KullanimKosullari");
            ViewBag.Hakkimizda = guncellemeBilgisi;
            return View();
        }
        public IActionResult Kaydet(KullanimKosullari hakkimizdaBilgileri)
        {

            KullanimKosullariRepository repository = new KullanimKosullariRepository();
            KullanimKosullari existingEntity = repository.Getir(hakkimizdaBilgileri.Id);
            if (existingEntity != null)
            {
                existingEntity.Metin = hakkimizdaBilgileri.Metin;
                existingEntity.Baslik = hakkimizdaBilgileri.Baslik;
                existingEntity.Baslik = hakkimizdaBilgileri.Baslik;
                existingEntity.AltBaslik = hakkimizdaBilgileri.AltBaslik;
                existingEntity.GuncellenmeTarihi = DateTime.Now;
                repository.Guncelle(existingEntity);
            }
            return RedirectToAction("Index", "AdminKullanimKosullari");

        }
    }
    }
