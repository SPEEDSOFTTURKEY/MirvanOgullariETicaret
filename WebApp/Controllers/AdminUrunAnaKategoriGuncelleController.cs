using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminUrunAnaKategoriGuncelleController : AdminBaseController
    {
        public IActionResult Index()
        {
            UrunAnaKategori guncellemeBilgisi = HttpContext.Session.GetObjectFromJson<UrunAnaKategori>("UrunAnaKategori");
            ViewBag.UrunAnaKategori = guncellemeBilgisi;
            return View();
        }
        public IActionResult Kaydet(UrunAnaKategori urunTip)
        {
           UrunAnaKategoriRepository repository = new UrunAnaKategoriRepository();
                UrunAnaKategori existingEntity = repository.Getir(urunTip.Id);
                if (existingEntity != null)
                {
                    existingEntity.Adi = urunTip.Adi;
                    existingEntity.GuncellenmeTarihi = DateTime.Now;
                    repository.Guncelle(existingEntity);
                }
                return RedirectToAction("Index", "AdminUrunAnaKategori");
          
        }
    }
}
