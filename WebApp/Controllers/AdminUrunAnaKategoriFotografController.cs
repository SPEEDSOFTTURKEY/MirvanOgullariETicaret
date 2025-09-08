using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminUrunAnaKategoriFotografController : AdminBaseController
    {
        [Obsolete]
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnvironment;

        [Obsolete]
        public AdminUrunAnaKategoriFotografController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
        {

            _HostEnvironment = hostEnvironment;

        }

        public IActionResult Index()
        {
            List<string>join=new List<string>();
            join.Add("UrunAnaKategori");
            UrunAnaKategoriFotografRepository repository = new UrunAnaKategoriFotografRepository();
            List<UrunAnaKategoriFotograf> modelListesi = repository.GetirList(x => x.Durumu == 1,join).ToList();
            ViewBag.FotografList = modelListesi;
            return View();
        }

        [Obsolete]
        public IActionResult Sil(int Id)
        {
            string pathBig = string.Empty;

            string pathSmall = string.Empty;

            UrunAnaKategoriFotograf anaSayfaFotograf = new UrunAnaKategoriFotograf();
            UrunAnaKategoriFotografRepository repository = new UrunAnaKategoriFotografRepository();
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
            return RedirectToAction("Index", "AdminUrunAnaKategoriFotograf");
        }
    }
}
