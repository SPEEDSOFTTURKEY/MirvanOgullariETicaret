using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminIletisimBilgileriEkleController : AdminBaseController
    {
    
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Kaydet(IletisimBilgileri İletisimBilgileri)
        {
           
                İletisimBilgileri.Durumu = 1;
                İletisimBilgileri.GuncellenmeTarihi = DateTime.Now;
                İletisimBilgileri.EklenmeTarihi=DateTime.Now;
                IletisimBilgileriRepository IletisimBilgileriRepository = new IletisimBilgileriRepository();
                IletisimBilgileriRepository.Ekle(İletisimBilgileri);
                return RedirectToAction("Index", "AdminIletisimBilgileri");
    
        }
    }
}
