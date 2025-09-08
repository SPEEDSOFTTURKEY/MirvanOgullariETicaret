using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminSiparisEkleController : AdminBaseController
    {
        public IActionResult Index()
        {
            List<SiparisDurumlari> siparisDurumlaris = new List<SiparisDurumlari>();
            SiparisDurumlarıRepository siparisDurumlarıRepository = new SiparisDurumlarıRepository();
            siparisDurumlaris = siparisDurumlarıRepository.GetirList(x => x.Adi != null);
            ViewBag.Durum = siparisDurumlaris;
            List<Uyeler> uyeler = new List<Uyeler>();
            UyelerRepository uyelerRepository = new UyelerRepository();
            uyeler = uyelerRepository.GetirList(x => x.Durumu == 1);
            ViewBag.Uyeler = uyeler;
            return View();
        }

        [HttpPost]
        public IActionResult Kaydet(Siparis siparis)
        {


            siparis.Durumu = 1;
            siparis.GuncellenmeTarihi = DateTime.Now;
            siparis.EklenmeTarihi = DateTime.Now;
            SiparisRepository siparisRepository = new SiparisRepository();
            siparisRepository.Ekle(siparis);
            return RedirectToAction("Index", "AdminSiparis");


        }
    }
}
