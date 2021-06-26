using Microsoft.AspNetCore.Builder;

namespace RSS.WebApi.Configurations
{
    public static class MvcConfig
    {
        public static IApplicationBuilder UseMvcConfiguration(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("Development");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            return app;
        }
    }
}
