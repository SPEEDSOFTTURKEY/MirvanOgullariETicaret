using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class SepetIndirimEkleController : BaseController
	{
		public IActionResult Index()
        {
           


            Uyeler uyeler = new Uyeler();
            uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");
            if (uyeler != null)
            {
                ViewBag.Uyeler = uyeler;
            }
            else
            {
                ViewBag.Uyeler = null;
            }
            return View();
		}
		[HttpPost]
		public IActionResult Kaydet(SepetIndirim sepetIndirim)
		{

			SepetIndirimRepository sepetIndirimRepo = new SepetIndirimRepository();
			sepetIndirim.EklemeTarihi = DateTime.Now;
			sepetIndirim.GuncellemeTarihi = DateTime.Now;
			sepetIndirim.Durumu = 1;
			sepetIndirimRepo.Ekle(sepetIndirim);
			return RedirectToAction("Index", "SepetIndirim");

		}
	}
}
