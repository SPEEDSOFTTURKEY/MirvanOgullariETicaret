using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using WebApp.Models;
using WebApp.Repositories;
namespace WebApp.Controllers
{
    public class AdminHakkimizdaFotografEkleController : AdminBaseController
    {
        [Obsolete]
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnvironment;

        [Obsolete]
        public AdminHakkimizdaFotografEkleController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
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
            HakkimizdaFotografRepository repository = new HakkimizdaFotografRepository();
            if (imagefile != null && imagefile.Count != 0)
            {
                string serverpath = _HostEnvironment.ContentRootPath;
                HakkimizdaFotograf fotograf = new HakkimizdaFotograf();
                foreach (IFormFile item in imagefile)
                {
                    fotograf = new HakkimizdaFotograf();
                    string extension = Path.GetExtension(item.FileName);
                    string newimagename = Guid.NewGuid() + extension;
                    string location = serverpath + "\\wwwroot\\WebAdminTheme\\Hakkimizda\\Buyuk\\" + newimagename;
                    FileStream stream = new FileStream(location, FileMode.Create);
                    item.CopyTo(stream);
                    Bitmap orjinal = new Bitmap(stream);
                    Bitmap kucuk = new Bitmap(orjinal, new Size(400, 400));
                    kucuk.Save(serverpath + "\\wwwroot\\WebAdminTheme\\Hakkimizda\\Kucuk\\" + newimagename);
                    fotograf.Durumu = 1;
                    fotograf.EklenmeTarihi = DateTime.Now;
                    fotograf.GuncellenmeTarihi = DateTime.Now;
                    fotograf.FotografBuyuk = "/WebAdminTheme/Hakkimizda/Buyuk/" + newimagename;
                    fotograf.FotografKucuk = "/WebAdminTheme/Hakkimizda/Kucuk/" + newimagename;
                    repository.Ekle(fotograf);
                    stream.Close();
                }
            }
            return RedirectToAction("Index", "AdminHakkimizdaFotograf");
        }
    }
}
