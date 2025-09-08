using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminEDMBilgileriController : AdminBaseController
    {
        EDMBilgileriRepository EDMBilgileriRepository = new EDMBilgileriRepository();
        [HttpGet]
        public IActionResult Index()
        {
            List<EDMBilgileri> EDMBilgileri = new List<EDMBilgileri>();
            EDMBilgileri = EDMBilgileriRepository.Listele().Where(x => x.Durumu == 1).ToList();
            ViewBag.EDMBilgileri = EDMBilgileri;
            return View();
        }

        public IActionResult Guncelleme(int Id)
        {
            EDMBilgileri EDMBilgileri = new EDMBilgileri();
            EDMBilgileri = EDMBilgileriRepository.Getir(Id);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "EDMBilgileri", EDMBilgileri);
            return RedirectToAction("Index", "AdminEDMBilgileriGuncelle");
        }
        public IActionResult Sil(int Id)
        {
            EDMBilgileri EDMBilgileri = new EDMBilgileri();
            EDMBilgileri = EDMBilgileriRepository.Getir(Id);
            EDMBilgileri.Durumu = 0;
            EDMBilgileri.GuncellenmeTarihi = DateTime.Now;
            EDMBilgileriRepository.Guncelle(EDMBilgileri);
            return RedirectToAction("Index", "AdminEDMBilgileri");
        }
    }
}
