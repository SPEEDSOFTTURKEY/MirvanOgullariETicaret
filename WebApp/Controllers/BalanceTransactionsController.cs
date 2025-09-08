using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
	public class BalanceTransactionsController : BaseController
	{
		public async Task<IActionResult> Index()
        {
            await LoadCommonData();

            return View();
		}
	}
}
