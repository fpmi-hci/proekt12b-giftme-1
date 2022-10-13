using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WishList.DAL.Core.Entities;

namespace WishList.Business.ModelsDto
{
    public class WLEventDto
    {
        public string EventId { get; set; }
        public string Title { get; set; }
        public DateTime EventDate { get; set; }
        public string Image { get; set; }
        public string Link { get; set; }
        public bool Completed { get; set; }
        public string Description { get; set; }
        public EventVisibility Visibility { get; set; }
        public List<GiftDto> Gifts { get; set; } = new List<GiftDto>();
    }
}
