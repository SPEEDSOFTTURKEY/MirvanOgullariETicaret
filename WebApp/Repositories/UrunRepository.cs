using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApp.Models;

namespace WebApp.Repositories
{
    public class UrunRepository : GenericRepository<Urun>
    {
        Context context = new Context();
        public IQueryable<Urun> GetirQueryable(Expression<Func<Urun, bool>> filter = null, params Expression<Func<Urun, object>>[] includeProperties)
        {
            IQueryable<Urun> query = context.Urun; // ya da DbSet adın neyse
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            return query;
        }

    }
}
