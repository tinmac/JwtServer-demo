using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Jwt.Server.Data
{
    public class SeedDb
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlite(connectionString, o => o.MigrationsAssembly(typeof(Startup).Assembly.FullName)));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    context.Database.EnsureCreated();
                    // context.Database.Migrate(); // Apply any migrations by code (same as PMC's add-migration/update-database)

                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();


                    // Wendy
                    var wendy = userMgr.FindByNameAsync("wendy@email.com").Result;
                    if (wendy == null)
                    {
                        wendy = new ApplicationUser
                        {
                            UserName = "wendy@email.com",
                            Email = "wendy@email.com",
                            EmailConfirmed = true,
                        };
                        var result = userMgr.CreateAsync(wendy, "Pass&7").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(wendy, new Claim[]{
                            new Claim("name", "Wendy Smith"),
                            new Claim("worker","true"),
                        }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Log.Debug("wendy created");
                    }
                    else
                    {
                        Log.Debug("wendy already exists");
                    }



                    // Bob 
                    var bob = userMgr.FindByNameAsync("bob@email.com").Result;
                    if (bob == null)
                    {
                        bob = new ApplicationUser
                        {
                            UserName = "bob@email.com",
                            Email = "bob@email.com",
                            EmailConfirmed = true,
                        };
                        var result = userMgr.CreateAsync(bob, "Pass&7").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(bob, new Claim[]{
                            new Claim("name", "Bob Cratchet"),
                            new Claim("booker","true"),

                        }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Log.Debug("bob created");
                    }
                    else
                    {
                        Log.Debug("bob already exists");
                    }


                }
            }
        }
    }

}
