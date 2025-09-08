using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminSiparisController : AdminBaseController
    {
        SiparisRepository repository = new SiparisRepository();
        SepetRepository sepetRepository = new SepetRepository();
        public IActionResult Index()
		{
			
			List<Siparis> list = new List<Siparis>();

			List<string> joinTables = new List<string>();
			joinTables.Add("Uyeler");
			joinTables.Add("Durum");
            //joinTables.Add("Sepets");
            list = repository.GetirList(x => x.Durumu == 1);
            ViewBag.SiparisList = list;

			return View();
		}
        public IActionResult Guncelleme(int id)
        {
            List<string> joinTables = new List<string>();
            joinTables.Add("Uyeler");
            joinTables.Add("Durum");
            Siparis siparis = repository.Getir(x=> x.Id==id ,joinTables);
            if (siparis != null)
            {
                HttpContext.Session.SetObjectAsJson("Siparis", siparis);
            }
            return RedirectToAction("Index", "AdminSiparisGuncelle");
        }
        public IActionResult Sil(int id)
        {
            Siparis siparis = repository.Getir(id);
            if (siparis != null)
            {
                siparis.Durumu = 0;
                siparis.GuncellenmeTarihi = DateTime.Now;
                repository.Guncelle(siparis);
            }
            return RedirectToAction("Index", "AdminSiparis");
        }
        [HttpPost]
        public IActionResult Sepet(int id)
        {
            List<Sepet> sepet = sepetRepository.GetirList(x => x.Durumu == 1 && x.SiparisId == id);
          
            if (sepet != null)
            {
                HttpContext.Session.SetObjectAsJson("SepetGuncelle", sepet);

                ViewBag.Sepet = sepet;
                // Sepet bilgilerini döneriz
                return Json(new { success = true, sepet = sepet });
            }
            return Json(new { success = false, message = "Sepet bulunamadı!" });
        }
        public IActionResult Detay(int id)
        {
            HttpContext.Session.Remove("SiparisSepetDetayId");
            HttpContext.Session.SetInt32("SiparisSepetDetayId", id);
            return RedirectToAction("Index", "AdminSiparisSepetDetay");
        }
        public IActionResult Search(string adi)
        {
            HttpContext.Session.Remove("SiparisListesi");
            List<Siparis> list = new List<Siparis>();
            SiparisRepository siparisRepository = new SiparisRepository();
            list = siparisRepository.GetirList(x => x.Durumu == 1&&x.MusteriAdiSoyadi.Contains(adi), new List<string>() { "Uyeler", "Sepets" }).OrderByDescending(x => x.Id).ToList();
            List<Sepet> sepets = new List<Sepet>();
            foreach (Siparis siparis in list)
            {
                sepets = new List<Sepet>();
                sepets = sepetRepository.GetirList(x => x.Durumu == 1 && x.SiparisId == siparis.Id, new List<string>() { "Uye", "Birim" }).ToList();
                siparis.Sepets = sepets;
            }
           
            HttpContext.Session.SetObjectAsJson("SiparisListesi", list);


            return RedirectToAction("Index", "AdminSiparis");
        }
        public IActionResult GetAllData()
        {

            HttpContext.Session.Remove("SiparisListesi");
            return RedirectToAction("Index", "AdminSiparis");
        }
        public IActionResult KargoBilgileriEkle(int id)
        {
            HttpContext.Session.SetInt32("SiparisId",id);




            return RedirectToAction("Index", "AdminSiparisKargoTakipEkle");
        }


    }
}
