using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminUrunFotografBilgileriController : AdminBaseController
    {
        private readonly IWebHostEnvironment _HostEnvironment;
        private readonly UrunFotografRepository _repository = new UrunFotografRepository();

        public AdminUrunFotografBilgileriController(IWebHostEnvironment hostEnvironment)
        {
            _HostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            List<UrunFotograf> urunFotografs = HttpContext.Session.GetObjectFromJsonCollection<UrunFotograf>("FotografListesi");
            List<Video> videos = new List<Video>();

            if (urunFotografs != null && urunFotografs.Count > 0)
            {
                RenklerRepository renkRepository = new RenklerRepository();
                foreach (var urunFotograf in urunFotografs)
                {
                    if (urunFotograf.RenkId.HasValue && urunFotograf.Renk == null)
                    {
                        urunFotograf.Renk = renkRepository.Getir(x => x.Id == urunFotograf.RenkId.Value);
                    }
                }

                VideoRepository videoRepository = new VideoRepository();
                videos = videoRepository.GetirList(x => x.Durumu == 1 && urunFotografs.Select(f => f.UrunId).Contains(x.UrunId), new List<string> { "Urun", "Renk" });

                ViewBag.FotografList = urunFotografs;
                ViewBag.VideoList = videos;
            }
            else
            {
                List<string> joinTables = new List<string> { "Urun", "Renk" };
                List<UrunFotograf> modelListesi = _repository.GetirList(x => x.Durumu == 1 && x.Urun != null, joinTables).ToList();

                VideoRepository videoRepository = new VideoRepository();
                videos = videoRepository.GetirList(x => x.Durumu == 1 && modelListesi.Select(f => f.UrunId).Contains(x.UrunId), new List<string> { "Urun", "Renk" });

                ViewBag.FotografList = modelListesi;
                ViewBag.VideoList = videos;
            }

            return View();
        }

        public IActionResult Guncelle(int id)
        {
            HttpContext.Session.Remove("Video"); // Clear video session to avoid conflicts
            UrunFotografRepository urunFotografRepository = new UrunFotografRepository();
            UrunFotograf urunFotograf = urunFotografRepository.Getir(x => x.Durumu == 1 && x.Id == id, new List<string> { "Urun", "Renk" });
            if (urunFotograf != null)
            {
                HttpContext.Session.SetObjectAsJson("UrunFotograf", urunFotograf);
            }
            return RedirectToAction("Index", "AdminUrunFotografGuncelle");
        }

        public IActionResult GuncelleVideo(int id)
        {
            HttpContext.Session.Remove("UrunFotograf"); // Clear photo session to avoid conflicts
            VideoRepository videoRepository = new VideoRepository();
            Video video = videoRepository.Getir(x => x.Durumu == 1 && x.Id == id, new List<string> { "Urun", "Renk" });
            if (video != null)
            {
                HttpContext.Session.SetObjectAsJson("Video", video);
            }
            return RedirectToAction("Index", "AdminUrunFotografGuncelle");
        }

        public IActionResult Sil(int Id)
        {
            UrunFotograf foto = _repository.Getir(Id);
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
                _repository.Sil(foto);
            }
            return RedirectToAction("Index", "AdminUrunFotografBilgileri");
        }

        public IActionResult SilVideo(int Id)
        {
            VideoRepository videoRepository = new VideoRepository();
            Video video = videoRepository.Getir(Id);
            if (video != null)
            {
                string pathVideo = Path.Combine(_HostEnvironment.ContentRootPath, "wwwroot", "WebAdminTheme", "Urunler", "Videolar", Path.GetFileName(video.DosyaYolu));
                if (System.IO.File.Exists(pathVideo))
                {
                    System.IO.File.Delete(pathVideo);
                }
                videoRepository.Sil(video);
            }
            return RedirectToAction("Index", "AdminUrunFotografBilgileri");
        }

        public IActionResult Search(string adi)
        {
            HttpContext.Session.Remove("FotografListesi");
            List<UrunFotograf> urunFotografs = new List<UrunFotograf>();
            UrunFotografRepository urunFotografRepository = new UrunFotografRepository();
            List<string> join = new List<string> { "Urun", "Renk" };
            urunFotografs = urunFotografRepository.GetirList(x => x.Durumu == 1 && x.Urun.Adi.Contains(adi), join);

            VideoRepository videoRepository = new VideoRepository();
            List<Video> videos = videoRepository.GetirList(x => x.Durumu == 1 && x.Urun.Adi.Contains(adi), new List<string> { "Urun", "Renk" });

            HttpContext.Session.SetObjectAsJson("FotografListesi", urunFotografs);
            ViewBag.FotografList = urunFotografs;
            ViewBag.VideoList = videos;

            return RedirectToAction("Index", "AdminUrunFotografBilgileri");
        }

        public IActionResult GetAllData()
        {
            HttpContext.Session.Remove("FotografListesi");
            return RedirectToAction("Index", "AdminUrunFotografBilgileri");
        }
    }
}