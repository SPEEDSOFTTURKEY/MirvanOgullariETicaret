namespace WebApp.Models
{
    public class IdefixCategory
    {
        public long Id { get; set; }
        public long ParentId { get; set; }
        public string Name { get; set; }
        public long TopCategory { get; set; }
        public List<IdefixCategory> Subs { get; set; }
        public List<IdefixAttribute> Attributes { get; set; }
    }
    public class IdefixAttributee
    {
        public long AttributeId { get; set; }
        public long AttributeValueId { get; set; }
        public string CustomAttributeValue { get; set; } // Opsiyonel, null olabilir
    }
    public class IdefixBrand
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public bool ExclusiveBrand { get; set; }
        public string Logo { get; set; }
        public bool BookPublisher { get; set; }
    }

    public class IdefixCargoCompany
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public string TaxNumber { get; set; }
    }

    public class IdefixVendorProfile
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public string Title { get; set; }
        public IdefixCargoCompany CargoCompany { get; set; }
        public bool Status { get; set; }
        public bool IsPlatformIntegrated { get; set; }
        public string CargoIntegrationUrl { get; set; }
        public string CargoTrackingUrl { get; set; }
        public List<string> CargoUserCredential { get; set; }
        public bool IsPlatformTrackingSupport { get; set; }
        public bool IsPlatformAgreementSupport { get; set; }
        public bool IsSellerAgreementSupport { get; set; }
        public bool IsPlatformCargoSend { get; set; }
        public bool IsSellerCargoSend { get; set; }
        public bool AcceptReturn { get; set; }
        public bool FullCoverage { get; set; }
        public bool AcceptHomeReturn { get; set; }
        public string StartAt { get; set; }
    }

    public class IdefixProductRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string ProductMainId { get; set; }
        public string Barcode { get; set; }
        public string VendorStockCode { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public decimal? ComparePrice { get; set; }
        public int VatRate { get; set; }
        public int? DeliveryDuration { get; set; }
        public string DeliveryType { get; set; }
        public int? CargoCompanyId { get; set; }
        public int? ShipmentAddressId { get; set; }
        public int? ReturnAddressId { get; set; }
        public decimal? Desi { get; set; }
        public decimal? Weight { get; set; }
        public List<string> Images { get; set; }
        public int? OriginCountryId { get; set; }
        public string Manufacturer { get; set; }
        public string Importer { get; set; }
        public bool? CeCompatibility { get; set; }
        public string UsageInstructionsImage { get; set; }
        public string PackageFrontImage { get; set; }
        public string PackageBackImage { get; set; }
        public string ProductInfoFormImage { get; set; }
        public string EnergyClassImage { get; set; }
        public string EnergyNutritionImage { get; set; }
    }
}