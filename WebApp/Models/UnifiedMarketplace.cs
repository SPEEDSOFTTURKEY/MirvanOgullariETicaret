using System.ComponentModel.DataAnnotations;
using WebApp.Models;

public class UnifiedMarketplace
{
    [Key]
    public int Id { get; set; }
    public int? UrunId { get; set; }
    public int? Durumu { get; set; }
    public string? UrunGorsel { get; set; }
    public DateTime GuncellenmeTarihi  { get; set; }

    public string? Title { get; set; }
    public string? ProductCode { get; set; }
    public string? Barcode { get; set; }
    public string? Description { get; set; }
    public decimal? ListPrice { get; set; }
    public int? VatRate { get; set; }

    // N11
    public long? N11_CategoryId { get; set; }
    public long? N11_ReportId { get; set; }
    public decimal? N11_SalePrice { get; set; }
    public int? N11_StockQuantity { get; set; }
    public bool? N11_Success { get; set; }
    public string? N11_Message { get; set; }
    public DateTime? N11_EklenmeTarihi { get; set; }
    public DateTime? N11_GuncellenmeTarihi { get; set; }
    public int? N11_Gonderildi { get; set; }





    // GoTurc
    public long? GoTurc_CategoryId { get; set; }
    public long? GoTurc_BrandId { get; set; }
    public long? GoTurc_DeliveryNo { get; set; }
    public decimal? GoTurc_SalePrice { get; set; }
    public int? GoTurc_StockQuantity { get; set; }
    public bool? GoTurc_Success { get; set; }
    public string? GoTurc_Message { get; set; }
    public string? GoTurc_ReportId { get; set; }
    public DateTime? GoTurc_EklenmeTarihi { get; set; }
    public DateTime? GoTurc_GuncellenmeTarihi { get; set; }
    public int? GoTurc_Gonderildi { get; set; }





    // Idefix
    public long? Idefix_CategoryId { get; set; }
    public long? Idefix_BrandId { get; set; }
    public decimal? Idefix_SalePrice { get; set; }
    public int? Idefix_StockQuantity { get; set; }
    public string? Idefix_VendorStockCode { get; set; }
    public decimal? Idefix_Weight { get; set; }
    public decimal? Idefix_ComparePrice { get; set; }
    public string? Idefix_ProductMainId { get; set; }
    public int? Idefix_DeliveryDuration { get; set; }
    public int? Idefix_CargoCompanyId { get; set; }
    public int? Idefix_ShipmentAddressId { get; set; }
    public int? Idefix_ReturnAddressId { get; set; }
    public string? Idefix_AttributesJson { get; set; }
    public bool? Idefix_Success { get; set; }
    public string? Idefix_Message { get; set; }
    public string? Idefix_ReportId { get; set; }
    public DateTime? Idefix_EklenmeTarihi { get; set; }
    public DateTime? Idefix_GuncellenmeTarihi { get; set; }
    public int? Idefix_Gonderildi { get; set; }





    // PTT
    public long? PTT_CategoryId { get; set; }
    public decimal? PTT_SalePrice { get; set; }
    public decimal? PTT_PriceWithVat { get; set; }
    public int? PTT_StockQuantity { get; set; }
    public decimal? PTT_Desi { get; set; }
    public decimal? PTT_Discount { get; set; }
    public int? PTT_EstimatedCourierDelivery { get; set; }
    public int? PTT_WarrantyDuration { get; set; }
    public int? PTT_CargoProfileId { get; set; }
    public int? PTT_BasketMaxQuantity { get; set; }
    public string? PTT_VariantsJson { get; set; }
    public bool? PTT_Success { get; set; }
    public string? PTT_Message { get; set; }
    public string? PTT_TrackingId { get; set; }
    public DateTime? PTT_EklenmeTarihi { get; set; }
    public DateTime? PTT_GuncellenmeTarihi { get; set; }
    public int? PTT_Gonderildi { get; set; }






    // Pazarama
    public long? Pazarama_CategoryId { get; set; }
    public long? Pazarama_BrandId { get; set; }
    public decimal? Pazarama_SalePrice { get; set; }
    public int? Pazarama_StockQuantity { get; set; }
    public bool? Pazarama_Success { get; set; }
    public string? Pazarama_Message { get; set; }
    public string? Pazarama_ReportId { get; set; }
    public DateTime? Pazarama_EklenmeTarihi { get; set; }
    public DateTime? Pazarama_GuncellenmeTarihi { get; set; }
    public int? Pazarama_Gonderildi { get; set; }





    // ÇiçekSepeti
    public long? CicekSepeti_CategoryId { get; set; }
    public decimal? CicekSepeti_SalePrice { get; set; }
    public int? CicekSepeti_StockQuantity { get; set; }
    public string? CicekSepeti_StockCode { get; set; }
    public string? CicekSepeti_CategoryName { get; set; }
    public string? CicekSepeti_ProductStatusType { get; set; }
    public string? CicekSepeti_VariantName { get; set; }
    public string? CicekSepeti_ReportId { get; set; }
    public bool? CicekSepeti_Success { get; set; }
    public string? CicekSepeti_Message { get; set; }
    public DateTime? CicekSepeti_EklenmeTarihi { get; set; }
    public DateTime? CicekSepeti_GuncellenmeTarihi { get; set; }
    public int? CicekSepeti_Gonderildi { get; set; }




    // Farmazon
    public long? Farmazon_Id { get; set; }
    public decimal? Farmazon_Price { get; set; }
    public int? Farmazon_Stock { get; set; }
    public int? Farmazon_State { get; set; }
    public string? Farmazon_Expiration { get; set; }
    public int? Farmazon_MaxCount { get; set; }
    public bool? Farmazon_IsFeatured { get; set; }
    public bool? Farmazon_FarmazonEnabled { get; set; }
    public string? Farmazon_StockCode { get; set; }
    public string? Farmazon_ReportId { get; set; }
    public bool? Farmazon_Success { get; set; }
    public string? Farmazon_Message { get; set; }
    public DateTime? Farmazon_EklenmeTarihi { get; set; }
    public DateTime? Farmazon_GuncellenmeTarihi { get; set; }
    public int? Farmazon_Gonderildi { get; set; }





    // FarmaBorsa
    public decimal? FarmaBorsa_SalePrice { get; set; }
    public int? FarmaBorsa_StockQuantity { get; set; }
    public int? FarmaBorsa_MaxQuantity { get; set; }
    public string? FarmaBorsa_ExpirationDate { get; set; }
    public bool? FarmaBorsa_ShowInBorsa { get; set; }
    public bool? FarmaBorsa_FarmaBorsaEnabled { get; set; }
    public int? FarmaBorsa_MiadTip { get; set; }
    public bool? FarmaBorsa_Success { get; set; }
    public string? FarmaBorsa_Message { get; set; }
    public DateTime? FarmaBorsa_EklenmeTarihi { get; set; }
    public DateTime? FarmaBorsa_GuncellenmeTarihi { get; set; }
    public int? FarmaBorsa_Gonderildi { get; set; }





    // Trendyol
    public long? Trendyol_CategoryId { get; set; }
    public long? Trendyol_BrandId { get; set; }
    public decimal? Trendyol_SalePrice { get; set; }
    public int? Trendyol_StockQuantity { get; set; }
    public bool? Trendyol_Success { get; set; }
    public string? Trendyol_Message { get; set; }
    public DateTime? Trendyol_EklenmeTarihi { get; set; }
    public DateTime? Trendyol_GuncellenmeTarihi { get; set; }
    public int? Trendyol_Gonderildi { get; set; }





    public string? ImgFilesJson { get; set; }
    public int Gonderildi { get; internal set; }

    public List<PazarYeriGorsel> PazarYeriGorsel { get; set; }
}
