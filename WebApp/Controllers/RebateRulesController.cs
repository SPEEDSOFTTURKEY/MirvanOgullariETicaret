using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class RebateRulesController : BaseController
	{
		public async Task<IActionResult> Index()
        {
            await LoadCommonData(); 
            Iade iade = new Iade();
            IadeRepository iadeRepository = new IadeRepository();
            iade = iadeRepository.Getir(x => x.Durumu == 1);
            ViewBag.Iade = iade;
            return View();
		}
	}
}
