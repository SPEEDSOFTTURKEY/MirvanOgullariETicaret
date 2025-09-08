using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class AdminLogoEkleController : AdminBaseController
    {
        [Obsolete]
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnvironment;

        [Obsolete]
        public AdminLogoEkleController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
        {
            _HostEnvironment = hostEnvironment;

        }

        public IActionResult Index()
        {
            return View();
        }

        [Obsolete]
        [HttpPost]

        public IActionResult Kaydet(List<IFormFile> imagefile)
        {
            LogoRepository repository = new LogoRepository();
            if (imagefile != null && imagefile.Count != 0)
            {
                string serverpath = _HostEnvironment.ContentRootPath;
                Logo fotograf = new Logo();
                foreach (IFormFile item in imagefile)
                {
                    fotograf = new Logo();
                    string extension = Path.GetExtension(item.FileName);
                    string newimagename = Guid.NewGuid() + extension;
                    string location = serverpath + "\\wwwroot\\WebAdminTheme\\Logo\\Buyuk\\" + newimagename;
                    FileStream stream = new FileStream(location, FileMode.Create);
                    item.CopyTo(stream);
                    Bitmap orjinal = new Bitmap(stream);
                    Bitmap kucuk = new Bitmap(orjinal, new Size(270, 80));
                    kucuk.Save(serverpath + "\\wwwroot\\WebAdminTheme\\Logo\\Kucuk\\" + newimagename);
                    fotograf.Durumu = 1;
                    fotograf.EklenmeTarihi = DateTime.Now;
                    fotograf.GuncellenmeTarihi = DateTime.Now;
                    fotograf.FotografBuyuk = "/WebAdminTheme/Logo/Buyuk/" + newimagename;
                    fotograf.FotografKucuk = "/WebAdminTheme/Logo/Kucuk/" + newimagename;
                    repository.Ekle(fotograf);
                    stream.Close();
                }
            }
            return RedirectToAction("Index", "AdminLogo");
        }
    }
}
