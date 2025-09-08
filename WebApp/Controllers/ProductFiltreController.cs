using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class ProductFiltreController : BaseController
	{
		public async Task<IActionResult> Index(int Id)
        {
            await LoadCommonData();
       
            UrunRepository urunRepository = new UrunRepository();
			List<Urun> urunler = new List<Urun>();
			List<string> join = new List<string>();
			join.Add("UrunFotograf");
			join.Add("Renk");
			join.Add("UrunAltKategori");
			join.Add("UrunAnaKategori");
			join.Add("UrunBirim");

			urunler = urunRepository.GetirList(x => x.Durumu == 1 && x.UrunAltKategoriId == Id, join);

			urunler = urunler
						.GroupBy(u => u.Id) // UrunId'ye göre gruplama
						.Select(g => g.First()) // Her gruptan ilk ürünü seç
						.ToList();

			ViewBag.Urunler = urunler;
			return View();
		}
	}
}
