using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using WishList.Business.IServices;
using WishList.Business.Models;
using WishList.WebApi.Auth;

namespace WishList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : BaseController
    {
        private readonly IEventService _eventService;
        public EventController(IEventService eventService, IAuthManager authManager) : base(authManager)
        {
            _eventService = eventService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateEvent([FromBody] EventModel eventModel)
        {
            var token = await HttpContext.GetTokenAsync("Bearer", "access_token");
            var userId = GetCurrentUserId(token);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found.");
            }
            var result = await _eventService.CreateEvent(userId, eventModel);
            return StatusCode((int)result.resultCode, result.Message);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateEvent([FromBody] UpdateEventModel updateEventModel)
        {
            var token = await HttpContext.GetTokenAsync("Bearer", "access_token");
            var userId = GetCurrentUserId(token);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found.");
            }
            var result = await _eventService.UpdateEvent(userId, updateEventModel.EventId, updateEventModel.EventModel);
            return StatusCode((int)result.resultCode, result.Message);
        }

        [HttpDelete("Archive")]
        public async Task<IActionResult> ArchiveEvent([FromBody] Guid eventId)
        {
            var token = await HttpContext.GetTokenAsync("Bearer", "access_token");
            var userId = GetCurrentUserId(token);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found.");
            }
            var result = await _eventService.ArchiveEvent(userId, eventId);
            return StatusCode((int)result.resultCode, result.Message);
        }

        [HttpGet("{link}")]
        public async Task<IActionResult> GetEvent(string link)
        {
            var eventDto = await _eventService.GetPublicEvent(link);
            if (eventDto == null)
            {
                return NotFound("Event not found.");
            }
            return Ok(eventDto);
        }

        [HttpGet("UserEvents")]
        public async Task<IActionResult> GetAllEvents()
        {
            var token = await HttpContext.GetTokenAsync("Bearer", "access_token");
            var userId = GetCurrentUserId(token);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found.");
            }
            var resultList = await _eventService.GetUserEvents(userId);
            if (resultList == null)
            {
                return NotFound("Events not found.");
            }
            return Ok(resultList);
        }

        [HttpGet("Info")]
        public async Task<IActionResult> GetById(Guid? eventId)
        {
            var token = await HttpContext.GetTokenAsync("Bearer", "access_token");
            var userId = GetCurrentUserId(token);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found.");
            }
            if (!eventId.HasValue)
            {
                return NotFound("Event not found.");
            }
            var result = await _eventService.GetById(userId, eventId.Value);
            if (result == null)
            {
                return NotFound("Event not found.");
            }
            return Ok(result);
        }
    }
}
