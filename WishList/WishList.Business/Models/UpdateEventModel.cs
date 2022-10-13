using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishList.Business.Models
{
    public class UpdateEventModel
    {
        public Guid EventId { get; set; }
        public EventModel EventModel { get; set; }
    }
}
