using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminUyelikSozlesmesiGuncelleController : AdminBaseController
    {
        public IActionResult Index()
        {
            UyelikSozlesmesi guncellemeBilgisi = HttpContext.Session.GetObjectFromJson<UyelikSozlesmesi>("UyelikSozlesmesi");
            ViewBag.UyelikSozlesmesi = guncellemeBilgisi;  
            return View();
        }
        public IActionResult Kaydet(UyelikSozlesmesi hakkimizdaBilgileri)
        {
          
                UyelikSozlesmesiRepository repository = new UyelikSozlesmesiRepository();
                UyelikSozlesmesi existingEntity = repository.Getir(hakkimizdaBilgileri.Id);
                if (existingEntity != null)
                {
                    existingEntity.Metin = hakkimizdaBilgileri.Metin;
                    existingEntity.Baslik = hakkimizdaBilgileri.Baslik;
                    existingEntity.AltBaslik = hakkimizdaBilgileri.AltBaslik;
                    existingEntity.GuncellenmeTarihi = DateTime.Now;
                    repository.Guncelle(existingEntity);
                }
                return RedirectToAction("Index", "AdminUyelikSozlesmesi");
    
        }
    }
   }

