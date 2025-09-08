using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminIletisimBilgileriController : AdminBaseController
    {
        public IActionResult Index()
        {

            IletisimBilgileriRepository repository = new IletisimBilgileriRepository();
            IletisimBilgileri modelListesi = repository.Getir(x => x.Durumu == 1);
            ViewBag.IletisimBilgileriList = modelListesi;
            return View();
        }


        public IActionResult Guncelleme(int id)
        {
            IletisimBilgileriRepository repository = new IletisimBilgileriRepository();
            IletisimBilgileri iletisimBilgileri = repository.Getir(id);
            if (iletisimBilgileri != null)
            {
                HttpContext.Session.SetObjectAsJson("IletisimBilgileri", iletisimBilgileri);
            }
            return RedirectToAction("Index", "AdminIletisimBilgileriGuncelle");
        }


        public IActionResult Sil(int id)
        {
            IletisimBilgileriRepository repository = new IletisimBilgileriRepository();
            IletisimBilgileri iletisimBilgileri = repository.Getir(id);
            if (iletisimBilgileri != null)
            {
                iletisimBilgileri.Durumu = 0;
                iletisimBilgileri.GuncellenmeTarihi = DateTime.Now;
                repository.Guncelle(iletisimBilgileri);
            }
            return RedirectToAction("Index", "AdminIletisimBilgileri");
        }
    }
}
