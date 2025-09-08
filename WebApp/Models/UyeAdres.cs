namespace WebApp.Models
{
	public class UyeAdres
	{
        public int Id { get; set; }
        public string? Adi { get; set; }
        public string? Soyadi { get; set; }
        public string? Telefon { get; set; }
        public string? PostaKodu { get; set; }
        public string? Ulke { get; set; }
        public string? il { get; set; }
        public string? ilce { get; set; }
        public string? Mahalle { get; set; }
        public string? Adres { get; set; }
        public string? AdresBasligi { get; set; }
        public string? FaturaTuru { get; set; }
        public int  UyeId { get; set; }
        public int?  Varsayılan { get; set; }
        public int  Durumu { get; set; }
        public DateTime  EklenmeTarihi { get; set; }
        public DateTime  GuncellenmeTarihi { get; set; }

        public virtual Uyeler? Uye { get; set; } // Made nullable to match EF conventions    

    }
}
