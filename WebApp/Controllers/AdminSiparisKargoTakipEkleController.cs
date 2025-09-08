using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminSiparisKargoTakipEkleController : AdminBaseController
    {
        public IActionResult Index()
        {
            int? id = HttpContext.Session.GetInt32("SiparisId");
            Siparis siparis=new Siparis();
            SiparisRepository siparisRepository=new SiparisRepository();
            siparis = siparisRepository.Getir(Convert.ToInt32(id));
            ViewBag.SiparisId = siparis.Id;
            ViewBag.UyelerId=siparis.UyelerId;

            return View();
        }
        public IActionResult Kaydet(SiparisKargoTakip siparisKargoTakip)
        {
            if (siparisKargoTakip != null)
            {
                siparisKargoTakip.GuncellenmeTarihi = DateTime.Now;
                siparisKargoTakip.EklenmeTarihi = DateTime.Now;
                siparisKargoTakip.Durumu = 1;
                SiparisKargoTakipRepository siparisKargoTakipRepository = new SiparisKargoTakipRepository();
                siparisKargoTakipRepository.Ekle(siparisKargoTakip);
                HttpContext.Session.Remove("SiparisId");
            }
            return RedirectToAction("Index","AdminSiparisKargoTakip");
        }


    }
}
