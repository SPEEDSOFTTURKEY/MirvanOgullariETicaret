using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminIletisimFotografController : AdminBaseController
    {
        [Obsolete]
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnvironment;

        IletisimFotografRepository repository = new IletisimFotografRepository();

        [Obsolete]
        public AdminIletisimFotografController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
        {
            _HostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            IletisimFotografRepository repository = new IletisimFotografRepository();

            List<IletisimFotograf> modelListesi = repository.Listele().Where(x => x.Durumu == 1).ToList();

            ViewBag.FotografList = modelListesi;
            return View();
        }

        [Obsolete]
        public IActionResult Sil(int Id)
        {
            string pathBig = string.Empty;

            string pathSmall = string.Empty;
            IletisimFotograf foto = new IletisimFotograf();
            IletisimFotografRepository repository = new IletisimFotografRepository();
            foto = repository.Getir(Id);
            if (foto != null)
            {
                pathBig = _HostEnvironment.ContentRootPath + "\\" + "wwwroot\\";
                pathSmall = _HostEnvironment.ContentRootPath + "\\" + "wwwroot\\";
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
            return RedirectToAction("Index", "AdminIletisimFotograf");
        }
    }
}
