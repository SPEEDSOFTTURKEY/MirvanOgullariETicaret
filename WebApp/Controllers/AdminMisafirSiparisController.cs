using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminMisafirSiparisController : AdminBaseController
    {
        SiparisGuestRepository repository = new SiparisGuestRepository();
        GuestSepetRepository sepetRepository = new GuestSepetRepository();
        public IActionResult Index()
        {
            // Session'dan arama sonucu varsa onu kullan
            var sessionList = HttpContext.Session.GetObjectFromJson<List<SiparisGuest>>("SiparisListesi");

            if (sessionList != null && sessionList.Any())
            {
                ViewBag.SiparisList = sessionList;
                HttpContext.Session.Remove("SiparisListesi"); // sadece bir defa gösterim için
            }
            else
            {
                // Normal sipariş listesini getir
                List<string> list = new List<string> { "Durum", "GuestSepet" };
                SiparisGuestRepository guestRepository = new SiparisGuestRepository();
                var guest = guestRepository
                            .GetirList(x => x.Durumu == 1 && x.SiparisKodu != null, list)
                            .OrderByDescending(x => x.EklenmeTarihi)
                            .ToList();

                ViewBag.SiparisList = guest;
            }

            return View();
        }

        public IActionResult Search(string adi)
        {
            HttpContext.Session.Remove("SiparisListesi");
            List<SiparisGuest> list = new List<SiparisGuest>();
            SiparisGuestRepository siparisRepository = new SiparisGuestRepository();
            list = siparisRepository.GetirList(x => x.SiparisKodu != null && x.MusteriAdiSoyadi.Contains(adi), new List<string>() { "GuestSepet" }).OrderByDescending(x => x.Id).ToList();
            List<GuestSepet> sepets = new List<GuestSepet>();
            foreach (SiparisGuest siparis in list)
            {
                sepets = new List<GuestSepet>();
                sepets = sepetRepository.GetirList(x => x.Durumu == 1 && x.SiparisGuestId == siparis.Id, new List<string>() { "Birim" }).ToList();
                siparis.GuestSepet = sepets;
            }

            HttpContext.Session.SetObjectAsJson("SiparisListesi", list);


            return RedirectToAction("Index", "AdminMisafirSiparis");
        }
        public IActionResult Guncelleme(int id)
        {List<string> list = new List<string>();
            list.Add("Durum");
            list.Add("GuestSepet");
            SiparisGuest siparis = repository.Getir(x=> x.Id==id,list);
            if (siparis != null)
            {
                HttpContext.Session.SetObjectAsJson("Siparis", siparis);
            }
            return RedirectToAction("Index", "AdminMisafirSiparisGuncelle");
        }
        public IActionResult Sil(int id)
        {
            SiparisGuest siparis = repository.Getir(id);
            if (siparis != null)
            {
                siparis.Durumu = 0;
                siparis.GuncellenmeTarihi = DateTime.Now;
                repository.Guncelle(siparis);
            }
            return RedirectToAction("Index", "AdminMisafirSiparis");
        }
        [HttpPost]
        public IActionResult Sepet(int id)
        {
            List<GuestSepet> sepet = sepetRepository.GetirList(x => x.Durumu == 1 && x.SiparisGuestId == id);

            if (sepet != null)
            {
                HttpContext.Session.SetObjectAsJson("SepetGuncelle", sepet);

                ViewBag.Sepet = sepet;
                // Sepet bilgilerini döneriz
                return Json(new { success = true, sepet = sepet });
            }
            return Json(new { success = false, message = "Sepet bulunamadı!" });
        }
        public IActionResult Detay(string id)
        {
            HttpContext.Session.Remove("SiparisSepetDetayId");
            HttpContext.Session.SetString("SiparisSepetDetayId", id);
            return RedirectToAction("Index", "AdminMisafirSiparisSepetDetay");
        }

        public IActionResult GetAllData()
        {

            HttpContext.Session.Remove("SiparisListesi");
            return RedirectToAction("Index", "AdminMisafirSiparis");
        }
        public IActionResult KargoBilgileriEkle(int id)
        {
            HttpContext.Session.SetInt32("SiparisGuestId", id);




            return RedirectToAction("Index", "AdminMisafirSiparisKargoTakipEkle");
        }
    }
}
