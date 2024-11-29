using AutoMapper;
using Domain.DTOs;
using Domain.Entities;

namespace Infrastructure
{
    public class MappingProfileData : Profile
    {
        public MappingProfileData()
        {
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();

        }
    }
}
