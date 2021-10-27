using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Pokedex.Middleware;
using Pokedex.Services.ApiClient;
using Pokedex.Services.PokemonBuilder;
using Pokedex.Services.PokemonService;
using RestSharp;

namespace Pokedex
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pokedex", Version = "v1" });
            });
            services.AddTransient<IRestClient, RestClient>();
            services.AddTransient<IApiClient, ApiClient>();
            services.AddTransient<IPokemonBuilder, PokemonBuilder>();
            services.AddTransient<IPokemonService, PokemonService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pokedex v1");
            });

            app.UseMiddleware<ExceptionAdapterMiddleware>();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
