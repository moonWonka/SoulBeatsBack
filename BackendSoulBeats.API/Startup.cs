using BackendSoulBeats.API.Configuration;
using BackendSoulBeats.API.Middleware;
using MediatR;
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

            // Configuración de políticas de autorización
            services.AddAuthorization(options =>
            {
                // Política para usuarios que tienen un email registrado
                options.AddPolicy("SoloUsuariosConEmail", policy =>
                    policy.RequireClaim(System.Security.Claims.ClaimTypes.Email));

                // Política para usuarios con rol de administrador
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireClaim("role", "admin"));
            });

            // Registro de MediatR
            services.AddMediatR(typeof(Startup).Assembly);

            // Otros servicios...
            ConfigureServicesDependencies(services);
            ConfigureRepositoryDependencies(services);

            // Inicializa Firebase Admin SDK
            FirebaseInitializer.Initialize();
        }

        /// <summary>
        /// Método para registrar las inyecciones de servicios (por ejemplo, servicios de negocio).
        /// </summary>
        private void ConfigureServicesDependencies(IServiceCollection services)
        {
            // Ejemplo:
            // services.AddScoped<IAuthService, AuthService>();
            // services.AddScoped<IOtroServicio, OtroServicio>();
        }

        /// <summary>
        /// Método para registrar las inyecciones de repositorios (por ejemplo, acceso a la base de datos).
        /// </summary>
        private void ConfigureRepositoryDependencies(IServiceCollection services)
        {
            // Ejemplo:
            // services.AddScoped<IUserRepository, UserRepository>();
            // services.AddScoped<IProductRepository, ProductRepository>();
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
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            // ¡IMPORTANTE! El orden es crucial
            app.UseAuthentication(); // Autenticación JWT de .NET Core

            // // Middleware personalizado de Firebase (sin usar extensión)
            // app.UseMiddleware<FirebaseAuthenticationMiddleware>();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
