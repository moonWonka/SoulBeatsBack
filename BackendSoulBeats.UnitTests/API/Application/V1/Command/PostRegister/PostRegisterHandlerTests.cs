using Xunit;
using Moq;
using BackendSoulBeats.API.Application.V1.Command.PostRegister;
using BackendSoulBeats.Domain.Application.V1.Services;
using System.Threading.Tasks;
using System.Threading;
using System; // For Exception

namespace BackendSoulBeats.UnitTests.API.Application.V1.Command.PostRegister
{
    public class PostRegisterHandlerTests
    {
        private readonly Mock<IGoogleAuthService> _mockGoogleAuthService;
        private readonly PostRegisterHandler _handler;

        public PostRegisterHandlerTests()
        {
            _mockGoogleAuthService = new Mock<IGoogleAuthService>();
            _handler = new PostRegisterHandler(_mockGoogleAuthService.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_CallsRegisterUserAsyncAndReturnsResponse()
        {
            // Arrange
            var request = new PostRegisterRequest { UserEmail = "test@example.com", Password = "password123" };
            var expectedUid = "firebase-user-uid";

            _mockGoogleAuthService
                .Setup(s => s.RegisterUserAsync(request.UserEmail, request.Password))
                .ReturnsAsync(expectedUid);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mockGoogleAuthService.Verify(s => s.RegisterUserAsync(request.UserEmail, request.Password), Times.Once);
            Assert.NotNull(response);
            Assert.Equal(expectedUid, response.MoreInformation);
            // Potentially check other default properties of PostRegisterResponse if they are set in the handler.
            // Based on current PostRegisterHandler, StatusCode and other BaseResponse properties are not set here,
            // they are set in the Controller based on this handler's outcome or other logic.
            // If PostRegisterResponse itself should have a default StatusCode, that logic would be in its constructor or here.
        }

        [Fact]
        public async Task Handle_GoogleAuthServiceThrowsException_ThrowsException()
        {
            // Arrange
            var request = new PostRegisterRequest { UserEmail = "test@example.com", Password = "password123" };
            var expectedException = new Exception("Firebase registration failed");

            _mockGoogleAuthService
                .Setup(s => s.RegisterUserAsync(request.UserEmail, request.Password))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var actualException = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal(expectedException.Message, actualException.Message);
            _mockGoogleAuthService.Verify(s => s.RegisterUserAsync(request.UserEmail, request.Password), Times.Once);
        }

        // Example of how PostRegisterResponse might be if it inherited from BaseResponse and set status codes
        // This is commented out as it's not how the current handler works.
        // [Fact]
        // public async Task Handle_WhenRegistrationSucceeds_ReturnsOkStatusCode()
        // {
        //     // Arrange
        //     var request = new PostRegisterRequest { UserEmail = "test@example.com", Password = "password123" };
        //     _mockGoogleAuthService
        //         .Setup(s => s.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>()))
        //         .ReturnsAsync("some-uid");

        //     // Act
        //     var response = await _handler.Handle(request, CancellationToken.None);

        //     // Assert
        //     // Assuming PostRegisterResponse would set its own StatusCode upon success
        //     // Assert.Equal((int)System.Net.HttpStatusCode.OK, response.StatusCode);
        // }
    }
}
