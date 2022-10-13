using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WishList.Business.IServices;
using WishList.Business.Models;
using WishList.Business.ModelsDto;
using WishList.Business.Tools;
using WishList.DAL.Core.Entities;
using WishList.DAL.Core.UnitOfWork;

namespace WishList.Business.Implementation
{
    public class GiftService : IGiftService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public GiftService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }
        public async Task<RequestResult> CreateGift(string userId, CreateGiftModel createGiftModel)
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
            var wlEvent = await _unitOfWork.EventRepository.GetById(createGiftModel.EventId);
            if (wlEvent == null)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.NotFound,
                    Message = "Event not found."
                };
            }
            var gift = _mapper.Map<Gift>(createGiftModel.GiftModel);
            gift.GiftId = Guid.NewGuid();
            gift.Event = wlEvent;
            gift.EventId = wlEvent.EventId;
            gift.IsReserved = false;
            gift.ReserverEmail = "";
            if (createGiftModel.GiftModel.Currency == 0)
            {
                if (user.Currency == Currency.None && !createGiftModel.GiftModel.Price.Equals(null))
                {
                    return new RequestResult
                    {
                        resultCode = HttpStatusCode.BadRequest,
                        Message = "The currency hasn't been chosen."
                    };
                }
                gift.Currency = (Currency)user.Currency;
            }
            await _unitOfWork.GiftRepository.Add(gift);
            await _unitOfWork.SaveChanges();
            return new RequestResult
            {
                resultCode = HttpStatusCode.OK,
                Message = $"The gift \"{gift.Name}\" is created and added to the event \"{wlEvent.Title}\" successfully."
            };
        }

        public async Task<RequestResult> DeleteGift(string userId, Guid giftId)
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
            var gift = await _unitOfWork.GiftRepository.Get()
                .FirstOrDefaultAsync(g => g.GiftId == giftId);
            if (gift == null)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.NotFound,
                    Message = "Gift not found."
                };
            }
            var wlEvent = await _unitOfWork.EventRepository.GetById(gift.EventId);
            if (wlEvent == null)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.NotFound,
                    Message = "Event for this gift is not found."
                };
            }
            if (wlEvent.UserId != user.Id)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.BadRequest,
                    Message = "You can't delete the gift created by another user."
                };
            }
            _unitOfWork.GiftRepository.Remove(gift);
            await _unitOfWork.SaveChanges();
            return new RequestResult
            {
                resultCode = HttpStatusCode.OK,
                Message = $"The gift \"{gift.Name}\" is successfully deleted from the event \"{wlEvent.Title}\"."
            };
        }

        public async Task<RequestResult> ReserveGift(string email, Guid giftId)
        {
            var gift = await _unitOfWork.GiftRepository.Get()
                .FirstOrDefaultAsync(g => g.GiftId == giftId);
            if (gift == null)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.NotFound,
                    Message = "Gift not found."
                };
            }
            var wlEvent = await _unitOfWork.EventRepository.GetById(gift.EventId);
            if (wlEvent == null)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.NotFound,
                    Message = "Event for this gift is not found."
                };
            }
            if (gift.IsReserved)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.BadRequest,
                    Message = "This gift is already reserved."
                };
            }
            gift.IsReserved = true;
            gift.ReserverEmail = email;
            _unitOfWork.GiftRepository.Update(gift);
            await _unitOfWork.SaveChanges();
            return new RequestResult
            {
                resultCode = HttpStatusCode.OK,
                Message = $"The gift \"{gift.Name}\" for the event \"{wlEvent.Title}\" is reserved successfully."
            };
        }

        public async Task<RequestResult> UpdateGift(string userId, UpdateGiftModel updateGiftModel)
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
            var gift = await _unitOfWork.GiftRepository.Get()
                .FirstOrDefaultAsync(g => g.GiftId == updateGiftModel.GiftId);
            if (gift == null)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.NotFound,
                    Message = "Gift not found."
                };
            }
            var wlEvent = await _unitOfWork.EventRepository.GetById(gift.EventId);
            if (wlEvent == null)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.NotFound,
                    Message = "Event for this gift is not found."
                };
            }
            if (wlEvent.UserId != user.Id)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.BadRequest,
                    Message = "You can't update the gift created by another user."
                };
            }
            _mapper.Map(updateGiftModel.GiftModel, gift);
            _unitOfWork.GiftRepository.Update(gift);
            await _unitOfWork.SaveChanges();
            return new RequestResult
            {
                resultCode = HttpStatusCode.OK,
                Message = $"The gift \"{gift.Name}\" for the event \"{wlEvent.Title}\" is updated successfully."
            };
        }
    }
}
