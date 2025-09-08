using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminGaleriFotografController : AdminBaseController
    {
        [Obsolete]
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnvironment;

        [Obsolete]
        public AdminGaleriFotografController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
        {

            _HostEnvironment = hostEnvironment;

        }

        public IActionResult Index()
        {
            GaleriFotografRepository repository = new GaleriFotografRepository();

            List<GaleriFotograf> modelListesi = repository.Listele().Where(x => x.Durumu == 1).ToList();

            ViewBag.FotografList = modelListesi;

            return View();
        }

        [Obsolete]
        public IActionResult Sil(int Id)
        {
            string pathBig = string.Empty;

            string pathSmall = string.Empty;

            GaleriFotograf anaSayfaFotograf = new GaleriFotograf();
            GaleriFotografRepository repository = new GaleriFotografRepository();
            anaSayfaFotograf = repository.Getir(Id);
            if (anaSayfaFotograf != null)
            {
                pathBig = _HostEnvironment.ContentRootPath + "\\" + "wwwroot\\";
                pathSmall = _HostEnvironment.ContentRootPath + "\\" + "wwwroot\\";
                pathBig += anaSayfaFotograf.FotografBuyuk;
                pathSmall += anaSayfaFotograf.FotografKucuk;
                if (System.IO.File.Exists(pathBig))
                {
                    System.IO.File.Delete(pathBig);
                }
                if (System.IO.File.Exists(pathSmall))
                {
                    System.IO.File.Delete(pathSmall);
                }
                repository.Sil(anaSayfaFotograf);
            }
            return RedirectToAction("Index", "AdminGaleriFotograf");
        }
    }
}
