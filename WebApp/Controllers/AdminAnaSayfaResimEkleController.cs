using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminAnaSayfaResimEkleController : AdminBaseController
    {
        AnaSayfaResimRepository repository = new AnaSayfaResimRepository();
        [Obsolete]
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnvironment;

        [Obsolete]
        public AdminAnaSayfaResimEkleController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
        {
            _HostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Obsolete]
        public IActionResult Kaydet(List<IFormFile> imagefile,string Link)
        {

            if (imagefile != null && imagefile.Count != 0)
            {
                string serverpath = _HostEnvironment.ContentRootPath;
                AnaSayfaResim fotograf = new AnaSayfaResim();
                foreach (IFormFile item in imagefile)
                {
                    fotograf = new AnaSayfaResim();
                    string extension = Path.GetExtension(item.FileName);
                    string newimagename = Guid.NewGuid() + extension;
                    string location = serverpath + "\\wwwroot\\WebAdminTheme\\AnaSayfa\\" + "Buyuk" + "\\" + newimagename;
                    FileStream stream = new FileStream(location, FileMode.Create);
                    item.CopyTo(stream);
                    Bitmap orjinal = new Bitmap(stream);
                    Bitmap kucuk = new Bitmap(orjinal, new Size(1429, 864));
                    kucuk.Save(serverpath + "\\wwwroot\\WebAdminTheme\\AnaSayfa\\Kucuk\\" + newimagename);
                    kucuk.Dispose();
                    fotograf.Durumu = 1;
                    fotograf.Link = Link;
                    fotograf.EklenmeTarihi = DateTime.Now;
                    fotograf.GuncellenmeTarihi = DateTime.Now;
                    fotograf.FotografBuyuk = "/WebAdminTheme/AnaSayfa/Buyuk/" + newimagename;
                    fotograf.FotografKucuk = "/WebAdminTheme/AnaSayfa/Kucuk/" + newimagename;
                    repository.Ekle(fotograf);
                    stream.Close();
                }
            }
            return RedirectToAction("Index", "AdminAnaSayfaResim");
        }
    }
}
