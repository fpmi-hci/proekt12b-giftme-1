using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WishList.DAL.Core.Entities;
using WishList.DAL.Core.Repositories.Interfaces;

namespace WishList.DAL.Core.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepository<Gift> GiftRepository { get; }
        IRepository<WLEvent> EventRepository { get; }
        public Task SaveChanges();
    }
}
