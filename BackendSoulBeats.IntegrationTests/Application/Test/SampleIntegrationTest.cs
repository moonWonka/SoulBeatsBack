using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BackendSoulBeats.IntegrationTests.Application.Test
{
    public class SampleIntegrationTest : IClassFixture<WebApplicationFactory<BackendSoulBeats.API.Program>>
    {
        private readonly WebApplicationFactory<BackendSoulBeats.API.Program> _factory;

        public SampleIntegrationTest(WebApplicationFactory<BackendSoulBeats.API.Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_SwaggerEndpoint_ReturnsSuccess()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/swagger/index.html");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}