using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminAcilKargoUcretController : Controller
    {
        public IActionResult Index()
        {
            AcilKargoUcretRepository repository = new AcilKargoUcretRepository();
            List<AcilKargoUcret> modelListesi = repository.GetirList(x => x.Durumu == 1).ToList();
            ViewBag.KargoUcretListesi = modelListesi;
            return View();
        }
        public IActionResult Guncelleme(int id)
        {
            AcilKargoUcretRepository repository = new AcilKargoUcretRepository();
            AcilKargoUcret kargoUcret = repository.Getir(id);
            if (kargoUcret != null)
            {
                HttpContext.Session.SetObjectAsJson("AcilKargoUcret", kargoUcret);
            }
            return RedirectToAction("Index", "AdminAcilKargoUcretGuncelle");
        }

        public IActionResult Sil(int id)
        {
            AcilKargoUcretRepository repository = new AcilKargoUcretRepository();
            AcilKargoUcret kargoUcret = repository.Getir(id);
            if (kargoUcret != null)
            {
                kargoUcret.Durumu = 0;
                kargoUcret.GuncellenmeTarihi = DateTime.Now;
                repository.Guncelle(kargoUcret);
            }
            return RedirectToAction("Index", "AdminAcilKargoUcret");
        }
    }
}
