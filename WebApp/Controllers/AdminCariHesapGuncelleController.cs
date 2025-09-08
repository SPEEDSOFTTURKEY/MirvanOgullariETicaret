using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminCariHesapGuncelleController : Controller
    {
        private readonly CariHesapRepository cariHesapRepository = new CariHesapRepository();
        private readonly DepoRepository depoRepository = new DepoRepository();
        private readonly UrunRepository urunRepository = new UrunRepository();
        private readonly UrunAnaKategoriRepository urunAnaKategoriRepository = new UrunAnaKategoriRepository();
        private readonly UrunAltKategoriRepository urunAltKategoriRepository = new UrunAltKategoriRepository();

        public IActionResult Index(int id)
        {
            List<string> join = new List<string>();
            join.Add("UrunAltKategori");
            join.Add("UrunAnaKategori");
            join.Add("Urun");
            var cariHesap = cariHesapRepository.Getir(x => x.Id == id && x.Durumu == 1,join);
            if (cariHesap == null)
                return NotFound();
            ViewBag.Cari = cariHesap;
            // Ensure Fiyat and ToplamFiyat are not null for the view
            cariHesap.Fiyat ??= 0;
            cariHesap.ToplamFiyat ??= 0;

            ViewBag.UrunAnaKategoriListesi = urunAnaKategoriRepository.GetirList(x => x.Durumu == 1);
            ViewBag.UrunAltKategoriListesi = urunAltKategoriRepository.GetirList(x => x.UrunAnaKategoriId == cariHesap.UrunAnaKategoriId && x.Durumu == 1);
            ViewBag.UrunListesi = urunRepository.GetirList(x => x.UrunAltKategoriId == cariHesap.UrunAltKategoriId && x.Durumu == 1);

            return View(cariHesap);
        }

        [HttpPost]
        public IActionResult Kaydet(CariHesap model)
        {
            var cariHesap = cariHesapRepository.Getir(x => x.Id == model.Id && x.Durumu == 1);
            if (cariHesap == null)
                return NotFound();

            var urun = urunRepository.Getir(x => x.Id == model.UrunId && x.Durumu == 1);
            if (urun == null)
            {
                ModelState.AddModelError("", "Seçilen ürün bulunamadı.");
                ViewBag.UrunAnaKategoriListesi = urunAnaKategoriRepository.GetirList(x => x.Durumu == 1);
                ViewBag.UrunAltKategoriListesi = urunAltKategoriRepository.GetirList(x => x.UrunAnaKategoriId == model.UrunAnaKategoriId && x.Durumu == 1);
                ViewBag.UrunListesi = urunRepository.GetirList(x => x.UrunAltKategoriId == model.UrunAltKategoriId && x.Durumu == 1);
                return View("Index", model);
            }

            var depo = depoRepository.Getir(x => x.UrunId == model.UrunId && x.Durumu == 1);
            if (depo == null || depo.Stok + cariHesap.Adet < model.Adet)
            {
                ModelState.AddModelError("", "Yeterli stok bulunmamaktadır.");
                ViewBag.UrunAnaKategoriListesi = urunAnaKategoriRepository.GetirList(x => x.Durumu == 1);
                ViewBag.UrunAltKategoriListesi = urunAltKategoriRepository.GetirList(x => x.UrunAnaKategoriId == model.UrunAnaKategoriId && x.Durumu == 1);
                ViewBag.UrunListesi = urunRepository.GetirList(x => x.UrunAltKategoriId == model.UrunAltKategoriId && x.Durumu == 1);
                return View("Index", model);
            }

            // Restore old Adet to stock
            var oldDepo = depoRepository.Getir(x => x.UrunId == cariHesap.UrunId && x.Durumu == 1);
            if (oldDepo != null)
            {
                oldDepo.Stok += cariHesap.Adet;
                oldDepo.GuncellenmeTarihi = DateTime.Now;
                depoRepository.Guncelle(oldDepo);
            }

            // Deduct new Adet from stock
            depo.Stok -= model.Adet;
            depo.GuncellenmeTarihi = DateTime.Now;

            cariHesap.UrunAnaKategoriId = model.UrunAnaKategoriId;
            cariHesap.UrunAltKategoriId = model.UrunAltKategoriId;
            cariHesap.UrunId = model.UrunId;
            cariHesap.Adet = model.Adet;
            cariHesap.Fiyat = urun.Fiyat ?? 0;
            cariHesap.ToplamFiyat = (urun.Fiyat ?? 0) * model.Adet;
            cariHesap.GuncellenmeTarihi = DateTime.Now;

            cariHesapRepository.Guncelle(cariHesap);
            depoRepository.Guncelle(depo);

            return RedirectToAction("Index", "AdminCariHesap");
        }
    }
}