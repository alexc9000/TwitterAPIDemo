using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TwitterAPIClient.Authentication;
using TwitterAPIClient.BLL;
using TwitterAPIClient.Helpers;
using TwitterAPIClient.Interfaces;
using TwitterAPIClient.Models;
using TwitterAPIClient.Service;
using TwitterAPIDemo.Helpers;
using TwitterAPIDemo.Interfaces;

namespace TwitterAPIDemo
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<TwitterService>();

            services.AddControllersWithViews();
            services.AddSingleton<IAuth, Auth>();
            services.AddSingleton<ITweetActions, TweetActions>();
            services.AddSingleton<ITweetStats, TweetStats>();
            services.AddSingleton<ITweetHelper, TweetHelper>();
            services.AddSingleton<ICalculateHelper, CalculateHelper>();
            services.AddSingleton<IClientError, ClientError>();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
