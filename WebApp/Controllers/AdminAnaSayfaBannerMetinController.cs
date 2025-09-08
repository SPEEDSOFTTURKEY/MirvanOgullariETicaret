using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminAnaSayfaBannerMetinController : AdminBaseController
    {

      
            public IActionResult Index()
            {
               
                AnaSayfaBannerMetinRepository repository = new AnaSayfaBannerMetinRepository();

            List<AnaSayfaBannerMetin> modelListesi = repository.Listele().Where(x => x.Durumu == 1).ToList();
             
                ViewBag.AnaSayfaBannerMetin = modelListesi;

                return View();
            }


        public IActionResult Guncelleme(int id)
        {
            AnaSayfaBannerMetinRepository repository = new AnaSayfaBannerMetinRepository();
            AnaSayfaBannerMetin hakkimizdaBilgileri = repository.Getir(id);

            if (hakkimizdaBilgileri != null)
            {
                HttpContext.Session.SetObjectAsJson("AnaSayfaBannerMetinGuncelle", hakkimizdaBilgileri);
            }
            return RedirectToAction("Index", "AdminAnaSayfaBannerMetinGuncelle");
        }


        public IActionResult Sil(int id)
        {
            AnaSayfaBannerMetinRepository repository = new AnaSayfaBannerMetinRepository();

            AnaSayfaBannerMetin hakkimizdaBilgileri = repository.Getir(id);

            if (hakkimizdaBilgileri != null)
            {
                hakkimizdaBilgileri.Durumu = 0;
                hakkimizdaBilgileri.GuncellenmeTarihi = DateTime.Now;
                repository.Guncelle(hakkimizdaBilgileri);
            }

            return RedirectToAction("Index", "AdminAnaSayfaBannerMetin");
        }

    }
}
