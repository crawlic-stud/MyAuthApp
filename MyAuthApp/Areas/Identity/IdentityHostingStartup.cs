using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyAuthApp.Areas.Identity.Data;
using MyAuthApp.Data;

[assembly: HostingStartup(typeof(MyAuthApp.Areas.Identity.IdentityHostingStartup))]
namespace MyAuthApp.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<UsersDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("UsersDbContextConnection")));

                services.AddDefaultIdentity<User>(options => { 
                    options.SignIn.RequireConfirmedAccount = false;
                    options.SignIn.RequireConfirmedEmail = false;
                })
                    .AddEntityFrameworkStores<UsersDbContext>();
            });
        }
    }
}