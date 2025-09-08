using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminCariHesapController : Controller
    {
      
        CariHesapRepository cariHesapRepository = new CariHesapRepository();
        DepoRepository depoRepository = new DepoRepository();
        UrunRepository urunRepository = new UrunRepository();
        UrunAnaKategoriRepository urunAnaKategoriRepository = new UrunAnaKategoriRepository();
        UrunAltKategoriRepository urunAltKategoriRepository = new UrunAltKategoriRepository();
        public IActionResult Index()
        {
            List<string> join = new List<string>();
            join.Add("UrunAltKategori");
            join.Add("UrunAnaKategori");
            join.Add("Urun");
            var cariHesaplar = cariHesapRepository.GetirList(x => x.Durumu == 1,join);
            ViewBag.CariHesapListesi = cariHesaplar;
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
            return Json(products.Select(x => new { Id = x.Id, Adi = x.Adi, Fiyat = x.Fiyat }));
        }

        public IActionResult Sil(int id)
        {
            var cariHesap = cariHesapRepository.Getir(x => x.Id == id && x.Durumu == 1);
            if (cariHesap == null)
                return NotFound();

            var depo = depoRepository.Getir(x => x.UrunId == cariHesap.UrunId && x.Durumu == 1);
            if (depo != null)
            {
                depo.Stok += cariHesap.Adet;
                depo.GuncellenmeTarihi = DateTime.Now;
                depoRepository.Guncelle(depo);
            }

            cariHesap.Durumu = 0;
            cariHesap.GuncellenmeTarihi = DateTime.Now;
            cariHesapRepository.Guncelle(cariHesap);

            return RedirectToAction("Index");
        }
    }
}