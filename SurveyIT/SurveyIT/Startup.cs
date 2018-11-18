using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SurveyIT.Interfaces.Services;
using SurveyIT.Services;
using SurveyIT.Models.DBModels;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using SurveyIT.Enums;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace SurveyIT
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
            services.AddAuthentication("FiverSecurityScheme")
                .AddCookie("FiverSecurityScheme", options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.Name = "Auth";
                    options.Events.OnRedirectToLogin = p =>
                    {
                        if (p.Request.Path.StartsWithSegments("/api") && p.Response.StatusCode == 200)
                        {
                            p.Response.StatusCode = 401;
                        }

                        return Task.CompletedTask;
                    };

                    options.Events.OnRedirectToAccessDenied = p =>
                    {
                        if (p.Request.Path.StartsWithSegments("/api") && p.Response.StatusCode == 200)
                        {
                            p.Response.StatusCode = 403;
                        }

                        return Task.CompletedTask;
                    };
                });
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("MustBeAdmin", p => p.RequireAuthenticatedUser().RequireRole(Role.Admin.ToString()));
            //});
            //.AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = Configuration["Jwt:Issuer"],
            //        ValidAudience = Configuration["Jwt:Issuer"],
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
            //    };
            //});

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builderr => builderr.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.DateFormatString = "dd-MM-yyyy";
                });

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<ISurveyService, SurveyService>();

            services.AddDbContext<SurveyIT.DB.MyDbContext>(options =>
            options.UseSqlServer(Configuration["ConnectionString"],
            b => b.MigrationsAssembly("SurveyIT")));

            var builder = services.AddIdentityCore<Users>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 4;
            });

            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder.AddEntityFrameworkStores<SurveyIT.DB.MyDbContext>().AddDefaultTokenProviders();

            services.AddScoped<RoleManager<IdentityRole>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            
            app.UseMvc();

        }
    }
}
