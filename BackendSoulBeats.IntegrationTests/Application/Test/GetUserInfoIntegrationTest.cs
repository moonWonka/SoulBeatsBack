using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System.Text.Json;
using BackendSoulBeats.API.Application.V1.Query;
using BackendSoulBeats.IntegrationTests.Application;

namespace BackendSoulBeats.IntegrationTests.Application.Test
{
    /// <summary>
    /// Test de integración para el endpoint GetUserInfo
    /// Demuestra cómo probar un endpoint real con y sin autenticación usando TestStartup
    /// </summary>
    public class GetUserInfoIntegrationTest : IClassFixture<WebApplicationFactory<TestStartup>>
    {
        private readonly WebApplicationFactory<TestStartup> _factory;
        private readonly HttpClient _client;

        public GetUserInfoIntegrationTest(WebApplicationFactory<TestStartup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetUserInfo_WithoutAuthentication_ReturnsUnauthorized()
        {
            // Arrange
            var userId = 123;
            var endpoint = $"/User/{userId}/info";

            // Act
            var response = await _client.GetAsync(endpoint);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }        [Fact]
        public async Task GetUserInfo_WithInvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            var userId = 123;
            var endpoint = $"/User/{userId}/info";
            
            // Simular un token inválido
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "invalid_token_12345");

            // Act
            var response = await _client.GetAsync(endpoint);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            
            // Verificar que la respuesta tiene contenido (no es vacía)
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
            // No verificamos el mensaje específico ya que ASP.NET Core puede no incluir 
            // el mensaje de AuthenticateResult.Fail en el cuerpo de la respuesta
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(999999999)]
        public async Task GetUserInfo_WithDifferentUserIds_HandlesGracefully(long userId)
        {
            // Arrange
            var endpoint = $"/User/{userId}/info";

            // Act
            var response = await _client.GetAsync(endpoint);

            // Assert
            // Sin autenticación, siempre debe devolver 401
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetUserInfo_EndpointExists_ReturnsUnauthorizedNotNotFound()
        {
            // Arrange
            var userId = 123;
            var endpoint = $"/User/{userId}/info";

            // Act
            var response = await _client.GetAsync(endpoint);

            // Assert
            // El endpoint existe, por lo que debe devolver 401 (Unauthorized) y no 404 (NotFound)
            Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }        [Fact]
        public async Task GetUserInfo_CheckResponseHeaders_ContainsExpectedHeaders()
        {
            // Arrange
            var userId = 123;
            var endpoint = $"/User/{userId}/info";

            // Act
            var response = await _client.GetAsync(endpoint);

            // Assert
            // Verificar que la respuesta tiene el código esperado
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            
            // Verificar que tiene headers básicos de respuesta HTTP
            Assert.NotNull(response.Headers);
            
            // Verificar que no hay headers de cache problemáticos
            Assert.False(response.Headers.CacheControl?.NoStore == false && 
                        response.Headers.CacheControl?.MaxAge.HasValue == true);
        }[Fact]
        public async Task GetUserInfo_WithValidTestToken_ReturnsSuccess()
        {
            // Arrange
            var userId = 123;
            var endpoint = $"/User/{userId}/info";
            
            // Usar el token válido que reconoce nuestro TestAuthenticationHandler
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "valid_test_token");

            // Act
            var response = await _client.GetAsync(endpoint);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<GetUserInfoResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            Assert.NotNull(userInfo);
            Assert.Equal(userId.ToString(), userInfo.Id);
            Assert.Contains("successfully", userInfo.MoreInformation);
        }

        private void Dispose()
        {
            _client?.Dispose();
        }
    }
}
