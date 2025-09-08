namespace WebApp.Models
{
    public class Favoriler
    {
        public int Id { get; set; }
        public int UyeId { get; set; }
        public int UrunId { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime GuncellenmeTarihi { get; set; }
        public int Durumu { get; set; }
        public virtual Urun Urun { get; set; }
        public virtual Uyeler Uye { get; set; }
    }
}
