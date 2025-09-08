using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminIadeEkleController : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Kaydet(Iade hakkimizdaBilgileri)
        {

            IadeRepository Repository = new IadeRepository();
            hakkimizdaBilgileri.EklenmeTarihi = DateTime.Now;
            hakkimizdaBilgileri.GuncellenmeTarihi = DateTime.Now;
            hakkimizdaBilgileri.Durumu = 1;
            Repository.Ekle(hakkimizdaBilgileri);
            return RedirectToAction("Index", "AdminIade");

        }
    }
}
