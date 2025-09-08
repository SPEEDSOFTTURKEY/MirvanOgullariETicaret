using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class EditAddressController : BaseController
	{
		public async Task<IActionResult> Index()
        {
            await LoadCommonData();


            Uyeler uyeler = new Uyeler();
            uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");
            if (uyeler != null)
            {
                Uyeler uyeler1 = new Uyeler();
                UyelerRepository uyelerRepository = new UyelerRepository();
                uyeler1 = uyelerRepository.Getir(uyeler.Id);
                ViewBag.Uyeler = uyeler1;
                UyeAdres uyeAdres = new UyeAdres();
                UyeAdresRepository uyeAdresRepository = new UyeAdresRepository();
                uyeAdres = uyeAdresRepository.Getir(x => x.UyeId == uyeler1.Id);
                ViewBag.Adres = uyeAdres;
            }
            else
            {
                ViewBag.Uyeler = null;
            }
            return View();
		}
        public async Task<IActionResult> AdressUpdate(UyeAdres model)
        {
            await LoadCommonData();

            UyeAdres uyeAdres = new UyeAdres();
            UyeAdresRepository uyeAdresRepository = new UyeAdresRepository();
            uyeAdres = uyeAdresRepository.Getir(model.Id);
            uyeAdres.Adi = model.Adi;
            uyeAdres.Soyadi = model.Soyadi;
            uyeAdres.Telefon = model.Telefon;
            uyeAdres.PostaKodu = model.PostaKodu;
            uyeAdres.Ulke=model.Ulke;
            uyeAdres.il=model.il;
            uyeAdres.ilce=model.ilce;
            uyeAdres.Mahalle=model.Mahalle;
            uyeAdres.Adres=model.Adres;
            uyeAdres.AdresBasligi=model.AdresBasligi;
            uyeAdres.FaturaTuru = model.FaturaTuru;
            uyeAdres.Varsayılan = model.Varsayılan;
            uyeAdres.GuncellenmeTarihi=DateTime.Now;
            uyeAdresRepository.Guncelle(uyeAdres);
            return RedirectToAction("Index","Address");

        }
}
}
