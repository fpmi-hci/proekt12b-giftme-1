using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WishList.DAL.Core.Entities
{
    public class WLEvent
    {
        [Key]
        public Guid EventId { get; set; }
        public string Title { get; set; }
        public DateTime? EventDate { get; set; }
        public string? Image { get; set; }
        public string? Link { get; set; }
        public string? Description { get; set; }
        public bool Completed { get; set; }

        [Range(0, 3, ErrorMessage = "Value for {0} must be between {1} and {2}")]
        public EventVisibility Visibility { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public List<Gift> Gifts { get; set; }
    }
}
