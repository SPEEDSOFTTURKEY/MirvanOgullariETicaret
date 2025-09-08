using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminGizlilikPolitikasiGuncelleController : AdminBaseController
    {
        public IActionResult Index()
        {
            GizlilikPolitikasi guncellemeBilgisi = HttpContext.Session.GetObjectFromJson<GizlilikPolitikasi>("GizlilikPolitikasi");
            ViewBag.Hakkimizda = guncellemeBilgisi;
            return View();
        }
        public IActionResult Kaydet(GizlilikPolitikasi hakkimizdaBilgileri)
        {

            GizlilikPolitikasiRepository repository = new GizlilikPolitikasiRepository();
            GizlilikPolitikasi existingEntity = repository.Getir(hakkimizdaBilgileri.Id);
            if (existingEntity != null)
            {
                existingEntity.Metin = hakkimizdaBilgileri.Metin;
                existingEntity.Baslik = hakkimizdaBilgileri.Baslik;
                existingEntity.AltBaslik = hakkimizdaBilgileri.AltBaslik;
                existingEntity.GuncellenmeTarihi = DateTime.Now;
                repository.Guncelle(existingEntity);
            }
            return RedirectToAction("Index", "AdminGizlilikPolitikasi");

        }
    }
    }
