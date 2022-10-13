using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WishList.DAL.Core.Entities
{
    public class User : IdentityUser<Guid>
    {
        [Range(0, 3, ErrorMessage = "Value for {0} must be between {1} and {2}")]
        public Currency? Currency { get; set; }
        public List<WLEvent> Events { get; set; } = new List<WLEvent>();
    }
}
