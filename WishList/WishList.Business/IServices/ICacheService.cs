using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishList.Business.IServices
{
    public interface ICacheService
    {
        T Get<T>(string key);
        void AddToCache<T>(T o, string key);
        void RemoveFromCache(string key);
    }
}
