using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminHakkimizdaBilgileriEkleController : AdminBaseController
    {

        public IActionResult Index()
        {
            return View();
        }


            [HttpPost]
            public IActionResult Kaydet(HakkimizdaBilgileri hakkimizdaBilgileri)
            {

                HakkimizdaBilgileriRepository Repository = new HakkimizdaBilgileriRepository();
                hakkimizdaBilgileri.EklenmeTarihi = DateTime.Now;
                    hakkimizdaBilgileri.GuncellenmeTarihi = DateTime.Now;
                    hakkimizdaBilgileri.Durumu = 1;
                    Repository.Ekle(hakkimizdaBilgileri);
                    return RedirectToAction("Index","AdminHakkimizdaBilgileri");
             
            }
        }
    }



