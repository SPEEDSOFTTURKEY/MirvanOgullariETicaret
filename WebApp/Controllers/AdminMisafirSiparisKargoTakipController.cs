using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminMisafirSiparisKargoTakipController : AdminBaseController
    {
        public IActionResult Index()
        {
            List<MisafirSiparisKargoTakip> siparisKargoTakip = new List<MisafirSiparisKargoTakip>();
            siparisKargoTakip = HttpContext.Session.GetObjectFromJsonCollection<MisafirSiparisKargoTakip>("KargoListesi");
            if(siparisKargoTakip!=null)
            {
                ViewBag.Kargo = siparisKargoTakip;
            }
            else
            {
                MisafirSiparisKargoTakipRepository siparisKargoTakipRepository = new MisafirSiparisKargoTakipRepository();
                siparisKargoTakip = siparisKargoTakipRepository.GetirList(x => x.Durumu == 1, new List<string>() {"SiparisGuest" }).OrderByDescending(x => x.Id).ToList();
                ViewBag.Kargo = siparisKargoTakip;
            }



 
            return View();
        }
        public IActionResult Search(string adi)
        {
            HttpContext.Session.Remove("KargoListesi");
            List<MisafirSiparisKargoTakip> siparisKargoTakip = new List<MisafirSiparisKargoTakip>();
            MisafirSiparisKargoTakipRepository siparisKargoTakipRepository = new MisafirSiparisKargoTakipRepository();
            siparisKargoTakip = siparisKargoTakipRepository.GetirList(x => x.Durumu == 1&&x.KargoTakipNumarasi.Contains(adi), new List<string>() {  "SiparisGuest" }).OrderByDescending(x => x.Id).ToList();
            HttpContext.Session.SetObjectAsJson("KargoListesi", siparisKargoTakip);
            return RedirectToAction("Index", "AdminMisafirSiparisKargoTakip");
        }
        public IActionResult GetAllData()
        {

            HttpContext.Session.Remove("KargoListesi");
            return RedirectToAction("Index", "AdminSiparisKargoTakip");
        }
        public IActionResult Guncelleme(int id)
        {
            MisafirSiparisKargoTakipRepository siparisKargoTakipRepository = new MisafirSiparisKargoTakipRepository();
            MisafirSiparisKargoTakip siparisKargoTakip = new MisafirSiparisKargoTakip();
            siparisKargoTakip=siparisKargoTakipRepository.Getir(x => x.Id == id);
            if (siparisKargoTakip != null)
            {
                HttpContext.Session.SetObjectAsJson("MisafirSiparisKargoTakip", siparisKargoTakip);
            }
            return RedirectToAction("Index", "AdminMisafirSiparisKargoTakipGuncelle");
        }
        public IActionResult Sil(int id)
        {
            HttpContext.Session.Remove("KargoListesi");
            MisafirSiparisKargoTakipRepository siparisKargoTakipRepository = new MisafirSiparisKargoTakipRepository();
            MisafirSiparisKargoTakip siparisKargoTakip =new MisafirSiparisKargoTakip();
            siparisKargoTakip = siparisKargoTakipRepository.Getir(id);
            if (siparisKargoTakip != null)
            {
                siparisKargoTakip.Durumu = 0;
                siparisKargoTakip.GuncellenmeTarihi = DateTime.Now;
                siparisKargoTakipRepository.Guncelle(siparisKargoTakip);
            }
            return RedirectToAction("Index", "AdminMisafirSiparisKargoTakip");
        }
    }
}
