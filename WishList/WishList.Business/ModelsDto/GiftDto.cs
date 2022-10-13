using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace WishList.Business.ModelsDto
{
    public class GiftDto
    {
        public string GiftId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public float Price { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public bool IsReserved { get; set; }
    }
}