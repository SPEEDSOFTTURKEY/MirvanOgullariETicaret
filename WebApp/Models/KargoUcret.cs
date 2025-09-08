namespace WebApp.Models
{
	public class KargoUcret
	{
		public int Id { get; set; }
		public decimal SepetTutari { get; set; }
		public decimal KargoUcreti { get; set; }
		public DateTime EklemeTarihi { get; set; }
		public DateTime GuncellemeTarihi { get; set; }
		public int Durumu { get; set; }
	}
}
