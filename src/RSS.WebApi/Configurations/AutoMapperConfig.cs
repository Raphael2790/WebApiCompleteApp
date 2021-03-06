using AutoMapper;
using RSS.Business.Models;
using RSS.WebApi.DTOs;

namespace RSS.WebApi.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            AllowNullCollections = true;
            CreateMap<Address, AddressDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>().ForMember(p => p.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name));
            CreateMap<ProductDTO, Product>();
            //Verificando se o objeto aninhado é nulo, caso não seja mapeia
            CreateMap<Supplier, SupplierDTO>().ForMember(s => s.Address, opt =>
            {
                opt.Condition(src => src.Adress != null);
                opt.MapFrom(src => src.Adress);
            })
            .ForMember(s => s.Products, opt =>
            {
                opt.Condition(src => src.Products != null);
                opt.MapFrom(src => src.Products);
            })
            .ReverseMap();

            CreateMap<ProductFormFileDTO, Product>();
            CreateMap<Product, ProductFormFileDTO>().ForMember(p => p.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name));
        }
    }
}
