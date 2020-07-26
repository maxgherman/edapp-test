using EdAppTest.Database;
using EdAppTest.Middleware;
using EdAppTest.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EdAppTest.Features.Users;
using EdAppTest.Features.Products;
using Microsoft.AspNetCore.Mvc.Authorization;
using EdAppTest.Features.Bids;
using System.Text.Json.Serialization;

namespace EdAppTest
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
            services.AddCors();
			services.AddOptions();
            
            services.AddSingleton<IDatabaseAdapter, DatabaseAdapter>();
            services.AddTransient<IApiTokenService, ApiTokenService>();
            services.AddTransient<IAuthenticateService, AuthenticateService>();
            services.AddTransient<IRefreshTokenService, RefreshTokenService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IBidsService, BidsService>();

            services.AddAuthentication(Configuration);
            
            services.AddControllers(options => {
                options.Filters.Add(new AuthorizeFilter());
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthentication();
			app.UseAuthorization();

            app.UseMiddleware<CustomExceptionMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
