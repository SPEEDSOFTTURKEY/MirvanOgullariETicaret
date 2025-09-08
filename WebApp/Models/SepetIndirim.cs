namespace WebApp.Models
{
	public class SepetIndirim
	{
		public int Id { get; set; }
		public decimal SepetTutari { get; set; }
		public decimal IndirimMiktari { get; set; }
		public DateTime EklemeTarihi { get; set; }
		public DateTime GuncellemeTarihi { get; set; }
		public int Durumu { get; set; }
	}
}
