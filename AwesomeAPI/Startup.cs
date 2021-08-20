using System;
using System.Security.Claims;
using System.Text;
using AwesomeAPI.Authentication;
using AwesomeAPI.Authentication.Entities;
using AwesomeAPI.Authentication.Requirements;
using AwesomeAPI.BusinessLayer.Services;
using AwesomeAPI.BusinessLayer.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

namespace AwesomeAPI
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
            var jwtSettings = Configure<JwtSettings>(nameof(JwtSettings));

            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "AwesomeAPI", Version = "v1" });
                // Configure swagger to use an header parameter to pass the Bearer
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Insert the bearer token",
                    Name = HeaderNames.Authorization,
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            services.AddDbContext<AuthenticationsDbContext>(options =>
            {
                var connectionString = Configuration.GetConnectionString("SqlLite");
                options.UseSqlite(connectionString, b => b.MigrationsAssembly("AwesomeAPI"));
                //options.UseNpgsql(connectionString, b => b.MigrationsAssembly("AwesomeAPI"));
                //options.UseSqlServer(connectionString, b => b.MigrationsAssembly("AwesomeAPI"));
            
            });

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
            })
                .AddEntityFrameworkStores<AuthenticationsDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey)),
                        RequireExpirationTime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                }
            );

            services.AddAuthorization(options =>
            {
                var policyBuilder = new AuthorizationPolicyBuilder().RequireAuthenticatedUser();
                policyBuilder.Requirements.Add(new UserActiveRequirement());
                options.FallbackPolicy = policyBuilder.Build();
                //options.FallbackPolicy = options.DefaultPolicy;                
            });

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IAuthorizationHandler, UserActiveHandler>();

            T Configure<T>(string sectionName) where T : class
            {
                var section = Configuration.GetSection(sectionName);
                var settings = section.Get<T>();
                services.Configure<T>(section);

                return settings;
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AwesomeAPI v1"));
            }

            app.UseHttpsRedirection();

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
