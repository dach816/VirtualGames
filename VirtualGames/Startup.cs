using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VirtualGames.Common;
using VirtualGames.Common.Interface;
using VirtualGames.Data;
using VirtualGames.Data.Password;

namespace VirtualGames
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            // Database
            var dbConfig = Configuration.GetSection("CosmosDb");
            services.AddSingleton<IRepository<Password>>(InitializeCosmosClientInstanceAsync<Password>(dbConfig));

            // Services
            services.AddSingleton<WeatherForecastService>();
            services.AddSingleton<PasswordService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private static Repository<T> InitializeCosmosClientInstanceAsync<T>(IConfiguration configuration)
        {
            var databaseName = configuration.GetSection("DatabaseName").Value;
            var account = configuration.GetSection("Account").Value;
            var key = configuration.GetSection("Key").Value;
            var client = new CosmosClient(account, key);
            return new Repository<T>(client, databaseName);
        }
    }
}
