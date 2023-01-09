using ApexDataApi.AuthHandler;
using ApexDataApi.Models;
using ApexDataApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace ApexDataApi
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
            services.Configure<ApexPlayerDatabaseSettings>(
                Configuration.GetSection("ApexPlayerDatabase"));

            services.AddSingleton<PlayersService>();
            services.AddSingleton<CharactersService>();

            services.AddControllers();
            services.AddControllersWithViews();

            services.AddEndpointsApiExplorer();

            #region Configure Swagger
            services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
                        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Apex Legends API", Version = "v1" });
                c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    In = ParameterLocation.Header,
                    Description = "Basic Authorization header using the Bearer scheme."
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "basic"
                                }
                            },
                            new string[] {}
                    }
                });
            });
            #endregion

            services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, AuthenticationHandler>("BasicAuthentication", null);

            services.AddScoped<Services.AuthenticationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BasicAuth v1"));
            }

            app.UseHttpsRedirection();

            app.UseDefaultFiles();

            //Adding static path to 'Content' directory for images, css
           app.UseStaticFiles(new StaticFileOptions
           {
               FileProvider = new PhysicalFileProvider(
                   Path.Combine(Directory.GetCurrentDirectory(), @"Content")),
               RequestPath = "/Content"
           });

           app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
