using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RSS.Data.Context;
using RSS.WebApi.Configurations;

namespace RSS.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                //configura��o global do cors em dev
                app.UseCors("Development");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //configura��o global em prod   
                app.UseCors("Production");
                //habilita o padr�o https ap�s a primeira requisi��o https
                //deve ser combinado com redirect https para for�ar um redirect inseguro
                app.UseHsts();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseMvcConfiguration();

            app.UseSwaggerConfig(provider);
        }
    }
}
