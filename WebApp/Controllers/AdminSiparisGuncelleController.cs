using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;


namespace WebApp.Controllers
{
    public class AdminSiparisGuncelleController : AdminBaseController
    {
        public IActionResult Index()
        {
            List<SiparisDurumlari> siparisDurumlaris = new List<SiparisDurumlari>();
            SiparisDurumlarıRepository siparisDurumlarıRepository = new SiparisDurumlarıRepository();
            siparisDurumlaris = siparisDurumlarıRepository.GetirList(x => x.Adi != null);
            ViewBag.Durum = siparisDurumlaris;
            Siparis guncellemeBilgisi = HttpContext.Session.GetObjectFromJson<Siparis>("Siparis");

            Uyeler uye = new Uyeler();
            UyelerRepository uyelerRepository = new UyelerRepository();
            uye = uyelerRepository.Getir(x => x.Durumu == 1 && x.Id == guncellemeBilgisi.UyelerId);
            ViewBag.Uye = uye;
            List<Uyeler> uyeler = new List<Uyeler>();
            uyeler = uyelerRepository.GetirList(x => x.Durumu == 1);
            ViewBag.Uyeler = uyeler;

            // ViewBag'e aktar
            ViewBag.Siparis = guncellemeBilgisi;

            return View();
        }
        public IActionResult Kaydet(Siparis siparis)
        {

            SiparisRepository siparisRepository = new SiparisRepository();
            Siparis existingEntity = siparisRepository.Getir(siparis.Id);
            if (existingEntity != null)
            {
                existingEntity.MusteriAdiSoyadi = siparis.MusteriAdiSoyadi;
                existingEntity.DurumId = siparis.DurumId;
                existingEntity.MusteriAdres = siparis.MusteriAdres;
                existingEntity.MusteriTelefon = siparis.MusteriTelefon;
                existingEntity.MusteriVergiNumarasi = siparis.MusteriVergiNumarasi;
                existingEntity.MusteriVergiDairesi = siparis.MusteriVergiDairesi;
                existingEntity.Durumu = 1;
                existingEntity.GuncellenmeTarihi = DateTime.Now;
                existingEntity.UyelerId = siparis.UyelerId;
                existingEntity.MusteriEMail = siparis.MusteriEMail;
                siparisRepository.Guncelle(existingEntity);
            }
            return RedirectToAction("Index", "AdminSiparis");

        }
    }
}
