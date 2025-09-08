using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NuGet.Protocol.Core.Types;
using System.Drawing;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminGaleriFotografEkleController : AdminBaseController
    {
        GaleriFotografRepository repository = new GaleriFotografRepository();
        [Obsolete]
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnvironment;

        public AdminGaleriFotografEkleController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
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

            if (imagefile != null && imagefile.Count != 0)
            {
                string serverpath = _HostEnvironment.ContentRootPath;
                GaleriFotograf fotograf = new GaleriFotograf();
                foreach (IFormFile item in imagefile)
                {
                    fotograf = new GaleriFotograf();
                    string extension = Path.GetExtension(item.FileName);
                    string newimagename = Guid.NewGuid() + extension;
                    string location = serverpath + "\\wwwroot\\WebAdminTheme\\Galeri\\" + "Buyuk" + "\\" + newimagename;
                    FileStream stream = new FileStream(location, FileMode.Create);
                    item.CopyTo(stream);
                    Bitmap orjinal = new Bitmap(stream);
                    Bitmap kucuk = new Bitmap(orjinal, new Size(400, 400));
                    kucuk.Save(serverpath + "\\wwwroot\\WebAdminTheme\\Galeri\\Kucuk\\" + newimagename);
                    fotograf.Durumu = 1;
                    fotograf.EklenmeTarihi = DateTime.Now;
                    fotograf.GuncellenmeTarihi = DateTime.Now;
                    fotograf.FotografBuyuk = "/WebAdminTheme/Galeri/Buyuk/" + newimagename;
                    fotograf.FotografKucuk = "/WebAdminTheme/Galeri/Kucuk/" + newimagename;
                    repository.Ekle(fotograf);
                    stream.Close();
                }
            }
            return RedirectToAction("Index", "AdminGaleriFotograf");
        }
    }
}
