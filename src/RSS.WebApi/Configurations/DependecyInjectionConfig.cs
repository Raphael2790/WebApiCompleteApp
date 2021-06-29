using Microsoft.Extensions.DependencyInjection;
using RSS.Business.Interfaces;
using RSS.Business.Notifications;
using RSS.Business.Services;
using RSS.Data.Context;
using RSS.Data.Repository;
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

            return services;
        }
    }
}
