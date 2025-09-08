using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class UrunFotograf
{
    public int Id { get; set; }

    public string? FotografBuyuk { get; set; }

    public string? FotografKucuk { get; set; }

    public int? Durumu { get; set; }

    public int? UrunId { get; set; }
    public int? RenkId { get; set; }

    public DateTime? EklenmeTarihi { get; set; }

    public DateTime? GuncellenmeTarihi { get; set; }

    public int? VitrinMi { get; set; }

    public virtual Urun? Urun { get; set; }
    public virtual Renkler? Renk { get; set; }
}
