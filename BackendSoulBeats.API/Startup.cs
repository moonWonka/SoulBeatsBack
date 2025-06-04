using BackendSoulBeats.API.Configuration;
using BackendSoulBeats.API.Middleware;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;               // Para ApiVersion
using Microsoft.AspNetCore.Mvc.Versioning;     // Para AddApiVersioning
using Microsoft.AspNetCore.Mvc.ApiExplorer;    // Para AddVersionedApiExplorer
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BackendSoulBeats.Domain.Application.V1.Services;
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
            services.AddSwaggerGen();

            // Registro de MediatR (buscando los handlers en el assembly)
            services.AddMediatR(Assembly.GetExecutingAssembly());

            // Configuración del versionado de la API
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0); // Versión por defecto 1.0
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            // Registro del explorador de versiones para Swagger
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";  // Ejemplo: v1
                options.SubstituteApiVersionInUrl = true;
            });

                // Política para usuarios con rol de administrador
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireClaim("role", "admin"));
            });

            // Registro de MediatR
            services.AddMediatR(typeof(Startup).Assembly);

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
            // Ejemplo:
            // services.AddScoped<IAuthService, AuthService>();
            // services.AddScoped<IOtroServicio, OtroServicio>();
            services.AddSingleton<IGoogleAuthService, IGoogleAuthService>();
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
                // Para entornos no Development (Producción, Staging, etc.)
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    //se puede configurar la descripción igual que arriba
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
