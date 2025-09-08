using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class AdminGaleriFotografGuncelleController : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
