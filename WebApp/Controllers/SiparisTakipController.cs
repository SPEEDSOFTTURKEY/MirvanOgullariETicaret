using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;
using System.Collections.Generic;

namespace WebApp.Controllers
{
    public class SiparisTakipController : BaseController
    {
        private readonly AnaSayfaBannerMetinRepository _anaSayfaBannerMetinRepository;
        private readonly SiparisGuestRepository _siparisGuestRepository;

        public SiparisTakipController()
        {
         
            _anaSayfaBannerMetinRepository = new AnaSayfaBannerMetinRepository();
            _siparisGuestRepository = new SiparisGuestRepository();
        }

        public async Task<IActionResult> Index()
        {
            await LoadCommonData();

            // Get current order information from session if exists
            var siparisGuest = HttpContext.Session.GetObjectFromJson<SiparisGuest>("SiparisGuest");
            ViewBag.Siparis = siparisGuest;
         

            // Get cart information for header
            SetCommonViewBagDataAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetirAsync(string SiparisNo)
        {
            await LoadCommonData();

            if (string.IsNullOrEmpty(SiparisNo))
            {
                return RedirectToAction("Index");
            }

            var includeList = new List<string> { "Durum" };
            var siparisGuest = _siparisGuestRepository.Getir(x => x.SiparisKodu == SiparisNo, includeList);

            if (siparisGuest != null)
            {
                HttpContext.Session.SetObjectAsJson("SiparisGuest", siparisGuest);
            }

            return RedirectToAction("Index");
        }

        private async Task SetCommonViewBagDataAsync()
        {
            await LoadCommonData();

            // Set cart count for header
            var sepets = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet");
            ViewBag.SepetSayi = sepets?.Count ?? 0;

            // Set user information for header
            var uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");
            ViewBag.Uyeler = uyeler;
        }
        public async Task<IActionResult> YeniAsync()
        {
            await LoadCommonData();

            var siparisGuest = HttpContext.Session.GetObjectFromJson<SiparisGuest>("SiparisGuest");
            ViewBag.Siparis = siparisGuest;
            if (siparisGuest != null)
            {
                HttpContext.Session.Clear();
            }
            return RedirectToAction("Index");

        }
        }
        
}