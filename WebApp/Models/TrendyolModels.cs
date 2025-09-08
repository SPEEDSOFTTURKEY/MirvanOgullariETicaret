using Newtonsoft.Json;

namespace WebApp.Models
{
    public class TrendyolProductRequest
    {
        public string Title { get; set; }
        public string Barcode { get; set; }
        public int StockQuantity { get; set; }
        public double SalePrice { get; set; }
        public double ListPrice { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public long CategoryId { get; set; }
        public long ProductId { get; set; }
        public int VatRate { get; set; }

    }

    public class TrendyolProductResponse
    {
        public List<TrendyolProduct> Content { get; set; }
        public int TotalCount { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public int ProductId { get; set; }
        [JsonProperty("batchRequestId")]
        public string BatchRequestId { get; set; }
    }
 


    public class TrendyolProduct
    {
        public string ProductId { get; set; }
        public string Title { get; set; }
        public string Barcode { get; set; }
        public int StockQuantity { get; set; }
        public double SalePrice { get; set; }
        public double ListPrice { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Status { get; set; }
        public int CategoryId { get; set; }
        public int VatRate { get; set; }
    }

    public class TrendyolProductListResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<TrendyolProduct> Content { get; set; }
    }
}