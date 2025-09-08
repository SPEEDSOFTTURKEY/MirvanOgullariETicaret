using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminKasaController : Controller
    {
        private readonly CariHesapRepository _cariHesapRepository = new CariHesapRepository();
        private readonly UrunAnaKategoriRepository _urunAnaKategoriRepository = new UrunAnaKategoriRepository();

        public IActionResult Index(int? selectedMonth = null, int? selectedYear = null)
        {
            List<string> join = new List<string>();
            join.Add("Urun");
            join.Add("UrunAltKategori");
            join.Add("UrunAnaKategori");
            // Tüm cari işlemleri getir
            var cariHesaplar = _cariHesapRepository.GetirList(x => x.Durumu == 1,join) ?? new List<CariHesap>();

            // Filtreleme
            if (selectedMonth.HasValue && selectedYear.HasValue)
            {
                cariHesaplar = cariHesaplar
                    .Where(x => x.EklenmeTarihi.Year == selectedYear.Value &&
                                x.EklenmeTarihi.Month == selectedMonth.Value)
                    .ToList();
            }

            // Toplam bilgileri
            ViewBag.ToplamIslem = cariHesaplar.Count;
            ViewBag.ToplamTutar = cariHesaplar.Sum(x => x.ToplamFiyat);
            ViewBag.ToplamUrunAdet = cariHesaplar.Sum(x => x.Adet);

            // Ay/Yıl filtre seçenekleri
            ViewBag.Months = Enumerable.Range(1, 12).Select(m => new
            {
                Value = m,
                Text = System.Globalization.CultureInfo.CreateSpecificCulture("tr-TR").DateTimeFormat.GetMonthName(m)
            }).ToList();

            ViewBag.Years = Enumerable.Range(DateTime.Now.Year - 4, 5).Select(y => new
            {
                Value = y,
                Text = y.ToString()
            }).OrderByDescending(x => x.Value).ToList();

            ViewBag.SelectedMonth = selectedMonth;
            ViewBag.SelectedYear = selectedYear;

            return View(cariHesaplar);
        }
    }
}