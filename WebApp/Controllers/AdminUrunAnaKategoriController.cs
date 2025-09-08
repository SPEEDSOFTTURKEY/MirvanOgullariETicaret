using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminUrunAnaKategoriController : AdminBaseController
    {
        public IActionResult Index()
        {
            UrunAnaKategoriRepository repository = new UrunAnaKategoriRepository();
            List<UrunAnaKategori> modelListesi = repository.GetirList(x => x.Durumu == 1).ToList();
            ViewBag.UrunlerList = modelListesi;
            return View();
        }
        public IActionResult Guncelleme(int id)
        {
            UrunAnaKategoriRepository repository = new UrunAnaKategoriRepository();
            UrunAnaKategori urun = repository.Getir(id);
            if (urun != null)
            {
                HttpContext.Session.SetObjectAsJson("UrunAnaKategori", urun);
            }
            return RedirectToAction("Index", "AdminUrunAnaKategoriGuncelle");
        }


        public IActionResult Sil(int id)
        {
            UrunAnaKategoriRepository repository = new UrunAnaKategoriRepository();
            UrunAnaKategori urun = repository.Getir(id);
            if (urun != null)
            {
                urun.Durumu = 0;
                urun.GuncellenmeTarihi = DateTime.Now;
                repository.Guncelle(urun);
            }
            return RedirectToAction("Index", "AdminUrunAnaKategori");
        }
    }
}
