using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminDepoController : AdminBaseController
    {
        UrunAnaKategoriRepository urunAnaKategoriRepository = new UrunAnaKategoriRepository();
        UrunAltKategoriRepository urunAltKategoriRepository = new UrunAltKategoriRepository();
        UrunRepository urunRepository = new UrunRepository();
        DepoRepository depoRepository = new DepoRepository();
        public IActionResult Index()
        {
            List<string> join = new List<string>();
            join.Add("UrunAltKategori");
            join.Add("UrunAnaKategori");
            join.Add("Urun");
            var depos = depoRepository.GetirList(x => x.Durumu == 1,join);
            ViewBag.DepoListesi = depos;
            return View();
        }
            
        [HttpPost]
        public IActionResult GetSubCategories(int urunAnaKategoriId)
        {
            var subCategories = urunAltKategoriRepository.GetirList(x => x.UrunAnaKategoriId == urunAnaKategoriId && x.Durumu == 1);
            return Json(subCategories.Select(x => new { Id = x.Id, Adi = x.Adi }));
        }

        [HttpPost]
        public IActionResult GetProducts(int urunAltKategoriId)
        {
            var products = urunRepository.GetirList(x => x.UrunAltKategoriId == urunAltKategoriId && x.Durumu == 1);
            return Json(products.Select(x => new { Id = x.Id, Adi = x.Adi }));
        }

        public IActionResult Sil(int id)
        {
            var depo = depoRepository.Getir(x => x.Id == id);
            if (depo == null)
                return NotFound();

            depo.Durumu = 0;
            depo.GuncellenmeTarihi = DateTime.Now;
            depoRepository.Guncelle(depo);
            return RedirectToAction("Index");
        }
    }
}