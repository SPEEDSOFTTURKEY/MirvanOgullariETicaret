using Newtonsoft.Json;
using System.Collections.Generic;

namespace WebApp.Models
{
    public class CicekSepetiDataResponse
    {
        [JsonProperty("products")]
        public List<ProductCicekSepeti> ProductCicekSepeti { get; set; }
    }

    public class ProductCicekSepeti
    {
        [JsonProperty("productName")]
        public string ProductName { get; set; }
        [JsonProperty("productCode")]
        public string ProductCode { get; set; }
        [JsonProperty("stockCode")]
        public string StockCode { get; set; }
        [JsonProperty("isActive")]
        public bool IsActive { get; set; }
        [JsonProperty("categoryId")]
        public long CategoryId { get; set; }
        [JsonProperty("categoryName")]
        public string CategoryName { get; set; }
        [JsonProperty("productStatusType")]
        public string ProductStatusType { get; set; }
        [JsonProperty("isUseStockQuantity")]
        public bool IsUseStockQuantity { get; set; }
        [JsonProperty("stockQuantity")]
        public int StockQuantity { get; set; }
        [JsonProperty("salesPrice")]
        public double SalesPrice { get; set; }
        [JsonProperty("listPrice")]
        public double ListPrice { get; set; }
        [JsonProperty("barcode")]
        public string Barcode { get; set; }
        [JsonProperty("commissionRate")]
        public string CommissionRate { get; set; }
        [JsonProperty("numberOfFavorites")]
        public int NumberOfFavorites { get; set; }
        [JsonProperty("variantName")]
        public string VariantName { get; set; }
        [JsonProperty("images")]
        public List<string> Images { get; set; }
    }

    public class CicekSepetiCategoryResponse
    {
        [JsonProperty("categories")]
        public List<CicekSepetiCategory> Categories { get; set; }
    }

    public class CicekSepetiCategory
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("parentCategoryId")]
        public int? ParentCategoryId { get; set; }
        [JsonProperty("subCategories")]
        public List<CicekSepetiCategory> SubCategories { get; set; }
    }
}