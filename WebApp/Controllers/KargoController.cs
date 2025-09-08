using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class KargoController : BaseController
	{
		public async Task<IActionResult> Index()
        {
            await LoadCommonData();


			List<SiparisKargoTakip> kargoTakips = new List<SiparisKargoTakip>();
			SiparisKargoTakipRepository siparisKargoTakipRepository = new SiparisKargoTakipRepository();
			kargoTakips = siparisKargoTakipRepository.GetirList(x => x.Durumu == 1).ToList();
			ViewBag.SiparisKargoTakip = kargoTakips;

			return View();
		}

		public async Task<IActionResult> SuratKargoYonlenme(string kargoTakipNo)
		{

			var redirectUrl = $"https://www.suratkargo.com.tr/kargoweb/bireysel.aspx?no={kargoTakipNo}";
			return Redirect(redirectUrl);

		}
	}
}
