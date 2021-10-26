using Moq;
using Pokedex.Exceptions;
using Pokedex.Services.ApiClient;
using Pokedex.Services.ApiClient.Exceptions;
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
        public async Task GIVEN_Success_Response_On_Client_Url_WHEN_Called_THEN_Return_Proper_Result()
        {
            //Arrange
            var url = "url";
            _mockRestClient
                .Setup(x => x.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse
                {
                    Content = jsonResult() ,
                    StatusCode = HttpStatusCode.OK,
                });

            //Act
            var response = await _apiClient.GetAsync<TestModel>(url);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(typeof(TestModel), response.GetType());
        }

        [Fact]
        public async Task GIVEN_Null_Response_WHEN_Called_THEN_Throw_Exception()
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
        public async Task GIVEN_Content_Null_Response_WHEN_Called_THEN_Throw_Exception()
        {
            //Arrange
            _mockRestClient
                .Setup(x => x.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse
                {
                    Content = null,
                    StatusCode = HttpStatusCode.OK,
                });

            //Act
            //Assert
            await Assert.ThrowsAsync<NullResponseException>(()
                => _apiClient.GetAsync<TestModel>("url"));
        }

        [Fact]
        public async Task GIVEN_NotOk_WHEN_Called_THEN_Throw_Exception()
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
