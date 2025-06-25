using Xunit;
using Moq;
using MediatR;
using BackendSoulBeats.API.Application.V1.Controllers;
using BackendSoulBeats.API.Application.V1.Query; // For GetUserInfoRequest and GetUserInfoResponse
using BackendSoulBeats.API.Application.V1.ViewModel.Common; // For BaseResponse
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BackendSoulBeats.UnitTests.API.Application.V1.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _userController = new UserController(_mockMediator.Object);

            // Mock HttpContext y User para pruebas que puedan necesitarlo
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                // Otros claims si son necesarios
            }, "mock"));

            _userController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task GetIUser_ValidId_SendsCorrectRequestToMediator()
        {
            // Arrange
            long userIdRouteParam = 123;
            var expectedRequest = new GetUserInfoRequest { UserId = userIdRouteParam.ToString() };
            var expectedResponse = new GetUserInfoResponse { StatusCode = (int)HttpStatusCode.OK, UserFriendly = "Success" };

            _mockMediator
                .Setup(m => m.Send(It.Is<GetUserInfoRequest>(req => req.UserId == expectedRequest.UserId), default))
                .ReturnsAsync(expectedResponse);

            // Act
            await _userController.GetIUser(userIdRouteParam);

            // Assert
            _mockMediator.Verify(m => m.Send(It.Is<GetUserInfoRequest>(req => req.UserId == expectedRequest.UserId), default), Times.Once);
        }

        [Theory]
        [InlineData(HttpStatusCode.OK)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public async Task GetIUser_MediatorReturnsResponse_ReturnsCorrectStatusCode(HttpStatusCode statusCode)
        {
            // Arrange
            long userIdRouteParam = 123;
            var request = new GetUserInfoRequest { UserId = userIdRouteParam.ToString() };
            var mediatorResponse = new GetUserInfoResponse { StatusCode = (int)statusCode, UserFriendly = "Test Message" };

            _mockMediator
                .Setup(m => m.Send(It.Is<GetUserInfoRequest>(req => req.UserId == request.UserId), default))
                .ReturnsAsync(mediatorResponse);

            // Act
            var result = await _userController.GetIUser(userIdRouteParam);

            // Assert
            var objectResult = Assert.IsAssignableFrom<ObjectResult>(result);
            Assert.Equal((int)statusCode, objectResult.StatusCode);
            Assert.Equal(mediatorResponse, objectResult.Value);
        }

        [Fact]
        public async Task GetIUser_MediatorReturnsOK_ReturnsOkObjectResult()
        {
            // Arrange
            long userIdRouteParam = 123;
            var request = new GetUserInfoRequest { UserId = userIdRouteParam.ToString() };
            var mediatorResponse = new GetUserInfoResponse { StatusCode = (int)HttpStatusCode.OK, UserFriendly = "Success" };

            _mockMediator
                .Setup(m => m.Send(It.Is<GetUserInfoRequest>(req => req.UserId == request.UserId), default))
                .ReturnsAsync(mediatorResponse);

            // Act
            var result = await _userController.GetIUser(userIdRouteParam);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(mediatorResponse, okResult.Value);
        }

        // Se podrían añadir pruebas similares para BadRequestObjectResult, UnauthorizedObjectResult, etc.
        // como en AuthControllerTests si se quiere ser exhaustivo con el tipo exacto de ObjectResult.

        // Nota: UserController no tiene un try-catch explícito para excepciones de MediatR.
        // Si MediatR lanza una excepción, ASP.NET Core la manejará (generalmente resultando en un 500).
        // Probar esto unitariamente requeriría que el controlador tuviera su propio manejo de excepciones
        // o depender del comportamiento del framework, lo cual es más una prueba de integración.
    }
}
