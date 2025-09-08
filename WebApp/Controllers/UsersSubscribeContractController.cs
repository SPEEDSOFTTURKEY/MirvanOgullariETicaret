using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class UsersSubscribeContractController : BaseController
    {
		public async Task<IActionResult> Index()
        {
            await LoadCommonData();
            UyelikSozlesmesiRepository uyelikSozlesmesiRepository = new UyelikSozlesmesiRepository();
            UyelikSozlesmesi uyelikSozlesmesi = new UyelikSozlesmesi();
            uyelikSozlesmesi = uyelikSozlesmesiRepository.Getir(x => x.Durumu == 1);
            ViewBag.UyelikSozlesmesi = uyelikSozlesmesi;

            return View();
		}
	}
}
