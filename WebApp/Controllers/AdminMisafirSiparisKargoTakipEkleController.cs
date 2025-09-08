using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminMisafirSiparisKargoTakipEkleController : AdminBaseController
    {
        public IActionResult Index()
        {
            int? id = HttpContext.Session.GetInt32("SiparisGuestId");
SiparisGuest siparis=new SiparisGuest();
            SiparisGuestRepository siparisRepository =new SiparisGuestRepository();
            siparis = siparisRepository.Getir(Convert.ToInt32(id));
            ViewBag.SiparisId = siparis.Id;

            return View();
        }
        public IActionResult Kaydet(MisafirSiparisKargoTakip siparisKargoTakip)
        {
            if (siparisKargoTakip != null)
            {
                siparisKargoTakip.GuncellenmeTarihi = DateTime.Now;
                siparisKargoTakip.EklenmeTarihi = DateTime.Now;
                siparisKargoTakip.Durumu = 1;
                MisafirSiparisKargoTakipRepository siparisKargoTakipRepository = new MisafirSiparisKargoTakipRepository();
                siparisKargoTakipRepository.Ekle(siparisKargoTakip);
                HttpContext.Session.Remove("SiparisId");
            }
            return RedirectToAction("Index","AdminMisafirSiparisKargoTakip");
        }


    }
}
