using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Drawing;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminIletisimFotografEkleController : AdminBaseController
    {
        [Obsolete]
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnvironment;

        public AdminIletisimFotografEkleController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
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
            IletisimFotografRepository repository = new IletisimFotografRepository();
            if (imagefile != null && imagefile.Count != 0)
            {
                string serverpath = _HostEnvironment.ContentRootPath;
                IletisimFotograf fotograf = new IletisimFotograf();
                foreach (IFormFile item in imagefile)
                {
                    fotograf = new IletisimFotograf();
                    string extension = Path.GetExtension(item.FileName);
                    string newimagename = Guid.NewGuid() + extension;
                    string location = serverpath + "\\wwwroot\\WebAdminTheme\\Iletisim\\Buyuk\\" + newimagename;
                    FileStream stream = new FileStream(location, FileMode.Create);
                    item.CopyTo(stream);
                    Bitmap orjinal = new Bitmap(stream);
                    Bitmap kucuk = new Bitmap(orjinal, new Size(400, 400));
                    kucuk.Save(serverpath + "\\wwwroot\\WebAdminTheme\\Iletisim\\Kucuk\\" + newimagename);
                    fotograf.Durumu = 1;
                    fotograf.EklenmeTarihi = DateTime.Now;
                    fotograf.GuncellenmeTarihi = DateTime.Now;
                    fotograf.FotografBuyuk = "/WebAdminTheme/Iletisim/Buyuk/" + newimagename;
                    fotograf.FotografKucuk = "/WebAdminTheme/Iletisim/Kucuk/" + newimagename;
                    repository.Ekle(fotograf);
                    stream.Close();
                }
            }
            return RedirectToAction("Index", "AdminIletisimFotograf");
        }
    }
}
