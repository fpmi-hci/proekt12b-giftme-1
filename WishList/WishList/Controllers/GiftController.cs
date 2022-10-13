using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using WishList.Business.IServices;
using WishList.Business.Models;
using WishList.WebApi.Auth;
using WishList.Business.Tools;
using System.Text;

namespace WishList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftController : BaseController
    {
        private readonly IGiftService _giftService;
        private readonly IEmailService _emailService;
        private const string salt = "Arseniy";

        public GiftController(IGiftService giftService, IAuthManager authManager, IEmailService emailService) : base(authManager)
        {
            _giftService = giftService;
            _emailService = emailService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateGift([FromBody] CreateGiftModel CreateGiftModel)
        {
            var token = await HttpContext.GetTokenAsync("Bearer", "access_token");
            var userId = GetCurrentUserId(token);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found.");
            }
            var result = await _giftService.CreateGift(userId, CreateGiftModel);
            return StatusCode((int)result.resultCode, result.Message);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateGift([FromBody] UpdateGiftModel updateGiftModel)
        {
            var token = await HttpContext.GetTokenAsync("Bearer", "access_token");
            var userId = GetCurrentUserId(token);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found.");
            }
            var result = await _giftService.UpdateGift(userId, updateGiftModel);
            return StatusCode((int)result.resultCode, result.Message);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteGift([FromBody] Guid giftId)
        {
            var token = await HttpContext.GetTokenAsync("Bearer", "access_token");
            var userId = GetCurrentUserId(token);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found.");
            }
            var result = await _giftService.DeleteGift(userId, giftId);
            return StatusCode((int)result.resultCode, result.Message);
        }

        [HttpPost("GetReservationEmail")]
        public async Task<IActionResult> GetReservationMessage([FromBody] ReserveGiftModel reserveGiftModel)
        {
            var code = Crypto.DigestString(reserveGiftModel.Email + reserveGiftModel.GiftId, salt);
            var callback = Url.Action(nameof(ReserveGift), "Gift",
                new { email = reserveGiftModel.Email, giftId = reserveGiftModel.GiftId, code = code },
                Request.Scheme);
            await _emailService.SendGiftReservationMessage(reserveGiftModel.Email, callback);
            return Ok($"To reserve a gift, check the inbox of {reserveGiftModel.Email} and follow the link from the message we've sent you.");
        }

        [AllowAnonymous]
        [HttpGet("Reserve", Name = "ReserveGiftRoute")]
        public async Task<IActionResult> ReserveGift(string email, Guid giftId, string code)
        {
            if (!Crypto.VerifyString(code, salt, email + giftId))
            {
                return BadRequest("The link is incorrect.");
            }
            var result = await _giftService.ReserveGift(email, giftId);
            return StatusCode((int)result.resultCode, result.Message);
        }
    }
}
