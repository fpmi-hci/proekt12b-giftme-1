using System.ComponentModel.DataAnnotations;

namespace WishList.Business.Models
{
    public class UserModel
    {
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [Range(1, 4, ErrorMessage = "Value for {0} must be between {1} and {2}")]
        public int Currency { get; set; }
    }
}
