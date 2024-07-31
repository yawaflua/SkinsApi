using AspNetCoreRateLimit;
using Microsoft.OpenApi.Models;
using SkinsApi.Interfaces.Services;
using SkinsApi.Services;
using System.Reflection;
using System.Text.Json.Serialization;

namespace SkinsApi
{
    public class Startup
    {
        public static readonly string PROFILE_MOJANG_API = "https://api.mojang.com/users/profiles/minecraft/";
        public static readonly string SESSION_MOJANG_API = "https://sessionserver.mojang.com/session/minecraft/profile/";
        public IConfiguration configuration { get; internal set; }
        public Startup()
        {
            configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: true)
            .Build();


        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(l => l.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            services
                .AddSwaggerGen(setup =>
                {
                    setup.SwaggerDoc("v1", new()
                    {
                        Description = "That`s API for getting slices of minecraft skin.",
                        Title = "yawaflua.ru API",
                        Version = "v1.0",
                        Contact = new OpenApiContact()
                        {
                            Name = "Dima Andreev",
                            Email = "skins-swagger@yawaflua.ru",
                            Url = new Uri("https://yawaflua.ru/")
                        },
                        License = new OpenApiLicense()
                        {
                            Name = "MIT Licence",
                            Url = new Uri("https://yawaflua.ru/eula")
                        },
                        TermsOfService = new Uri("https://yawaflua.ru/privacy")
                    });

                    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    // setup.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), true);
                    setup.AddServer(new OpenApiServer() { Url = "https://skins.yawaflua.ru/", Description = "Default web-server" });
#if DEBUG
                    setup.AddServer(new OpenApiServer() { Url = "http://localhost/", Description = "Dev web-server" });
#endif
                })
                .AddRouting()
                .AddSingleton(new HttpClient())
                .AddSingleton<ISkinService, SkinService>()
                .AddSingleton(configuration)
                .AddMemoryCache();
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
            services.AddInMemoryRateLimiting();

            // Добавление Rate Limiting Middleware
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }



            app.UseSwagger(c =>
            {
                c.RouteTemplate = "/swagger/v1/swagger.json";
                c.SerializeAsV2 = true;
            });
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
                options.DisplayRequestDuration();
                options.EnablePersistAuthorization();
                options.OAuthAppName("yawaflua.ru/api");
                options.ShowExtensions();
            });

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors(k => { k.AllowAnyHeader(); k.AllowAnyMethod(); k.AllowAnyOrigin(); });
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapSwagger();
                endpoints.MapControllers().AllowAnonymous();

            });
        }
    }
}
