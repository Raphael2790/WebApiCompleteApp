using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RSS.Business.Interfaces;
using RSS.Business.Notifications;
using RSS.Business.Services;
using RSS.Data.Context;
using RSS.Data.Repository;
using RSS.WebApi.Extensions;
using RSS.WebApi.Services;
using RSS.WebApi.Services.Interfaces;

namespace RSS.WebApi.Configurations
{
    public static class DependecyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddScoped<CompleteAppDbContext>();

            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<INotifiable, Notifiable>();
            services.AddScoped<IUser, AspNetUser>();

            //injeção de dependencia que viabiliza o acesso ao contexto da requisição em qualquer classe que receba a injeção de dependencia
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services;
        }
    }
}
