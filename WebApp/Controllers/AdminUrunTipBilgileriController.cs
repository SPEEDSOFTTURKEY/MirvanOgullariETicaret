using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminUrunTipBilgileriController : AdminBaseController
    {
        public IActionResult Index()
        {
            UrunAltKategoriRepository repository = new UrunAltKategoriRepository();
            List<string> join=new List<string>();
            join.Add("UrunAnaKategori");
            List<UrunAltKategori> modelListesi = repository.GetirList(x => x.Durumu == 1,join).ToList();
            
            ViewBag.UrunlerList = modelListesi;
            return View();
        }
        public IActionResult Guncelleme(int id)
        {
            UrunAltKategoriRepository repository = new UrunAltKategoriRepository();
            UrunAltKategori urun = repository.Getir(id);
            if (urun != null)
            {
                HttpContext.Session.SetObjectAsJson("UrunAltKategori", urun);
            }
            return RedirectToAction("Index", "AdminUrunTipBilgileriGuncelle");
        }


        public IActionResult Sil(int id)
        {
            UrunAltKategoriRepository repository = new UrunAltKategoriRepository();
            UrunAltKategori urun = repository.Getir(id);
            if (urun != null)
            {
                urun.Durumu = 0;
                urun.GuncellenmeTarihi = DateTime.Now;
                repository.Guncelle(urun);
            }
            return RedirectToAction("Index", "AdminUrunTipBilgileri");
        }
    }
}
