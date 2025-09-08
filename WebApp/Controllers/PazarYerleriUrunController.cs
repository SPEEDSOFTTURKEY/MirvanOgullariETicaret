using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class PazarYerleriUrunController : Controller
    {
        UnifiedMarketplaceRepository unifiedMarketplaceRepository = new UnifiedMarketplaceRepository();

        public IActionResult Index()
        {
            List<string> joni = new List<string>();
            joni.Add("PazarYeriGorsel");
            List<UnifiedMarketplace> list = new List<UnifiedMarketplace>();
            list = unifiedMarketplaceRepository.GetirList(x => x.Durumu == 1,joni);
            ViewBag.List = list;


            return View();
        }
        [HttpGet]
        public IActionResult GetProductImages(int id)
        {
            var pazarYeriGorselRepository = new PazarYeriGorselRepository();
            var gorseller = pazarYeriGorselRepository
                .GetirList(g => g.UnifiedMarketplaceId == id)
                .Select(g => g.FotografYolu)
                .ToList();

            return Json(gorseller);
        }

        [HttpPost]
        public IActionResult Search(string title, string marketplace)
        {
            var query = unifiedMarketplaceRepository.GetirList(x => x.Durumu == 1);

            // Filter by title
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(x => x.Title != null && x.Title.ToLower().Contains(title.ToLower())).ToList();
            }

            // Filter by marketplace
            if (!string.IsNullOrEmpty(marketplace))
            {
                query = marketplace switch
                {
                    "N11" => query.Where(x => x.N11_CategoryId != null).ToList(),
                    "GoTurc" => query.Where(x => x.GoTurc_CategoryId != null).ToList(),
                    "Idefix" => query.Where(x => x.Idefix_CategoryId != null).ToList(),
                    "PTT" => query.Where(x => x.PTT_CategoryId != null).ToList(),
                    "Pazarama" => query.Where(x => x.Pazarama_CategoryId != null).ToList(),
                    "CicekSepeti" => query.Where(x => x.CicekSepeti_CategoryId != null).ToList(),
                    "Farmazon" => query.Where(x => x.Farmazon_Id != 0).ToList(),
                    "FarmaBorsa" => query.Where(x => x.FarmaBorsa_SalePrice != null).ToList(),
                    "Trendyol" => query.Where(x => x.Trendyol_CategoryId != null).ToList(),
                    _ => query
                };
            }

            ViewBag.List = query;
            return View("Index");
        }

        [HttpGet]
        public IActionResult GetProductDetails(int id)
        {
            var product = unifiedMarketplaceRepository.GetirList(x => x.Id == id && x.Durumu == 1).FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }

            var details = new Dictionary<string, string>
            {
                { "Id", product.Id.ToString() },
                { "UrunId", product.UrunId?.ToString() ?? "" },
                { "Title", product.Title ?? "" },
                { "ProductCode", product.ProductCode ?? "" },
                { "Barcode", product.Barcode ?? "" },
                { "Description", product.Description ?? "" },
                { "ListPrice", product.ListPrice?.ToString("F2") ?? "" },
                { "VatRate", product.VatRate?.ToString() ?? "" },
                // N11
                { "N11_CategoryId", product.N11_CategoryId?.ToString() ?? "" },
                { "N11_ReportId", product.N11_ReportId?.ToString() ?? "" },
                { "N11_SalePrice", product.N11_SalePrice?.ToString("F2") ?? "" },
                { "N11_StockQuantity", product.N11_StockQuantity?.ToString() ?? "" },
                { "N11_Success", product.N11_Success?.ToString() ?? "" },
                { "N11_Message", product.N11_Message ?? "" },
                { "N11_EklenmeTarihi", product.N11_EklenmeTarihi?.ToString("g") ?? "" },
                { "N11_GuncellenmeTarihi", product.N11_GuncellenmeTarihi?.ToString("g") ?? "" },
                { "N11_Gonderildi", product.N11_Gonderildi?.ToString() ?? "" },
                // GoTurc
                { "GoTurc_CategoryId", product.GoTurc_CategoryId?.ToString() ?? "" },
                { "GoTurc_BrandId", product.GoTurc_BrandId?.ToString() ?? "" },
                { "GoTurc_DeliveryNo", product.GoTurc_DeliveryNo?.ToString() ?? "" },
                { "GoTurc_SalePrice", product.GoTurc_SalePrice?.ToString("F2") ?? "" },
                { "GoTurc_StockQuantity", product.GoTurc_StockQuantity?.ToString() ?? "" },
                { "GoTurc_Success", product.GoTurc_Success?.ToString() ?? "" },
                { "GoTurc_Message", product.GoTurc_Message ?? "" },
                { "GoTurc_ReportId", product.GoTurc_ReportId ?? "" },
                { "GoTurc_EklenmeTarihi", product.GoTurc_EklenmeTarihi?.ToString("g") ?? "" },
                { "GoTurc_GuncellenmeTarihi", product.GoTurc_GuncellenmeTarihi?.ToString("g") ?? "" },
                { "GoTurc_Gonderildi", product.GoTurc_Gonderildi?.ToString() ?? "" },
                // Idefix
                { "Idefix_CategoryId", product.Idefix_CategoryId?.ToString() ?? "" },
                { "Idefix_BrandId", product.Idefix_BrandId?.ToString() ?? "" },
                { "Idefix_SalePrice", product.Idefix_SalePrice?.ToString("F2") ?? "" },
                { "Idefix_StockQuantity", product.Idefix_StockQuantity?.ToString() ?? "" },
                { "Idefix_VendorStockCode", product.Idefix_VendorStockCode ?? "" },
                { "Idefix_Weight", product.Idefix_Weight?.ToString("F2") ?? "" },
                { "Idefix_ComparePrice", product.Idefix_ComparePrice?.ToString("F2") ?? "" },
                { "Idefix_ProductMainId", product.Idefix_ProductMainId ?? "" },
                { "Idefix_DeliveryDuration", product.Idefix_DeliveryDuration?.ToString() ?? "" },
                { "Idefix_CargoCompanyId", product.Idefix_CargoCompanyId?.ToString() ?? "" },
                { "Idefix_ShipmentAddressId", product.Idefix_ShipmentAddressId?.ToString() ?? "" },
                { "Idefix_ReturnAddressId", product.Idefix_ReturnAddressId?.ToString() ?? "" },
                { "Idefix_AttributesJson", product.Idefix_AttributesJson ?? "" },
                { "Idefix_Success", product.Idefix_Success?.ToString() ?? "" },
                { "Idefix_Message", product.Idefix_Message ?? "" },
                { "Idefix_ReportId", product.Idefix_ReportId ?? "" },
                { "Idefix_EklenmeTarihi", product.Idefix_EklenmeTarihi?.ToString("g") ?? "" },
                { "Idefix_GuncellenmeTarihi", product.Idefix_GuncellenmeTarihi?.ToString("g") ?? "" },
                { "Idefix_Gonderildi", product.Idefix_Gonderildi?.ToString() ?? "" },
                // PTT
                { "PTT_CategoryId", product.PTT_CategoryId?.ToString() ?? "" },
                { "PTT_SalePrice", product.PTT_SalePrice?.ToString("F2") ?? "" },
                { "PTT_PriceWithVat", product.PTT_PriceWithVat?.ToString("F2") ?? "" },
                { "PTT_StockQuantity", product.PTT_StockQuantity?.ToString() ?? "" },
                { "PTT_Desi", product.PTT_Desi?.ToString("F2") ?? "" },
                { "PTT_Discount", product.PTT_Discount?.ToString("F2") ?? "" },
                { "PTT_EstimatedCourierDelivery", product.PTT_EstimatedCourierDelivery?.ToString() ?? "" },
                { "PTT_WarrantyDuration", product.PTT_WarrantyDuration?.ToString() ?? "" },
                { "PTT_CargoProfileId", product.PTT_CargoProfileId?.ToString() ?? "" },
                { "PTT_BasketMaxQuantity", product.PTT_BasketMaxQuantity?.ToString() ?? "" },
                { "PTT_VariantsJson", product.PTT_VariantsJson ?? "" },
                { "PTT_Success", product.PTT_Success?.ToString() ?? "" },
                { "PTT_Message", product.PTT_Message ?? "" },
                { "PTT_TrackingId", product.PTT_TrackingId ?? "" },
                { "PTT_EklenmeTarihi", product.PTT_EklenmeTarihi?.ToString("g") ?? "" },
                { "PTT_GuncellenmeTarihi", product.PTT_GuncellenmeTarihi?.ToString("g") ?? "" },
                { "PTT_Gonderildi", product.PTT_Gonderildi?.ToString() ?? "" },
                // Pazarama
                { "Pazarama_CategoryId", product.Pazarama_CategoryId?.ToString() ?? "" },
                { "Pazarama_BrandId", product.Pazarama_BrandId?.ToString() ?? "" },
                { "Pazarama_SalePrice", product.Pazarama_SalePrice?.ToString("F2") ?? "" },
                { "Pazarama_StockQuantity", product.Pazarama_StockQuantity?.ToString() ?? "" },
                { "Pazarama_Success", product.Pazarama_Success?.ToString() ?? "" },
                { "Pazarama_Message", product.Pazarama_Message ?? "" },
                { "Pazarama_ReportId", product.Pazarama_ReportId ?? "" },
                { "Pazarama_EklenmeTarihi", product.Pazarama_EklenmeTarihi?.ToString("g") ?? "" },
                { "Pazarama_GuncellenmeTarihi", product.Pazarama_GuncellenmeTarihi?.ToString("g") ?? "" },
                { "Pazarama_Gonderildi", product.Pazarama_Gonderildi?.ToString() ?? "" },
                // ÇiçekSepeti
                { "CicekSepeti_CategoryId", product.CicekSepeti_CategoryId?.ToString() ?? "" },
                { "CicekSepeti_SalePrice", product.CicekSepeti_SalePrice?.ToString("F2") ?? "" },
                { "CicekSepeti_StockQuantity", product.CicekSepeti_StockQuantity?.ToString() ?? "" },
                { "CicekSepeti_StockCode", product.CicekSepeti_StockCode ?? "" },
                { "CicekSepeti_CategoryName", product.CicekSepeti_CategoryName ?? "" },
                { "CicekSepeti_ProductStatusType", product.CicekSepeti_ProductStatusType ?? "" },
                { "CicekSepeti_VariantName", product.CicekSepeti_VariantName ?? "" },
                { "CicekSepeti_ReportId", product.CicekSepeti_ReportId ?? "" },
                { "CicekSepeti_Success", product.CicekSepeti_Success?.ToString() ?? "" },
                { "CicekSepeti_Message", product.CicekSepeti_Message ?? "" },
                { "CicekSepeti_EklenmeTarihi", product.CicekSepeti_EklenmeTarihi?.ToString("g") ?? "" },
                { "CicekSepeti_GuncellenmeTarihi", product.CicekSepeti_GuncellenmeTarihi?.ToString("g") ?? "" },
                { "CicekSepeti_Gonderildi", product.CicekSepeti_Gonderildi?.ToString() ?? "" },
                // Farmazon
                { "Farmazon_Id", product.Farmazon_Id?.ToString() ?? "" },
                { "Farmazon_Price", product.Farmazon_Price?.ToString("F2") ?? "" },
                { "Farmazon_Stock", product.Farmazon_Stock?.ToString() ?? "" },
                { "Farmazon_State", product.Farmazon_State?.ToString() ?? "" },
                { "Farmazon_Expiration", product.Farmazon_Expiration ?? "" },
                { "Farmazon_MaxCount", product.Farmazon_MaxCount?.ToString() ?? "" },
                { "Farmazon_IsFeatured", product.Farmazon_IsFeatured?.ToString() ?? "" },
                { "Farmazon_FarmazonEnabled", product.Farmazon_FarmazonEnabled?.ToString() ?? "" },
                { "Farmazon_StockCode", product.Farmazon_StockCode ?? "" },
                { "Farmazon_ReportId", product.Farmazon_ReportId ?? "" },
                { "Farmazon_Success", product.Farmazon_Success?.ToString() ?? "" },
                { "Farmazon_Message", product.Farmazon_Message ?? "" },
                { "Farmazon_EklenmeTarihi", product.Farmazon_EklenmeTarihi?.ToString("g") ?? "" },
                { "Farmazon_GuncellenmeTarihi", product.Farmazon_GuncellenmeTarihi?.ToString("g") ?? "" },
                { "Farmazon_Gonderildi", product.Farmazon_Gonderildi?.ToString() ?? "" },
                // FarmaBorsa
                { "FarmaBorsa_SalePrice", product.FarmaBorsa_SalePrice?.ToString("F2") ?? "" },
                { "FarmaBorsa_StockQuantity", product.FarmaBorsa_StockQuantity?.ToString() ?? "" },
                { "FarmaBorsa_MaxQuantity", product.FarmaBorsa_MaxQuantity?.ToString() ?? "" },
                { "FarmaBorsa_ExpirationDate", product.FarmaBorsa_ExpirationDate ?? "" },
                { "FarmaBorsa_FarmaBorsaEnabled", product.FarmaBorsa_FarmaBorsaEnabled?.ToString() ?? "" },
                { "FarmaBorsa_MiadTip", product.FarmaBorsa_MiadTip?.ToString() ?? "" },
                { "FarmaBorsa_Success", product.FarmaBorsa_Success?.ToString() ?? "" },
                { "FarmaBorsa_Message", product.FarmaBorsa_Message ?? "" },
                { "FarmaBorsa_EklenmeTarihi", product.FarmaBorsa_EklenmeTarihi?.ToString("g") ?? "" },
                { "FarmaBorsa_GuncellenmeTarihi", product.FarmaBorsa_GuncellenmeTarihi?.ToString("g") ?? "" },
                { "FarmaBorsa_Gonderildi", product.FarmaBorsa_Gonderildi?.ToString() ?? "" },
                // Trendyol
                { "Trendyol_CategoryId", product.Trendyol_CategoryId?.ToString() ?? "" },
                { "Trendyol_BrandId", product.Trendyol_BrandId?.ToString() ?? "" },
                { "Trendyol_SalePrice", product.Trendyol_SalePrice?.ToString("F2") ?? "" },
                { "Trendyol_StockQuantity", product.Trendyol_StockQuantity?.ToString() ?? "" },
                { "Trendyol_Success", product.Trendyol_Success?.ToString() ?? "" },
                { "Trendyol_Message", product.Trendyol_Message ?? "" },
                { "Trendyol_EklenmeTarihi", product.Trendyol_EklenmeTarihi?.ToString("g") ?? "" },
                { "Trendyol_GuncellenmeTarihi", product.Trendyol_GuncellenmeTarihi?.ToString("g") ?? "" },
                { "Trendyol_Gonderildi", product.Trendyol_Gonderildi?.ToString() ?? "" },
                { "ImgFilesJson", product.ImgFilesJson ?? "" },
                { "Gonderildi", product.Gonderildi.ToString() }
            };

            return Json(details);
        }

        public IActionResult ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            UnifiedMarketplaceRepository unifiedMarketplaceRepository = new UnifiedMarketplaceRepository();
            var list = unifiedMarketplaceRepository.GetirList(x => x.Durumu == 1);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Ürün Bilgileri");

                string[] headers = new string[]
                {
                    "Ürün Adı", "Ürün Kodu", "Barkod", "Liste Fiyatı", "KDV Oranı",
                    "PTT Kategori", "PTT KDV Hariç Fiyat", "PTT KDV Dahil Fiyat", "PTT Stok Miktarı",
                    "PTT Desi", "PTT İndirim", "PTT Tahmini Kargo Teslim Süresi", "PTT Garanti Süresi", "PTT Garanti Sağlayıcı",
                    "N11 Kategori", "N11 Satış Fiyatı", "N11 Stok Miktarı", "N11 İndirim Yüzdesi",
                    "Idefix Kategori", "Idefix Marka", "Idefix Satış Fiyatı", "Idefix Stok Miktarı", "Idefix Tedarikçi Stok Kodu",
                    "Idefix Ağırlık", "Idefix Karşılaştırma Fiyatı", "Idefix Teslimat Süresi",
                    "Idefix Kargo Şirketi", "Idefix Gönderim Adresi", "Idefix İade Adresi",
                    "FarmaBorsa Satış Fiyatı", "FarmaBorsa Stok Miktarı", "FarmaBorsa Maksimum Satış Miktarı",
                    "FarmaBorsa Son Kullanım Tarihi", "FarmaBorsa Son Kullanım Tipi", "FarmaBorsa Borsada Göster",
                    "Farmazon Ürün ID", "Farmazon Satış Fiyatı", "Farmazon Stok Miktarı", "Farmazon Stok Kodu",
                    "Farmazon Son Kullanım Tarihi", "Farmazon Maksimum Satış Miktarı", "Farmazon Öne Çıkan Ürün",
                    "Pazarama Kategori", "Pazarama Marka", "Pazarama Satış Fiyatı", "Pazarama Stok Miktarı",
                    "ÇiçekSepeti Kategori", "ÇiçekSepeti Satış Fiyatı", "ÇiçekSepeti Stok Miktarı", "ÇiçekSepeti Stok Kodu",
                    "ÇiçekSepeti Kategori Adı", "ÇiçekSepeti Ürün Durum Tipi", "ÇiçekSepeti Varyant Adı",
                    "GoTurc Kategori", "GoTurc Marka", "GoTurc Teslimat Yöntemi", "GoTurc Satış Fiyatı", "GoTurc Stok Miktarı",
                    "Trendyol Kategori", "Trendyol Marka", "Trendyol Satış Fiyatı", "Trendyol Stok Miktarı",
                    "Açıklama", "Kısa Açıklama",
                    "PTT Varyant Barkod", "PTT Varyant Fiyat", "PTT Varyant Stok", "PTT Varyant Nitelik Tanımı", "PTT Varyant Nitelik Değeri"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                for (int i = 0; i < list.Count; i++)
                {
                    var urun = list[i];
                    int row = i + 2;

                    worksheet.Cells[row, 1].Value = urun.Title ?? "";
                    worksheet.Cells[row, 2].Value = urun.ProductCode ?? "";
                    worksheet.Cells[row, 3].Value = urun.Barcode ?? "";
                    worksheet.Cells[row, 4].Value = urun.ListPrice?.ToString("F2") ?? "";
                    worksheet.Cells[row, 5].Value = urun.VatRate?.ToString() ?? "";
                    worksheet.Cells[row, 66].Value = urun.Description ?? "";

                    worksheet.Cells[row, 6].Value = urun.PTT_CategoryId?.ToString() ?? "";
                    worksheet.Cells[row, 8].Value = urun.PTT_SalePrice?.ToString("F2") ?? "";
                    worksheet.Cells[row, 9].Value = urun.PTT_PriceWithVat?.ToString("F2") ?? "";
                    worksheet.Cells[row, 10].Value = urun.PTT_StockQuantity?.ToString() ?? "";
                    worksheet.Cells[row, 11].Value = urun.PTT_Desi?.ToString("F2") ?? "";
                    worksheet.Cells[row, 12].Value = urun.PTT_Discount?.ToString("F2") ?? "";
                    worksheet.Cells[row, 13].Value = urun.PTT_EstimatedCourierDelivery?.ToString() ?? "";
                    worksheet.Cells[row, 14].Value = urun.PTT_WarrantyDuration?.ToString() ?? "";
                    worksheet.Cells[row, 68].Value = urun.PTT_VariantsJson ?? "";
                    worksheet.Cells[row, 69].Value = "";
                    worksheet.Cells[row, 70].Value = "";
                    worksheet.Cells[row, 71].Value = "";
                    worksheet.Cells[row, 72].Value = "";

                    worksheet.Cells[row, 16].Value = urun.N11_CategoryId?.ToString() ?? "";
                    worksheet.Cells[row, 17].Value = urun.N11_SalePrice?.ToString("F2") ?? "";
                    worksheet.Cells[row, 18].Value = urun.N11_StockQuantity?.ToString() ?? "";

                    worksheet.Cells[row, 20].Value = urun.Idefix_CategoryId?.ToString() ?? "";
                    worksheet.Cells[row, 21].Value = urun.Idefix_BrandId?.ToString() ?? "";
                    worksheet.Cells[row, 22].Value = urun.Idefix_SalePrice?.ToString("F2") ?? "";
                    worksheet.Cells[row, 23].Value = urun.Idefix_StockQuantity?.ToString() ?? "";
                    worksheet.Cells[row, 24].Value = urun.Idefix_VendorStockCode ?? "";
                    worksheet.Cells[row, 25].Value = urun.Idefix_Weight?.ToString("F2") ?? "";
                    worksheet.Cells[row, 26].Value = urun.Idefix_ComparePrice?.ToString("F2") ?? "";
                    worksheet.Cells[row, 27].Value = urun.Idefix_DeliveryDuration?.ToString() ?? "";
                    worksheet.Cells[row, 29].Value = urun.Idefix_CargoCompanyId?.ToString() ?? "";
                    worksheet.Cells[row, 30].Value = urun.Idefix_ShipmentAddressId?.ToString() ?? "";
                    worksheet.Cells[row, 31].Value = urun.Idefix_ReturnAddressId?.ToString() ?? "";
                    worksheet.Cells[row, 33].Value = urun.FarmaBorsa_SalePrice?.ToString("F2") ?? "";
                    worksheet.Cells[row, 34].Value = urun.FarmaBorsa_StockQuantity?.ToString() ?? "";
                    worksheet.Cells[row, 35].Value = urun.FarmaBorsa_MaxQuantity?.ToString() ?? "";
                    worksheet.Cells[row, 36].Value = urun.FarmaBorsa_ExpirationDate ?? "";
                    worksheet.Cells[row, 37].Value = urun.FarmaBorsa_MiadTip?.ToString() ?? "";

                    worksheet.Cells[row, 39].Value = urun.Farmazon_Id?.ToString() ?? "";
                    worksheet.Cells[row, 40].Value = urun.Farmazon_Price?.ToString("F2") ?? "";
                    worksheet.Cells[row, 41].Value = urun.Farmazon_Stock?.ToString() ?? "";
                    worksheet.Cells[row, 42].Value = urun.Farmazon_StockCode ?? "";
                    worksheet.Cells[row, 43].Value = urun.Farmazon_Expiration ?? "";
                    worksheet.Cells[row, 44].Value = urun.Farmazon_MaxCount?.ToString() ?? "";
                    worksheet.Cells[row, 45].Value = urun.Farmazon_IsFeatured == true ? "Evet" : "Hayır";

                    worksheet.Cells[row, 46].Value = urun.Pazarama_CategoryId?.ToString() ?? "";
                    worksheet.Cells[row, 47].Value = urun.Pazarama_BrandId?.ToString() ?? "";
                    worksheet.Cells[row, 48].Value = urun.Pazarama_SalePrice?.ToString("F2") ?? "";
                    worksheet.Cells[row, 49].Value = urun.Pazarama_StockQuantity?.ToString() ?? "";

                    worksheet.Cells[row, 50].Value = urun.CicekSepeti_CategoryId?.ToString() ?? "";
                    worksheet.Cells[row, 51].Value = urun.CicekSepeti_SalePrice?.ToString("F2") ?? "";
                    worksheet.Cells[row, 52].Value = urun.CicekSepeti_StockQuantity?.ToString() ?? "";
                    worksheet.Cells[row, 53].Value = urun.CicekSepeti_StockCode ?? "";
                    worksheet.Cells[row, 54].Value = urun.CicekSepeti_CategoryName ?? "";
                    worksheet.Cells[row, 55].Value = urun.CicekSepeti_ProductStatusType ?? "";
                    worksheet.Cells[row, 56].Value = urun.CicekSepeti_VariantName ?? "";

                    worksheet.Cells[row, 57].Value = urun.GoTurc_CategoryId?.ToString() ?? "";
                    worksheet.Cells[row, 58].Value = urun.GoTurc_BrandId?.ToString() ?? "";
                    worksheet.Cells[row, 59].Value = urun.GoTurc_DeliveryNo?.ToString() ?? "";
                    worksheet.Cells[row, 60].Value = urun.GoTurc_SalePrice?.ToString("F2") ?? "";
                    worksheet.Cells[row, 61].Value = urun.GoTurc_StockQuantity?.ToString() ?? "";

                    worksheet.Cells[row, 62].Value = urun.Trendyol_CategoryId?.ToString() ?? "";
                    worksheet.Cells[row, 63].Value = urun.Trendyol_BrandId?.ToString() ?? "";
                    worksheet.Cells[row, 64].Value = urun.Trendyol_SalePrice?.ToString("F2") ?? "";
                    worksheet.Cells[row, 65].Value = urun.Trendyol_StockQuantity?.ToString() ?? "";
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Urun_Bilgileri.xlsx");
            }
        }

        public IActionResult Sil(int id)
        {
            UnifiedMarketplace unifiedMarketplace = unifiedMarketplaceRepository.Getir(x => x.Durumu == 1);
            unifiedMarketplace.Durumu = 0;
            unifiedMarketplace.GuncellenmeTarihi = DateTime.Now;
            unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
            return RedirectToAction("Index", "PazarYerleriUrun");
        }
        [HttpPost]
        public IActionResult BulkDelete(List<int> idList)
        {
            if (idList == null || idList.Count == 0)
            {
                TempData["ErrorMessage"] = "Silmek için en az bir ürün seçmelisiniz.";
                return RedirectToAction("Index", "PazarYerleriUrun");
            }

            foreach (var id in idList)
            {
                var unifiedMarketplace = unifiedMarketplaceRepository.Getir(x => x.Id == id && x.Durumu == 1);
                if (unifiedMarketplace != null)
                {
                    unifiedMarketplace.Durumu = 0;
                    unifiedMarketplace.GuncellenmeTarihi = DateTime.Now;
                    unifiedMarketplaceRepository.Guncelle(unifiedMarketplace);
                }
            }

            TempData["SuccessMessage"] = $"{idList.Count} ürün başarıyla silindi.";
            return RedirectToAction("Index", "PazarYerleriUrun");
        }
    }
}