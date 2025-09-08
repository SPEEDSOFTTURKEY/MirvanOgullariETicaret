namespace WebApp.Models
{
    public class AcilKargoUcret
    {
        public int Id { get; set; }
        public decimal Fiyat { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime GuncellenmeTarihi { get; set; }
        public int Durumu { get; set; }
    }
}
