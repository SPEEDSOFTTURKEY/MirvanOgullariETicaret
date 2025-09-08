using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class ProductSearhController : BaseController
	{
		public async Task<IActionResult> Index()
        {
            await LoadCommonData();

           
            UrunRepository urunRepository = new UrunRepository();
			List<Urun> urunler = new List<Urun>();
			List<string> join = new List<string>();
			join.Add("UrunFotograf");
			join.Add("UrunAltKategori");
			join.Add("UrunAnaKategori");
			string? search = HttpContext.Session.GetString("searchvalue");
			urunler = urunRepository.GetirList(x => x.Durumu == 1&&x.Adi.Contains(search), join).ToList();
			ViewBag.Urunler = urunler;
			return View();
		}
	}
}
