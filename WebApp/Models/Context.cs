using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Models;

public partial class Context : DbContext
{
    public Context()
    {
    }

    public Context(DbContextOptions<Context> options)
        : base(options)
    {
    }
    public virtual DbSet<AnaSayfaFotograf> AnaSayfaFotograf { get; set; }
    public virtual DbSet<AnaSayfaRakamlari> AnaSayfaRakamlari { get; set; }
    public virtual DbSet<GaleriFotograf> GaleriFotograf { get; set; }
    public virtual DbSet<HakkimizdaBilgileri> HakkimizdaBilgileri { get; set; }
    public virtual DbSet<HakkimizdaFotograf> HakkimizdaFotograf { get; set; }
    public virtual DbSet<UrunBirim> UrunBirim { get; set; }
    public virtual DbSet<UrunStok> UrunStok { get; set; }
    public virtual DbSet<Favoriler> Favoriler { get; set; }
    public virtual DbSet<Renkler> Renkler { get; set; }
    public virtual DbSet<PazarYerleri> PazarYerleri { get; set; }
    public virtual DbSet<PazarYeriGirisBilgileri> PazarYeriGirisBilgileri { get; set; }
    public virtual DbSet<CariHesap> CariHesap { get; set; }
    public virtual DbSet<Depo> Depo { get; set; }
    public virtual DbSet<PazarYeriGorsel> PazarYeriGorsel { get; set; }
    public virtual DbSet<UnifiedMarketplace> UnifiedMarketplace { get; set; }
    public virtual DbSet<AcilKargoUcret> AcilKargoUcret { get; set; }
    public virtual DbSet<UyelikSozlesmesi> UyelikSozlesmesi { get; set; }
    public virtual DbSet<KullanimKosullari> KullanimKosullari { get; set; }
    public virtual DbSet<GizlilikPolitikasi> GizlilikPolitikasi { get; set; }
    public virtual DbSet<Iade> Iade { get; set; }
    public virtual DbSet<SiparisDetay> SiparisDetay { get; set; }
    public virtual DbSet<Bayilik> Bayilik { get; set; }
    public virtual DbSet<InvoiceRequestDto> InvoiceRequestDto { get; set; }

    public virtual DbSet<EDMBilgileri> EDMBilgileri { get; set; }
    public virtual DbSet<InvoiceLineDto> InvoiceLineDto { get; set; }
    public virtual DbSet<IletisimBilgileri> IletisimBilgileri { get; set; }
    public virtual DbSet<IletisimFotograf> IletisimFotograf { get; set; }
    public virtual DbSet<SiparisDurumlari> SiparisDurumlari { get; set; }
    public virtual DbSet<SiparisGuest> SiparisGuest { get; set; }
    public virtual DbSet<Kullanicilar> Kullanicilar { get; set; }
    public virtual DbSet<Birimler> Birimler { get; set; }
    public virtual DbSet<GuestSepet> GuestSepet { get; set; }
    public virtual DbSet<MisafirSiparisKargoTakip> MisafirSiparisKargoTakip { get; set; }

    public virtual DbSet<Urun> Urun { get; set; }
    public virtual DbSet<iller> iller { get; set; }
    public virtual DbSet<ilceler> ilceler { get; set; }
    public virtual DbSet<UrunFotograf> UrunFotograf { get; set; }
    public virtual DbSet<UrunAltKategori> UrunAltKategori { get; set; }
    public virtual DbSet<Video> Video { get; set; }
    public virtual DbSet <AnaSayfaBannerResim> AnaSayfaBannerResim { get; set; }
    public virtual DbSet <AnaSayfaBannerMetin> AnaSayfaBannerMetin { get; set; }
    public virtual DbSet<AnaSayfaVideo> AnaSayfaVideo { get; set; }
    public virtual DbSet<UrunAnaKategori> UrunAnaKategori { get; set; }
    public virtual DbSet<UrunAnaKategoriFotograf> UrunAnaKategoriFotograf { get; set; }
    public virtual DbSet<UrunAltKategoriFotograf> UrunAltKategoriFotograf { get; set; }
    public virtual DbSet<Uyeler> Uyeler {  get; set; }
    public virtual DbSet<UrunGaleri> UrunGaleri { get; set; }
    public virtual DbSet<Logo> Logo { get; set; }
	public virtual DbSet<Sepet> Sepet { get; set; }
	public virtual DbSet<Siparis> Siparis { get; set; }
	public virtual DbSet<StokTuru> StokTuru {  get; set; }
    public virtual DbSet<SiparisKargoTakip> SiparisKargoTakip {  get; set; }
    public virtual DbSet<AnaSayfaResim> AnaSayfaResim {  get; set; }
    public virtual DbSet<Odeme> Odeme { get; set; }
    public virtual DbSet<SepetIndirim> SepetIndirim { get; set; }
    public virtual DbSet<KargoUcret> KargoUcret { get; set; }
    public virtual DbSet<IndirimKodu> IndirimKodu { get; set; }
    public virtual DbSet<IndirimUye> IndirimUye { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseSqlServer("Server=tcp:89.19.21.42,1433;Initial Catalog=MirvanogullariEticaret;Persist Security Info=False;User ID=speedsoft;Password=5063664643msb*;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnaSayfaFotograf>(entity =>
        {
            entity.ToTable("AnaSayfaFotograf");
            entity.Property(e => e.EklenmeTarihi).HasColumnType("datetime");
            entity.Property(e => e.GuncellenmeTarihi).HasColumnType("datetime");
        });

        modelBuilder.Entity<AnaSayfaRakamlari>(entity =>
        {
            entity.ToTable("AnaSayfaRakamlari");
            entity.Property(e => e.EklenmeTarihi).HasColumnType("datetime");
            entity.Property(e => e.GuncellenmeTarihi).HasColumnType("datetime");
        });

        modelBuilder.Entity<GaleriFotograf>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Galeri");
            entity.ToTable("GaleriFotograf");
            entity.Property(e => e.EklenmeTarihi).HasColumnType("datetime");
            entity.Property(e => e.GuncellenmeTarihi).HasColumnType("datetime");
        });

        modelBuilder.Entity<HakkimizdaBilgileri>(entity =>
        {
            entity.ToTable("HakkimizdaBilgileri");
            entity.Property(e => e.EklenmeTarihi).HasColumnType("datetime");
            entity.Property(e => e.GuncellenmeTarihi).HasColumnType("datetime");
        });

        modelBuilder.Entity<HakkimizdaFotograf>(entity =>
        {
            entity.ToTable("HakkimizdaFotograf");
            entity.Property(e => e.EklenmeTarihi).HasColumnType("datetime");
            entity.Property(e => e.GuncellenmeTarihi).HasColumnType("datetime");
        });

        modelBuilder.Entity<Renkler>(entity =>
        {
            entity.ToTable("Renkler");

            entity.Property(e => e.Adi).HasMaxLength(255);
            entity.Property(e => e.EklenmeTarihi).HasColumnType("datetime");
            entity.Property(e => e.GuncellenmeTarihi).HasColumnType("datetime");
        });
        modelBuilder.Entity<IletisimBilgileri>(entity =>
        {
            entity.ToTable("IletisimBilgileri");
            entity.Property(e => e.BankaAdi).HasMaxLength(50);
            entity.Property(e => e.Email1)
                .HasMaxLength(255)
                .HasColumnName("EMail1");
            entity.Property(e => e.Email2)
                .HasMaxLength(50)
                .HasColumnName("EMail2");
            entity.Property(e => e.Faks).HasMaxLength(50);
            entity.Property(e => e.IbanNo).HasMaxLength(50);
            entity.Property(e => e.Telefon1)
                .HasMaxLength(50)
                .HasColumnName("Telefon1");
            entity.Property(e => e.Telefon2)
                .HasMaxLength(50)
                .HasColumnName("Telefon2");
            entity.Property(e => e.Telefon3)
                .HasMaxLength(50)
                .HasColumnName("Telefon3");
            entity.Property(e => e.Telefon4)
                .HasMaxLength(50)
                .HasColumnName("Telefon4");
            entity.Property(e => e.WhatsApp).HasMaxLength(50);
        });

        modelBuilder.Entity<IletisimFotograf>(entity =>
        {
            entity.ToTable("IletisimFotograf");
            entity.Property(e => e.EklenmeTarihi).HasColumnType("datetime");
            entity.Property(e => e.GuncellenmeTarihi).HasColumnType("datetime");
        });

        modelBuilder.Entity<Kullanicilar>(entity =>
        {
            entity.ToTable("Kullanicilar");
            entity.Property(e => e.Adi).HasMaxLength(50);
            entity.Property(e => e.EklenmeTarihi).HasColumnType("datetime");
            entity.Property(e => e.GuncellenmeTarihi).HasColumnType("datetime");
            entity.Property(e => e.Sifre).HasMaxLength(50);
        });

        // Bedenler ile diğer ilişki
        modelBuilder.Entity<UrunStok>()
            .HasOne<Birimler>(us => us.Birimler) // UrunStok'un Bedenler ile ilişkisi
            .WithMany() // Bedenler'in diğer ilişkisi yoksa boş bırakılabilir
            .OnDelete(DeleteBehavior.Restrict); // Alternatif bir silme davranışı

        modelBuilder.Entity<Urun>(entity =>
        {
            entity.ToTable("Urun");

            entity.Property(e => e.Adi).HasMaxLength(255);
            entity.Property(e => e.EklenmeTarihi).HasColumnType("datetime");
            entity.Property(e => e.Fiyat).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GuncellenmeTarihi).HasColumnType("datetime");

            entity.HasOne(d => d.UrunAltKategori).WithMany(p => p.Uruns)
                .HasForeignKey(d => d.UrunAltKategoriId)
                .HasConstraintName("FK_Urun_UrunTip");
        });
        
        modelBuilder.Entity<UrunFotograf>(entity =>
        {
            entity.ToTable("UrunFotograf");

            entity.Property(e => e.EklenmeTarihi).HasColumnType("datetime");
            entity.Property(e => e.GuncellenmeTarihi).HasColumnType("datetime");

      
        });
		modelBuilder.Entity<Urun>()
	  .HasMany(u => u.UrunFotograf)
	  .WithOne(uf => uf.Urun)
	  .HasForeignKey(uf => uf.UrunId);

		modelBuilder.Entity<UrunAltKategori>(entity =>
        {
            entity.ToTable("UrunAltKategori");

            entity.Property(e => e.Adi).HasMaxLength(255);
            entity.Property(e => e.EklenmeTarihi).HasColumnType("datetime");
            entity.Property(e => e.GuncellenmeTarihi).HasColumnType("datetime");
        });

        modelBuilder.Entity<Video>(entity =>
        {
            entity.ToTable("Video");

            entity.Property(e => e.EklenmeTarihi).HasColumnType("datetime");
            entity.Property(e => e.GuncellenmeTarihi).HasColumnType("datetime");
        });

        modelBuilder.Entity<Odeme>(entity =>
        {
            entity.ToTable("Odeme");
            entity.Property(e => e.PaymentId).HasColumnType("string");
            entity.Property(e => e.OdemeTutari).HasColumnType("string");
            entity.Property(e => e.OdemeDurumu).HasColumnType("string");
            entity.Property(e => e.EklenmeTarihi).HasColumnType("datetime");
            entity.Property(e => e.BuyerSurname).HasColumnType("string");
            entity.Property(e => e.BuyerGsmNumber).HasColumnType("string");
            entity.Property(e => e.BuyerName).HasColumnType("string");
        });

        modelBuilder.Entity<SepetIndirim>(entity =>
        {
            entity.ToTable("SepetIndirim");
            entity.Property(e => e.SepetTutari).HasColumnType("decimal(18,2)");
            entity.Property(e => e.IndirimMiktari).HasColumnType("decimal(18,2)");
            entity.Property(e => e.EklemeTarihi).HasColumnType("datetime");
            entity.Property(e => e.GuncellemeTarihi).HasColumnType("datetime");
            entity.Property(e => e.Durumu).HasColumnType("int");
        });

        modelBuilder.Entity<KargoUcret>(entity =>
        {
            entity.ToTable("KargoUcret");
            entity.Property(e => e.SepetTutari).HasColumnType("decimal(18,2)");
            entity.Property(e => e.KargoUcreti).HasColumnType("decimal(18,2)");
            entity.Property(e => e.EklemeTarihi).HasColumnType("datetime");
            entity.Property(e => e.GuncellemeTarihi).HasColumnType("datetime");
            entity.Property(e => e.Durumu).HasColumnType("int");
        });

        //     modelBuilder.Entity<Siparis>(entity =>
        //     {
        //         entity.ToTable("Siparis");
        //         entity.Property(e => e.MusteriAdiSoyadi).HasColumnType("string");
        //         entity.Property(e => e.MusteriAdres).HasColumnType("string");
        //         entity.Property(e => e.MusteriTelefon).HasColumnType("string");
        //         entity.Property(e => e.MusteriVergiNumarasi).HasColumnType("string");
        //         entity.Property(e => e.MusteriVergiDairesi).HasColumnType("string");
        //         entity.Property(e => e.Durumu).HasColumnType("int");
        //         entity.Property(e => e.EklenmeTarihi).HasColumnType("datetime");
        //         entity.Property(e => e.GuncellenmeTarihi).HasColumnType("datetime");
        //         entity.Property(e => e.UyelerId).HasColumnType("int");
        //         entity.Property(e => e.MusteriEMail).HasColumnType("string");
                
        //     });

        //modelBuilder.Entity<Sepet>(entity =>
        //{
        //    entity.ToTable("Sepet");
        //    entity.Property(e => e.UyeId).HasColumnType("int");
        //    entity.Property(e => e.BedenId).HasColumnType("int");
        //    entity.Property(e => e.UrunId).HasColumnType("int");
        //    entity.Property(e => e.Fiyat).HasColumnType("decimal(18,2)");
        //    entity.Property(e => e.UrunAdi).HasColumnType("string");
        //    entity.Property(e => e.Birim).HasColumnType("string");
        //    entity.Property(e => e.Miktar).HasColumnType("int");
        //    entity.Property(e => e.UrunResmi).HasColumnType("string");
        //    entity.Property(e => e.SiparisId).HasColumnType("int");
        //    entity.Property(e => e.Toplam).HasColumnType("decimal(18,2)");
        //    entity.Property(e => e.Durumu).HasColumnType("int");
        //    entity.Property(e => e.EklenmeTarihi).HasColumnType("datetime");
        //    entity.Property(e => e.GuncellenmeTarihi).HasColumnType("datetime");

        //});


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
