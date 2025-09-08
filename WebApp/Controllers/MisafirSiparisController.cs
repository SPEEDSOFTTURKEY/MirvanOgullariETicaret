using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class MisafirSiparisController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MisafirSiparisController> _logger;
        public MisafirSiparisController(IConfiguration configuration, ILogger<MisafirSiparisController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            await LoadCommonData();
            Uyeler uyeler = new Uyeler();
            uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");
            if (uyeler == null)
            {
                List<Sepet> sepetList = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();
                ViewBag.Sepet = sepetList;
                ViewBag.SepetSayi = sepetList.Count;
                _logger.LogInformation("Cart loaded with {Count} items", sepetList.Count);

                List <Sepet> list = new List<Sepet>();
                List<Birimler> birimList = new List<Birimler>();
                list = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet");
                birimList = HttpContext.Session.GetObjectFromJsonCollection<Birimler>("Birim");
                ViewBag.Birim = birimList;
                ViewBag.Sepet = list;
                return View();
            }
            return View();

        }


    }
}
