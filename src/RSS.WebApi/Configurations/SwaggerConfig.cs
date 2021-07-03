using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSS.WebApi.Configurations
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
        {
            services.AddSwaggerGen(c => {
                c.OperationFilter<SwaggerDefaultValues>();

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { 
                    Description = "Insira seu token desta maneira. Ex:Bearer {seu token} ",
                    Name="Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement 
                {
                    {
                       new OpenApiSecurityScheme
                       {
                           Reference = new OpenApiReference
                           {
                               Type = ReferenceType.SecurityScheme,
                               Id = "Bearer"
                           }
                       },
                        new string[] { }
                    }
                });
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerConfig(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseMiddleware<SwaggerAuthorizedMiddleware>();

            app.UseSwagger();

            app.UseSwaggerUI(options => {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });

            return app;
        }
    }

    public class SwaggerDefaultValues : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            operation.Deprecated = apiDescription.IsDeprecated();

            if (operation.Parameters == null)
                return;

            foreach (var parameter in operation.Parameters)
            {
                var description = context.ApiDescription
                   .ParameterDescriptions
                   .First(p => p.Name == parameter.Name);

                var routeInfo = description.RouteInfo;

                operation.Deprecated = OpenApiOperation.DeprecatedDefault;

                if (parameter.Description == null)
                {
                    parameter.Description = description.ModelMetadata?.Description;
                }

                if (routeInfo == null)
                {
                    continue;
                }

                if (parameter.In != ParameterLocation.Path && parameter.Schema.Default == null)
                {
                    parameter.Schema.Default = new OpenApiString(routeInfo.DefaultValue.ToString());
                }

                parameter.Required |= description.IsRequired;
            }
            
        }
    }

    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        readonly IApiVersionDescriptionProvider provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;
        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
        }

        private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo
            {
                Title = "API - RSS Web Api",
                Version = description.ApiVersion.ToString(),
                Description = "Está API foi construida durante o curso WebApi desenvolvedor.io",
                Contact = new OpenApiContact {Name = "Raphael Dev", Email = "rss@dev.com.br" },
                License = new OpenApiLicense {Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") },
                TermsOfService = new Uri("https://opensource.org/licenses/MIT")
            };

            if (description.IsDeprecated)
                info.Description += "- Está versão está depreciada";

            return info;
        }
    }

    //middleware de permissão ao swagger
    public class SwaggerAuthorizedMiddleware
    {
        private readonly RequestDelegate _next;

        public SwaggerAuthorizedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if(context.Request.Path.StartsWithSegments("/swagger") 
                && !context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            //invoca o próximo middleware no fluxo da pipeline
            await _next.Invoke(context);
        }
    }
}
