using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminAnaSayfaVideoEkleController : AdminBaseController
    {
        [Obsolete]
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnvironment;

        [Obsolete]
        public AdminAnaSayfaVideoEkleController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
        {
            _HostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Obsolete]
        public IActionResult Kaydet(List<IFormFile> imagefile)
        {
            AnaSayfaVideoRepository repository = new AnaSayfaVideoRepository();
            if (imagefile != null && imagefile.Count != 0)
            {
                string serverpath = _HostEnvironment.ContentRootPath;
                AnaSayfaVideo fotograf = new AnaSayfaVideo();
                foreach (IFormFile item in imagefile)
                {
                    fotograf = new AnaSayfaVideo();
                    string extension = Path.GetExtension(item.FileName);
                    string newimagename = Guid.NewGuid() + extension;
                    string location = serverpath + "\\wwwroot\\WebAdminTheme\\AnaSayfa\\Video\\" + newimagename;
                    FileStream stream = new FileStream(location, FileMode.Create);
                    item.CopyTo(stream);
                    fotograf.Durumu = 1;
                    fotograf.EklenmeTarihi = DateTime.Now;
                    fotograf.GuncellenmeTarihi = DateTime.Now;
                    fotograf.DosyaYolu = "/WebAdminTheme/AnaSayfa/Video/" + newimagename;
                    repository.Ekle(fotograf);
                    stream.Close();
                }
            }
            return RedirectToAction("Index", "AdminAnaSayfaVideo");
        }
    }
}
