using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class UsedRulesController : BaseController
	{
		public async Task<IActionResult> Index()
        {
			KullanimKosullari kullanimKosullari = new KullanimKosullari();
			KullanimKosullariRepository kullanimKosullariRepository = new KullanimKosullariRepository();

			kullanimKosullari = kullanimKosullariRepository.Getir(x => x.Durumu == 1);
			ViewBag.KullanimKosullari = kullanimKosullari;
            await LoadCommonData();
            return View();
		}
	}
}
