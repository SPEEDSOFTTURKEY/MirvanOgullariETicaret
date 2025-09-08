using Iyzipay.Model;

namespace WebApp.Models
{
	public class SiparisVeSepet
	{
		public Siparis Siparis { get; set; }
		public List<Sepet> Sepet { get; set; }
		public UyeAdres UyeAdres { get; set; }
	    public string OdenecekTutar { get; set; }
	    public string uyeId { get; set; }
	    public string? indirimId { get; set; }
		public Odeme Odeme { get; set; }
		
	}
}
