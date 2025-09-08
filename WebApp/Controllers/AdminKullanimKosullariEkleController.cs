using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminKullanimKosullariEkleController : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Kaydet(KullanimKosullari hakkimizdaBilgileri)
        {
          
                KullanimKosullariRepository Repository = new KullanimKosullariRepository();
                hakkimizdaBilgileri.EklenmeTarihi = DateTime.Now;
                hakkimizdaBilgileri.GuncellenmeTarihi = DateTime.Now;
                hakkimizdaBilgileri.Durumu = 1;
                Repository.Ekle(hakkimizdaBilgileri);
                return RedirectToAction("Index", "AdminKullanimKosullari");
         
        }
    }
}
