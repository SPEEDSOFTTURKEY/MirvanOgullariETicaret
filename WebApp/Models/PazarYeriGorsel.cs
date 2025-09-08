namespace WebApp.Models
{
    public class PazarYeriGorsel
    {
        public int Id { get; set; }
        public int UnifiedMarketplaceId { get; set; }
        public string FotografYolu { get; set; }
        public int Durumu { get; set; } 
        public DateTime EklenmeTarihi { get; set; } 
        public DateTime GuncellenmeTarihi { get; set; } 
        public UnifiedMarketplace UnifiedMarketplace { get; set; }
    }
}
