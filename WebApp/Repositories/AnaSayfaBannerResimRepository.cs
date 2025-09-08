using WebApp.Models;

namespace WebApp.Repositories
{
    public class AnaSayfaBannerResimRepository : GenericRepository<AnaSayfaBannerResim>
    {
        public static implicit operator AnaSayfaBannerResimRepository(AnaSayfaFotografRepository v)
        {
            throw new NotImplementedException();
        }
    }
}
