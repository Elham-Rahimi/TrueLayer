using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Xunit;
using Pokedex.Models;

namespace Pokedex.Test.IntegrationTest
{
    public class GetPokemonTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public GetPokemonTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GIVEN_Valid_Pokemon_name_WHEN_Call_Api_THEN_Successful()
        {
            //Arrange
            var pokemonName = "pidgeot";
            var expected = new
            {
                Name = "pidgeot",
                Habitat = "forest",
                IsLegendary = false,
                Description = "When hunting, it\nskims the surface\nof water at high\fspeed to pick off\nunwary prey such\nas MAGIKARP."
            };
            var requestUrl = GenerateRequestUrl(pokemonName);

            //Act
            var response = await _client.GetAsync(requestUrl);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var pokemonJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PokemonResult>(pokemonJson);

            Assert.Equal(expected.Name, result.Name);
            Assert.Equal(expected.Habitat, result.Habitat);
            Assert.Equal(expected.Description, result.Description);
            Assert.Equal(expected.IsLegendary, result.IsLegendary);

        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GIVEN_null_or_empty_name_WHEN_Call_Api_THEN_NotFound(string pokemonName)
        {
            //Arrange           
            var requestUrl = GenerateRequestUrl(pokemonName);

            //Act
            var response = await _client.GetAsync(requestUrl);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Theory]
        [InlineData("noname")]
        public async Task GIVEN_invalid_name_WHEN_Call_Api_THEN_NotFound(string pokemonName)
        {
            //Arrange           
            var requestUrl = GenerateRequestUrl(pokemonName);

            //Act
            var response = await _client.GetAsync(requestUrl);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        private string GenerateRequestUrl(string pokemonName)
        {
            return $"/Pokemon/{pokemonName}";
        }
    }
}
