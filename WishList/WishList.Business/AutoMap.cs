using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using WishList.Business.Models;
using WishList.Business.ModelsDto;
using WishList.DAL.Core.Entities;

namespace WishList.Business
{
    public class AutoMap : Profile
    {
        public AutoMap()
        {
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<UserDto, UserModel>();
            CreateMap<UserModel, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(
                    (source, destination) => !string.IsNullOrEmpty(source.UserName) ? source.UserName : destination.UserName));

            CreateMap<Gift, GiftDto>();

            CreateMap<GiftDto, Gift>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(
                    (source, destination) => !string.IsNullOrEmpty(source.Name) ? source.Name : destination.Name))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(
                    (source, destination) => !string.IsNullOrEmpty(source.Image) ? source.Image : destination.Image))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(
                    (source, destination) => !source.Price.Equals(default) ? source.Price : destination.Price));

            CreateMap<WLEvent, WLEventPublicDto>();

            CreateMap<WLEvent, WLEventDto>();

            CreateMap<EventModel, WLEvent>();

            CreateMap<GiftModel, Gift>();
        }
    }
}
