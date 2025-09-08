using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Controllers
{
    public class AdminPazarYerleriGirisBilgileriController : Controller
    {
        PazarYerleriRepository _pazarYerleriRepository = new PazarYerleriRepository();
      PazarYeriGirisBilgileriRepository _girisBilgileriRepository = new PazarYeriGirisBilgileriRepository();

   

        public IActionResult Index(int? pazarYeriId)
        {
            var pazarYerleris = _pazarYerleriRepository.GetirList(x => x.Durumu == 1);
            ViewBag.Pazaryerleri = pazarYerleris;

            var girisBilgileri = pazarYeriId.HasValue
                ? _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId.Value)
                : new List<PazarYeriGirisBilgileri>();
            ViewBag.GirisBilgileri = girisBilgileri;
            ViewBag.SelectedPazarYeriId = pazarYeriId;

            return View();
        }

        [HttpPost]
        public IActionResult Ekle(PazarYeriGirisBilgileri model)
        {
          
                model.EklenmeTarihi = DateTime.Now;
                model.GuncellenmeTarihi = DateTime.Now;
                model.Durumu = 1;
                _girisBilgileriRepository.Ekle(model);
                return RedirectToAction("Index", new { pazarYeriId = model.PazarYerleriId });

        }

        [HttpPost]
        public IActionResult Guncelle(PazarYeriGirisBilgileri model)
        {
            if (ModelState.IsValid)
            {
                var existing = _girisBilgileriRepository.Getir(x => x.Id == model.Id);
                if (existing != null)
                {
                    existing.ApiKey = model.ApiKey;
                    existing.SecretKey = model.SecretKey;
                    existing.KullaniciAdi = model.KullaniciAdi;
                    existing.Sifre = model.Sifre;
                    existing.GuncellenmeTarihi = DateTime.Now;
                    _girisBilgileriRepository.Guncelle(existing);
                    return RedirectToAction("Index", new { pazarYeriId = model.PazarYerleriId });
                }
                return Json(new { success = false, message = "Kayıt bulunamadı." });
            }
            return Json(new { success = false, message = "Geçersiz veri." });
        }

        [HttpPost]
        public IActionResult Sil(int id, int pazarYeriId)
        {
            var entity = _girisBilgileriRepository.Getir(x => x.Id == id);
            if (entity != null)
            {
                entity.Durumu = 0;
                _girisBilgileriRepository.Guncelle(entity);
                return RedirectToAction("Index", new { pazarYeriId });
            }
            return Json(new { success = false, message = "Kayıt bulunamadı." });
        }
    }
}