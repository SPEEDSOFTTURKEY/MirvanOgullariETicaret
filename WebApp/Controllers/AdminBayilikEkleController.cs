using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminBayilikEkleController : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Kaydet(Bayilik hakkimizdaBilgileri)
        {

            BayilikRepository Repository = new BayilikRepository();
            hakkimizdaBilgileri.EklenmeTarihi = DateTime.Now;
            hakkimizdaBilgileri.GuncellenmeTarihi = DateTime.Now;
            hakkimizdaBilgileri.Durumu = 1;
            Repository.Ekle(hakkimizdaBilgileri);
            return RedirectToAction("Index", "AdminBayilik");

        }
    }
}
