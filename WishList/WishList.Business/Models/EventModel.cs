using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishList.Business.Models
{
    public class EventModel
    {
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        public string EventDate { get; set; }
        public string Description { get; set; }

        [Range(0, 2, ErrorMessage = "Value for {0} must be between {1} and {2}")]
        public int Visibility { get; set; }
        public string Image { get; set; }
    }
}
