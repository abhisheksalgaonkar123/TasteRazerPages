using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Taste.DataAccess;
using Taste.DataAccess.Data.Repository.IRepository;
using Taste.DataAccess.Data.Repository;
using Microsoft.AspNetCore.Identity.UI.Services;
using Taste.Utility;
using Microsoft.CodeAnalysis.Options;
using System.Configuration;
using Stripe;
using Microsoft.AspNetCore.Authentication.Facebook;

namespace Taste
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser,IdentityRole>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            services.AddMvc(options => options.EnableEndpointRouting = false)
                    .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.ConfigureApplicationCookie(options =>

            {

                options.LoginPath = $"/Identity/Account/Login";

                options.LogoutPath = $"/Identity/Account/Logout";

                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";

            });
            services.AddAuthentication().AddFacebook(fbOptions =>
            {
                fbOptions.AppId = "810625529751981";
                fbOptions.AppSecret = "fffcbf64c721af687e68592a11b4d598";
            });
            services.AddAuthentication().AddMicrosoftAccount(msoptions =>
            {
                msoptions.ClientId = "df6c74b2-9f06-4d41-80d5-59683030116a";
                msoptions.ClientSecret = " X6qTj1pq_3Fm9d~1KT77J~5~pBEQaTfm7m";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMvc();
            StripeConfiguration.ApiKey = Configuration.GetSection("Stripe")["SecretKey"];
        }
    }
}
