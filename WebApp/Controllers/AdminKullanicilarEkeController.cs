using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminKullanicilarEkeController : AdminBaseController
    {
        public IActionResult Index()
        {
          
            return View();
        }
        [HttpPost]
        public IActionResult Kaydet(Kullanicilar kullanicilar)
        {
         
                KullanicilarRepository BilgileriRepository = new KullanicilarRepository();
                kullanicilar.EklenmeTarihi = DateTime.Now;
                kullanicilar.GuncellenmeTarihi = DateTime.Now;
                kullanicilar.Durumu = 1;
                BilgileriRepository.Ekle(kullanicilar);
                return RedirectToAction("Index", "AdminKullanicilar");
           
        }
    }
}
