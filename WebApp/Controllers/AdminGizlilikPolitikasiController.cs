using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminGizlilikPolitikasiController : AdminBaseController
    {
        public IActionResult Index()
        {

           GizlilikPolitikasiRepository repository = new GizlilikPolitikasiRepository();

            GizlilikPolitikasi modelListesi = repository.Getir(x => x.Durumu == 1);

            ViewBag.HakkimizdaBilgileriList = modelListesi;

            return View();
        }


        public IActionResult Guncelleme(int id)
        {
            GizlilikPolitikasiRepository repository = new GizlilikPolitikasiRepository();
            GizlilikPolitikasi hakkimizdaBilgileri = repository.Getir(id);

            if (hakkimizdaBilgileri != null)
            {
                HttpContext.Session.SetObjectAsJson("GizlilikPolitikasi", hakkimizdaBilgileri);
            }
            return RedirectToAction("Index", "AdminGizlilikPolitikasiGuncelle");
        }
        public IActionResult Sil(int id)
        {
            GizlilikPolitikasiRepository repository = new GizlilikPolitikasiRepository();

            GizlilikPolitikasi hakkimizdaBilgileri = repository.Getir(id);

            if (hakkimizdaBilgileri != null)
            {
                hakkimizdaBilgileri.Durumu = 0;
                hakkimizdaBilgileri.GuncellenmeTarihi = DateTime.Now;
                repository.Guncelle(hakkimizdaBilgileri);
            }

            return RedirectToAction("Index", "AdminGizlilikPolitikasi");
        }
    }
}
