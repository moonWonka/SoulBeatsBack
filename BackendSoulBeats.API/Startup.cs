using BackendSoulBeats.API.Configuration;
using BackendSoulBeats.API.Middleware;
using Microsoft.OpenApi.Models;

namespace BackendSoulBeats.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Registro de controladores y Swagger
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SoulBeats API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("SoloUsuariosConEmail", policy =>
                    policy.RequireClaim(System.Security.Claims.ClaimTypes.Email));

                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireClaim("role", "admin"));
            });

            ConfigureServicesDependencies(services);
            ConfigureRepositoryDependencies(services);

            FirebaseInitializer.Initialize();
        }

        private void ConfigureServicesDependencies(IServiceCollection services)
        {
            // Ejemplo:
            // services.AddScoped<IAuthService, AuthService>();
        }

        private void ConfigureRepositoryDependencies(IServiceCollection services)
        {
            // Ejemplo:
            // services.AddScoped<IUserRepository, UserRepository>();
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                });
            }
            else
            {
                // Para entornos no Development (Producción, Staging, etc.)
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    //se puede configurar la descripción igual que arriba
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                });
            }
            app.UseRouting();
            app.UseMiddleware<FirebaseAuthenticationMiddleware>();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
