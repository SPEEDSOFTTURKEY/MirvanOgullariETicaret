using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminAnaSayfaRakamlariEkleController : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Kaydet(AnaSayfaRakamlari rakam)
        {
           
                AnaSayfaRakamlariRepository repository = new Repositories.AnaSayfaRakamlariRepository();
                rakam.EklenmeTarihi = DateTime.Now;
                rakam.GuncellenmeTarihi = DateTime.Now;
                rakam.Durumu = 1;
                repository.Ekle(rakam);
                return RedirectToAction("Index", "AdminAnaSayfaRakamlari");
        
        }
    }
}
