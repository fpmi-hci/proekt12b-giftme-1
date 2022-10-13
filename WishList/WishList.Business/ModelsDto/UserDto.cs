using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace WishList.Business.ModelsDto
{
    public class UserDto
    {
        [JsonIgnore]
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        [JsonIgnore]
        public string SecurityStamp { get; set; }
    }
}
