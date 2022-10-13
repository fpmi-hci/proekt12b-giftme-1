using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WishList.DAL.Core.Entities;
using WishList.DAL.Core.Repositories.Interfaces;

namespace WishList.DAL.Core.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WishListContext _context;
        public IRepository<Gift> GiftRepository { get; }
        public IRepository<WLEvent> EventRepository { get; }

        public UnitOfWork(WishListContext context, IRepository<Gift> giftsRepository, IRepository<WLEvent> eventsRepository)
        {
            _context = context;
            GiftRepository = giftsRepository;
            EventRepository = eventsRepository;
        }
        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}