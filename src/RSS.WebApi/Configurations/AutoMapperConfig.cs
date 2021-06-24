using AutoMapper;
using RSS.Business.Models;
using RSS.WebApi.DTOs;

namespace RSS.WebApi.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Address, AddressDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Supplier, SupplierDTO>().ReverseMap();
        }
    }
}
