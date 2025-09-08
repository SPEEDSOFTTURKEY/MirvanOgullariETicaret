namespace WebApp.Models
{
    public class PTTUrun
    {
        public string Name { get; set; }
        public string Barcode { get; set; }
        public decimal Price { get; set; }
        public decimal? PriceWithVat { get; set; }
        public int CategoryId { get; set; }
        public double? Desi { get; set; }
        public int Quantity { get; set; }
        public int VATRate { get; set; }
        public string ImageUrl { get; set; }
        public string LongDescription { get; set; }
        public string ShortDescription { get; set; }
        public bool? NoShippingProduct { get; set; }
        public bool? Active { get; set; }
        public int? BasketMaxQuantity { get; set; }
        public int? CargoProfileId { get; set; }
        public decimal? Discount { get; set; }
        public int? EstimatedCourierDelivery { get; set; }
        public string Gtin { get; set; }
        public bool? SingleBox { get; set; }
        public int? WarrantyDuration { get; set; }
        public string WarrantySupplier { get; set; }
        public List<PTTVaryant> Variants { get; set; }
    }

    public class PTTVaryant
    {
        public string Name { get; set; }
        public string Barcode { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public List<PTTVaryantAttribute> Attributes { get; set; }
    }

    public class PTTVaryantAttribute
    {
        public string Definition { get; set; }
        public string Value { get; set; }
    }

        public class PTTCategory
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int? ParentId { get; set; }
            public string UpdatedAt { get; set; }
            public List<PTTCategory> Children { get; set; }
        }

        public class PTTCategoryResponse
        {
            public bool Success { get; set; }
            public List<PTTCategory> CategoryTree { get; set; }
            public string Error { get; set; }
        }

        public class PTTProductResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string TrackingId { get; set; }
            public int CountOfProductsToBeProcessed { get; set; }
        }
    }
