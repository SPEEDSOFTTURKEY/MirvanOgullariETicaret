using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminUrunTipBilgileriGuncelleController : AdminBaseController
    {
        public IActionResult Index()
        {
            UrunAltKategori guncellemeBilgisi = HttpContext.Session.GetObjectFromJson<UrunAltKategori>("UrunAltKategori");
            ViewBag.UrunTip = guncellemeBilgisi;
            UrunAnaKategori urunAnaKategori = new UrunAnaKategori();
            UrunAnaKategoriRepository urunAnaKategoriRepository =new UrunAnaKategoriRepository();
            urunAnaKategori = urunAnaKategoriRepository.Getir(Convert.ToInt32(guncellemeBilgisi.UrunAnaKategoriId));
            ViewBag.UrunAnaKategoriAdi = urunAnaKategori.Adi;
            ViewBag.UrunAnaKategoriId=urunAnaKategori.Id;
            List<UrunAnaKategori> urunAnaKategoriListesi = new List<UrunAnaKategori>();
            urunAnaKategoriListesi = urunAnaKategoriRepository.GetirList(x => x.Durumu == 1);
            ViewBag.UrunAnaKategoriListesi = urunAnaKategoriListesi;
            return View();
        }
        public IActionResult Kaydet(UrunAltKategori urunTip)
        {
                UrunAltKategoriRepository repository = new UrunAltKategoriRepository();
                UrunAltKategori existingEntity = repository.Getir(urunTip.Id);
                if (existingEntity != null)
                {
                    existingEntity.Adi = urunTip.Adi;
                    existingEntity.UrunAnaKategoriId = urunTip.UrunAnaKategoriId;
                    existingEntity.GuncellenmeTarihi = DateTime.Now;
                    repository.Guncelle(existingEntity);
                }
                return RedirectToAction("Index", "AdminUrunTipBilgileri");
        
        }
    }
}
