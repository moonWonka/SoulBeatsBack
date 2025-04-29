using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;               // Para ApiVersion
using Microsoft.AspNetCore.Mvc.Versioning;     // Para AddApiVersioning
using Microsoft.AspNetCore.Mvc.ApiExplorer;    // Para AddVersionedApiExplorer
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

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

            // Registro de otros servicios (lógica de negocio)
            ConfigureServicesDependencies(services);

            // Registro de repositorios (acceso a datos)
            ConfigureRepositoryDependencies(services);

            // Inicializa Firebase Admin SDK
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("Secrets/serviceAccountKey.json")
            });
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
