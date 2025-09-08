using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class UnifiedMarketplaceModel
    {
        public string Title { get; set; }
        public int UrunId { get; set; }
        public string ProductCode { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public List<IFormFile> ImgFiles { get; set; } = new List<IFormFile>();
        public decimal? ListPrice { get; set; }
        public int? VatRate { get; set; }

        public N11UrunModel N11 { get; set; } = new N11UrunModel();
        public GoTurcMarketplaceModel GoTurc { get; set; } = new GoTurcMarketplaceModel();
        public IdefixUrunModel Idefix { get; set; } = new IdefixUrunModel();
        public PTTUrunModel PTT { get; set; } = new PTTUrunModel();
        public PazaramaUrunModel Pazarama { get; set; } = new PazaramaUrunModel();
        public CicekSepetiUrunModel CicekSepeti { get; set; } = new CicekSepetiUrunModel();
        public FarmazonUrunModel Farmazon { get; set; } = new FarmazonUrunModel();
        public FarmaBorsaUrunModel FarmaBorsa { get; set; } = new FarmaBorsaUrunModel();
        public TrendyolUrunModel Trendyol { get; set; } = new TrendyolUrunModel();
    }

    public class N11UrunModel
    {
        public long? CategoryId { get; set; }
        public decimal? SalePrice { get; set; }
        public int? StockQuantity { get; set; }
    }

    public class GoTurcMarketplaceModel
    {
        public long? CategoryId { get; set; }
        public long? BrandId { get; set; }
        public long? DeliveryNo { get; set; }
        public decimal? SalePrice { get; set; }
        public int? StockQuantity { get; set; }
    }

    public class IdefixUrunModel
    {
        public long? CategoryId { get; set; }
        public long? BrandId { get; set; }
        public decimal? SalePrice { get; set; }
        public int? StockQuantity { get; set; }
        public string VendorStockCode { get; set; }
        public decimal? Weight { get; set; }
        public decimal? ComparePrice { get; set; }
        public string ProductMainId { get; set; }
        public int? DeliveryDuration { get; set; }
        public int? CargoCompanyId { get; set; }
        public int? ShipmentAddressId { get; set; }
        public int? ReturnAddressId { get; set; }

        public List<IdefixAttribute> Attributes { get; set; } // Yeni eklenen özellik listesi
    }

    public class IdefixAttribute
    {
        public long AttributeId { get; set; }
        public long AttributeValueId { get; set; }
        public string CustomAttributeValue { get; set; } // Opsiyonel
    }

    public class PTTUrunModel
    {
        public long? CategoryId { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? PriceWithVat { get; set; }
        public int? StockQuantity { get; set; }
        public decimal? Desi { get; set; }
        public decimal? Discount { get; set; }
        public int? EstimatedCourierDelivery { get; set; }
        public int? WarrantyDuration { get; set; }
        public int? CargoProfileId { get; set; }
        public int? BasketMaxQuantity { get; set; }
        public List<PTTVariantModel> Variants { get; set; } = new List<PTTVariantModel>();
    }

    public class PTTVariantModel
    {
        public string Barcode { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public List<PTTVariantAttributeModel> Attributes { get; set; } = new List<PTTVariantAttributeModel>();
    }

    public class PTTVariantAttributeModel
    {
        public string Definition { get; set; }
        public string Value { get; set; }
    }

    public class PazaramaUrunModel
    {
        public long? CategoryId { get; set; }
        public long? BrandId { get; set; }
        public decimal? SalePrice { get; set; }
        public int? StockQuantity { get; set; }
    }

    public class CicekSepetiUrunModel
    {
        public long? CategoryId { get; set; }
        public decimal? SalePrice { get; set; }
        public int? StockQuantity { get; set; }
        public string StockCode { get; set; }
        public string CategoryName { get; set; }
        public string ProductStatusType { get; set; }
        public string VariantName { get; set; }
    }

    public class FarmazonUrunModel
    {
        public long Id { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int State { get; set; } = 1;
        public string Expiration { get; set; }
        public int? MaxCount { get; set; }
        public bool IsFeatured { get; set; }
        public bool FarmazonEnabled { get; set; }
        public string StockCode { get; set; }
     

    }

    public class FarmazonProductDetails
    {
        public int Id { get; set; }
        public decimal? Psf { get; set; }
       
    }

    public class FarmaBorsaUrunModel
    {
        public decimal? SalePrice { get; set; }
        public int? StockQuantity { get; set; }
        public int? MaxQuantity { get; set; }
        public string ExpirationDate { get; set; }
        public bool ShowInBorsa { get; set; }
        public bool FarmaBorsaEnabled { get; set; }
        public int? MiadTip { get; set; }
    }

    public class TrendyolUrunModel
    {
        public long? CategoryId { get; set; }
        public long? BrandId { get; set; }
        public decimal? SalePrice { get; set; }
        public int? StockQuantity { get; set; }
    }

    public class FarmazonProductRequest
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("price")]
        public decimal? Price { get; set; }

        [JsonProperty("stock")]
        public int? Stock { get; set; }

        [JsonProperty("state")]
        public int? State { get; set; } = 1;

        [JsonProperty("expiration")]
        public string? Expiration { get; set; }

        [JsonProperty("maxCount")]
        public int? MaxCount { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; } = "";

        [JsonProperty("isFeatured")]
        public bool? IsFeatured { get; set; }
        public int? Vat { get; set; }
        public string Sku { get; set; }


    }

    public class FarmazonPriceStockRequest
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("stock")]
        public int Stock { get; set; }
    }

    public class FarmazonProductResponse
    {
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("statusMessage")]
        public string StatusMessage { get; set; }

        [JsonProperty("result")]
        public List<FarmazonProductResultItem> Result { get; set; }

        [JsonProperty("errors")]
        public List<FarmazonError> Errors { get; set; }
    }

    public class FarmazonPriceStockResponse
    {
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("statusMessage")]
        public string StatusMessage { get; set; }
    }

    public class FarmazonProductResultItem
    {
        [JsonProperty("requestItem")]
        public FarmazonProductRequest RequestItem { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("errors")]
        public List<FarmazonError> Errors { get; set; }
    }

    public class FarmazonError
    {
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("messageGroup")]
        public string MessageGroup { get; set; }
    }

    public class FarmazonProductListResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("result")]
        public FarmazonListingsResult Result { get; set; }
    }

    public class FarmazonListingsResult
    {
        [JsonProperty("items")]
        public List<FarmazonProductRequest> Items { get; set; }
    }

    public class FarmazonLoginResponse
    {
        [JsonProperty("result")]
        public FarmazonLoginResult Result { get; set; }
    }

    public class FarmazonLoginResult
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}