using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishList.Business.Models
{
    public class UpdateGiftModel
    {
        public Guid GiftId { get; set; }
        public GiftModel GiftModel { get; set; }
    }
}
