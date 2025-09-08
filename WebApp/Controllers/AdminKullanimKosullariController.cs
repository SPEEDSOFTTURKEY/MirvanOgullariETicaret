using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminKullanimKosullariController : AdminBaseController
    {
        public IActionResult Index()
        {

            KullanimKosullariRepository repository = new KullanimKosullariRepository();

            KullanimKosullari modelListesi = repository.Getir(x => x.Durumu == 1);

            ViewBag.HakkimizdaBilgileriList = modelListesi;

            return View();
        }


        public IActionResult Guncelleme(int id)
        {
            KullanimKosullariRepository repository = new KullanimKosullariRepository();
            KullanimKosullari hakkimizdaBilgileri = repository.Getir(id);

            if (hakkimizdaBilgileri != null)
            {
                HttpContext.Session.SetObjectAsJson("KullanimKosullari", hakkimizdaBilgileri);
            }
            return RedirectToAction("Index", "AdminKullanimKosullariGuncelle");
        }


        public IActionResult Sil(int id)
        {
            KullanimKosullariRepository repository = new KullanimKosullariRepository();

            KullanimKosullari hakkimizdaBilgileri = repository.Getir(id);

            if (hakkimizdaBilgileri != null)
            {
                hakkimizdaBilgileri.Durumu = 0;
                hakkimizdaBilgileri.GuncellenmeTarihi = DateTime.Now;
                repository.Guncelle(hakkimizdaBilgileri);
            }

            return RedirectToAction("Index", "AdminKullanimKosullari");
        }

    }
}
