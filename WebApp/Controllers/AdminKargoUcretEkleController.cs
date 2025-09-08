using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class AdminKargoUcretEkleController : AdminBaseController
    {
		public IActionResult Index()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Kaydet(KargoUcret kargoUcret)
		{

			KargoUcretRepository kargoUcretRepo = new KargoUcretRepository();
			kargoUcret.EklemeTarihi = DateTime.Now;
			kargoUcret.GuncellemeTarihi = DateTime.Now;
			kargoUcret.Durumu = 1;
			kargoUcretRepo.Ekle(kargoUcret);
			return RedirectToAction("Index", "AdminKargoUcret");

		}
	}
}
