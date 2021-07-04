using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RSS.Data.Context;
using RSS.WebApi.Configurations;
using RSS.WebApi.Extensions;

namespace RSS.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IHostEnvironment hostEnvironment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();

            //Adiciona dados de produção na pasta do usuário local caso desejar acessar produção localmente
            if (hostEnvironment.IsProduction())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CompleteAppDbContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
         
            services.AddAutoMapper(typeof(Startup));

            services.AddWebApiConfig();

            services.AddSwaggerConfig();

            services.ResolveDependencies();

            services.AddIdentityConfig(Configuration);

            services.AddLogConfiguration(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                //configuração global do cors em dev
                app.UseCors("Development");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //configuração global em prod   
                app.UseCors("Production");
                //habilita o padrão https após a primeira requisição https
                //deve ser combinado com redirect https para forçar um redirect inseguro
                app.UseHsts();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseMvcConfiguration();

            app.UseSwaggerConfig(provider);

            app.UseLogConfiguration(Configuration, env);
        }
    }
}
