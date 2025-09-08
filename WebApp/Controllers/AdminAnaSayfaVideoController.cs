using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminAnaSayfaVideoController : AdminBaseController
    {
        [Obsolete]
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnvironment;

        AnaSayfaVideoRepository repository = new AnaSayfaVideoRepository();

        [Obsolete]
        public AdminAnaSayfaVideoController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
        {
            _HostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            AnaSayfaVideoRepository repository = new AnaSayfaVideoRepository();

            List<AnaSayfaVideo> modelListesi = repository.Listele().Where(x => x.Durumu == 1).ToList();

            ViewBag.VideoListesi = modelListesi;

            return View();
        }
        public IActionResult Sil(int Id)
        {
            string path = string.Empty;
            AnaSayfaVideo video = new AnaSayfaVideo();
            repository = new AnaSayfaVideoRepository();
            video = repository.Getir(Id);
            if (video != null)
            {
                path = _HostEnvironment.ContentRootPath + "\\" + "wwwroot";
                path += video.DosyaYolu.Replace("/","\\");
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                repository.Sil(video);
            }
            return RedirectToAction("Index", "AdminAnaSayfaVideo");
        }

    }
}
