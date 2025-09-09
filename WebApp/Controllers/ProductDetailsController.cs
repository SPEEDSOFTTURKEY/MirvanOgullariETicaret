using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class ProductDetailsController : BaseController
    {
        private readonly ILogger<ProductDetailsController> _logger;

        public ProductDetailsController(ILogger<ProductDetailsController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? id, string adi)
        {
            UrunBirimRepository urunBirimRepository = new UrunBirimRepository();

            UrunBirim urunBirim1 = urunBirimRepository.Getir(x => x.UrunId == id && x.Durumu==1);
            ViewBag.Birims = urunBirim1;
          
            await LoadCommonData();

            Uyeler uyeler = new Uyeler();
            uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");
            if (uyeler != null)
            {
                List<UyeAdres> uyeAdres = new List<UyeAdres>();
                UyeAdresRepository uyeAdresRepository = new UyeAdresRepository();
                uyeAdres = uyeAdresRepository.GetirList(x => x.Durumu == 1 && x.UyeId == uyeler.Id);
                ViewBag.UyeAdres = uyeAdres;
                ViewBag.Uyeler = uyeler;
            }
            else
            {
                ViewBag.Uyeler = null;
            }

            // Sepet hatası kontrolü
            ProductDetailForSepet hata = HttpContext.Session.GetObjectFromJson<ProductDetailForSepet>("Hata");

            if (hata != null)
            {
                id = hata.Id;
                adi = hata.UrunAdi;

                HttpContext.Session.Remove("Hata");
            }
            UrunRepository urunRepository = new UrunRepository();

            // Ürün bilgilerini getir
            Urun urun = new Urun();
            List<string> join = new List<string> { "UrunFotograf", "UrunAltKategori", "UrunAnaKategori", "UrunStok", "Renk", "Video" };
            urun = urunRepository.Getir(x => x.Id == id && x.Durumu == 1, join);
            ViewBag.Urun = urun;
    
            AcilKargoUcret acilKargoUcret = new AcilKargoUcret();
            AcilKargoUcretRepository acilKargoUcretRepository = new AcilKargoUcretRepository();
            acilKargoUcret = acilKargoUcretRepository.Getir(x => x.Durumu == 1);
            ViewBag.AcilKargoUcret = acilKargoUcret;

           

            List<Urun> urunler = new List<Urun>();
            List<string> list = new List<string>();
            list.Add("UrunFotograf");
            list.Add("Renk");
            list.Add("UrunAltKategori");
            list.Add("UrunAnaKategori");

            urunler = urunRepository.GetirList(x => x.Durumu == 1 && x.UrunAltKategoriId == urun.UrunAltKategoriId, list).OrderByDescending(x => x.Id).Take(10).ToList();
            ViewBag.YeniUrun = urunler;

            // Ürün fotoğraflarını getir
            UrunFotografRepository urunFotografRepository = new UrunFotografRepository();
            List<UrunFotograf> urunFotograf = urunFotografRepository.GetirList(x => x.Urun.Adi == adi && x.Durumu == 1, new List<string> { "Renk", "Urun" });

            // Video bilgilerini getir
            VideoRepository videoRepository = new VideoRepository();
            List<Video> videos = videoRepository.GetirList(x => x.Urun.Adi == adi && x.Durumu == 1, new List<string> { "Renk", "Urun" });

            // Renk seçeneklerini grupla
            var groupedFotograf = urunFotograf
                .GroupBy(x => x.RenkId)
                .Select(g => g.First())
                .ToList();
            ViewBag.Fotograf = groupedFotograf;
            ViewBag.Videos = videos;

            // Birim bilgilerini getir
            List<UrunBirim> urunBirim = urunBirimRepository.GetirList(x => x.UrunId == id && x.Durumu == 1, new List<string> { "Urun", "Birimler" });

            // Seçilen birim kontrolü
            var secilenbirim = HttpContext.Session.GetObjectFromJson<Birimler>("SecilenBirim");
            if (secilenbirim != null)
            {
                ViewBag.Birim = secilenbirim;
            }
            else
            {
                ViewBag.Birim = urunBirim;
            }

            // Seçilen birimler için stok kontrolü
            List<Birimler> birimler = HttpContext.Session.GetObjectFromJson<List<Birimler>>("SecilenBirimler");
            if (birimler != null)
            {
                var birimIds = birimler.Select(b => b.Id).ToList();
                UrunStokRepository urunStokRepository = new UrunStokRepository();
                List<UrunStok> urunStoks = urunStokRepository.GetirList(x => birimIds.Contains(x.BirimlerId) && x.Durumu == 1 && x.Stok > 0 && x.UrunId == id);

                BirimlerRepository birimlerRepository = new BirimlerRepository();
                var urunstokbirimIds = urunStoks.Select(s => s.BirimlerId).ToList();
                birimler = birimlerRepository.GetirList(x => urunstokbirimIds.Contains(x.Id) && x.Durumu == 1);
                ViewBag.SeciliBirim = birimler;
            }
            else
            {
                UrunStokRepository urunStokRepository = new UrunStokRepository();
                List<UrunStok> urunStoks = urunStokRepository.GetirList(x => x.Durumu == 1 && x.Stok > 0 && x.UrunId == id);

                if (urunStoks.Any())
                {
                    BirimlerRepository birimlerRepository = new BirimlerRepository();
                    var urunstokbirimIds = urunStoks.Select(s => s.BirimlerId).ToList();
                    var birimlerList = birimlerRepository.GetirList(x => urunstokbirimIds.Contains(x.Id) && x.Durumu == 1);
                    ViewBag.Birim = birimlerList;
                }
                else
                {
                    ViewBag.Birim = null;
                }
            }
            List<Sepet> sepets = new List<Sepet>();
            sepets = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet");
            ViewBag.Sepet = sepets;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(int UrunId, int BirimId, string UrunAdi)
        {
            await LoadCommonData();
            List<Sepet> sepets = new List<Sepet>();
            sepets = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet");
            ViewBag.Sepet = sepets;
            // Ürün bilgilerini getir
            UrunRepository urunRepository = new UrunRepository();
            Urun urun = new Urun();
            List<string> join = new List<string> { "UrunFotograf", "UrunAltKategori", "UrunAnaKategori", "UrunStok", "Video" };
            urun = urunRepository.Getir(x => x.Id == UrunId && x.Durumu == 1 && x.UrunStok.BirimlerId == BirimId, join);
            if (urun != null && urun.UrunStok != null)
            {
                ViewBag.Urun = urun;
            }

            // Renk bilgilerini getir
            List<Urun> urunrenk = urunRepository.GetirList(x => x.Adi == UrunAdi && x.Durumu == 1, new List<string> { "UrunFotograf" });
            ViewBag.UrunRenk = urunrenk;

            // Birim ve stok bilgilerini getir
            UrunStokRepository urunstokrepo = new UrunStokRepository();
            UrunBirimRepository urunBirimRepository = new UrunBirimRepository();
            List<UrunBirim> urunBirim = urunBirimRepository.GetirList(x => x.UrunId == UrunId && x.Durumu == 1, new List<string> { "Urun", "Birimler" });
            ViewBag.Birim = urunBirim;

            var urunstok = urunstokrepo.Getir(x => x.UrunId == UrunId && x.BirimlerId == BirimId);

            // Stok durumunu JSON olarak döndür
            if (urunstok != null && urunstok.Stok > 0)
            {
                return Json(new { success = true, stok = urunstok.Stok });
            }
            else
            {
                return Json(new { success = false, stok = 0 });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Favori(int UrunId, int UyeId)
        {
            List<Sepet> sepets = new List<Sepet>();
            sepets = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet");
            ViewBag.Sepet = sepets;
            await LoadCommonData();
            Favoriler favoriler = new Favoriler();
            FavorilerRepository favorilerRepository = new FavorilerRepository();
            favoriler.EklenmeTarihi = DateTime.Now;
            favoriler.GuncellenmeTarihi = DateTime.Now;
            favoriler.Durumu = 1;
            favoriler.UrunId = UrunId;
            favoriler.UyeId = UyeId;
            favorilerRepository.Ekle(favoriler);
            return Json(new { success = true, favoriler = favoriler });
        }

        [HttpPost]
        public async Task<IActionResult> SilFavori(int id)
        {
            List<Sepet> sepets = new List<Sepet>();
            sepets = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet");
            ViewBag.Sepet = sepets;
            await LoadCommonData();
            Favoriler favoriler = new Favoriler();
            FavorilerRepository favorilerRepository = new FavorilerRepository();
            favoriler = favorilerRepository.Getir(id);
            favoriler.Durumu = 0;
            favorilerRepository.Guncelle(favoriler);
            return RedirectToAction("Index", "ShoppingList");
        }
    }
}
