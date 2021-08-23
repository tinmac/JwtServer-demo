using Jwt.Client_SignalR.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Jwt.Client_SignalR
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddRazorPages();
            services.AddSignalR();

            #region AddAuthentication - WORKING

            var key = Encoding.ASCII.GetBytes("supercalifragilisticexpialidocious");

            services.AddAuthentication(options =>
            {
                // Identity made Cookie authentication the default.
                // However, we want JWT Bearer Auth to be the default.
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;// resolves to 'Bearer'
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;// resolves to 'Bearer'

            })
            .AddJwtBearer(options =>
            {

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key), 
                };


                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Is token in Querystring?
                        var accessToken = context.Request.Query["access_token"];

                        Debug.WriteLine($"JwtBearer - OnMessageReceived: {ShorterJWT(accessToken)}");

                        if (!string.IsNullOrWhiteSpace(accessToken))
                        {
                            var path = context.HttpContext.Request.Path;
                            if (path.StartsWithSegments("/chatHub"))
                            {
                                // The token is meant for chatHub (not some other endpoint)  
                                context.Token = accessToken; // add the token to the context, the Auth middleware will then have something to validate when the user accesse's an endpoint like SignalR (in this case) or for eg Api address in another case.

                                Debug.WriteLine($"Path: {path} - Token: {ShorterJWT(accessToken)}");

                                // This manual 'ValidateJwt' is adding overhead & isnt needed (only to see the result), the middleware does the same validaiton too.
                                if (ValidateJwt(accessToken) == true)
                                {
                                    Debug.WriteLine("Token  VALIDATED");
                                }
                                else
                                {
                                    Debug.WriteLine("Token  NOT  VALID");
                                }
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
            });
            #endregion

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
                //app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }



        // HELPERS
        private string ShorterJWT(string token)
        {
            // Pass back the first & last 10 digits of the Token
            if (!string.IsNullOrWhiteSpace(token))
                return $"{token.Substring(0, 10)}...{token.Substring(token.Length - 10, 10)}";

            return "no token!";
        }


        private bool ValidateJwt(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                //var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var key = Encoding.ASCII.GetBytes("LenToveRyanLiamLillyPoppyLenny");
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                // I want to rely soley on claims and not touch the Users DB (which would have to be done via an api as the DB is in another project)
                // var userId = jwtToken.Claims.First(x => x.Type == "id").Value;

                // attach user to context on successful jwt validation
                // var usr = userService.GetById(userId);
                // context.Items["User"] = usr;

                // Here I set a 'context.Item' with a key of 'user_type' and set is value to the claim that is true.
                // The 'context.item["user_type"]' is read and used by the 'AuthorizeAttribute'  

                string ut = "'user_type' NOT SET";
                if (jwtToken.Claims.FirstOrDefault(o => o?.Type == "worker")?.Value == "true") { ut = "worker"; }
                else if (jwtToken.Claims.FirstOrDefault(o => o?.Type == "booker")?.Value == "true") { ut = "booker"; }
                else if (jwtToken.Claims.FirstOrDefault(o => o?.Type == "agent")?.Value == "true") { ut = "agent"; }
                else { }

                Debug.WriteLine($"user_type: {ut}");

                return true;
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
                return false;
            }
        }

    }
}
