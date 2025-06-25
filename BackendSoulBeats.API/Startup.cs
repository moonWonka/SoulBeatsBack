using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Versioning;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using BackendSoulBeats.API.Middleware;
using BackendSoulBeats.Domain.Application.V1.Services;
using BackendSoulBeats.Infra.Application.V1.Services;
using BackendSoulBeats.API.Configuration;

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
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SoulBeats API",
                    Version = "v1",
                    Description = "Backend para la aplicación SoulBeats, proporcionando servicios de autenticación, gestión de usuarios, y otras funcionalidades."
                });

                // Configuración de seguridad para Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Ingrese el token JWT en el formato: Bearer {token}"
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
                        new string[] {}
                    }
                });
            });

            // Registro de MediatR
            services.AddMediatR(typeof(Startup).Assembly);

            // Configuración del versionado de la API
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;

                // Configurar para usar encabezados en lugar de query parameters
                options.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });

            // Registro del explorador de versiones para Swagger
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";  // Ejemplo: v1
                options.SubstituteApiVersionInUrl = true;
            });

            // Configuración de políticas de autorización
            services.AddAuthorization(options =>
            {
                options.AddPolicy("FirebaseAuthenticated", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddAuthenticationSchemes("Firebase");
                });

                options.AddPolicy("AllowAnonymous", policy =>
                {
                    policy.RequireAssertion(_ => true); // Permitir acceso sin autenticación
                });
            });

            // Otros servicios...
            ConfigureServicesDependencies(services);

            // Registro de repositorios (acceso a datos)
            ConfigureRepositoryDependencies(services);
        }

        /// <summary>
        /// Método para registrar las inyecciones de servicios (por ejemplo, servicios de negocio).
        /// </summary>
        private void ConfigureServicesDependencies(IServiceCollection services)
        {
            services.AddSingleton<IGoogleAuthService, GoogleAuthService>();
        }

        /// <summary>
        /// Método para registrar las inyecciones de repositorios (por ejemplo, acceso a la base de datos).
        /// </summary>
        private void ConfigureRepositoryDependencies(IServiceCollection services)
        {
            // Ejemplo:
            // services.AddScoped<IUserRepository, UserRepository>();
            // services.AddScoped<IProductRepository, ProductRepository>();
        }        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            // Inicializar Firebase usando el FirebaseInitializer
            FirebaseInitializer.Initialize();

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
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication(); // Autenticación JWT de .NET Core

            app.UseMiddleware<FirebaseAuthenticationMiddleware>(); // Middleware de Firebase

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
