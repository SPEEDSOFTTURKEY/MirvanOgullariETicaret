namespace WebApp.Models
{
    public class N11CategoryResponse
    {
        public List<N11Category> categories { get; set; }
    }

    public class CategoryAttributeValue
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
    }

    public class CategoryAttribute
    {
        public int AttributeId { get; set; }
        public int CategoryId { get; set; }
        public string AttributeName { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public bool IsVariant { get; set; }
        public bool IsSlicer { get; set; }
        public bool IsCustomValue { get; set; }
        public bool IsN11Grouping { get; set; }
        public int AttributeOrder { get; set; }
        public List<CategoryAttributeValue> AttributeValues { get; set; } = new();
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<CategoryAttribute> CategoryAttributes { get; set; } = new();
    }

    public class N11Category
    {
        public long id { get; set; }
        public long? parentId { get; set; } // Yeni eklenen alan
        public string name { get; set; }
        public List<N11Category> subCategories { get; set; }
    }

    public class N11ProductRequest
    {
        public string title { get; set; }
        public string description { get; set; }
        public long categoryId { get; set; }
        public string currencyType { get; set; }
        public string productMainId { get; set; }
        public bool deleteProductMainId { get; set; }
        public int preparingDay { get; set; }
        public string shipmentTemplate { get; set; }
        public int? maxPurchaseQuantity { get; set; }
        public bool deleteMaxPurchaseQuantity { get; set; }
        public string stockCode { get; set; }
        public string barcode { get; set; }
        public int quantity { get; set; }
        public List<string> Images { get; set; }
        public List<N11Attribute> attributes { get; set; }
        public double salePrice { get; set; }
        public double listPrice { get; set; }
        public int vatRate { get; set; }
        public string status { get; set; }
    }

    public class N11Attribute
    {
        public long id { get; set; }
        public long? valueId { get; set; }
        public string customValue { get; set; }
    }

    public class N11ProductResponse
    {
        public long id { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public List<string> reasons { get; set; }
    }

    public class ProductResponse
    {
        public List<Product> Content { get; set; }
        public Pageable Pageable { get; set; }
        public int TotalElements { get; set; }
        public int TotalPages { get; set; }
        public bool Last { get; set; }
        public int NumberOfElements { get; set; }
        public bool First { get; set; }
        public int Size { get; set; }
        public int Number { get; set; }
        public bool Empty { get; set; }
    }

    public class Product
    {
        public long N11ProductId { get; set; }
        public long SellerId { get; set; }
        public string SellerNickname { get; set; }
        public string StockCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long CategoryId { get; set; }
        public object ProductMainId { get; set; }
        public string Status { get; set; }
        public string SaleStatus { get; set; }
        public int PreparingDay { get; set; }
        public string ShipmentTemplate { get; set; }
        public int? MaxPurchaseQuantity { get; set; }
        public List<CustomTextOption> CustomTextOptions { get; set; }
        public long CatalogId { get; set; }
        public string Barcode { get; set; }
        public long GroupId { get; set; }
        public string CurrencyType { get; set; }
        public double SalePrice { get; set; }
        public double ListPrice { get; set; }
        public int Quantity { get; set; }
        public List<ProductAttribute> Attributes { get; set; }
        public List<string> ImageUrls { get; set; }
        public int VatRate { get; set; }
        public double CommissionRate { get; set; }
    }

    public class ProductAttribute
    {
        public long AttributeId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
    }

    public class CustomTextOption
    {
    }

    public class Pageable
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int Offset { get; set; }
        public bool Paged { get; set; }
        public bool Unpaged { get; set; }
    }
}