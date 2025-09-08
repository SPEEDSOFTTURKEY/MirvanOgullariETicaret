using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminDepoEkleController : AdminBaseController
    {
        UrunAnaKategoriRepository urunAnaKategoriRepository = new UrunAnaKategoriRepository();
        DepoRepository depoRepository = new DepoRepository();

        public IActionResult Index()
        {
            ViewBag.UrunAnaKategoriListesi = urunAnaKategoriRepository.GetirList(x => x.Durumu == 1);
            return View();
        }

        [HttpPost]
        public IActionResult Kaydet(Depo model)
        {
            // Aynı UrunId'ye sahip aktif bir depo kaydı var mı kontrol et
            var mevcutDepo = depoRepository.Getir(x =>
                x.UrunId == model.UrunId &&
                x.UrunAnaKategoriId == model.UrunAnaKategoriId &&
                x.UrunAltKategoriId == model.UrunAltKategoriId &&
                x.Durumu == 1
            );

            if (mevcutDepo != null)
            {
                // Zaten varsa, sadece stoğu artır
                mevcutDepo.Stok += model.Stok;
                mevcutDepo.GuncellenmeTarihi = DateTime.Now;

                depoRepository.Guncelle(mevcutDepo);
            }
            else
            {
                // Yoksa yeni kayıt olarak ekle
                var yeniDepo = new Depo
                {
                    UrunAnaKategoriId = model.UrunAnaKategoriId,
                    UrunAltKategoriId = model.UrunAltKategoriId,
                    UrunId = model.UrunId,
                    Stok = model.Stok,
                    Durumu = 1,
                    EklenmeTarihi = DateTime.Now,
                    GuncellenmeTarihi = DateTime.Now
                };

                depoRepository.Ekle(yeniDepo);
            }

            return RedirectToAction("Index", "AdminDepo");
        }
    }
}