using Moq;
using Pokedex.Exceptions;
using Pokedex.Services.ApiClient;
using RestSharp;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Pokedex.Test.Services
{
    public class ApiClientTest
    {
        private Mock<IRestClient> _mockRestClient;
        private ApiClient _apiClient;

        public ApiClientTest()
        {
            _mockRestClient = new Mock<IRestClient>();
            _apiClient = new ApiClient(_mockRestClient.Object);
        }

        [Fact]
        public async Task GIVEN_Available_url_WHEN_Called_THEN_Return_Result()
        {
            //Arrange
            _mockRestClient
                .Setup(x => x.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse
                {
                    Content = jsonResult() ,
                    StatusCode = HttpStatusCode.OK,
                });

            //Act
            var response = await _apiClient.GetAsync<TestModel>("url");

            //Assert
            Assert.NotNull(response);
            Assert.Equal(typeof(TestModel), response.GetType());
        }

        [Fact]
        public async Task GIVEN_null_response_WHEN_Called_THEN_throw_exception()
        {
            //Arrange
            _mockRestClient
                .Setup(x => x.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RestResponse)null);

            //Act
            //Assert
            await Assert.ThrowsAsync<NullResponseException>(()
                => _apiClient.GetAsync<TestModel>("url"));
        }

        [Fact]
        public async Task GIVEN_notfound_WHEN_Called_THEN_throw_exception()
        {
            //Arrange
            _mockRestClient
                .Setup(x => x.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse
                {
                    StatusCode = HttpStatusCode.NotFound,
                });

            //Act
            //Assert
            await Assert.ThrowsAsync<ApiClientNotFoundException>(()
                => _apiClient.GetAsync<TestModel>("url"));
        }

        [Fact]
        public async Task GIVEN_InternalServerError_WHEN_Called_THEN_throw_exception()
        {
            //Arrange
            _mockRestClient
                .Setup(x => x.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                });

            //Act
            //Assert
            await Assert.ThrowsAsync<ApiClientNotFoundException>(()
                => _apiClient.GetAsync<TestModel>("url"));
        }

        private string jsonResult()
        {
            return @"{
	            'testProperty': 'xxx',
                'date': '2021-04-07'
                }";
        }
    }

    public class TestModel
    {
        public string TestProperty { get; set; }
    }
}
