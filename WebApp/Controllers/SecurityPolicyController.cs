using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class SecurityPolicyController : BaseController
	{
		public async Task<IActionResult> Index()
        {
            await LoadCommonData();

            GizlilikPolitikasi gizlilikPolitikasi = new GizlilikPolitikasi();
            GizlilikPolitikasiRepository gizlilikPolitikasiRepository = new GizlilikPolitikasiRepository();
            gizlilikPolitikasi = gizlilikPolitikasiRepository.Getir(x => x.Durumu == 1);
            ViewBag.GizlilikPolitikasi = gizlilikPolitikasi;
            return View();
		}
	}
}
