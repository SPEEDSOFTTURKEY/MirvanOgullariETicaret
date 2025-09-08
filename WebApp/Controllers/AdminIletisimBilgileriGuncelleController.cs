using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminIletisimBilgileriGuncelleController : AdminBaseController
    {
        public IActionResult Index()
        {
            IletisimBilgileri guncellemeBilgisi = HttpContext.Session.GetObjectFromJson<IletisimBilgileri>("IletisimBilgileri");
            ViewBag.IletisimBilgileri = guncellemeBilgisi;
            return View();
        }
        public IActionResult Kaydet(IletisimBilgileri iletisimBilgileri)
        {
            IletisimBilgileriRepository repository = new IletisimBilgileriRepository();
                IletisimBilgileri existingEntity = repository.Getir(iletisimBilgileri.Id);
                if (existingEntity != null)
                {
                    existingEntity.Adi = iletisimBilgileri.Adi;
                    existingEntity.Email1 = iletisimBilgileri.Email1;
                    existingEntity.YouTube = iletisimBilgileri.YouTube;
                    existingEntity.Durumu = 1;
                    existingEntity.BankaAdi= iletisimBilgileri.BankaAdi;
                    existingEntity.EklenmeTarihi = iletisimBilgileri.EklenmeTarihi;
                    existingEntity.Email2 = iletisimBilgileri.Email2;
                    existingEntity.Facebook = iletisimBilgileri.Facebook;
                    existingEntity.Faks= iletisimBilgileri.Faks;
                    existingEntity.GoogleMapsBaglanti= iletisimBilgileri.GoogleMapsBaglanti;
                    existingEntity.GuncellenmeTarihi = DateTime.Now;
                    existingEntity.IbanNo= iletisimBilgileri.IbanNo;
                    existingEntity.Instagram= iletisimBilgileri.Instagram;
                    existingEntity.Linkedin = iletisimBilgileri.Linkedin;
                    existingEntity.Telefon1= iletisimBilgileri.Telefon1;
                    existingEntity.Telefon2 = iletisimBilgileri.Telefon2;
                    existingEntity.Telefon3 = iletisimBilgileri.Telefon3;
                    existingEntity.Telefon4 = iletisimBilgileri.Telefon4;
                    existingEntity.Twitter= iletisimBilgileri.Twitter;
                    existingEntity.WhatsApp= iletisimBilgileri.WhatsApp;
                    existingEntity.YouTube=iletisimBilgileri.YouTube;
                    existingEntity.GuncellenmeTarihi = DateTime.Now;
                    existingEntity.Adres= iletisimBilgileri.Adres;
                    repository.Guncelle(existingEntity);
                }
                return RedirectToAction("Index", "AdminIletisimBilgileri");
   
        }
    }
}
