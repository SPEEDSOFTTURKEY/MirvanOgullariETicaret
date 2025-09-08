using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using WebApp.Models;
using WebApp.Repositories;
namespace WebApp.Controllers
{
    public class AdminHakkimizdaFotografController : AdminBaseController
    {
        [Obsolete]
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnvironment;

        HakkimizdaFotografRepository repository=new HakkimizdaFotografRepository();

        [Obsolete]
        public AdminHakkimizdaFotografController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
        {
            _HostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {

            HakkimizdaFotografRepository repository = new HakkimizdaFotografRepository();

            List<HakkimizdaFotograf> modelListesi = repository.Listele().Where(x => x.Durumu == 1).ToList();

            ViewBag.FotografList = modelListesi;
            return View();
        }
        public IActionResult Sil(int Id)
        {
            string pathBig = string.Empty;

            string pathSmall = string.Empty;
            HakkimizdaFotograf foto = new HakkimizdaFotograf();
            HakkimizdaFotografRepository repository = new HakkimizdaFotografRepository();
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
            return RedirectToAction("Index", "AdminHakkimizdaFotograf");
        }
    }
}
