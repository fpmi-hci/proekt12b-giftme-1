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
    public interface IGiftService
    {
        public Task<RequestResult> CreateGift(string userId, CreateGiftModel createGiftModel);
        public Task<RequestResult> UpdateGift(string userId, UpdateGiftModel updateGiftModel);
        public Task<RequestResult> DeleteGift(string userId, Guid giftId);
        public Task<RequestResult> ReserveGift(string reserverEmail, Guid giftId);
    }
}
