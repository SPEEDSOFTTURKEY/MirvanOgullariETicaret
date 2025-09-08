namespace WebApp.Models
{
	public class Odeme
	{
		public int Id { get; set; }
		public string PaymentId {get;set;}
		public string OdemeTutari { get; set;}
		public string OdemeDurumu { get; set;}
		public DateTime EklenmeTarihi { get; set; }
		public string BuyerSurname {  get; set; }
		public string BuyerGsmNumber { get; set; }
		public string BuyerName { get; set; }

	}
}
