// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Net;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc.Testing;
// using Xunit;

// namespace BackendSoulBeats.IntegrationTests.Application.Test
// {
//     public class SampleIntegrationTest : IClassFixture<WebApplicationFactory<BackendSoulBeats.API.Program>>
//     {
//         private readonly WebApplicationFactory<BackendSoulBeats.API.Program> _factory;

//         public SampleIntegrationTest(WebApplicationFactory<BackendSoulBeats.API.Program> factory)
//         {
//             _factory = factory;
//         }

//         [Fact]
//         public async Task Get_Endpoint_ReturnsSuccess()
//         {
//             var client = _factory.CreateClient();
//             var response = await client.GetAsync("/swagger/index.html");
//             Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//         }
//     }
// }