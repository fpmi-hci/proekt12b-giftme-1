using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WishList.Business.IServices;
using WishList.Business.Models;
using WishList.Business.ModelsDto;
using WishList.DAL.Core.Entities;
using WishList.DAL.Core.UnitOfWork;
using WishList.Business.Tools;
using System.Net;
using Microsoft.AspNetCore.WebUtilities;

namespace WishList.Business.Implementation
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public EventService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<RequestResult> CreateEvent(string userId, EventModel eventModel)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.Unauthorized,
                    Message = "User not found."
                };
            }
            var wlEvent = _mapper.Map<WLEvent>(eventModel);
            wlEvent.EventId = Guid.NewGuid();
            wlEvent.User = user;
            wlEvent.UserId = user.Id;
            wlEvent.Link = WebEncoders.Base64UrlEncode(wlEvent.EventId.ToByteArray()).Substring(0, 6);
            wlEvent.Completed = false;
            wlEvent.Gifts = new List<Gift>();
            await _unitOfWork.EventRepository.Add(wlEvent);
            await _unitOfWork.SaveChanges();
            return new RequestResult
            {
                resultCode = HttpStatusCode.OK,
                Message = $"The event \"{wlEvent.Title}\" is created successfully."
            };
        }

        public async Task<RequestResult> UpdateEvent(string userId, Guid eventId, EventModel eventModel)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.Unauthorized,
                    Message = "User not found."
                };
            }
            var wlEvent = await _unitOfWork.EventRepository.Get()
                .FirstOrDefaultAsync(e => e.EventId == eventId);
            if (wlEvent == null)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.NotFound,
                    Message = "Event not found."
                };
            }
            if (wlEvent.UserId != user.Id)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.BadRequest,
                    Message = "You can't update the event created by another user."
                };
            }
            _mapper.Map(eventModel, wlEvent);
            _unitOfWork.EventRepository.Update(wlEvent);
            await _unitOfWork.SaveChanges();
            return new RequestResult
            {
                resultCode = HttpStatusCode.OK,
                Message = $"The event \"{wlEvent.Title}\" is updated successfully."
            };
        }

        public async Task<RequestResult> ArchiveEvent(string userId, Guid eventId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.Unauthorized,
                    Message = "User not found."
                };
            }
            var wlEvent = await _unitOfWork.EventRepository.Get()
                .FirstOrDefaultAsync(e => e.EventId == eventId);
            if (wlEvent == null)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.NotFound,
                    Message = "Event not found."
                };
            }
            if (wlEvent.UserId != user.Id)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.BadRequest,
                    Message = "You can't archive the event created by another user."
                };
            }
            if (wlEvent.Visibility == EventVisibility.archived)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.BadRequest,
                    Message = $"The event \"{wlEvent.Title}\" is already archived."
                };
            }
            wlEvent.Visibility = EventVisibility.archived;
            _unitOfWork.EventRepository.Update(wlEvent);
            await _unitOfWork.SaveChanges();
            return new RequestResult
            {
                resultCode = HttpStatusCode.OK,
                Message = $"The event \"{wlEvent.Title}\" is archived successfully."
            };
        }

        public async Task<List<WLEventDto>> GetUserEvents(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            var eventsList = await _unitOfWork.EventRepository.Get()
                .Include(e => e.Gifts)
                .Where(e => e.UserId.ToString() == userId)
                .ToListAsync();
            var eventDtosList = _mapper.Map<List<WLEventDto>>(eventsList);

            return eventDtosList;
        }

        public async Task<WLEventPublicDto> GetPublicEvent(string eventLink)
        {
            var wlEvent = await _unitOfWork.EventRepository.Get()
                .Include(e => e.Gifts)
                .FirstOrDefaultAsync(e => e.Link == eventLink);
            if (wlEvent == null)
            {
                throw new ArgumentException("Event not found.");
            }
            if (wlEvent.Visibility == EventVisibility.privateEvent)
            {
                throw new ArgumentException($"The event \"{wlEvent.Title}\" can't be shared due to its protection level.");
            }
            var wlEventDto = _mapper.Map<WLEventPublicDto>(wlEvent);
            return wlEventDto;
        }

        public async Task<WLEventDto> GetById(string userId, Guid eventId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }
            var wlEvent = await _unitOfWork.EventRepository.Get()
                .Include(e => e.Gifts)
                .FirstOrDefaultAsync(e => e.EventId == eventId);
            if (wlEvent.UserId != user.Id)
            {
                throw new ArgumentException("You can't get the info of the event created by another user.");
            }
            if (wlEvent == null)
            {
                throw new ArgumentException("Event not found.");
            }
            var wlEventDto = _mapper.Map<WLEventDto>(wlEvent);
            return wlEventDto;
        }
    }
}
