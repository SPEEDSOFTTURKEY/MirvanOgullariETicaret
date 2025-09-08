using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminHakkimizdaBilgileriGuncelleController : AdminBaseController
    {
        public IActionResult Index()
        {
            HakkimizdaBilgileri guncellemeBilgisi = HttpContext.Session.GetObjectFromJson<HakkimizdaBilgileri>("HakkimizdaBilgileri");
            ViewBag.Hakkimizda = guncellemeBilgisi;  
            return View();
        }
        public IActionResult Kaydet(HakkimizdaBilgileri hakkimizdaBilgileri)
        {
           
                HakkimizdaBilgileriRepository repository = new HakkimizdaBilgileriRepository();
                HakkimizdaBilgileri existingEntity = repository.Getir(hakkimizdaBilgileri.Id);
                if (existingEntity != null)
                {
                    existingEntity.Metin = hakkimizdaBilgileri.Metin;
                    existingEntity.Baslik = hakkimizdaBilgileri.Baslik;
                    existingEntity.AltBaslik = hakkimizdaBilgileri.AltBaslik;
                    existingEntity.GuncellenmeTarihi = DateTime.Now;
                    repository.Guncelle(existingEntity);
                }
                return RedirectToAction("Index", "AdminHakkimizdaBilgileri");
            
        }
    }
   }

