namespace WebApp.Models
{


    public class PazaramaProductListResponse
    {
        public List<PazaramaProduct> Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
    public class PazaramaCategoryListResponse
    {
        public List<PazaramaCategory> Data { get; set; }
    }

    public class PazaramaBrandListResponse
    {
        public List<PazaramaBrand> Data { get; set; }
    }
 
    public class PazaramaProduct
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string BrandId { get; set; }
        public string StockCode { get; set; }
        public int StockCount { get; set; }
        public decimal SalePrice { get; set; }
        public decimal ListPrice { get; set; }
        public int VatRate { get; set; }
        public string Description { get; set; }
        public List<object> Attributes { get; set; }
        public List<object> Images { get; set; }
        public string Status { get; set; }
    }
    public class PazaramaCategory
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public bool Leaf { get; set; }
        public List<PazaramaCategory> SubCategories { get; internal set; }
    }

    public class PazaramaBrand
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class PazaramaPriceUpdateRequest
    {
        public string Code { get; set; }
        public decimal SalePrice { get; set; }
        public decimal ListPrice { get; set; }
    }

    public class PazaramaStockUpdateRequest
    {
        public string Code { get; set; }
        public int StockCount { get; set; }
    }

    public class CategoryDto
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool isLeaf { get; internal set; }
        public List<CategoryDto> subCategories { get; internal set; }
    }

    public class BrandDto
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}