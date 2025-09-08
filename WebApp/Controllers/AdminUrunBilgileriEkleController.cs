using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminUrunBilgileriEkleController : AdminBaseController
    {
        UrunAltKategoriRepository repository = new UrunAltKategoriRepository();

        public IActionResult Index()
        {
            UrunStok urunStok = new UrunStok();
            UrunAltKategoriRepository urunAltKategoriRepository = new UrunAltKategoriRepository();

            List<UrunAltKategori> UrunTipListesi = repository.GetirList(x => x.Durumu == 1).ToList();
            ViewBag.UrunTipListesi = UrunTipListesi;

            UrunAnaKategoriRepository krepository = new UrunAnaKategoriRepository();
            List<UrunAnaKategori> UrunAnaKategoriListesi = krepository.GetirList(x => x.Durumu == 1).ToList();
            ViewBag.UrunAnaKategoriListesi = UrunAnaKategoriListesi;

            BirimlerRepository birimRepository = new BirimlerRepository();
            List<Birimler> birimListesi = birimRepository.GetirList(x => x.Durumu == 1).ToList();
            ViewBag.BirimListesi = birimListesi;

            RenklerRepository renklerRepository = new RenklerRepository();
            List<Renkler> renkListesi = renklerRepository.GetirList(x => x.Durumu == 1).ToList();
            ViewBag.RenkListesi = renkListesi;

            return View();
        }
        public JsonResult GetirAltKategori(int AnaKategoriId)
        {
            // Alt kategorileri getir
            var urunAltKategori = repository.GetirList(x => x.UrunAnaKategoriId == AnaKategoriId && x.Durumu == 1).ToList();

            // Boş liste döndürme
            return Json(urunAltKategori);
        }

        public JsonResult GetirUrun(int AltKategoriId)
        {
            UrunRepository repository = new UrunRepository();

            // Alt kategorileri getir
            var urun = repository.GetirList(x => x.UrunAltKategoriId == AltKategoriId && x.Durumu == 1).ToList();

            // Boş liste döndürme
            return Json(urun);
        }
        public JsonResult GetirRenk(int UrunId)
        {
            RenklerRepository repository = new RenklerRepository();
            UrunRepository urunRepository = new UrunRepository();
            Urun urun = new Urun();
            urun = urunRepository.Getir(x => x.Id == UrunId);
            // Alt kategorileri getir
            if (urun != null)
            {
                List<Renkler> renk = repository.GetirList(x => x.Durumu == 1 && x.Id == urun.RenkId).ToList();
                // Boş liste döndürme
                return Json(renk);
            }
            return Json(null);

        }
        [HttpPost]
        public JsonResult Kaydet(Urun urun, int[] birimIds, int stok)
        {
            if (urun != null)
            {
                UrunRepository BilgileriRepository = new UrunRepository();
                urun.EklenmeTarihi = DateTime.Now;
                urun.GuncellenmeTarihi = DateTime.Now;
                urun.Durumu = 1;
                BilgileriRepository.Ekle(urun);
                birimIds = birimIds.Distinct().ToArray();
                // UrunBirim kayıtları
                foreach (int birimId in birimIds)
                {
                    UrunBirim urunBirim = new UrunBirim
                    {
                        Durumu = 1,
                        UrunId = urun.Id,
                        BirimlerId = birimId,
                        EklenmeTarihi = DateTime.Now,
                        GuncellenmeTarihi = DateTime.Now
                    };
                    UrunBirimRepository urunBirimRepository = new UrunBirimRepository();
                    urunBirimRepository.Ekle(urunBirim);

                    // Stok Bilgisi Ekleme
                    UrunStok urunStok = new UrunStok
                    {
                        UrunId = urun.Id,
                        UrunAnaKategoriId = urun.UrunAnaKategoriId ?? 0,
                        UrunAltKategoriId = urun.UrunAltKategoriId ?? 0,
                        UrunRenkId = urun.RenkId,
                        BirimlerId = birimId, // Eğer UrunBirim null ise 0 değeri atanır
                        Stok = stok,
                        Durumu = 1,
                        EklenmeTarihi = DateTime.Now,
                        GuncellenmeTarihi = DateTime.Now,
                        StokTuruId = 1
                    };

                    UrunStokRepository urunStokRepository = new UrunStokRepository();
                    urunStokRepository.Ekle(urunStok);

                }
            }
            return Json(urun);
        }
    }
}
