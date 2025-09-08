namespace WebApp.Models
{
	public class UrunGaleri
	{
		public int Id { get; set; }

		public string? FotografBuyuk { get; set; }

		public string? FotografKucuk { get; set; }

		public int? Durumu { get; set; }

		public int? UrunId { get; set; }

		public DateTime? EklenmeTarihi { get; set; }

		public DateTime? GuncellenmeTarihi { get; set; }

		public virtual Urun? Urun { get; set; }

	}
}
