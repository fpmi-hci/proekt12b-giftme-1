using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishList.DAL.Core.Entities
{
    public enum EventVisibility
    {
        publicEvent = 0,
        byUrlEvent = 1,
        privateEvent = 2,
        archived = 3
    }
}
