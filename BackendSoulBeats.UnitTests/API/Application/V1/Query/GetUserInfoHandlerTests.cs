using Xunit;
using BackendSoulBeats.API.Application.V1.Query;
using System.Threading.Tasks;
using System.Threading;

namespace BackendSoulBeats.UnitTests.API.Application.V1.Query
{
    public class GetUserInfoHandlerTests
    {
        private readonly GetUserInfoHandler _handler;

        public GetUserInfoHandlerTests()
        {
            // No dependencies to mock for now
            _handler = new GetUserInfoHandler();
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsResponseWithCorrectIdAndMessage()
        {
            // Arrange
            var request = new GetUserInfoRequest { UserId = "test-user-123" };

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(request.UserId, response.Id);
            Assert.Equal("User information retrieved successfully.", response.MoreInformation);
            // Similar to PostRegisterHandler, StatusCode and other BaseResponse properties
            // are not set by this handler. They are typically set by the controller.
            // If GetUserInfoResponse had default status codes, they would be tested here.
        }

        [Fact]
        public async Task Handle_DifferentUserId_ReturnsResponseWithThatUserId()
        {
            // Arrange
            var request = new GetUserInfoRequest { UserId = "another-user-456" };

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(request.UserId, response.Id);
        }
    }
}
