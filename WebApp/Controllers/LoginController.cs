using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;
using WebApp.Repositories;
namespace WebApp.Controllers
{
    public class LoginController : Controller
    {
        KullanicilarRepository kullanicilarRepository = new KullanicilarRepository();
        [HttpGet]
        public IActionResult Index()
        {

            return View();
        }
        [HttpPost]
        public JsonResult Giris(string KullaniciAdi, string Sifre)
        {
            bool sonuc = false;
            string mesaj = "Kullanıcı Adı ve Şifre Yanlış.";
            KullanicilarRepository kullanicilarRepository = new KullanicilarRepository();
            List<Kullanicilar> kullanicilarListe = kullanicilarRepository.Listele().Where(x => x.Adi == KullaniciAdi && x.Sifre == Sifre).ToList();
            Kullanicilar kullanici = new Kullanicilar();
            ProgramBilgileriContext programBilgileriContext = new ProgramBilgileriContext();

            if (kullanicilarListe.Count > 0)
            {
                kullanici = kullanicilarListe[0];
                if (kullanici != null)
                {
                    var LisansListesi = programBilgileriContext.Lisans.Where(x => x.Durumu == 1 && x.LisansAnahtari == kullanici.LisansAnahtari).ToList();
                    if (LisansListesi != null && LisansListesi.Count > 0)
                    {
                        Lisans Lisans = LisansListesi.First();
                        DateTime bugun = DateTime.Now.Date;
                        DateTime lisansTarihi = Lisans.BitisTarihi.Value.Date;

                        if (lisansTarihi >= bugun)
                        {
                            int lisansKalanGun = (lisansTarihi - bugun).Days;
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "LisansKalanGun", lisansKalanGun);
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "Kullanici", kullanici);
                            sonuc = true;
                        }
                        else
                        {
                            mesaj = $"Lisansınız {lisansTarihi.Date.ToShortDateString()} tarihi itibari ile sonlanmıştır. Devam etmek için lütfen lisans satın alınız.";
                        }
                    }
                    else
                    {
                        mesaj = "Aktif bir lisans bulunamadı. Lütfen lisans satın alınız.";
                    }
                }
            }



            return Json(new { sonuc = sonuc, mesaj = mesaj });
        }


        public IActionResult Cikis()
        {
            foreach (var item in HttpContext.Session.Keys)
            {
                HttpContext.Session.Remove(item);
            }
            return RedirectToAction("Index", "Login");
        }

    }
}


