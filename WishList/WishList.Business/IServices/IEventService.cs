using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WishList.Business.Models;
using WishList.Business.ModelsDto;
using WishList.Business.Tools;

namespace WishList.Business.IServices
{
    public interface IEventService
    {
        public Task<RequestResult> CreateEvent(string userId, EventModel eventModel);
        public Task<RequestResult> UpdateEvent(string userId, Guid eventId, EventModel eventModel);
        public Task<RequestResult> ArchiveEvent(string userId, Guid eventId);
        public Task<List<WLEventDto>> GetUserEvents(string userId);
        public Task<WLEventDto> GetById(string userId, Guid eventId);
        public Task<WLEventPublicDto> GetPublicEvent (string eventLink);
    }
}
