using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using RehabRally.Web.Core.Models;
using RehabRally.Web.Core.ViewModels;

namespace RehabRally.Web.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Categories
            CreateMap<Category, CategoryViewModel>();
            CreateMap<CategoryFormViewModel, Category>().ReverseMap();
            CreateMap<Category, SelectListItem>()
              .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));
            //Exercises
            CreateMap<Exercise, ExerciseFormViewModel>()
                .ReverseMap();
            CreateMap<Exercise, ExerciseViewModel>()
              .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category!.Name))
              ;

            //users
            CreateMap<ApplicationUser, UserViewModel>();
            CreateMap<UserFormViewModel, ApplicationUser>()
                .ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(src => src.Email.ToUpper()))
                .ForMember(dest => dest.NormalizedUserName, opt => opt.MapFrom(src => src.UserName.ToUpper()))
                .ReverseMap()
                ;
        }
    }
}
