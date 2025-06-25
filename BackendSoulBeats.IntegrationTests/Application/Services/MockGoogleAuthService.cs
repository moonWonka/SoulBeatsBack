using Moq;

namespace BackendSoulBeats.IntegrationTests.Application.Services
{
    /// <summary>
    /// Mock del servicio de autenticación de Google para tests de integración
    /// </summary>
    public static class MockGoogleAuthService
    {
        /// <summary>
        /// Crea un mock básico del IGoogleAuthService
        /// </summary>
        public static Mock<object> Create()
        {
            var mock = new Mock<object>();
            
            // Aquí puedes configurar comportamientos específicos del mock
            // Por ejemplo:
            // mock.Setup(x => x.AuthenticateAsync(It.IsAny<string>()))
            //     .ReturnsAsync(new AuthResult { Success = true });

            return mock;
        }

        /// <summary>
        /// Crea un mock que simula fallo de autenticación
        /// </summary>
        public static Mock<object> CreateWithFailure()
        {
            var mock = new Mock<object>();
            
            // Configurar comportamiento de fallo
            // mock.Setup(x => x.AuthenticateAsync(It.IsAny<string>()))
            //     .ThrowsAsync(new AuthenticationException("Mock authentication failed"));

            return mock;
        }
    }
}
