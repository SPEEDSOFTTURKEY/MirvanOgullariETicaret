using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminAcilKargoUcretEkleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Kaydet(AcilKargoUcret kargoUcret)
        {

            AcilKargoUcretRepository kargoUcretRepo = new AcilKargoUcretRepository();
            kargoUcret.EklenmeTarihi = DateTime.Now;
            kargoUcret.GuncellenmeTarihi = DateTime.Now;
            kargoUcret.Durumu = 1;
            kargoUcretRepo.Ekle(kargoUcret);
            return RedirectToAction("Index", "AdminAcilKargoUcret");

        }
    }
}
