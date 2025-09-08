using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminBayilikController : AdminBaseController
    {
        public IActionResult Index()
        {

            BayilikRepository repository = new BayilikRepository();

            Bayilik modelListesi = repository.Getir(x => x.Durumu == 1);

            ViewBag.HakkimizdaBilgileriList = modelListesi;

            return View();
        }


        public IActionResult Guncelleme(int id)
        {
            BayilikRepository repository = new BayilikRepository();
            Bayilik hakkimizdaBilgileri = repository.Getir(id);

            if (hakkimizdaBilgileri != null)
            {
                HttpContext.Session.SetObjectAsJson("Bayilik", hakkimizdaBilgileri);
            }
            return RedirectToAction("Index", "AdminBayilikGuncelle");
        }


        public IActionResult Sil(int id)
        {
            BayilikRepository repository = new BayilikRepository();

            Bayilik hakkimizdaBilgileri = repository.Getir(id);

            if (hakkimizdaBilgileri != null)
            {
                hakkimizdaBilgileri.Durumu = 0;
                hakkimizdaBilgileri.GuncellenmeTarihi = DateTime.Now;
                repository.Guncelle(hakkimizdaBilgileri);
            }

            return RedirectToAction("Index", "AdminBayilik");
        }
    }
}
