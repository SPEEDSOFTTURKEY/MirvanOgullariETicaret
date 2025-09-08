using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminIadeController : AdminBaseController
    {

        public IActionResult Index()
        {

            IadeRepository repository = new IadeRepository();

            Iade modelListesi = repository.Getir(x => x.Durumu == 1);

            ViewBag.HakkimizdaBilgileriList = modelListesi;

            return View();
        }


        public IActionResult Guncelleme(int id)
        {
            IadeRepository repository = new IadeRepository();
            Iade hakkimizdaBilgileri = repository.Getir(id);

            if (hakkimizdaBilgileri != null)
            {
                HttpContext.Session.SetObjectAsJson("Iade", hakkimizdaBilgileri);
            }
            return RedirectToAction("Index", "AdminIadeGuncelle");
        }


        public IActionResult Sil(int id)
        {
            IadeRepository repository = new IadeRepository();

            Iade hakkimizdaBilgileri = repository.Getir(id);

            if (hakkimizdaBilgileri != null)
            {
                hakkimizdaBilgileri.Durumu = 0;
                hakkimizdaBilgileri.GuncellenmeTarihi = DateTime.Now;
                repository.Guncelle(hakkimizdaBilgileri);
            }

            return RedirectToAction("Index", "AdminIade");
        }
    }
}
