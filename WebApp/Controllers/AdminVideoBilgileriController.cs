using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminVideoBilgileriController : AdminBaseController
    {
        [Obsolete]
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnvironment;

        VideoRepository repository=new VideoRepository();

        [Obsolete]
        public AdminVideoBilgileriController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
        {

            _HostEnvironment = hostEnvironment;

        }

        public IActionResult Index()
        {
            VideoRepository repository = new VideoRepository();

            List<Video> modelListesi = repository.Listele().Where(x => x.Durumu == 1).ToList();

            ViewBag.FotografList = modelListesi;

            return View();
        }

        public IActionResult Sil(int Id)
        {
           string path = string.Empty;
            Video video = new Video();
            VideoRepository repository = new VideoRepository();
            video = repository.Getir(Id);
            if (video != null)
            {
                path = _HostEnvironment.ContentRootPath + "\\" + "wwwroot\\";
                path += video.DosyaYolu;
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                repository.Sil(video);
            }
            return RedirectToAction("Index", "AdminVideoBilgileri");
        }
    }
}
