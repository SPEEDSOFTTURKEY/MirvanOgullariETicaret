using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class MessageController : BaseController
	{
		public async Task<IActionResult> Index()
        {
            await LoadCommonData();

			Mesaj mesaj= new Mesaj();
			mesaj = HttpContext.Session.GetObjectFromJson<Mesaj>("Mesaj");
			ViewBag.Mesaj = mesaj;	


			return View();
		}
	}
}
