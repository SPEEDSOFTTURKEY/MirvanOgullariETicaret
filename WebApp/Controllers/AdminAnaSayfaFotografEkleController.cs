using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;
using Microsoft.AspNetCore.Hosting;
using WebApp.Repositories;
using Microsoft.AspNetCore.Authentication;

namespace WebApp.Controllers
{
    public class AdminAnaSayfaFotografEkleController : AdminBaseController
    {
        AnaSayfaFotografRepository repository = new AnaSayfaFotografRepository();
        [Obsolete]
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnvironment;

        [Obsolete]
        public AdminAnaSayfaFotografEkleController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
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
                AnaSayfaFotograf fotograf = new AnaSayfaFotograf();
                foreach (IFormFile item in imagefile)
                {
                    fotograf = new AnaSayfaFotograf();
                    string extension = Path.GetExtension(item.FileName);
                    string newimagename = Guid.NewGuid() + extension;
                    string location = serverpath + "\\wwwroot\\WebAdminTheme\\AnaSayfa\\"+"Buyuk"+"\\" + newimagename;
                    FileStream stream = new FileStream(location, FileMode.Create);
                    item.CopyTo(stream);
                    Bitmap orjinal = new Bitmap(stream);
                    Bitmap kucuk = new Bitmap(orjinal, new Size(400, 400));
                    kucuk.Save(serverpath + "\\wwwroot\\WebAdminTheme\\AnaSayfa\\Kucuk\\" + newimagename);
                    fotograf.Durumu = 1;
                    fotograf.EklenmeTarihi = DateTime.Now;
                    fotograf.GuncellenmeTarihi = DateTime.Now;
                    fotograf.FotografBuyuk= "/WebAdminTheme/AnaSayfa/Buyuk/" + newimagename;
                    fotograf.FotografKucuk= "/WebAdminTheme/AnaSayfa/Kucuk/" + newimagename;
                    repository.Ekle(fotograf);
                    stream.Close();
                }
            }
            return RedirectToAction("Index", "AdminAnaSayfaFotograf");
        }
    }
}
