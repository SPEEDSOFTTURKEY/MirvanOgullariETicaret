namespace WebApp.Models
{
    public class SiparisDetay
    {
        public int Id { get; set; }
        public int SiparisId { get; set; }
        public int UrunId { get; set; }
        public int BedenId { get; set; }
        public int Durumu { get; set; }
        public int Miktar { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal? ToplamFiyat { get; set; }
        public DateTime EklenmeTarihi { get; set; } 
    
    }
}
