using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WishList.DAL.Core.Repositories.Interfaces;

namespace WishList.DAL.Core.Repositories.Implementation
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly WishListContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(WishListContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }
        public async Task Add(TEntity obj)
        {
            await _dbSet.AddAsync(obj);
        }

        public async Task AddRange(IEnumerable<TEntity> objs)
        {
            await _dbSet.AddRangeAsync(objs);
        }


        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }

        public virtual async Task<TEntity> GetById(Guid id)
        {
            var obj = await _dbSet.FindAsync(id);

            return obj;
        }

        public virtual IQueryable<TEntity> Get()
        {
            return _dbSet.AsNoTracking();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            var objList = await _dbSet.AsNoTracking().ToListAsync();

            return objList;
        }

        public async Task Remove(Guid id)
        {
            var obj = await _dbSet.FindAsync(id);
            Remove(obj);
        }

        public void Remove(TEntity obj)
        {
            _dbSet.Remove(obj);
        }

        public void RemoveRange(IEnumerable<TEntity> objs)
        {
            _dbSet.RemoveRange(objs);
        }

        public void Update(TEntity obj)
        {
            _dbSet.Update(obj);
        }

        public void UpdateRange(IEnumerable<TEntity> objs)
        {
            _dbSet.UpdateRange(objs);
        }
    }
}
