using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class GoTurcCategory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<GoTurcFilter> Filters { get; set; }
        public List<GoTurcCategory> SubCategory { get; set; }
    }

    public class GoTurcFilter
    {
        public long Id { get; set; }
        public string Label { get; set; }
        public bool IsMandatoryField { get; set; }
        public List<GoTurcFilterAttribute> Attrs { get; set; }
    }

    public class GoTurcFilterAttribute
    {
        public long Id { get; set; }
        public string Label { get; set; }
    }

    public class GoTurcBrand
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class GoTurcBrandListResponse
    {
        public int Total { get; set; }
        public List<GoTurcBrand> Brands { get; set; }
    }

    public class GoTurcDeliveryMethod
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class GoTurcProductRequest
    {
        [Required]
        public long? CategoryId { get; set; }
        [Required]
        public long? BrandId { get; set; }
        [Required, StringLength(65, MinimumLength = 5)]
        public string? Title { get; set; }
        [StringLength(65)]
        public string? SubTitle { get; set; }
        [Required]
        public string? ShopProductCode { get; set; }
        public bool? IsDomesticProduction { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? ProductState { get; set; } = 1;
        public int? PreparationDay { get; set; } = 2;
        public bool? IsActive { get; set; } = true;
        [Required]
        public int? DeliveryNo { get; set; }
        public List<GoTurcImageUrl>? ImageUrls { get; set; }
        public List<GoTurcAttribute>? Attrs { get; set; }
        public List<GoTurcSalesInfo>? SalesInfo { get; set; }
        public int? DiscountType { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }
        public int? MaxCustomerQuantity { get; set; }
        public string? DescriptionHTML { get; set; }
        public string? DeliveryDescription { get; set; }
        public string? ReturnExchangeInformation { get; set; }
        public string? AfterSalesService { get; set; }
    }

    public class GoTurcImageUrl
    {
        public int RowNumber { get; set; }
        public string ImageUrl { get; set; }
    }

    public class GoTurcAttribute
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class GoTurcSalesInfo
    {
        public string CurrencyUnit { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public List<GoTurcVariant> Variants { get; set; }
    }

    public class GoTurcVariant
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class GoTurcStockPriceRequest
    {
        [Required]
        public string ShopProductCode { get; set; }
        [Required]
        public List<GoTurcSalesInfo> SalesInfo { get; set; }
    }

    public class GoTurcProductResponse
    {
        public string ReportId { get; set; }
        public string Error { get; set; }
    }

    public class GoTurcProduct
    {
        public string? ShopProductCode { get; set; }
        public string? Title { get; set; }
        public string SubTitle { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public string DescriptionHTML { get; set; }
        public List<string> ImageUrl { get; set; } // çoklu resim desteği
        public string Status { get; set; }
        public long CategoryId { get; set; }
        public long BrandId { get; set; }
        public int DeliveryNo { get; set; }
    }

    public class GoTurcProductListResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<GoTurcProduct> Products { get; set; }
    }
}