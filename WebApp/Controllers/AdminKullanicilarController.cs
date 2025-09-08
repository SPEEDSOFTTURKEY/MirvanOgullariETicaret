using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminKullanicilarController : AdminBaseController
    {
        public IActionResult Index()
        {

            KullanicilarRepository repository = new KullanicilarRepository();

            List<Kullanicilar> modelListesi = repository.Listele().Where(x => x.Durumu == 1).ToList();

            ViewBag.KullanicilarList = modelListesi;

            return View();
        }


        public IActionResult Guncelleme(int id)
        {
            KullanicilarRepository repository = new KullanicilarRepository();
            Kullanicilar kullanicilar = repository.Getir(id);

            if (kullanicilar != null)
            {
                HttpContext.Session.SetObjectAsJson("Kullanici", kullanicilar);
            }
            return RedirectToAction("Index", "AdminKullanicilarGuncelle");
        }


        public IActionResult Sil(int id)
        {
            KullanicilarRepository repository = new KullanicilarRepository();

            Kullanicilar kullanicilar = repository.Getir(id);

            if (kullanicilar != null)
            {
                kullanicilar.Durumu = 0;
                kullanicilar.GuncellenmeTarihi = DateTime.Now;
                repository.Guncelle(kullanicilar);
            }

            return RedirectToAction("Index", "AdminKullanicilar");
        }
    }
}
