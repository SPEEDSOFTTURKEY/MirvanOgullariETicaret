namespace WebApp.Models
{
    public partial class EDMBilgileri
    {
        public int Id { get; set; }
        public string Unvan { get; set; }
        public string VergiDairesi { get; set; }
        public long VergiNumarasi { get; set; }
        public string Adres { get; set; }
        public string Il { get; set; }
        public string Ilce { get; set; }
        public string PostaKodu { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
        public DateTime? EklenmeTarihi { get; set; }
        public DateTime? GuncellenmeTarihi { get; set; }
        public int? Durumu { get; set; }
    }
}
