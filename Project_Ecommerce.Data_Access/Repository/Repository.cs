using Microsoft.EntityFrameworkCore;
using Project_Ecommerce.Data_Access.Data;
using Project_Ecommerce.Data_Access.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecommerce.Data_Access.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbset;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            dbset = context.Set<T>();
        }


        public void Add(T entity)
        {
            dbset.Add(entity);
        }

        public T FirstOrDefault(Expression<Func<T, 
            bool>> filter = null, 
            string includeProperties = null)
        {
            IQueryable<T> query = dbset;
            if(filter != null)                                          //Filter table
                query = query.Where(filter);
            if(includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new[] {','},
                    StringSplitOptions.RemoveEmptyEntries ))
                {                                                     //Split Tables in array
                    query=query.Include(includeProp);
                }
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, 
            bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
            string includeProperties = null)
        {
            IQueryable<T> query = dbset;
            if(filter != null)                                      //Filter Table 
                query= query.Where(filter);
            if(includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new[] { ','},
                    StringSplitOptions.RemoveEmptyEntries))         //Split Table
                {
                    query=query.Include(includeProp);

                }
            }
            if(orderBy != null)
                return orderBy(query).ToList();                     //Sort Table
            return query.ToList();
        }

        public T Get(int id)
        {
            return dbset.Find(id);
        }

        public void Remove(T entity)
        {
            dbset.Remove(entity);
        }

        public void Remove(int id)
        {
            T entity = dbset.Find(id);
            dbset.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> values)
        {
            dbset.RemoveRange(values);
        }

        public void Update(T entity)
        {
            _context.ChangeTracker.Clear();
            dbset.Update(entity);
        }
    }
}
