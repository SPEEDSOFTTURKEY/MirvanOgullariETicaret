using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminIndırımKoduController : AdminBaseController
    {
        public IActionResult Index()
        {
            List<IndirimKodu> ındirimKodu = new List<IndirimKodu>();
            IndirimKoduRepository ındirimKoduRepository = new IndirimKoduRepository();
            ındirimKodu = ındirimKoduRepository.GetirList(x => x.Durumu == 1);
            ViewBag.IndirimKodu = ındirimKodu;
            return View();
        }
        public IActionResult Guncelle(int id)
        {
            IndirimKoduRepository ındirimKoduRepository = new IndirimKoduRepository();
            IndirimKodu ındirimKodu = ındirimKoduRepository.Getir(id);
            if (ındirimKodu != null)
            {
                HttpContext.Session.SetObjectAsJson("IndirimKodu", ındirimKodu);

            }
            return RedirectToAction("Index", "AdminIndirimKoduGuncelle");
        }
        public IActionResult Sil(int id)
        {
            IndirimKoduRepository ındirimKoduRepository = new IndirimKoduRepository();
            IndirimKodu ındirimKodu = ındirimKoduRepository.Getir(id);
            if (ındirimKodu != null)
            {
                ındirimKodu.Durumu = 0;
                ındirimKodu.GuncellenmeTarihi = DateTime.Now;
                ındirimKoduRepository.Guncelle(ındirimKodu);

            }
            return RedirectToAction("Index", "AdminIndırımKodu");

        }
    }
}
