using AdvertAPI.Model;
using AutoMapper;

namespace WebAdvert.API.Services
{
    public class AdvertProfile : Profile
    {
        public AdvertProfile()
        {
            CreateMap<AdvertModel, AdvertDbModel>();
        }        
    }
}
