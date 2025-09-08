//using Newtonsoft.Json;

//namespace WebApp.Models
//{
//    public class FarmazonProductRequest
//    {
//        [JsonProperty("id")]
//        public long Id { get; set; }

//        [JsonProperty("price")]
//        public decimal Price { get; set; }

//        [JsonProperty("stock")]
//        public int Stock { get; set; }

//        [JsonProperty("state")]
//        public int State { get; set; } = 1;

//        [JsonProperty("expiration")]
//        public string Expiration { get; set; }

//        [JsonProperty("maxCount")]
//        public int? MaxCount { get; set; }

//        [JsonProperty("description")]
//        public string Description { get; set; } = "";

//        [JsonProperty("isFeatured")]
//        public bool IsFeatured { get; set; }

//        [JsonProperty("product")]
//        public FarmazonProductDetails Product { get; set; }
//    }

//    public class FarmazonPriceStockRequest
//    {
//        [JsonProperty("id")]
//        public long Id { get; set; }

//        [JsonProperty("price")]
//        public decimal Price { get; set; }

//        [JsonProperty("stock")]
//        public int Stock { get; set; }
//    }

//    public class FarmazonProductDetails
//    {
//        [JsonProperty("id")]
//        public int Id { get; set; }

//        [JsonProperty("name")]
//        public string Name { get; set; }

//        [JsonProperty("barcode")]
//        public string Barcode { get; set; }

//        [JsonProperty("psf")]
//        public decimal? Psf { get; set; }

//        [JsonProperty("vat")]
//        public int? Vat { get; set; }

//        [JsonProperty("sku")]
//        public string Sku { get; set; }

//        [JsonProperty("image")]
//        public string Image { get; set; }
//    }

//    public class FarmazonProductResponse
//    {
//        [JsonProperty("statusCode")]
//        public int StatusCode { get; set; }

//        [JsonProperty("statusMessage")]
//        public string StatusMessage { get; set; }

//        [JsonProperty("result")]
//        public List<FarmazonProductResultItem> Result { get; set; }

//        [JsonProperty("errors")]
//        public List<FarmazonError> Errors { get; set; }
//    }

//    public class FarmazonPriceStockResponse
//    {
//        [JsonProperty("statusCode")]
//        public int StatusCode { get; set; }

//        [JsonProperty("statusMessage")]
//        public string StatusMessage { get; set; }
//    }

//    public class FarmazonProductResultItem
//    {
//        [JsonProperty("requestItem")]
//        public FarmazonProductRequest RequestItem { get; set; }

//        [JsonProperty("success")]
//        public bool Success { get; set; }

//        [JsonProperty("errors")]
//        public List<FarmazonError> Errors { get; set; }
//    }

//    public class FarmazonError
//    {
//        [JsonProperty("statusCode")]
//        public int StatusCode { get; set; }

//        [JsonProperty("message")]
//        public string Message { get; set; }

//        [JsonProperty("messageGroup")]
//        public string MessageGroup { get; set; }
//    }

//    public class FarmazonProductListResponse
//    {
//        [JsonProperty("success")]
//        public bool Success { get; set; }

//        [JsonProperty("message")]
//        public string Message { get; set; }

//        [JsonProperty("result")]
//        public FarmazonListingsResult Result { get; set; }
//    }

//    public class FarmazonListingsResult
//    {
//        [JsonProperty("items")]
//        public List<FarmazonProductRequest> Items { get; set; }
//    }

//    public class FarmazonLoginResponse
//    {
//        [JsonProperty("result")]
//        public FarmazonLoginResult Result { get; set; }
//    }

//    public class FarmazonLoginResult
//    {
//        [JsonProperty("token")]
//        public string Token { get; set; }
//    }
//}