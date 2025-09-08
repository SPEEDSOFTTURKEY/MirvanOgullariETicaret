using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class LoginMemberController : BaseController
	{
		public async Task<IActionResult> Index()
        {
            await LoadCommonData();
            return View();
        }
        public async Task<IActionResult> Login(Uyeler model)
        {
            await LoadCommonData();

            Uyeler uyeler = new Uyeler();
            UyelerRepository repository = new UyelerRepository();
            uyeler = repository.Getir(x => x.EMail == model.EMail && x.Sifre == model.Sifre && x.Durumu == 1);
            if (uyeler != null)
            {

                HttpContext.Session.SetObjectAsJson("Uyeler", uyeler);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
    }
}

