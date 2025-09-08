using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminBirimEkleController : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Kaydet(Birimler birimler)
        {

                BirimlerRepository birimlerRepository = new BirimlerRepository();
                birimler.EklenmeTarihi = DateTime.Now;
                birimler.GuncellenmeTarihi = DateTime.Now;
                birimler.Durumu = 1;
                birimlerRepository.Ekle(birimler);
                return RedirectToAction("Index", "AdminBirim");
            }
          
    }
}
