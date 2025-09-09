using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks.Dataflow;

namespace WebApp.Controllers
{
    public class ProductsController : BaseController
    {
        private const int DefaultPageSize = 12;

        private async Task<(List<Urun> urunler, List<Birimler> birimler, bool hideBirimFilter, int totalItems, int totalPages, Dictionary<int, bool> stockStatus)> GetFilteredProducts(
            int? anaKategoriId, int? altKategoriId, List<int> birims, decimal? minPrice, decimal? maxPrice, int page, string sort, int limit, bool onlyInStock)
        {
            UrunRepository urunRepository = new UrunRepository();
            UrunStokRepository urunStokRepository = new UrunStokRepository();
            UrunBirimRepository urunBirimRepository = new UrunBirimRepository();
            BirimlerRepository birimlerRepository = new BirimlerRepository();

            // Build query
            IQueryable<Urun> urunQuery = urunRepository.GetirQueryable(
                x => x.Durumu == 1,
                x => x.UrunFotograf,
                x => x.UrunAltKategori,
                x => x.UrunAnaKategori
            );

            // Category filtering
            if (altKategoriId.HasValue)
            {
                urunQuery = urunQuery.Where(x => x.UrunAltKategoriId == altKategoriId.Value);
            }
            else if (anaKategoriId.HasValue)
            {
                urunQuery = urunQuery.Where(x => x.UrunAnaKategoriId == anaKategoriId.Value);
            }

            // Unit filtering
            if (birims != null && birims.Any())
            {
                var birimFilteredUrunIds = urunStokRepository.GetirList(ub => birims.Contains(ub.BirimlerId) && ub.Stok > 0 && ub.Durumu == 1)
                    .Select(ub => ub.UrunId)
                    .Distinct()
                    .ToList();
                urunQuery = urunQuery.Where(u => birimFilteredUrunIds.Contains(u.Id));
            }

            // Price filtering
            if (minPrice.HasValue || maxPrice.HasValue)
            {
                urunQuery = urunQuery.Where(x =>
                    (!minPrice.HasValue || (x.IndirimYuzdesi > 0
                        ? x.Fiyat - (x.Fiyat * ((x.IndirimYuzdesi ?? 0) / 100.0m))
                        : x.Fiyat) >= minPrice.Value) &&
                    (!maxPrice.HasValue || (x.IndirimYuzdesi > 0
                        ? x.Fiyat - (x.Fiyat * ((x.IndirimYuzdesi ?? 0) / 100.0m))
                        : x.Fiyat) <= maxPrice.Value)
                );
            }

            // Stock status filtering
            if (onlyInStock)
            {
                var inStockUrunIds = urunStokRepository.GetirList(x => x.Stok > 0 && x.Durumu == 1)
                    .Select(x => x.UrunId)
                    .Distinct()
                    .ToList();
                urunQuery = urunQuery.Where(u => inStockUrunIds.Contains(u.Id));
            }

            // Sorting
            switch (sort)
            {
                case "name_asc":
                    urunQuery = urunQuery.OrderBy(x => x.Adi);
                    break;
                case "name_desc":
                    urunQuery = urunQuery.OrderByDescending(x => x.Adi);
                    break;
                case "price_asc":
                    urunQuery = urunQuery.OrderBy(x => x.Fiyat - (x.Fiyat * ((x.IndirimYuzdesi ?? 0) / 100.0m)));
                    break;
                case "price_desc":
                    urunQuery = urunQuery.OrderByDescending(x => x.Fiyat - (x.Fiyat * ((x.IndirimYuzdesi ?? 0) / 100.0m)));
                    break;
                default:
                    urunQuery = urunQuery.OrderByDescending(x => x.Id);
                    break;
            }

            // Pagination
            int totalItems = urunQuery.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / limit);
            page = Math.Max(1, Math.Min(page, totalPages == 0 ? 1 : totalPages));
            var urunler = urunQuery.Skip((page - 1) * limit).Take(limit).ToList();

            // Stock status and photo filtering
            var stockStatus = new Dictionary<int, bool>();
            foreach (var urun in urunler)
            {
                if (urun.UrunFotograf != null)
                {
                    urun.UrunFotograf = urun.UrunFotograf.Where(f => f.VitrinMi == 1).ToList();
                }
                stockStatus[urun.Id] = urunStokRepository.GetirList(x => x.UrunId == urun.Id && x.Stok > 0 && x.Durumu == 1).Any();
            }

            // Units
            var urunIds = urunler.Select(u => u.Id).ToList();
            var birimIds = urunBirimRepository.GetirList(ub => urunIds.Contains(ub.UrunId) && ub.Durumu == 1)
                .Select(ub => ub.BirimlerId)
                .Distinct()
                .ToList();
            var birimler = birimlerRepository.GetirList(b => birimIds.Contains(b.Id) && b.Durumu == 1 && b.Id != 32).ToList();
            bool hideBirimFilter = !birimIds.Any() || (birimIds.Count == 1 && birimIds.Contains(32));

            return (urunler, birimler, hideBirimFilter, totalItems, totalPages, stockStatus);
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? anaKategoriId, int? altKategoriId, int page = 1, string sort = null, int limit = DefaultPageSize, bool onlyInStock = false)
        {
            await LoadCommonData();

            var (urunler, birimler, hideBirimFilter, totalItems, totalPages, stockStatus) = await GetFilteredProducts(
                anaKategoriId, altKategoriId, null, null, null, page, sort, limit, onlyInStock);

            // Set ViewBag
            ViewBag.Urunler = urunler;
            ViewBag.Birimler = hideBirimFilter ? new List<Birimler>() : birimler;
            ViewBag.HideBirimFilter = hideBirimFilter;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = limit;
            ViewBag.Sort = sort;
            ViewBag.AnaKategoriId = anaKategoriId;
            ViewBag.AltKategoriId = altKategoriId;
            ViewBag.MinPrice = null;
            ViewBag.MaxPrice = null;
            ViewBag.OnlyInStock = onlyInStock;
            ViewBag.SecilenBirimIds = new List<int>();

            foreach (var kvp in stockStatus)
            {
                ViewData[$"StockStatus_{kvp.Key}"] = kvp.Value;
            }

            // Cart and user
            List<Sepet> sepets = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();
            ViewBag.SepetSayi = sepets.Count;
            ViewBag.Uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index( List<int> birims,
            decimal? minPrice, decimal? maxPrice, int page = 1, string sort = null,
            int limit = DefaultPageSize, bool onlyInStock = false)
        {
            // Repository'leri oluştur
            UrunRepository urunRepository = new UrunRepository();
            UrunStokRepository urunStokRepository = new UrunStokRepository();
            BirimlerRepository birimlerRepository = new BirimlerRepository();
            UrunBirimRepository urunBirimRepository = new UrunBirimRepository();

            // Join listesi
            List<string> join = new List<string> { "UrunFotograf", "UrunGaleri" };

            // Temel ürün sorgusu
            List<Urun> uruns = urunRepository.GetirList(x => x.Durumu == 1, join);

            // Fiyat filtreleme
            if (minPrice.HasValue)
                uruns = uruns.Where(x => x.Fiyat >= minPrice.Value).ToList();

            if (maxPrice.HasValue)
                uruns = uruns.Where(x => x.Fiyat <= maxPrice.Value).ToList();

            // Stok kontrolü
            if (onlyInStock)
            {
                List<UrunStok> stokluUrunler = urunStokRepository.GetirList(x => x.Stok > 0);
                List<int> stokluUrunIds = stokluUrunler.Select(x => x.UrunId).Distinct().ToList();
                uruns = uruns.Where(x => stokluUrunIds.Contains(x.Id)).ToList();
            }

            // Birim filtreleme
            if (birims != null && birims.Any())
            {
                List<UrunBirim> filtreliUrunBirimler = urunBirimRepository
                    .GetirList(x => x.Durumu == 1 && birims.Contains(x.BirimlerId));

                List<int> filtreliUrunIds = filtreliUrunBirimler.Select(x => x.UrunId).Distinct().ToList();
                uruns = uruns.Where(x => filtreliUrunIds.Contains(x.Id)).ToList();
            }


            // Sayfalama
            int totalCount = uruns.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)limit);
            uruns = uruns.Skip((page - 1) * limit).Take(limit).ToList();

            // Sıralama
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower())
                {
                    case "price_asc":
                        uruns = uruns.OrderBy(x => x.Fiyat).ToList();
                        break;
                    case "price_desc":
                        uruns = uruns.OrderByDescending(x => x.Fiyat).ToList();
                        break;
                    case "name_asc":
                        uruns = uruns.OrderBy(x => x.Adi).ToList();
                        break;
                    case "name_desc":
                        uruns = uruns.OrderByDescending(x => x.Adi).ToList();
                        break;
                    default:
                        uruns = uruns.OrderBy(x => x.Adi).ToList();
                        break;
                }
            }

            // Birim verilerini al
            List<Birimler> tumBirimler = birimlerRepository.GetirList(x => x.Durumu == 1);
            List<Birimler> secilenBirimler = new List<Birimler>();

            if (birims != null && birims.Any())
            {
                secilenBirimler = tumBirimler.Where(x => birims.Contains(x.Id)).ToList();
                HttpContext.Session.SetObjectAsJson("SecilenBirimler", secilenBirimler);
            }
            else
            {
                HttpContext.Session.Remove("SecilenBirimler");
            }

            // Ortak verileri yükle
            await LoadCommonData();

            // ViewBag ayarları
            ViewBag.Urunler = uruns;
            ViewBag.Birimler = tumBirimler;
            ViewBag.HideBirimFilter = secilenBirimler;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = limit;
            ViewBag.Sort = sort;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.OnlyInStock = onlyInStock;
            ViewBag.SecilenBirimIds = birims ?? new List<int>();
            ViewBag.SecilenBirim = secilenBirimler;

         
            // Sepet ve kullanıcı bilgileri
            List<Sepet> sepets = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();
            ViewBag.SepetSayi = sepets.Count;
            sepets = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet");
            ViewBag.Sepet = sepets;
            ViewBag.Uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SessionTemizle()
        {
            await LoadCommonData();
            HttpContext.Session.Remove("SecilenBirimler");
            return RedirectToAction("Index");
        }
    }
}