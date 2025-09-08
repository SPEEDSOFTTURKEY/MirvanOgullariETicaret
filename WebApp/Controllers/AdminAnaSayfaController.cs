using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminAnaSayfaController : AdminBaseController
    {
        private readonly SiparisRepository _siparisRepository = new SiparisRepository();
        private readonly SiparisGuestRepository _siparisGuestRepository = new SiparisGuestRepository();

        public IActionResult Index(int? statusId = null)
        {

            // Fetch Siparis (Member Orders)
            var siparisList = _siparisRepository.GetirList(x => x.Durumu == 1, new List<string> { "Uyeler", "Durum" })
                .OrderByDescending(x => x.EklenmeTarihi)
                .ToList();

            // Fetch SiparisGuest (Guest Orders)
            var siparisGuestList = _siparisGuestRepository.GetirList(x => x.Durumu == 1 && x.SiparisKodu != null, new List<string> { "Durum" })
                .OrderByDescending(x => x.EklenmeTarihi)
                .ToList();

            // Combine and filter by status if provided
            var allOrders = new List<dynamic>();
            allOrders.AddRange(siparisList.Select(s => new { Order = s, IsGuest = false }));
            allOrders.AddRange(siparisGuestList.Select(s => new { Order = s, IsGuest = true }));

            if (statusId.HasValue)
            {
                allOrders = allOrders.Where(o =>
                    (!o.IsGuest && ((Siparis)o.Order).DurumId == statusId) ||
                    (o.IsGuest && ((SiparisGuest)o.Order).DurumId == statusId)
                ).ToList();
                HttpContext.Session.SetInt32("SelectedStatusId", statusId.Value);
            }
            else
            {
                HttpContext.Session.Remove("SelectedStatusId");
            }

            // Count orders by status
            var statusCounts = new Dictionary<int, int>
            {
                { 1, allOrders.Count(o => !o.IsGuest && ((Siparis)o.Order).DurumId == 1 || o.IsGuest && ((SiparisGuest)o.Order).DurumId == 1) }, // Bekliyor
                { 2, allOrders.Count(o => !o.IsGuest && ((Siparis)o.Order).DurumId == 2 || o.IsGuest && ((SiparisGuest)o.Order).DurumId == 2) }, // Hazırlanıyor
                { 3, allOrders.Count(o => !o.IsGuest && ((Siparis)o.Order).DurumId == 3 || o.IsGuest && ((SiparisGuest)o.Order).DurumId == 3) }, // Kargoya Verildi
                { 4, allOrders.Count(o => !o.IsGuest && ((Siparis)o.Order).DurumId == 4 || o.IsGuest && ((SiparisGuest)o.Order).DurumId == 4) }, // Teslim Edildi
                { 5, allOrders.Count(o => !o.IsGuest && ((Siparis)o.Order).DurumId == 5 || o.IsGuest && ((SiparisGuest)o.Order).DurumId == 5) }  // İptal Edildi
            };

            ViewBag.StatusCounts = statusCounts;
            ViewBag.AllOrders = allOrders;
            ViewBag.SelectedStatusId = statusId ?? HttpContext.Session.GetInt32("SelectedStatusId");

            return View();
        }

        public IActionResult GetAllData()
        {
            HttpContext.Session.Remove("SelectedStatusId");
            return RedirectToAction("Index");
        }
    }
}