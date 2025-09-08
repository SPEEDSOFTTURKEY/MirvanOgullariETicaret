using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminIndirimKoduEkleController : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Kaydet(IndirimKodu ındirimKodu)
        {
           
                IndirimKoduRepository ındirimKoduRepository = new IndirimKoduRepository();
           
            ındirimKodu.EklenmeTarihi = DateTime.Now;
            ındirimKodu.GuncellenmeTarihi = DateTime.Now;
            ındirimKodu.Durumu = 1;
            ındirimKoduRepository.Ekle(ındirimKodu);
                return RedirectToAction("Index", "AdminIndırımKodu");
         
           

        }
    }
}
