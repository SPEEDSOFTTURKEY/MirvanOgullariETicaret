using Microsoft.EntityFrameworkCore;
using WebApp.Repositories;

#nullable disable

namespace WebApp.Models
{
    public partial class ProgramBilgileriContext : DbContext
    {
        public ProgramBilgileriContext()
        {
        }

        public ProgramBilgileriContext(DbContextOptions<ProgramBilgileriContext> options)
            : base(options)
        {

        }

        public virtual DbSet<Lisans> Lisans { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

                  => optionsBuilder.UseSqlServer("Server=tcp:89.19.21.42,1433;Initial Catalog=ProgramBilgileri;Persist Security Info=False;User ID=speedsoft;Password=5063664643msb*;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Turkish_CI_AS");
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
