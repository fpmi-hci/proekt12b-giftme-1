using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishList.DAL.Core.Repositories.Interfaces
{
    public interface IRepository<TEntity>: IDisposable where TEntity : class
    {
        public Task<TEntity> GetById(Guid id);
        public Task<IEnumerable<TEntity>> GetAll();

        IQueryable<TEntity> Get();

        public Task Add(TEntity obj);
        public Task AddRange(IEnumerable<TEntity> objs);

        public Task Remove(Guid id);
        public void Remove(TEntity obj);
        public void RemoveRange(IEnumerable<TEntity> objs);

        public void Update(TEntity obj);
        public void UpdateRange(IEnumerable<TEntity> objs);
    }
}
