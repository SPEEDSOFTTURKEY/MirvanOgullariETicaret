using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class AdminUrunGaleriBilgileriEkleController : AdminBaseController
    {
        [Obsolete]
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnvironment;

        [Obsolete]
        public AdminUrunGaleriBilgileriEkleController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
        {
            _HostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            UrunRepository repository = new UrunRepository();

            List<Urun> urunListesi = new List<Urun>();
            urunListesi = repository.Listele().Where(x => x.Durumu == 1).ToList();
            ViewBag.UrunListesi = urunListesi;
            return View();
        }
        [HttpPost]
        [Obsolete]
        public IActionResult Kaydet(List<IFormFile> imagefile, int UrunId)
        {
            UrunGaleriRepository repository = new UrunGaleriRepository();
            if (imagefile != null && imagefile.Count != 0)
            {
                string serverpath = _HostEnvironment.ContentRootPath;
                UrunGaleri fotograf = new UrunGaleri();
                foreach (IFormFile item in imagefile)
                {
                    fotograf = new UrunGaleri();
                    string extension = Path.GetExtension(item.FileName);
                    string newimagename = Guid.NewGuid() + extension;
                    string location = serverpath + "\\wwwroot\\WebAdminTheme\\Urunler\\Buyuk\\" + newimagename;
                    FileStream stream = new FileStream(location, FileMode.Create);
                    item.CopyTo(stream);
                    Bitmap orjinal = new Bitmap(stream);
                    Bitmap kucuk = new Bitmap(orjinal, new Size(500, 637));
                    kucuk.Save(serverpath + "\\wwwroot\\WebAdminTheme\\Urunler\\Kucuk\\" + newimagename);
                    fotograf.Durumu = 1;
                    fotograf.EklenmeTarihi = DateTime.Now;
                    fotograf.GuncellenmeTarihi = DateTime.Now;
                    fotograf.FotografBuyuk = "/WebAdminTheme/Urunler/Buyuk/" + newimagename;
                    fotograf.FotografKucuk = "/WebAdminTheme/Urunler/Kucuk/" + newimagename;
                    fotograf.UrunId = UrunId;
                    repository.Ekle(fotograf);
                    stream.Close();
                }
            }
            return RedirectToAction("Index", "AdminUrunGaleriBilgileri");
        }
    }
}

