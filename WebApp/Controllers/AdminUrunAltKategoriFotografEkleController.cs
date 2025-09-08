using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminUrunAltKategoriFotografEkleController : AdminBaseController
    {
        UrunAltKategoriFotografRepository repositoryFoto = new UrunAltKategoriFotografRepository();
        [Obsolete]
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnvironment;

        [Obsolete]
        public AdminUrunAltKategoriFotografEkleController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
        {

            _HostEnvironment = hostEnvironment;

        }
        public IActionResult Index()
        {

            UrunAltKategoriRepository repository = new UrunAltKategoriRepository();
            List<UrunAltKategori> modelListesi = repository.GetirList(x => x.Durumu == 1).ToList();
            ViewBag.UrunAltKategori = modelListesi;
            return View();
        }
        [HttpPost]
        [Obsolete]
        public IActionResult Kaydet(List<IFormFile> imagefile, int UrunAltKategoriId)
        {

            if (imagefile != null && imagefile.Count != 0)
            {
                string serverpath = _HostEnvironment.ContentRootPath;
                UrunAltKategoriFotograf fotograf = new UrunAltKategoriFotograf();
                foreach (IFormFile item in imagefile)
                {
                    fotograf = new UrunAltKategoriFotograf();
                    string extension = Path.GetExtension(item.FileName);
                    string newimagename = Guid.NewGuid() + extension;
                    string location = serverpath + "\\wwwroot\\WebAdminTheme\\UrunAltKategori\\" + "Buyuk" + "\\" + newimagename;
                    FileStream stream = new FileStream(location, FileMode.Create);
                    item.CopyTo(stream);
                    Bitmap orjinal = new Bitmap(stream);
                    Bitmap kucuk = new Bitmap(orjinal, new Size(270, 334));
                    kucuk.Save(serverpath + "\\wwwroot\\WebAdminTheme\\UrunAltKategori\\Kucuk\\" + newimagename);
                    fotograf.Durumu = 1;
                    fotograf.EklenmeTarihi = DateTime.Now;
                    fotograf.GuncellenmeTarihi = DateTime.Now;
                    fotograf.FotografBuyuk = "/WebAdminTheme/UrunAltKategori/Buyuk/" + newimagename;
                    fotograf.FotografKucuk = "/WebAdminTheme/UrunAltKategori/Kucuk/" + newimagename;
                    fotograf.UrunAltKategoriId = UrunAltKategoriId;
                    repositoryFoto.Ekle(fotograf);
                    stream.Close();
                }
            }
            return RedirectToAction("Index", "AdminUrunAltKategoriFotograf");
        }
    }
}
