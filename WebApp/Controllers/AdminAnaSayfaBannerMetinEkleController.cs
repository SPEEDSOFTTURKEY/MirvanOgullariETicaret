using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminAnaSayfaBannerMetinEkleController : AdminBaseController
    {

        public IActionResult Index()
        {
            return View();
        }


            [HttpPost]
            public IActionResult Kaydet(AnaSayfaBannerMetin hakkimizdaBilgileri)
            {
              
                AnaSayfaBannerMetinRepository Repository = new AnaSayfaBannerMetinRepository();
                hakkimizdaBilgileri.EklenmeTarihi = DateTime.Now;
                    hakkimizdaBilgileri.GuncellenmeTarihi = DateTime.Now;
                    hakkimizdaBilgileri.Durumu = 1;
                    Repository.Ekle(hakkimizdaBilgileri);
                    return RedirectToAction("Index", "AdminAnaSayfaBannerMetin");
                
            }
        }
    }



