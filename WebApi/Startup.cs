using FeatureAuthorize.PolicyCode;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;
using WebApi.Authorization;
using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Helpers;
using WebApi.Services;

namespace WebApi
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
            services.AddDbContext<DataContext>(options => options.UseNpgsql(Configuration.GetConnectionString("Postgresql")));
            services.AddControllers();

            // configure strongly typed settings objects
            IConfigurationSection appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            AppSettings appSettings = appSettingsSection.Get<AppSettings>();
            SymmetricSecurityKey Secret = new SymmetricSecurityKey(Encoding.Default.GetBytes(appSettings.SigningKey));
            SymmetricSecurityKey Key = new SymmetricSecurityKey(Encoding.Default.GetBytes(appSettings.EncryptionKey));

            // configure DI for application services
            //Scoped
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IErrorLogService, ErrorLogService>();
            services.AddScoped<IEmailConfigService, EmailConfigService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<IEmailSenderService, EmailSenderService>();
            services.AddScoped<IPermissionService, PermissionService>();
            //Identities
            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequiredLength = IdentitySettings.PasswordLength;
                options.Lockout.MaxFailedAccessAttempts = IdentitySettings.LockoutTries;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

            //Singletons
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //Register the Permission policy handlers
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();

            //Authentication
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(x =>
           {
               x.Events = new JwtBearerEvents
               {
                   OnTokenValidated = context =>
                   {
                       DateTime Expiration = DateTime.MinValue.AddSeconds(Convert.ToInt64(context.Principal.FindFirst("exp").Value));
                       DateTime CurrentTime = DateTime.Now;
                       TimeSpan TimeLeft = CurrentTime - Expiration;
                       if (TimeLeft.TotalMinutes < JwtSettings.RefreshTime) context.Response.Headers.Add("should_refresh", "true");
                       return Task.CompletedTask;
                   },
                   OnAuthenticationFailed = context =>
                   {
                       if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                       {
                           context.Response.Headers.Add("token_expired", "true");
                           context.Response.Headers.Add("should_refresh", "true");
                       }
                       return Task.CompletedTask;
                   }
               };
               x.RequireHttpsMetadata = false;
               x.SaveToken = true;
               x.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = Secret,
                   TokenDecryptionKey = Key,
                   ValidateIssuer = false,
                   ValidateAudience = false
               };
           });

            services.AddAuthorization(options =>
            {
                AuthorizationPolicyBuilder defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme);

                defaultAuthorizationPolicyBuilder =
                    defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();

                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}