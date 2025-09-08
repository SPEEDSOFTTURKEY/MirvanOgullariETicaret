using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminUyelerEkleController : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Kaydet(Uyeler uyeler)
        {
            if (uyeler != null)
            {
                UyelerRepository BilgileriRepository = new UyelerRepository();
                uyeler.EklenmeTarihi = DateTime.Now;
                uyeler.GuncellenmeTarihi = DateTime.Now;
                uyeler.Durumu = 1;
                BilgileriRepository.Ekle(uyeler);
                return RedirectToAction("Index", "AdminUyeler");
            }
            return View("Index", uyeler);
        }
    }
}
