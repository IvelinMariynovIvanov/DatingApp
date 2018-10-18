namespace DatingApp.API.Helpers
{
    using System.Linq;
    using AutoMapper;
    using DatingApp.API.Dtos;
    using DatingApp.API.Models;

    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                // set PhotoUrl in UserForListDto
                .ForMember(destination => destination.PhotoUrl, option => {
                    option.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => {
                    // set age using dateofbirth / calcute property
                    opt.ResolveUsing(d =>d.DateOfBirth.CalculateAge());
                });

            CreateMap<User, UserForDetailedDto>()
             .ForMember(destination => destination.PhotoUrl, option => {
                    option.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => {
                    // set age using dateofbirth / calcute property
                    opt.ResolveUsing(d =>d.DateOfBirth.CalculateAge());
                });

            CreateMap<Photo, PhotoForDetailedDto>();   

            CreateMap<UserForUpdateDto, User>();  

            CreateMap<Photo, PhotoForReturnDto>();

            CreateMap<PhotoForCreationDto, Photo>();    

            CreateMap<UserForRegisterDto, User>(); 

            // int both ways
            CreateMap<MessageForCreationDto, Message>().ReverseMap();

            CreateMap<Message, MessageToReturnDto>()
            // manually map photurl for sender and recipent
            .ForMember(m => m.SenderPhotoUrl, opt => opt.MapFrom(u => u.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
            .ForMember(m => m.RecepientPhotoUrl, opt => opt.MapFrom(u => u.Recepient.Photos.FirstOrDefault(p => p.IsMain).Url));
        }
    }
}