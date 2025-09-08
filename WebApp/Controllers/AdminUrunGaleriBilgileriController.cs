using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class AdminUrunGaleriBilgileriController : AdminBaseController
    {
        [Obsolete]
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnvironment;

        UrunGaleriRepository repository = new UrunGaleriRepository();

        [Obsolete]
        public AdminUrunGaleriBilgileriController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
        {
            _HostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            UrunGaleriRepository repository = new UrunGaleriRepository();
            List<string> joinTables = new List<string>();
            joinTables.Add("Urun");
            List<UrunGaleri> modelListesi = repository.GetirList(x => x.Durumu == 1, joinTables).ToList();
            ViewBag.FotografList = modelListesi;
            return View();
        }
        [Obsolete]
        public IActionResult Sil(int Id)
        {
            string pathBig = string.Empty;
            string pathSmall = string.Empty;
            UrunGaleri foto = new UrunGaleri();
            UrunGaleriRepository repository = new UrunGaleriRepository();
            foto = repository.Getir(Id);
            if (foto != null)
            {
                pathBig = _HostEnvironment.ContentRootPath + "\\" + "wwwroot\\WebAdminTheme\\Urunler\\Buyuk\\";
                pathSmall = _HostEnvironment.ContentRootPath + "\\" + "wwwroot\\WebAdminTheme\\Urunler\\Kucuk\\";
                pathBig += foto.FotografBuyuk;
                pathSmall += foto.FotografKucuk;
                if (System.IO.File.Exists(pathBig))
                {
                    System.IO.File.Delete(pathBig);
                }
                if (System.IO.File.Exists(pathSmall))
                {
                    System.IO.File.Delete(pathSmall);
                }
                repository.Sil(foto);
            }
            return RedirectToAction("Index", "AdminUrunGaleriBilgileri");
        }
    }
}

