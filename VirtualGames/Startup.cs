using System;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VirtualGames.Common;
using VirtualGames.Common.Interface;
using VirtualGames.Data;
using VirtualGames.Data.Boggle;
using VirtualGames.Data.GuessWho;
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
            services.AddApplicationInsightsTelemetry();

            // Database
            var dbConfig = Configuration.GetSection("CosmosDb");
            services.AddSingleton<IRepository<Game>>(InitializeCosmosClientInstanceAsync<Game>(dbConfig));
            services.AddSingleton<IRepository<Password>>(InitializeCosmosClientInstanceAsync<Password>(dbConfig));
            services.AddSingleton<IRepository<GuessWhoItem>>(InitializeCosmosClientInstanceAsync<GuessWhoItem>(dbConfig));
            services.AddSingleton<IRepository<BoggleDie>>(InitializeCosmosClientInstanceAsync<BoggleDie>(dbConfig));

            // Services
            services.AddSingleton<PasswordService>();
            services.AddSingleton<GuessWhoService>();
            services.AddSingleton<BoggleService>();


            if (!services.Any(x => x.ServiceType == typeof(HttpClient)))
            {
                // Setup HttpClient for server side in a client side compatible fashion
                services.AddScoped<HttpClient>(s =>
                {
                    // Creating the URI helper needs to wait until the JS Runtime is initialized, so defer it.
                    var uriHelper = s.GetRequiredService<NavigationManager>();
                    return new HttpClient
                    {
                        BaseAddress = new Uri(uriHelper.BaseUri)
                    };
                });
            }
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
            where T : BaseDataItem
        {
            var databaseName = configuration.GetSection("DatabaseName").Value;
            var account = configuration.GetSection("Account").Value;
            var key = configuration.GetSection("Key").Value;
            var client = new CosmosClient(account, key);
            return new Repository<T>(client, databaseName);
        }
    }
}
