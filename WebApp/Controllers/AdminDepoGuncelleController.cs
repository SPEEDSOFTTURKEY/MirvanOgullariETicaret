using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminDepoGuncelleController : AdminBaseController
    {
 
        UrunAnaKategoriRepository urunAnaKategoriRepository = new UrunAnaKategoriRepository();
        UrunAltKategoriRepository urunAltKategoriRepository = new UrunAltKategoriRepository();
        UrunRepository urunRepository = new UrunRepository();
        DepoRepository depoRepository = new DepoRepository();
        public IActionResult Index(int id)
        {
            var depo = depoRepository.Getir(x => x.Id == id && x.Durumu == 1);
            if (depo == null)
                return NotFound();

            ViewBag.UrunAnaKategoriListesi = urunAnaKategoriRepository.GetirList(x => x.Durumu == 1);
            ViewBag.UrunAltKategoriListesi = urunAltKategoriRepository.GetirList(x => x.UrunAnaKategoriId == depo.UrunAnaKategoriId && x.Durumu == 1);
            ViewBag.UrunListesi = urunRepository.GetirList(x => x.UrunAltKategoriId == depo.UrunAltKategoriId && x.Durumu == 1);

            return View(depo);
        }

        [HttpPost]
        public IActionResult Kaydet(Depo model)
        {
     

            var depo = depoRepository.Getir(x => x.Id == model.Id);
            if (depo == null)
                return NotFound();

            depo.UrunAnaKategoriId = model.UrunAnaKategoriId;
            depo.UrunAltKategoriId = model.UrunAltKategoriId;
            depo.UrunId = model.UrunId;
            depo.Stok = model.Stok;
            depo.GuncellenmeTarihi = DateTime.Now;

            depoRepository.Guncelle(depo);
            return RedirectToAction("Index", "AdminDepo");
        }
    }
}