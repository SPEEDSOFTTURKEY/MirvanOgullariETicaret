using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminSepetGuncelleController : AdminBaseController
    {
       
        public IActionResult Index()
        {
            List<Sepet> sepet = HttpContext.Session.GetObjectFromJson<List<Sepet>>("SepetGuncelle");

            ViewBag.Sepet = sepet;
            List<Urun> urun = new List<Urun>();    
            UrunRepository urunRepository = new UrunRepository();
            urun = urunRepository.GetirList(x => x.Durumu == 1);
            ViewBag.Urun=urun;
                return View();
        }
        public IActionResult Kaydet(Sepet sepet)
        {
            Urun urun = new Urun();
            UrunRepository urunRepository=new UrunRepository();
            urun=urunRepository.Getir(x => x.Durumu == 1&& x.Id==sepet.UrunId);
            SepetRepository sepetRepository = new SepetRepository();
            Sepet existingEntity = sepetRepository.Getir(sepet.Id);
            if (existingEntity != null)
            {
                existingEntity.UrunId = urun.Id;
                existingEntity.UrunAdi=urun.Adi;
                existingEntity.Miktar = sepet.Miktar;
                existingEntity.Birim = sepet.Birim;
                existingEntity.Fiyat = sepet.Fiyat;
                existingEntity.Toplam = sepet.Toplam;
                existingEntity.EklenmeTarihi = sepet.EklenmeTarihi;
                existingEntity.GuncellenmeTarihi = DateTime.Now;
                sepetRepository.Guncelle(existingEntity);

            }
            return RedirectToAction("Index", "AdminSiparis");
        }
    }
}
