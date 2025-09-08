using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminUrunTipBilgileriEkleController : AdminBaseController
    {
        public IActionResult Index()
        {
            UrunAnaKategoriRepository repository = new UrunAnaKategoriRepository();
            List<UrunAnaKategori> UrunAnaKategoriListesi = new List<UrunAnaKategori>();
            UrunAnaKategoriListesi = repository.GetirList(x => x.Durumu == 1).ToList();
            ViewBag.UrunAnaKategoriListesi = UrunAnaKategoriListesi;
            return View();
        }
        [HttpPost]
        public IActionResult Kaydet(UrunAltKategori urun)
        {
          
                UrunAltKategoriRepository BilgileriRepository = new UrunAltKategoriRepository();
                urun.EklenmeTarihi = DateTime.Now;
                urun.GuncellenmeTarihi = DateTime.Now;
                urun.Durumu = 1;
                BilgileriRepository.Ekle(urun);
                return RedirectToAction("Index", "AdminUrunTipBilgileri");
   
        }
    }
}
