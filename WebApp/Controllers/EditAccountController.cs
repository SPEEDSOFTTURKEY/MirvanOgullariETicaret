using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class EditAccountController : BaseController
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
            }
            else
            {
                ViewBag.Uyeler = null;
            }
            return View();
        }

        public async Task<IActionResult> AccountUpdate(Uyeler uyeler)
        {
            await LoadCommonData();

            Uyeler uye = new Uyeler();
            UyelerRepository uyelerRepository = new UyelerRepository();
            uye = uyelerRepository.Getir(x => x.Id == uyeler.Id && x.Durumu == 1);
            uye.Adi = uyeler.Adi;
            uye.Soyadi = uyeler.Soyadi;
            uye.EMail = uyeler.EMail;
            uye.Telefon = uyeler.Telefon;
            uyelerRepository.Guncelle(uye);
            return RedirectToAction("Index", "Account");

                 


        }
	}
}
