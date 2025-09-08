using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminUrunAnaKategoriEkleController : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Kaydet(UrunAnaKategori urun)
        {
           
                UrunAnaKategoriRepository BilgileriRepository = new UrunAnaKategoriRepository();
                urun.EklenmeTarihi = DateTime.Now;
                urun.GuncellenmeTarihi = DateTime.Now;
                urun.Durumu = 1;
                BilgileriRepository.Ekle(urun);
                return RedirectToAction("Index", "AdminUrunAnaKategori");
      
        }
    }
}
