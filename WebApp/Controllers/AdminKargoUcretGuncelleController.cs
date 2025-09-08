using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class AdminKargoUcretGuncelleController : AdminBaseController
    {
		public IActionResult Index()
		{
			KargoUcret kargoUcret = HttpContext.Session.GetObjectFromJson<KargoUcret>("KargoUcret");
			ViewBag.KargoUcret = kargoUcret;
			return View();
		}
		public IActionResult Kaydet(KargoUcret kargoUcret)
		{

			KargoUcretRepository repository = new KargoUcretRepository();
			KargoUcret existingEntity = repository.Getir(kargoUcret.Id);
			if (existingEntity != null)
			{
				existingEntity.SepetTutari = kargoUcret.SepetTutari;
				existingEntity.KargoUcreti = kargoUcret.KargoUcreti;
				existingEntity.GuncellemeTarihi = DateTime.Now;
				repository.Guncelle(existingEntity);
			}
			return RedirectToAction("Index", "AdminKargoUcret");

		}
	}
}
