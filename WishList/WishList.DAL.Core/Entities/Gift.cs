using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WishList.DAL.Core.Entities
{
    public class Gift
    {
        [Key]
        public Guid GiftId { get; set; }
        public string Name { get; set; }
        public bool IsReserved { get; set; }
        public string ReserverEmail { get; set; }
        public Guid EventId { get; set; }
        public WLEvent Event { get; set; }
        public string? Description { get; set; }
        public float? Price { get; set; }
        public Currency? Currency { get; set; }
        public string? Image { get; set; }
        public string? Link { get; set; }
    }
}
