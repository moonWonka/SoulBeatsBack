using Moq;
using BackendSoulBeats.Domain.Application.V1.Services;

namespace BackendSoulBeats.UnitTests.Application.Services
{
    /// <summary>
    /// Mock del servicio de autenticación de Google para tests unitarios
    /// </summary>
    public class MockGoogleAuthService
    {
        public static Mock<IGoogleAuthService> Create()
        {
            var mock = new Mock<IGoogleAuthService>();
            
            // Configurar comportamiento por defecto para RegisterUserAsync
            mock.Setup(x => x.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("test-firebase-uid-123");

            // Configurar comportamiento por defecto para IsUserRegisteredAsync
            mock.Setup(x => x.IsUserRegisteredAsync(It.IsAny<string>()))
                .ReturnsAsync(false); // Por defecto, usuarios no están registrados

            // Configurar casos específicos
            mock.Setup(x => x.IsUserRegisteredAsync("existing@example.com"))
                .ReturnsAsync(true);

            return mock;
        }

        public static Mock<IGoogleAuthService> CreateWithFailure()
        {
            var mock = new Mock<IGoogleAuthService>();
            
            // Simular fallo en el registro
            mock.Setup(x => x.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Google Auth registration failed"));

            // Simular fallo en la verificación
            mock.Setup(x => x.IsUserRegisteredAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Google Auth verification failed"));

            return mock;
        }

        public static Mock<IGoogleAuthService> CreateForExistingUser()
        {
            var mock = new Mock<IGoogleAuthService>();
            
            // Usuario ya existe
            mock.Setup(x => x.IsUserRegisteredAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            // Registro fallará porque el usuario ya existe
            mock.Setup(x => x.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("User already exists"));

            return mock;
        }
    }
}
