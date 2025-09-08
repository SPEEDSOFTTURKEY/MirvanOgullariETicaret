using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class UrunAltKategori
{
    public int Id { get; set; }

    public string? Adi { get; set; }

    public DateTime? EklenmeTarihi { get; set; }

    public DateTime? GuncellenmeTarihi { get; set; }

    public int? Durumu { get; set; }
    public int ? UrunAnaKategoriId { get; set; }
    public virtual ICollection<Urun> Uruns { get; set; } = new List<Urun>();
    public virtual UrunAnaKategori? UrunAnaKategori { get; set; }
    public List<UrunAltKategoriFotograf> UrunAltKategoriFotograf { get; set; }

    public virtual List<UrunStok> UrunStoklari { get; set; }

}
