using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishList.Business.Models
{
    public class CreateGiftModel
    {
        public Guid EventId { get; set; }
        public GiftModel GiftModel { get; set; }
    }
}
