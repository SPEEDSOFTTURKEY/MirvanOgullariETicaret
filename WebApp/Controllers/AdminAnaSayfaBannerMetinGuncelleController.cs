using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminAnaSayfaBannerMetinGuncelleController : AdminBaseController
    {
        public IActionResult Index()
        {
            AnaSayfaBannerMetin guncellemeBilgisi = HttpContext.Session.GetObjectFromJson<AnaSayfaBannerMetin>("AnaSayfaBannerMetinGuncelle");
            ViewBag.AnaSayfaBannerMetin = guncellemeBilgisi;  
            return View();
        }
        public IActionResult Kaydet(AnaSayfaBannerMetin hakkimizdaBilgileri)
        {
           
                AnaSayfaBannerMetinRepository repository = new AnaSayfaBannerMetinRepository();
				AnaSayfaBannerMetin existingEntity = repository.Getir(hakkimizdaBilgileri.Id);
                if (existingEntity != null)
                {
                    existingEntity.Metin = hakkimizdaBilgileri.Metin;
          
                    existingEntity.GuncellenmeTarihi = DateTime.Now;
                    repository.Guncelle(existingEntity);
                }
                return RedirectToAction("Index", "AdminAnaSayfaBannerMetin");
        
        }
    }
   }

