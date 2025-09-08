using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminUyelikSozlesmesiEkleController : AdminBaseController
    {

        public IActionResult Index()
        {
            return View();
        }


            [HttpPost]
            public IActionResult Kaydet(UyelikSozlesmesi hakkimizdaBilgileri)
            {
             
                UyelikSozlesmesiRepository Repository = new UyelikSozlesmesiRepository();
                hakkimizdaBilgileri.EklenmeTarihi = DateTime.Now;
                    hakkimizdaBilgileri.GuncellenmeTarihi = DateTime.Now;
                    hakkimizdaBilgileri.Durumu = 1;
                    Repository.Ekle(hakkimizdaBilgileri);
                    return RedirectToAction("Index", "AdminUyelikSozlesmesi");
            
            }
        }
    }



