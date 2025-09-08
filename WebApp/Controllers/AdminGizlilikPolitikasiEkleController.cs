using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminGizlilikPolitikasiEkleController : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Kaydet(GizlilikPolitikasi hakkimizdaBilgileri)
        {
            
                GizlilikPolitikasiRepository Repository = new GizlilikPolitikasiRepository();
                hakkimizdaBilgileri.EklenmeTarihi = DateTime.Now;
                hakkimizdaBilgileri.GuncellenmeTarihi = DateTime.Now;
                hakkimizdaBilgileri.Durumu = 1;
                Repository.Ekle(hakkimizdaBilgileri);
                return RedirectToAction("Index", "AdminGizlilikPolitikasi");
       
        }
    }
}

