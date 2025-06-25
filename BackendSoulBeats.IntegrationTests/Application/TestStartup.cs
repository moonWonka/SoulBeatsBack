using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using BackendSoulBeats.UnitTests.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace BackendSoulBeats.IntegrationTests.Application
{
    /// <summary>
    /// Startup personalizado para tests de integración
    /// Permite configurar mocks y dependencias específicas para testing
    /// </summary>
    public class TestStartup
    {
        public TestStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            // Configurar servicios básicos
            services.AddControllers()
                .AddApplicationPart(typeof(BackendSoulBeats.API.Application.V1.Controllers.UserController).Assembly); // Usar el assembly del controlador

            services.AddEndpointsApiExplorer();

            // Configurar API Versioning (igual que en el Startup principal)
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });

            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SoulBeats API Test", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
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
            });            // Registrar mocks usando el factory de unit tests
            MockServicesFactory.RegisterMockServicesForIntegrationTests(services);            // Configurar MediatR para MediatR 11.x
            services.AddMediatR(typeof(BackendSoulBeats.API.Application.V1.Query.GetUserInfoHandler).Assembly);// Configurar autenticación para tests
            services.AddAuthentication("Test")
                .AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                    "Test", options => { });

            // Configurar autorización con política personalizada para tests
            services.AddAuthorization(options =>
            {
                options.AddPolicy("FirebaseAuthenticated", policy =>
                {
                    policy.AuthenticationSchemes.Add("Test");
                    policy.RequireAuthenticatedUser();
                });

                options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder("Test")
                    .RequireAuthenticatedUser()
                    .Build();
            });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SoulBeats API Test v1"));
            }

            app.UseRouting();

            // Usar middleware de autenticación personalizado para tests
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // Debug: Log registered endpoints
                var endpointDataSource = endpoints.DataSources.FirstOrDefault();
                if (endpointDataSource != null)
                {
                    Console.WriteLine("=== Registered Endpoints ===");
                    foreach (var endpoint in endpointDataSource.Endpoints)
                    {
                        Console.WriteLine($"Endpoint: {endpoint.DisplayName}");
                        if (endpoint is Microsoft.AspNetCore.Routing.RouteEndpoint routeEndpoint)
                        {
                            Console.WriteLine($"  Route Pattern: {routeEndpoint.RoutePattern}");
                        }
                    }
                    Console.WriteLine("=== End Endpoints ===");
                }
            });
        }
    }

    /// <summary>
    /// Opciones para el esquema de autenticación de test
    /// </summary>
    public class TestAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public string DefaultUserId { get; set; } = "test-user-123";
        public string DefaultUserEmail { get; set; } = "test@example.com";
        public bool SimulateFailure { get; set; } = false;
    }

    /// <summary>
    /// Handler de autenticación personalizado para tests
    /// Permite simular diferentes escenarios de autenticación
    /// </summary>
    public class TestAuthenticationHandler : AuthenticationHandler<TestAuthenticationSchemeOptions>
    {
        public TestAuthenticationHandler(IOptionsMonitor<TestAuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Verificar si se debe simular un fallo de autenticación
            if (Options.SimulateFailure)
            {
                return Task.FromResult(AuthenticateResult.Fail("Simulated authentication failure"));
            }

            // Verificar si hay token de autorización
            var authorizationHeader = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            // Simular validación de token
            if (token == "invalid_token_12345")
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid token"));
            }

            if (token == "valid_test_token")
            {
                // Crear claims para usuario autenticado
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, Options.DefaultUserId),
                    new Claim("firebase_uid", Options.DefaultUserId),
                    new Claim(ClaimTypes.Email, Options.DefaultUserEmail)
                };

                var identity = new ClaimsIdentity(claims, "Test");
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, "Test");

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }

            return Task.FromResult(AuthenticateResult.NoResult());
        }
    }
}
