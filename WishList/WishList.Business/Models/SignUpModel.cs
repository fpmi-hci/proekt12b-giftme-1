using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WishList.Business.Models
{
    public class SignUpModel
    {
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$ %^&*-]).{8,}", ErrorMessage = "Invalid Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Repeating password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string RepeatPassword { get; set; }
    }
}
