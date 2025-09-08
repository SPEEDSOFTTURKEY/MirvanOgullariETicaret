using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminAnaSayfaResimController : AdminBaseController
    {
        [Obsolete]
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnvironment;

        [Obsolete]
        public AdminAnaSayfaResimController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
        {
            _HostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            AnaSayfaResimRepository repository = new AnaSayfaResimRepository();
            List<AnaSayfaResim> modelListesi = repository.Listele().Where(x => x.Durumu == 1).ToList();
            ViewBag.FotografList = modelListesi;

            return View();
        }
        [Obsolete]
        public IActionResult Sil(int Id)
        {
            string pathBig = string.Empty;
            string pathSmall = string.Empty;
            AnaSayfaResim anaSayfaFotograf = new AnaSayfaResim();
            AnaSayfaResimRepository repository = new AnaSayfaResimRepository();
            anaSayfaFotograf = repository.Getir(Id);
            if (anaSayfaFotograf != null)
            {
                pathBig = _HostEnvironment.ContentRootPath + "\\" + "wwwroot";
                pathSmall = _HostEnvironment.ContentRootPath + "\\" + "wwwroot";
                pathBig += anaSayfaFotograf.FotografBuyuk.Replace("/", "\\");
                pathSmall += anaSayfaFotograf.FotografKucuk.Replace("/", "\\");
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
            return RedirectToAction("Index", "AdminAnaSayfaResim");
        }
    }
}
