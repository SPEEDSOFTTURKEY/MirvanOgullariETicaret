using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApp.Models;
namespace WebApp.Repositories
{
    public class GenericRepository<T> where T : class, new()
    {
        Context c = new Context();

        public List<T> Listele()
        {
            return c.Set<T>().ToList();
        }
        public void Ekle(T entity)
        {
            c.Set<T>().Add(entity);
            c.SaveChanges();
        }
        public void Sil(T entity)
        {
            c.Set<T>().Remove(entity);
            c.SaveChanges();
        }
        public void Guncelle(T entity)
        {
            c.Set<T>().Update(entity);
            c.SaveChanges();

        }
        public T Getir(int Id)
        {
            return c.Set<T>().Find(Id);

        }
        public T Getir(Expression<Func<T, bool>> predicate)
        {
            return c.Set<T>().Where(predicate).FirstOrDefault();
        }
        public T GetirTracking(Expression<Func<T, bool>> predicate)
        {
            return c.Set<T>().Where(predicate).AsNoTracking().FirstOrDefault();
        }
        public List<T> GetirList(Expression<Func<T, bool>> predicate, List<string> SqlJoinTables)
        {
            IQueryable<T> query = c.Set<T>().AsNoTracking();

            if (SqlJoinTables != null && SqlJoinTables.Count > 0)
            {
                query = AddIncludeClauses(query, SqlJoinTables);
            }

            return query.Where(predicate).ToList();
        }
        public T Getir(Expression<Func<T, bool>> predicate, List<string> SqlJoinTables)
        {
            IQueryable<T> query = c.Set<T>();

            if (SqlJoinTables != null && SqlJoinTables.Count > 0)
            {
                query = AddIncludeClauses(query, SqlJoinTables);
            }

            return query.Where(predicate).FirstOrDefault();
        }
        public List<T> GetirList(Expression<Func<T, bool>> predicate)
        {
            return c.Set<T>().Where(predicate).ToList();
        }
        public List<T> GetirListTracking(Expression<Func<T, bool>> predicate)
        {
            return c.Set<T>().Where(predicate).AsNoTracking().ToList();
        }
        private IQueryable<T> AddIncludeClauses(IQueryable<T> query, List<string> SqlJoinTables)
        {
            foreach (string table in SqlJoinTables)
            {
                // Assuming table is a navigation property in your entities
                query = query.AsNoTracking().Include(table);
            }

            return query;
        }








        public List<T> Listele(string p)
        {
            return c.Set<T>().Include(p).ToList();
        }
        public List<T> Listele(string p1, string p2)
        {
            return c.Set<T>().Include(p1).Include(p2).ToList();
        }
        public List<T> Listele(string p1, string p2, string p3)
        {
            return c.Set<T>().Include(p1).Include(p2).Include(p3).ToList();
        }
        public List<T> Listele(string p1, string p2, string p3, string p4)
        {
            return c.Set<T>().Include(p1).Include(p2).Include(p3).Include(p4).ToList();
        }
        public List<T> Listele(string p1, string p2, string p3, string p4, string p5)
        {
            return c.Set<T>().Include(p1).Include(p2).Include(p3).Include(p4).Include(p5).ToList();
        }
        public List<T> Listele(string p1, string p2, string p3, string p4, string p5, string p6)
        {
            return c.Set<T>().Include(p1).Include(p2).Include(p3).Include(p4).Include(p5).Include(p6).ToList();
        }
        public List<T> Listele(string p1, string p2, string p3, string p4, string p5, string p6, string p7)
        {
            return c.Set<T>().Include(p1).Include(p2).Include(p3).Include(p4).Include(p5).Include(p6).Include(p7).ToList();
        }
        public List<T> Listele(string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8)
        {
            return c.Set<T>().Include(p1).Include(p2).Include(p3).Include(p4).Include(p5).Include(p6).Include(p7).Include(p8).ToList();
        }
        public List<T> Listele(string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9)
        {
            return c.Set<T>().Include(p1).Include(p2).Include(p3).Include(p4).Include(p5).Include(p6).Include(p7).Include(p8).Include(p9).ToList();
        }
        public List<T> Listele(string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9, string p10)
        {
            return c.Set<T>().Include(p1).Include(p2).Include(p3).Include(p4).Include(p5).Include(p6).Include(p7).Include(p8).Include(p9).Include(p10).ToList();
        }
        public void Ekle(List<T> entity)
        {
            c.Set<T>().AddRange(entity);
            c.SaveChanges();
        }

    }
}
