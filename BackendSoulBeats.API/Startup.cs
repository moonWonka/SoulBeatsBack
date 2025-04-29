using BackendSoulBeats.API.Configuration;
using BackendSoulBeats.API.Middleware;

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

            // Configuración de políticas de autorización
            services.AddAuthorization(options =>
            {
                options.AddPolicy("SoloUsuariosConEmail", policy =>
                    policy.RequireClaim(System.Security.Claims.ClaimTypes.Email));
            });

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

            app.UseMiddleware<FirebaseAuthenticationMiddleware>(); // Middleware de autenticación
            app.UseAuthorization(); // Sin argumentos

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
