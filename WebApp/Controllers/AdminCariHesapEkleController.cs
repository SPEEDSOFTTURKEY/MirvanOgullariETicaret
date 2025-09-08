using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminCariHesapEkleController : Controller
    {
        private readonly CariHesapRepository cariHesapRepository = new CariHesapRepository();
        private readonly DepoRepository depoRepository = new DepoRepository();
        private readonly UrunRepository urunRepository = new UrunRepository();
        private readonly UrunAnaKategoriRepository urunAnaKategoriRepository = new UrunAnaKategoriRepository();
        private readonly UrunAltKategoriRepository urunAltKategoriRepository = new UrunAltKategoriRepository();

        public IActionResult Index()
        {
            ViewBag.UrunAnaKategoriListesi = urunAnaKategoriRepository.GetirList(x => x.Durumu == 1);
            // Pass an empty model to initialize form fields
            return View(new CariHesap { Adet = 1, Fiyat = 0, ToplamFiyat = 0 });
        }

        [HttpPost]
        public IActionResult Kaydet(CariHesap model)
        {
            var urun = urunRepository.Getir(x => x.Id == model.UrunId && x.Durumu == 1);
            if (urun == null)
            {
                ModelState.AddModelError("", "Seçilen ürün bulunamadı.");
                ViewBag.UrunAnaKategoriListesi = urunAnaKategoriRepository.GetirList(x => x.Durumu == 1);
                model.Fiyat ??= 0;
                model.ToplamFiyat ??= 0;
                return View("Index", model);
            }

            var depo = depoRepository.Getir(x => x.UrunId == model.UrunId && x.Durumu == 1);
            if (depo == null || depo.Stok < model.Adet)
            {
                ModelState.AddModelError("", "Yeterli stok bulunmamaktadır.");
                ViewBag.UrunAnaKategoriListesi = urunAnaKategoriRepository.GetirList(x => x.Durumu == 1);
                model.Fiyat ??= 0;
                model.ToplamFiyat ??= 0;
                return View("Index", model);
            }

            var cariHesap = new CariHesap
            {
                UrunAnaKategoriId = model.UrunAnaKategoriId,
                UrunAltKategoriId = model.UrunAltKategoriId,
                UrunId = model.UrunId,
                Adet = model.Adet,
                Fiyat = urun.Fiyat ?? 0,
                ToplamFiyat = (urun.Fiyat ?? 0) * model.Adet,
                Durumu = 1,
                EklenmeTarihi = DateTime.Now,
                GuncellenmeTarihi = DateTime.Now
            };

            depo.Stok -= model.Adet;
            depo.GuncellenmeTarihi = DateTime.Now;

            cariHesapRepository.Ekle(cariHesap);
            depoRepository.Guncelle(depo);

            return RedirectToAction("Index", "AdminCariHesap");
        }
    }
}