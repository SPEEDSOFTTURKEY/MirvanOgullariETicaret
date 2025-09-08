using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminSiparisKargoTakipController : AdminBaseController
    {
        public IActionResult Index()
        {
            List<SiparisKargoTakip> siparisKargoTakip = new List<SiparisKargoTakip>();
            siparisKargoTakip = HttpContext.Session.GetObjectFromJsonCollection<SiparisKargoTakip>("KargoListesi");
            if(siparisKargoTakip!=null)
            {
                ViewBag.Kargo = siparisKargoTakip;
            }
            else
            {
                SiparisKargoTakipRepository siparisKargoTakipRepository = new SiparisKargoTakipRepository();
                siparisKargoTakip = siparisKargoTakipRepository.GetirList(x => x.Durumu == 1, new List<string>() { "Uyeler", "Siparis" }).OrderByDescending(x => x.Id).ToList();
                ViewBag.Kargo = siparisKargoTakip;
            }



 
            return View();
        }
        public IActionResult Search(string adi)
        {
            HttpContext.Session.Remove("KargoListesi");
            List<SiparisKargoTakip> siparisKargoTakip = new List<SiparisKargoTakip>();
            SiparisKargoTakipRepository siparisKargoTakipRepository = new SiparisKargoTakipRepository();
            siparisKargoTakip = siparisKargoTakipRepository.GetirList(x => x.Durumu == 1&&x.KargoTakipNumarasi.Contains(adi), new List<string>() { "Uyeler", "Siparis" }).OrderByDescending(x => x.Id).ToList();
            HttpContext.Session.SetObjectAsJson("KargoListesi", siparisKargoTakip);
            return RedirectToAction("Index", "AdminSiparisKargoTakip");
        }
        public IActionResult GetAllData()
        {

            HttpContext.Session.Remove("KargoListesi");
            return RedirectToAction("Index", "AdminSiparisKargoTakip");
        }
        public IActionResult Guncelleme(int id)
        {
            SiparisKargoTakipRepository siparisKargoTakipRepository = new SiparisKargoTakipRepository();
            SiparisKargoTakip siparisKargoTakip = new SiparisKargoTakip();
            siparisKargoTakip=siparisKargoTakipRepository.Getir(x => x.Id == id);
            if (siparisKargoTakip != null)
            {
                HttpContext.Session.SetObjectAsJson("SiparisKargoTakip", siparisKargoTakip);
            }
            return RedirectToAction("Index", "AdminSiparisKargoTakipGuncelle");
        }
        public IActionResult Sil(int id)
        {
            HttpContext.Session.Remove("KargoListesi");
            SiparisKargoTakipRepository siparisKargoTakipRepository = new SiparisKargoTakipRepository();
            SiparisKargoTakip siparisKargoTakip=new SiparisKargoTakip();
            siparisKargoTakip = siparisKargoTakipRepository.Getir(id);
            if (siparisKargoTakip != null)
            {
                siparisKargoTakip.Durumu = 0;
                siparisKargoTakip.GuncellenmeTarihi = DateTime.Now;
                siparisKargoTakipRepository.Guncelle(siparisKargoTakip);
            }
            return RedirectToAction("Index", "AdminSiparisKargoTakip");
        }
    }
}
