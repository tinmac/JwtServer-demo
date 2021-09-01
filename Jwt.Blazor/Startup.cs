using Jwt.Blazor.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jwt.Blazor
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
            services.AddHttpClient();

            services.AddSingleton<WeatherForecastService>();



            //// Custom Auth - needs casting whenever the service is used.
            //services.AddScoped<AuthenticationStateProvider, MyAuthenticationStateProvider>();

            //// Custom Auth - avoids casting where the service is used.
            services.AddScoped<MyAuthenticationStateProvider, MyAuthenticationStateProvider>();

            services.AddScoped<AuthenticationStateProvider>(
              p => p.GetService<MyAuthenticationStateProvider>());



            // Authorization (what are you allowed to do)
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy(MyPolicies.HasSub, MyPolicies.HasSubPolicy());
                opt.AddPolicy(MyPolicies.IsMultiUser, MyPolicies.IsMultiUserPolicy());
                opt.AddPolicy(MyPolicies.IsWorker, MyPolicies.IsWorkerPolicy());
                opt.AddPolicy(MyPolicies.IsAgent, MyPolicies.IsAgentPolicy());
                opt.AddPolicy(MyPolicies.IsBooker, MyPolicies.IsBookerPolicy());
            });

            // My Sevices
            services.AddSingleton(Configuration);


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
    }
}
