using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Drawing;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminUrunFotografGuncelleController : AdminBaseController
    {
        private readonly IWebHostEnvironment _HostEnvironment;

        public AdminUrunFotografGuncelleController(IWebHostEnvironment hostEnvironment)
        {
            _HostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            UrunFotograf guncellemeBilgisi = HttpContext.Session.GetObjectFromJson<UrunFotograf>("UrunFotograf");
            Video videoBilgisi = HttpContext.Session.GetObjectFromJson<Video>("Video");

            ViewBag.UrunFotograf = guncellemeBilgisi;
            ViewBag.Video = videoBilgisi;

            List<Renkler> renkler = new RenklerRepository().GetirList(x => x.Durumu == 1);
            ViewBag.RenkListesi = renkler;

            return View();
        }

        [HttpPost]
        public JsonResult Kaydet(int renkId, int Id, List<IFormFile> newImages, IFormFile newVideo, int VitrinMi, string mediaType)
        {
            if (mediaType == "photo")
            {
                UrunFotografRepository urunFotografRepository = new UrunFotografRepository();
                UrunFotograf urunFotograf = urunFotografRepository.Getir(Id);

                if (urunFotograf != null)
                {
                    urunFotograf.RenkId = renkId;
                    urunFotograf.GuncellenmeTarihi = DateTime.Now;

                    if (newImages != null && newImages.Count > 0)
                    {
                        string serverpath = _HostEnvironment.ContentRootPath;

                        if (!string.IsNullOrEmpty(urunFotograf.FotografBuyuk))
                        {
                            string eskiBuyukFotograf = Path.Combine(serverpath, "wwwroot", urunFotograf.FotografBuyuk.TrimStart('/'));
                            string eskiKucukFotograf = Path.Combine(serverpath, "wwwroot", urunFotograf.FotografKucuk.TrimStart('/'));
                            if (System.IO.File.Exists(eskiBuyukFotograf))
                                System.IO.File.Delete(eskiBuyukFotograf);
                            if (System.IO.File.Exists(eskiKucukFotograf))
                                System.IO.File.Delete(eskiKucukFotograf);
                        }

                        string newImageName = Guid.NewGuid().ToString();
                        foreach (var item in newImages)
                        {
                            string extension = Path.GetExtension(item.FileName);
                            string newImagename = newImageName + extension;

                            string largeImagePath = Path.Combine(serverpath, "wwwroot", "WebAdminTheme", "Urunler", "Buyuk", newImagename);
                            using (var stream = new FileStream(largeImagePath, FileMode.Create))
                            {
                                item.CopyTo(stream);
                            }

                            using (Bitmap orjinal = new Bitmap(largeImagePath))
                            {
                                using (Bitmap kucuk = new Bitmap(orjinal, new Size(270, 334)))
                                {
                                    string smallImagePath = Path.Combine(serverpath, "wwwroot", "WebAdminTheme", "Urunler", "Kucuk", newImagename);
                                    kucuk.Save(smallImagePath);
                                }
                            }

                            urunFotograf.FotografBuyuk = "/WebAdminTheme/Urunler/Buyuk/" + newImagename;
                            urunFotograf.FotografKucuk = "/WebAdminTheme/Urunler/Kucuk/" + newImagename;
                        }
                    }

                    urunFotograf.VitrinMi = VitrinMi == 1 ? 1 : 0;
                    urunFotograf.Durumu = 1;
                    urunFotografRepository.Guncelle(urunFotograf);

                    HttpContext.Session.Remove("UrunFotograf");
                    return Json(true);
                }
            }
            else if (mediaType == "video")
            {
                VideoRepository videoRepository = new VideoRepository();
                Video video = videoRepository.Getir(Id);

                if (video != null)
                {
                    video.RenkId = renkId;
                    video.GuncellenmeTarihi = DateTime.Now;

                    if (newVideo != null)
                    {
                        string serverpath = _HostEnvironment.ContentRootPath;

                        if (!string.IsNullOrEmpty(video.DosyaYolu))
                        {
                            string eskiVideo = Path.Combine(serverpath, "wwwroot", video.DosyaYolu.TrimStart('/'));
                            if (System.IO.File.Exists(eskiVideo))
                                System.IO.File.Delete(eskiVideo);
                        }

                        string extension = Path.GetExtension(newVideo.FileName);
                        string newVideoName = Guid.NewGuid() + extension;
                        string videoPath = Path.Combine(serverpath, "wwwroot", "WebAdminTheme", "Urunler", "Videolar", newVideoName);

                        using (var stream = new FileStream(videoPath, FileMode.Create))
                        {
                            newVideo.CopyTo(stream);
                        }

                        video.DosyaYolu = "/WebAdminTheme/Urunler/Videolar/" + newVideoName;
                    }

                    video.Durumu = 1;
                    videoRepository.Guncelle(video);

                    HttpContext.Session.Remove("Video");
                    return Json(true);
                }
            }

            return Json(false);
        }
    }
}