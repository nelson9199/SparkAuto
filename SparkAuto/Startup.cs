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
using SparkAuto.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Xml;
using SparkAuto.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.UI.Services;
using SparkAuto.Email;

namespace SparkAuto
{
    public class Startup
    {
        List<CultureInfo> cultures = new List<CultureInfo> { new CultureInfo("es"), new CultureInfo("en") };
        IStringLocalizerFactory localizerFactory;
        IStringLocalizer localizer;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDbInitializer, DbInitializer>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders()
                 .AddErrorDescriber<SpanishIdentityErrorDescriber>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            //Aqui puedo poner los path especificos que yo desee a las paginas de identity server 4
            services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
                 options =>
                {
                    options.LoginPath = "/Identity/Account/Login";
                    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                    options.LogoutPath = "/Identity/Account/AccessDenied/Logout";
                });

            services.AddSingleton<IEmailSender, EmailSender>();
            //Registra una Instancia Configurada con IOptions
            services.Configure<EmailOptions>(Configuration); //Solo se pone Configuration porque como EmailOptions tiene una propiedad que se llama igual a una Key del appsettings.json Configuration va a asignar automaticamente el valor de esa Key en la propiedad con el mismo nombre en la Clase que se esta configurando aqui que es EmailOptions

            //Añadiedo la Localizacion y donde encontrar el ResorcePath
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddRazorPages()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization()
                .AddMvcOptions(o =>
                {
                    if (localizerFactory != null)
                    {
                        localizer = localizerFactory.Create("ModelBindingMessages", "SparkAuto");
                        o.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(x => localizer["The field {0} must be a number.", x]);

                        o.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => localizer["The value '{0}' is not valid for {1}.", x, y]);
                        o.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(x => localizer["A value for the '{0}' property was not provided", x]);
                        o.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => localizer["A value is required."]);

                        o.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(x => localizer["Null value is invalid.", x]);

                        o.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(x => localizer["The supplied value is invalid for {0}.", x]);

                        o.ModelBindingMessageProvider.SetValueIsInvalidAccessor(x => localizer["The value '{0}' is invalid.", x]);

                    }
                })
                .AddRazorRuntimeCompilation();

            services.AddAuthentication()
                .AddFacebook(fb =>
                {
                    fb.AppId = "954528118307287";
                    fb.AppSecret = "576a702b54a9cc10d26e596d5e248bb8";
                })
            .AddGoogle(go =>
            {
                go.ClientId = "1071762629918-ftn0qeeccl8bvs1hgbj8vbs5naof2g0e.apps.googleusercontent.com";
                go.ClientSecret = "GoNnGdYT8_HqZHjYRZsuOiyj";
            });
        }

        private IStringLocalizerFactory ObtenerIStringLocalizerFactory(IServiceProvider provider)
        {
            return provider.GetService<IStringLocalizerFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
        {
            var applicationServices = app.ApplicationServices;

            localizerFactory = ObtenerIStringLocalizerFactory(applicationServices);

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
            dbInitializer.Initialize();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseRequestLocalization(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("es");
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
