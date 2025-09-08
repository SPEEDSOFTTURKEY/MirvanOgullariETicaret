using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class SuppliersContractController : BaseController
	{
		public async Task<IActionResult> Index  ()
        {
            await LoadCommonData();
            HttpContext.Session.Remove("UrunListesi");
            Bayilik bayilik = new Bayilik();
            BayilikRepository bayilikRepository = new BayilikRepository();
            bayilik = bayilikRepository.Getir(x => x.Durumu == 1);
            ViewBag.Bayilik = bayilik;
            return View();
		}
	}
}
