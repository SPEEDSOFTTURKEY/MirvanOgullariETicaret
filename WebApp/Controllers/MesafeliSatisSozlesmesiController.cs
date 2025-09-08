using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class MesafeliSatisSozlesmesiController : BaseController
    {
        public async Task<IActionResult> Index()
        {
            await LoadCommonData();

			return View();
        }
    }
}
