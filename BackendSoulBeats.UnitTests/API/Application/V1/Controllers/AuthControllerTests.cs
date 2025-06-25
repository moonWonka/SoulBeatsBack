using Xunit;
using Moq;
using MediatR;
using Microsoft.Extensions.Logging;
using BackendSoulBeats.API.Application.V1.Controllers;
using BackendSoulBeats.API.Application.V1.Command.PostRegister;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using System;
using BackendSoulBeats.API.Application.V1.ViewModel.Common; // For BaseResponse

namespace BackendSoulBeats.UnitTests.API.Application.V1.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _authController = new AuthController(_mockMediator.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task PostRegister_ValidRequest_SendsRequestToMediator()
        {
            // Arrange
            var request = new PostRegisterRequest { UserEmail = "test@example.com", Password = "password" };
            var expectedResponse = new PostRegisterResponse { StatusCode = (int)HttpStatusCode.OK, MoreInformation = "fake-uid" };

            _mockMediator
                .Setup(m => m.Send(request, default))
                .ReturnsAsync(expectedResponse);

            // Act
            await _authController.PostRegister(request);

            // Assert
            _mockMediator.Verify(m => m.Send(request, default), Times.Once);
        }

        [Theory]
        [InlineData(HttpStatusCode.OK)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.NotFound)] // Another example
        public async Task PostRegister_MediatorReturnsResponse_ReturnsCorrectStatusCode(HttpStatusCode statusCode)
        {
            // Arrange
            var request = new PostRegisterRequest { UserEmail = "test@example.com", Password = "password" };
            var mediatorResponse = new PostRegisterResponse { StatusCode = (int)statusCode, UserFriendly = "Test message" };

            _mockMediator
                .Setup(m => m.Send(request, default))
                .ReturnsAsync(mediatorResponse);

            // Act
            var result = await _authController.PostRegister(request);

            // Assert
            var objectResult = Assert.IsAssignableFrom<ObjectResult>(result);
            Assert.Equal((int)statusCode, objectResult.StatusCode);
            Assert.Equal(mediatorResponse, objectResult.Value);
        }

        [Fact]
        public async Task PostRegister_MediatorReturnsOK_ReturnsOkObjectResult()
        {
            // Arrange
            var request = new PostRegisterRequest { UserEmail = "test@example.com", Password = "password" };
            var mediatorResponse = new PostRegisterResponse { StatusCode = (int)HttpStatusCode.OK, UserFriendly = "Success" };

            _mockMediator
                .Setup(m => m.Send(request, default))
                .ReturnsAsync(mediatorResponse);

            // Act
            var result = await _authController.PostRegister(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(mediatorResponse, okResult.Value);
        }

        [Fact]
        public async Task PostRegister_MediatorReturnsBadRequest_ReturnsBadRequestObjectResult()
        {
            // Arrange
            var request = new PostRegisterRequest { UserEmail = "test@example.com", Password = "password" };
            var mediatorResponse = new PostRegisterResponse { StatusCode = (int)HttpStatusCode.BadRequest, UserFriendly = "Bad request" };

            _mockMediator
                .Setup(m => m.Send(request, default))
                .ReturnsAsync(mediatorResponse);

            // Act
            var result = await _authController.PostRegister(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(mediatorResponse, badRequestResult.Value);
        }

        [Fact]
        public async Task PostRegister_MediatorReturnsUnauthorized_ReturnsUnauthorizedObjectResult()
        {
            // Arrange
            var request = new PostRegisterRequest { UserEmail = "test@example.com", Password = "password" };
            var mediatorResponse = new PostRegisterResponse { StatusCode = (int)HttpStatusCode.Unauthorized, UserFriendly = "Unauthorized" };

            _mockMediator
                .Setup(m => m.Send(request, default))
                .ReturnsAsync(mediatorResponse);

            // Act
            var result = await _authController.PostRegister(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(mediatorResponse, unauthorizedResult.Value);
        }

        [Fact]
        public async Task PostRegister_MediatorThrowsException_LogsErrorAndReturnsInternalServerError()
        {
            // Arrange
            var request = new PostRegisterRequest { UserEmail = "test@example.com", Password = "password" };
            var exception = new Exception("Test exception");

            _mockMediator
                .Setup(m => m.Send(request, default))
                .ThrowsAsync(exception);

            // Act
            var result = await _authController.PostRegister(request);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error), // Nivel de Log
                    It.IsAny<EventId>(), // EventId
                    It.Is<It.IsAnyType>((o, t) => o != null && o.ToString()!.Contains("Error en el registro/autenticación")), // Estado (mensaje)
                    exception, // Excepción
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()), // Formateador
                Times.Once);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
            var errorResponse = Assert.IsType<BaseResponse>(statusCodeResult.Value);
            Assert.Equal("Error interno del servidor", errorResponse.UserFriendly);
        }
    }
}
