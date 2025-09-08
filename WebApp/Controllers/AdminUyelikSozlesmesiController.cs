using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminUyelikSozlesmesiController : AdminBaseController
    {

      
            public IActionResult Index()
            {

            UyelikSozlesmesiRepository repository = new UyelikSozlesmesiRepository();

            List<UyelikSozlesmesi> modelListesi = repository.Listele().Where(x => x.Durumu == 1).ToList();
             
                ViewBag.UyelikSozlesmesiList = modelListesi;

                return View();
            }


        public IActionResult Guncelleme(int id)
        {
            UyelikSozlesmesiRepository repository = new UyelikSozlesmesiRepository();
            UyelikSozlesmesi hakkimizdaBilgileri = repository.Getir(id);

            if (hakkimizdaBilgileri != null)
            {
                HttpContext.Session.SetObjectAsJson("UyelikSozlesmesi", hakkimizdaBilgileri);
            }
            return RedirectToAction("Index", "AdminUyelikSozlesmesiGuncelle");
        }


        public IActionResult Sil(int id)
        {
            UyelikSozlesmesiRepository repository = new UyelikSozlesmesiRepository();

            UyelikSozlesmesi hakkimizdaBilgileri = repository.Getir(id);

            if (hakkimizdaBilgileri != null)
            {
                hakkimizdaBilgileri.Durumu = 0;
                hakkimizdaBilgileri.GuncellenmeTarihi = DateTime.Now;
                repository.Guncelle(hakkimizdaBilgileri);
            }

            return RedirectToAction("Index", "AdminUyelikSozlesmesi");
        }

    }
}
