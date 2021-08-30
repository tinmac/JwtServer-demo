using Jwt.Blazor.Auth;
using Jwt.Blazor.Policies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<TokenProvider>();

            services.AddRazorPages();

            services.AddServerSideBlazor();


            #region Auth

            var key = Encoding.ASCII.GetBytes("supercalifragilisticexpialidocious");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
            //.AddCookie(options  => options.SlidingExpiration = true)
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;// Q. Does this save a cookie? - No it stores the token in 'AuthenticationProperties'

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
                        Debug.WriteLine($"JwtBearer - OnMessageReceived  -  access_token in QS: {ShorterJWT(accessToken)}");

                        //
                        // Try to attach the token manually  
                        //

                        // Wendy Worker Token -  10 year - Retreived by calling Jwt.Server's 'https://localhost:5001/token/authenticate' using Postman
                        //accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6ImVhMjQ3OGRlLTYyMTctNDdlYy1iNTMwLTNkMDllODFlOWExMiIsIm5hbWUiOiJXZW5keSBTbWl0aCIsIndvcmtlciI6InRydWUiLCJuYmYiOjE2Mjk3MzcxNjgsImV4cCI6MTk0NTI2OTk2OCwiaWF0IjoxNjI5NzM3MTY4fQ.lBoLrG2bu_4hSWw-tE3a1jvrkRoKS35m-9bR_MD0CyA";

                        // Bob Booker Token - 10 year 
                        accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjNjMzIxMjI2LTFhNGYtNGQ3NS1iZjM1LTIwYjQ1NmVlYmYzOSIsImJvb2tlciI6InRydWUiLCJuYW1lIjoiQm9iIENyYXRjaGV0IiwibmJmIjoxNjI5NzM3MTk4LCJleHAiOjE5NDUyNjk5OTgsImlhdCI6MTYyOTczNzE5OH0.N9HUFM3zIl9BkoPJlPj_WQIUhhQ2Rk53ymEEm42LTD4";

                        context.Token = accessToken;

                        Debug.WriteLine($"    manually setting context.Token: {ShorterJWT(accessToken)}");

                        return Task.CompletedTask;
                    }, 
                    
                    OnTokenValidated = context => 
                    {
                        Debug.WriteLine($"JwtBearer - OnTokenValidated ");

                        ClaimsPrincipal userPrincipal = context.Principal;
                        foreach(var clm in userPrincipal.Claims)
                        {
                            Debug.WriteLine($"    '{clm.Type}':'{clm.Value}'");
                        }
                        return Task.CompletedTask;
                    }, 

                    OnAuthenticationFailed = context =>
                    {
                        Debug.WriteLine($"JwtBearer - OnAuthenticationFailed ");
                        return Task.CompletedTask;
                    }, 
                    
                    OnChallenge = context =>
                    {
                        Debug.WriteLine($"JwtBearer - OnChallenge ");
                        return Task.CompletedTask;
                    },

                    OnForbidden = context =>
                    {
                        Debug.WriteLine($"JwtBearer - OnForbidden ");
                        return Task.CompletedTask;
                    }



                };
            });


            // Authorization (what are you allowed to do)
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy(MyPolicies.IsWorker, MyPolicies.IsWorkerPolicy());
                opt.AddPolicy(MyPolicies.IsBooker, MyPolicies.IsBookerPolicy());
            });

            #endregion

        }


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

            // Auth
            app.UseAuthentication();
            //app.UseAuthorization();
            app.UseMiddleware<JwtAuthorizationMiddleware>();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
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

                Debug.WriteLine("Token  VALIDATED");
                Debug.WriteLine($"user_type: {ut}");

                return true;
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes

                Debug.WriteLine("Token  NOT  VALID");
                return false;
            }
        }

        private void MoveCmdWindow()
        {
            var ps1File = @".\move_cmd.ps1";
            var startInfo = new ProcessStartInfo()
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy unrestricted -file \"{ps1File}\"",
                UseShellExecute = false
            };
            Process.Start(startInfo);

            #region different method
            //PowerShell ps = PowerShell.Create();
            //ps.AddCommand("Get-Process");
            //Console.WriteLine(ps); // this line OR ps.Invoke() NOT BOTH
            ////ps.Invoke();
            #endregion
        }

    }
}
