using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WishList.Business.Models
{
    public class PasswordUpdateModel
    {
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$ %^&*-]).{8,}", ErrorMessage = "Invalid Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        [RegularExpression(@"(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$ %^&*-]).{8,}", ErrorMessage = "Invalid New Password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Repeating new password is required.")]
        [RegularExpression(@"(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$ %^&*-]).{8,}", ErrorMessage = "Invalid Repeating New Password")]
        public string RepeatNewPassword { get; set; }
    }
}
