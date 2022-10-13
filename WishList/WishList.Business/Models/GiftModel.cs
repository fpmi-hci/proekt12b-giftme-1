using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishList.Business.Models
{
    public class GiftModel
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        public string Description { get; set; }

        [RegularExpression(@"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&\/\/=]*)", ErrorMessage = "Wrong URL")]
        public string Link { get; set; }

        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "The price must have at most two decimal places.")]
        [Range(0, 9999999999999999.99)]
        public decimal Price { get; set; }

        [Range(0, 4, ErrorMessage = "Value for {0} must be between {1} and {2}")]
        public int Currency { get; set; }
        public string Image { get; set; }
    }
}
