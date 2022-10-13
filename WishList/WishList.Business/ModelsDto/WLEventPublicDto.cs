using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishList.Business.ModelsDto
{
    public class WLEventPublicDto
    {
        public string Title { get; set; }
        public DateTime EventDate { get; set; }
        public string Description { get; set; }
        public List<GiftDto> Gifts { get; set; } = new List<GiftDto>();
    }
}
