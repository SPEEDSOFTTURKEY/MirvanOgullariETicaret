using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Drawing;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminUrunAnaKategoriFotografEkleController:Controller
    {
        UrunAnaKategoriFotografRepository repositoryFoto = new UrunAnaKategoriFotografRepository();
        [Obsolete]
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnvironment;

        [Obsolete]
        public AdminUrunAnaKategoriFotografEkleController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
        {

            _HostEnvironment = hostEnvironment;

        }
        public IActionResult Index()
        {

            UrunAnaKategoriRepository repository = new UrunAnaKategoriRepository();
            List<UrunAnaKategori> modelListesi = repository.GetirList(x => x.Durumu == 1).ToList();
            ViewBag.UrunAnaKategori = modelListesi;
            return View();
        }
        [HttpPost]
        [Obsolete]
        public IActionResult Kaydet(List<IFormFile> imagefile,int UrunAnaKategoriId)
        {

            if (imagefile != null && imagefile.Count != 0)
            {
                string serverpath = _HostEnvironment.ContentRootPath;
                UrunAnaKategoriFotograf fotograf = new UrunAnaKategoriFotograf();
                foreach (IFormFile item in imagefile)
                {
                    fotograf = new UrunAnaKategoriFotograf();
                    string extension = Path.GetExtension(item.FileName);
                    string newimagename = Guid.NewGuid() + extension;
                    string location = serverpath + "\\wwwroot\\WebAdminTheme\\UrunAnaKategori\\" + "Buyuk" + "\\" + newimagename;
                    FileStream stream = new FileStream(location, FileMode.Create);
                    item.CopyTo(stream);
                    Bitmap orjinal = new Bitmap(stream);
                    Bitmap kucuk = new Bitmap(orjinal, new Size(553, 371));
                    kucuk.Save(serverpath + "\\wwwroot\\WebAdminTheme\\UrunAnaKategori\\Kucuk\\" + newimagename);
                    fotograf.Durumu = 1;
                    fotograf.EklenmeTarihi = DateTime.Now;
                    fotograf.GuncellenmeTarihi = DateTime.Now;
                    fotograf.FotografBuyuk = "/WebAdminTheme/UrunAnaKategori/Buyuk/" + newimagename;
                    fotograf.FotografKucuk = "/WebAdminTheme/UrunAnaKategori/Kucuk/" + newimagename;
                    fotograf.UrunAnaKategoriId = UrunAnaKategoriId;
                    repositoryFoto.Ekle(fotograf);
                    stream.Close();
                }
            }
            return RedirectToAction("Index", "AdminUrunAnaKategoriFotograf");
        }

    }
}
