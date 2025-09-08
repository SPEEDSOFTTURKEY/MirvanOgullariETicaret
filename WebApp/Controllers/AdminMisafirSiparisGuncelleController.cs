using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;


namespace WebApp.Controllers
{
    public class AdminMisafirSiparisGuncelleController : AdminBaseController
    {
        public IActionResult Index()
        {
            List<SiparisDurumlari> siparisDurumlaris = new List<SiparisDurumlari>();
            SiparisDurumlarıRepository siparisDurumlarıRepository = new SiparisDurumlarıRepository();
            siparisDurumlaris = siparisDurumlarıRepository.GetirList(x => x.Adi != null);
            ViewBag.Durum = siparisDurumlaris;
            SiparisGuest guncellemeBilgisi = HttpContext.Session.GetObjectFromJson<SiparisGuest>("Siparis");
            ViewBag.Siparis = guncellemeBilgisi;

            return View();
        }
        public IActionResult Kaydet(SiparisGuest siparis)
        {
  
                SiparisGuestRepository siparisRepository = new SiparisGuestRepository();
                SiparisGuest existingEntity = siparisRepository.Getir(siparis.Id);
                if (existingEntity != null)
                {
                    existingEntity.MusteriAdiSoyadi = siparis.MusteriAdiSoyadi;
                    existingEntity.MusteriAdres = siparis.MusteriAdres;
                    existingEntity.MusteriTelefon = siparis.MusteriTelefon;
                    existingEntity.MusteriVergiNumarasi = siparis.MusteriVergiNumarasi;
                    existingEntity.MusteriVergiDairesi = siparis.MusteriVergiDairesi;
                existingEntity.DurumId= siparis.DurumId;
                    existingEntity.GuncellenmeTarihi = DateTime.Now;
                    existingEntity.MusteriEmail = siparis.MusteriEmail;
                    siparisRepository.Guncelle(existingEntity);
                }
                return RedirectToAction("Index", "AdminMisafirSiparis");

			}
			}
}
