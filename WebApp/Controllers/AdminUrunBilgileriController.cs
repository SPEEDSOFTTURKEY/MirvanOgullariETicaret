using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NuGet.Protocol.Core.Types;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminUrunBilgileriController : AdminBaseController
    {
        private readonly IWebHostEnvironment _HostEnvironment;

        UrunAnaKategoriRepository kategoriRepository = new UrunAnaKategoriRepository();
        UrunAltKategoriRepository AltKategoriRepository = new UrunAltKategoriRepository();
        UrunRepository urunRepository = new UrunRepository();
        RenklerRepository renklerRepository = new RenklerRepository();

        public AdminUrunBilgileriController(IWebHostEnvironment hostEnvironment)
        {
            _HostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            List<UrunAnaKategori> anaKategori = new List<UrunAnaKategori>();
            anaKategori = kategoriRepository.GetirList(x => x.Durumu == 1);
            ViewBag.AnaKategori = anaKategori;

            UrunRepository repository = new UrunRepository();
            List<Urun> uruns = new List<Urun>();
            uruns = HttpContext.Session.GetObjectFromJsonCollection<Urun>("UrunListesi");
            if (uruns != null && uruns.Count > 0)
            {
             
                ViewBag.UrunlerList = uruns;

            }
            else 
            {
                List<string> JoinTables = new List<string>();
                JoinTables.Add("UrunAltKategori");
                JoinTables.Add("UrunAnaKategori");
                JoinTables.Add("Renk");
                JoinTables.Add("UrunFotograf");
                List<Urun> modelListesi = repository.GetirList(x => x.Durumu == 1, JoinTables).ToList();
                ViewBag.UrunlerList = modelListesi;
            }




            return View();
        }

        public IActionResult Guncelleme(int id)
        {
            UrunRepository repository = new UrunRepository();
            List<string> JoinTables = new List<string>();
            JoinTables.Add("UrunAltKategori");
            JoinTables.Add("UrunBirim");
            JoinTables.Add("UrunAnaKategori");
            JoinTables.Add("UrunStok");
            JoinTables.Add("Renk");
        Urun urun = repository.Getir(x=>x.Id == id,JoinTables);
            if (urun != null)
            {
                HttpContext.Session.SetObjectAsJson("Urun", urun);
            }
            return RedirectToAction("Index", "AdminUrunBilgileriGuncelle");
        }


        public IActionResult Sil(int id,int renkId)
        {
         
            UrunFotografRepository urunFotografRepository = new UrunFotografRepository();
            List<UrunFotograf> urunFotografList = urunFotografRepository.GetirList(x => x.Durumu == 1 && x.UrunId == id && x.RenkId==renkId);

            foreach (var foto in urunFotografList)
            {
                if (foto != null)
                {
                    string pathBig = Path.Combine(_HostEnvironment.ContentRootPath, "wwwroot", "WebAdminTheme", "Urunler", "Buyuk", Path.GetFileName(foto.FotografBuyuk));
                    string pathSmall = Path.Combine(_HostEnvironment.ContentRootPath, "wwwroot", "WebAdminTheme", "Urunler", "Kucuk", Path.GetFileName(foto.FotografKucuk));

                    if (System.IO.File.Exists(pathBig))
                    {
                        System.IO.File.Delete(pathBig);
                    }
                    if (System.IO.File.Exists(pathSmall))
                    {
                        System.IO.File.Delete(pathSmall);
                    }

                    urunFotografRepository.Sil(foto); // Veya urunFotografRepository.Sil(foto); kullandığın repository nesnesine göre değiştir.
                }
            }
            VideoRepository videoRepository = new VideoRepository();
            List<Video> videoListesi = videoRepository.GetirList(x => x.Durumu == 1 && x.UrunId == id);

            foreach (var video in videoListesi)
            {
                if (video != null)
                {
                    string pathVideo = Path.Combine(_HostEnvironment.ContentRootPath, "wwwroot", "WebAdminTheme", "Urunler", "Videolar", Path.GetFileName(video.DosyaYolu));

                    if (System.IO.File.Exists(pathVideo))
                    {
                        System.IO.File.Delete(pathVideo);
                    }

                    videoRepository.Sil(video);
                }
            }
            UrunBirimRepository urunBirimRepository = new UrunBirimRepository();
            List<UrunBirim> urunBirimListesi = urunBirimRepository.GetirList(x => x.Durumu == 1 && x.UrunId == id);

            foreach (var urunBirim in urunBirimListesi)
            {
                urunBirim.Durumu = 0;
                urunBirim.GuncellenmeTarihi = DateTime.Now;
                urunBirimRepository.Guncelle(urunBirim); // Güncelle metodun varsa
            }

            UrunStokRepository urunStokRepository = new UrunStokRepository();

            // Şartlara uyan stok kayıtlarını çekiyoruz
            List<UrunStok> stokListesi = urunStokRepository.GetirList(x => x.UrunId == id && x.UrunRenkId == renkId && x.Durumu == 1);

            foreach (var stok in stokListesi)
            {
                stok.Durumu = 0;
                stok.GuncellenmeTarihi = DateTime.Now;
                urunStokRepository.Guncelle(stok); // Guncelle() metodun varsa
            }
            UrunRepository repository = new UrunRepository();
            Urun urun = repository.Getir(x => x.Id == id && x.RenkId == renkId);
            if (urun != null)
            {
                urun.Durumu = 0;
                urun.GuncellenmeTarihi = DateTime.Now;
                repository.Guncelle(urun);
            }
            return RedirectToAction("Index", "AdminUrunBilgileri");
        }
        [HttpPost]
        public IActionResult TopluSil(List<int> idList)
        {
            UrunRepository repository = new UrunRepository();

            foreach (int id in idList)
            {
                Urun urun = repository.Getir(id);
                if (urun != null)
                {
                    urun.Durumu = 0;
                    urun.GuncellenmeTarihi = DateTime.Now;
                    repository.Guncelle(urun);
                }
            }

            return RedirectToAction("Index", "AdminUrunBilgileri");
        }


        public IActionResult Search(string adi)
        {
            HttpContext.Session.Remove("UrunListesi");
            List<Urun> urunStok = new List<Urun>();
            UrunRepository urunStokRepository = new UrunRepository();
            List<string> JoinTables = new List<string>();
            JoinTables.Add("UrunAltKategori");
            JoinTables.Add("UrunAnaKategori");
            JoinTables.Add("Renk");
            JoinTables.Add("UrunFotograf");
            urunStok = urunStokRepository.GetirList(x => x.Durumu == 1 && x.UrunFotograf != null && x.Adi.Contains(adi),JoinTables);
            HttpContext.Session.SetObjectAsJson("UrunListesi", urunStok);


            return RedirectToAction("Index", "AdminUrunBilgileri");
        }
        public IActionResult GetAllData()
        {

            HttpContext.Session.Remove("UrunListesi");
            return RedirectToAction("Index", "AdminUrunBilgileri");
        }

        public IActionResult TumIndirim(int indirim)
        {
            UrunRepository urunRepository = new UrunRepository();
            List<Urun> urunListesi = urunRepository.GetirList(x => x.Durumu == 1);

            foreach (var urun in urunListesi)
            {
                urun.IndirimYuzdesi = indirim;
                urunRepository.Guncelle(urun); // Güncelleme işlemi
            }

            return RedirectToAction("Index","AdminUrunBilgileri"); // İşlemin ardından yönlendirme
        }
        public JsonResult GetirAltKategori(int AnaKategoriId)
        {
            // Alt kategorileri getir
            var urunAltKategori = AltKategoriRepository.GetirList(x => x.UrunAnaKategoriId == AnaKategoriId && x.Durumu == 1).ToList();

            // Boş liste döndürme
            return Json(urunAltKategori);
        }
        public IActionResult AnaKategoriIndirim(int AnaKategoriId, int indirim)
        {
            UrunRepository urunRepository = new UrunRepository();
            List<Urun> urunListesi = urunRepository.GetirList(x => x.Durumu == 1 && x.UrunAnaKategoriId == AnaKategoriId);

            foreach (var urun in urunListesi)
            {
                urun.IndirimYuzdesi = indirim;
                urunRepository.Guncelle(urun); // Güncelleme işlemi
            }

            return RedirectToAction("Index", "AdminUrunBilgileri"); // İşlemin ardından yönlendirme
        }
        public IActionResult KategoriIndirim(int UrunAltKategoriId,int indirim)
        {
            UrunRepository urunRepository = new UrunRepository();
            List<Urun> urunListesi = urunRepository.GetirList(x => x.Durumu == 1&& x.UrunAltKategoriId== UrunAltKategoriId);

            foreach (var urun in urunListesi)
            {
                urun.IndirimYuzdesi = indirim;
                urunRepository.Guncelle(urun); // Güncelleme işlemi
            }

            return RedirectToAction("Index", "AdminUrunBilgileri"); // İşlemin ardından yönlendirme
        }

    }
}
