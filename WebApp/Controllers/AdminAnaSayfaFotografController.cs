using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminAnaSayfaFotografController : AdminBaseController
    {
        [Obsolete]
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnvironment;

        [Obsolete]
        public AdminAnaSayfaFotografController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
        {
            _HostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            AnaSayfaFotografRepository repository = new AnaSayfaFotografRepository();

            List<AnaSayfaFotograf> modelListesi = repository.Listele().Where(x => x.Durumu == 1).ToList();

            ViewBag.FotografList = modelListesi;

            return View();
        }

        [Obsolete]
        public IActionResult Sil(int Id)
        {
            string pathBig = string.Empty;
            
            string pathSmall = string.Empty;
            
            AnaSayfaFotograf anaSayfaFotograf = new AnaSayfaFotograf();
            AnaSayfaFotografRepository repository = new AnaSayfaFotografRepository();
            anaSayfaFotograf = repository.Getir(Id);
            if (anaSayfaFotograf != null)
            {
                pathBig = _HostEnvironment.ContentRootPath+"\\"+"wwwroot\\";
                pathSmall= _HostEnvironment.ContentRootPath + "\\" + "wwwroot\\";
                pathBig += anaSayfaFotograf.FotografBuyuk;
                pathSmall += anaSayfaFotograf.FotografKucuk;
                if(System.IO.File.Exists(pathBig))
                {
                    System.IO.File.Delete(pathBig);
                }
                if (System.IO.File.Exists(pathSmall))
                {
                    System.IO.File.Delete(pathSmall);
                }
                repository.Sil(anaSayfaFotograf);
            }
            return RedirectToAction("Index", "AdminAnaSayfaFotograf");
        }
    }
}
