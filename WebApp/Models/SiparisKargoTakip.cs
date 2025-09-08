namespace WebApp.Models
{
	public class SiparisKargoTakip
	{
		public int Id { get; set; }
		public string? KargoTakipNumarasi { get; set; }
		public string? KargoFirmasi { get; set; }
		public int UyelerId { get; set; }
		public int SiparisId { get; set; }
		public string? Aciklama {  get; set; }
		public DateTime? EklenmeTarihi { get; set; }
		public DateTime? GuncellenmeTarihi { get; set; }
		public int? Durumu { get; set; }
		public virtual Uyeler? Uyeler { get; set; }
		public virtual Siparis? Siparis { get; set; }


	}
}
