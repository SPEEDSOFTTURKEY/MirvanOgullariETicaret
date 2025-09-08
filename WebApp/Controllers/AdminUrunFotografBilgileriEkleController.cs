using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Drawing;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminUrunFotografBilgileriEkleController : AdminBaseController
    {
        private readonly IWebHostEnvironment _HostEnvironment;

        public AdminUrunFotografBilgileriEkleController(IWebHostEnvironment hostEnvironment)
        {
            _HostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            UrunRepository repository = new UrunRepository();
            RenklerRepository renkrepository = new RenklerRepository();
            UrunAnaKategoriRepository anarepository = new UrunAnaKategoriRepository();
            UrunAltKategoriRepository altrepository = new UrunAltKategoriRepository();

            List<Urun> urunListesi = new List<Urun>();
            urunListesi = repository.Listele().Where(x => x.Durumu == 1).ToList();
            ViewBag.UrunListesi = urunListesi;

            List<Renkler> renkler = new List<Renkler>();
            renkler = renkrepository.Listele().Where(x => x.Durumu == 1).ToList();
            ViewBag.RenkListesi = renkler;

            List<UrunAnaKategori> anaKategori = new List<UrunAnaKategori>();
            anaKategori = anarepository.Listele().Where(x => x.Durumu == 1).ToList();
            ViewBag.AnaKategoriList = anaKategori;

            List<UrunAltKategori> altKategori = new List<UrunAltKategori>();
            altKategori = altrepository.Listele().Where(x => x.Durumu == 1).ToList();
            ViewBag.AltKategoriList = altKategori;
            return View();
        }

        public JsonResult GetirUrun(int AltKategoriId)
        {
            UrunRepository repository = new UrunRepository();
            var urun = repository.GetirList(x => x.UrunAltKategoriId == AltKategoriId && x.Durumu == 1).ToList();
            return Json(urun);
        }

        public JsonResult GetirRenk(int UrunId)
        {
            RenklerRepository repository = new RenklerRepository();
            UrunRepository urunRepository = new UrunRepository();
            Urun urun = new Urun();
            urun = urunRepository.Getir(x => x.Id == UrunId);
            if (urun != null)
            {
                List<Renkler> renk = repository.GetirList(x => x.Durumu == 1 && x.Id == urun.RenkId).ToList();
                return Json(renk);
            }
            return Json(null);
        }

        public JsonResult GetirAltKategori(int AnaKategoriId)
        {
            UrunAltKategoriRepository repository = new UrunAltKategoriRepository();
            var urunAltKategori = repository.GetirList(x => x.UrunAnaKategoriId == AnaKategoriId && x.Durumu == 1).ToList();
            return Json(urunAltKategori);
        }

        [HttpPost]
        public IActionResult Kaydet(List<IFormFile> image, IFormFile video, int UrunId, int UrunRenkId, int VitrinMi)
        {
            UrunFotografRepository fotografRepository = new UrunFotografRepository();
            VideoRepository videoRepository = new VideoRepository();
            string serverpath = _HostEnvironment.ContentRootPath;

            // Handle image upload
            if (image != null && image.Count != 0)
            {
                foreach (IFormFile item in image)
                {
                    UrunFotograf fotograf = new UrunFotograf();
                    string extension = Path.GetExtension(item.FileName);
                    string newimagename = Guid.NewGuid() + extension;
                    string largeImagePath = Path.Combine(serverpath, "wwwroot", "WebAdminTheme", "Urunler", "Buyuk", newimagename);
                    string smallImagePath = Path.Combine(serverpath, "wwwroot", "WebAdminTheme", "Urunler", "Kucuk", newimagename);

                    // Save large image
                    using (FileStream stream = new FileStream(largeImagePath, FileMode.Create))
                    {
                        item.CopyTo(stream);
                    }

                    // Create and save small image
                    using (Bitmap original = new Bitmap(largeImagePath))
                    {
                        using (Bitmap small = new Bitmap(original, new Size(270, 334)))
                        {
                            small.Save(smallImagePath);
                        }
                    }

                    fotograf.VitrinMi = VitrinMi == 1 ? 1 : 0;
                    fotograf.Durumu = 1;
                    fotograf.EklenmeTarihi = DateTime.Now;
                    fotograf.GuncellenmeTarihi = DateTime.Now;
                    fotograf.FotografBuyuk = "/WebAdminTheme/Urunler/Buyuk/" + newimagename;
                    fotograf.FotografKucuk = "/WebAdminTheme/Urunler/Kucuk/" + newimagename;
                    fotograf.UrunId = UrunId;
                    fotograf.RenkId = UrunRenkId;
                    fotografRepository.Ekle(fotograf);
                }
            }

            // Handle video upload
            if (video != null)
            {
                string videoExtension = Path.GetExtension(video.FileName);
                string newVideoName = Guid.NewGuid() + videoExtension;
                string videoPath = Path.Combine(serverpath, "wwwroot", "WebAdminTheme", "Urunler", "Videolar", newVideoName);

                // Save video
                using (FileStream stream = new FileStream(videoPath, FileMode.Create))
                {
                    video.CopyTo(stream);
                }

                Video videoModel = new Video
                {
                    DosyaYolu = "/WebAdminTheme/Urunler/Videolar/" + newVideoName,
                    Durumu = 1,
                    EklenmeTarihi = DateTime.Now,
                    GuncellenmeTarihi = DateTime.Now,
                    UrunId = UrunId,
                    RenkId = UrunRenkId
                };
                videoRepository.Ekle(videoModel);
            }

            return RedirectToAction("Index", "AdminUrunFotografBilgileri");
        }
    }
}