using Microsoft.Extensions.DependencyInjection;
using Moq;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using BackendSoulBeats.Domain.Application.V1.Services;

namespace BackendSoulBeats.UnitTests.Application.Services
{
    /// <summary>
    /// Factory para crear y configurar todos los mocks necesarios para los tests
    /// </summary>
    public static class MockServicesFactory
    {
        /// <summary>
        /// Registra todos los mocks necesarios en el contenedor de servicios
        /// </summary>
        public static void RegisterMockServices(IServiceCollection services)
        {
            // Mock de TelemetryClient
            var telemetryConfig = new TelemetryConfiguration();
            var mockTelemetryClient = new TelemetryClient(telemetryConfig);
            services.AddSingleton(mockTelemetryClient);            // Mock de IGoogleAuthService
            var googleAuthMock = MockGoogleAuthService.Create();
            services.AddSingleton<IGoogleAuthService>(googleAuthMock.Object);

            // Aquí puedes agregar más mocks según sea necesario
            // RegisterOtherMockServices(services);
        }

        /// <summary>
        /// Registra mocks adicionales específicos para diferentes escenarios de test
        /// </summary>
        public static void RegisterMockServicesForFailureScenarios(IServiceCollection services)
        {
            // Mock de TelemetryClient que simula fallos
            var telemetryConfig = new TelemetryConfiguration();
            var mockTelemetryClient = new TelemetryClient(telemetryConfig);
            services.AddSingleton(mockTelemetryClient);            // Mock de IGoogleAuthService que simula fallos
            var googleAuthMock = MockGoogleAuthService.CreateWithFailure();
            services.AddSingleton<IGoogleAuthService>(googleAuthMock.Object);
        }

        /// <summary>
        /// Registra mocks específicos para tests de integración
        /// </summary>
        public static void RegisterMockServicesForIntegrationTests(IServiceCollection services)
        {
            RegisterMockServices(services);
            
            // Aquí puedes agregar configuraciones específicas para tests de integración
        }
    }
}
