using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class AdminLogoController : AdminBaseController
    {
        [Obsolete]
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnvironment;

        LogoRepository repository = new LogoRepository();

        [Obsolete]
        public AdminLogoController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
        {
            _HostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {

            LogoRepository repository = new LogoRepository();

            List<Logo> modelListesi = repository.Listele().Where(x => x.Durumu == 1).ToList();

            ViewBag.FotografList = modelListesi;
            return View();
        }
        public IActionResult Sil(int Id)
        {
            string pathBig = string.Empty;

            string pathSmall = string.Empty;
            Logo foto = new Logo();
            LogoRepository repository = new LogoRepository();
            foto = repository.Getir(Id);
            if (foto != null)
            {
                pathBig = _HostEnvironment.ContentRootPath + "\\" + "wwwroot\\Logo\\Buyuk\\";
                pathSmall = _HostEnvironment.ContentRootPath + "\\" + "wwwroot\\Logo\\Kucuk\\";
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
            return RedirectToAction("Index", "AdminLogo");
        }
    }
}
