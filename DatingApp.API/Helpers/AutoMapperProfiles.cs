using System.Linq;
using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, opt => 
                    opt.MapFrom(src => src.Photos.FirstOrDefault(photo => photo.IsMain).Url))
                .ForMember(dest => dest.Age, opt => 
                    opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<User, UserForDetailedDto>()
                .ForMember(dest => dest.PhotoUrl,  opt => 
                    opt.MapFrom(src => src.Photos.FirstOrDefault(photo => photo.IsMain).Url))
                .ForMember(dest => dest.Age, opt => 
                    opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotosForDetailedDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<NewPhotoDto, Photo>();
            CreateMap<NewUserDto, User>()
                .ForMember(dest => dest.Created, opt =>
                    opt.MapFrom(src => src.RegisteredDate));
            CreateMap<MessageForCreationDto, Message>()
                .ReverseMap();
            CreateMap<Message, MessageToReturnDto>()
                .ForMember(message => message.SenderPhotoUrl, opt => opt
                    .MapFrom(message => message.Sender.Photos.FirstOrDefault(photo => photo.IsMain).Url))
                .ForMember(message => message.RecipientPhotoUrl, opt => opt
                    .MapFrom(message => message.Recipient.Photos.FirstOrDefault(photo => photo.IsMain).Url));
        }
    }
}