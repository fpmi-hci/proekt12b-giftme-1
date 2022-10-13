using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WishList.Business.ModelsDto;

namespace WishList.Business.IServices
{
    public interface IEmailService
    {
        public Task SendEmailConfirmMessage(string email, string callback);
        public Task SendGiftReservationMessage(string email, string callback);
        public Task<IdentityResult> ConfirmEmail(string email, string code);
        public Task<string> GenerateEmailConfirmationToken(UserDto userDto);
    }
}