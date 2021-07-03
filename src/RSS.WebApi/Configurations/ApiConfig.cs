using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace RSS.WebApi.Configurations
{
    public static class ApiConfig
    {
        public static IServiceCollection AddWebApiConfig(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

            //configuração global da versão default da api e mostrar todas as versões da mesma
            //atualizado o pacote nuget de versionamento para 4.1 devido falta de suporte no 3.1
            services.AddApiVersioning(options => {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(2, 0);
                options.ReportApiVersions = true;
            });

            //modelo de no da api na url
            services.AddVersionedApiExplorer(options => {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddCors(options => {
                options.AddPolicy("Development", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
                options.AddPolicy("Production", builder => builder.WithMethods("GET").WithOrigins("http://localhost:4000").SetIsOriginAllowedToAllowWildcardSubdomains().AllowAnyHeader());
            });

            services.AddControllers();
            return services;
        }
    }
}
