namespace WebApp.Models
{
	public class Uyeler
	{
		public int Id { get; set; }

		public string? Adi { get; set; }
		public string? Soyadi { get; set; }
		public string? Sifre { get; set; }
		public string? EMail { get; set; }
		public string? Telefon { get; set; }
        public int? Durumu { get; set; }
        public int? Turu { get; set; }
        public int? EBulten { get; set; }
        public Int64? TCKimlikNo { get; set; }

        public DateTime? EklenmeTarihi { get; set; }

        public DateTime? GuncellenmeTarihi { get; set; }
        public ICollection<UyeAdres> UyeAdres { get; set; } = new List<UyeAdres>();
    }
}
