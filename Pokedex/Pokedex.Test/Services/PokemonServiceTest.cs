using Microsoft.Extensions.Configuration;
using Moq;
using System.Linq;
using Pokedex.Exceptions;
using Pokedex.Services.ApiClient;
using Pokedex.Services.PokemonService;
using Pokedex.Services.PokemonService.Contracts;
using Pokedex.Services.PokemonService.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Pokedex.Services.PokemonBuilder;

namespace Pokedex.Test.Services
{
    public class PokemonServiceTest
    {
        private Mock<IApiClient> _mockApiClient;
        private Mock<IConfiguration> _mockConfiguration;
        private PokemonService _pokemonService;

        public PokemonServiceTest()
        {
            _mockApiClient = new Mock<IApiClient>();
            _mockConfiguration = new Mock<IConfiguration>();
            _pokemonService = new PokemonService(_mockApiClient.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task GIVEN_Valid_name_WHEN_Called_THEN_Return_Result()
        {
            //Arrange
            var url = "http://somthing.com/";
            var pokemonSpecies = MockPokemonSpecies();
            var expectedDescription = pokemonSpecies.FlavorTextEntries
                        .FirstOrDefault(s => s.Language.Name.ToLower() == "en")?.FlavorText;

            _mockConfiguration
                .Setup(x => x[It.Is<string>(s => s == "Pokedex:PokeBaseUrl")])
                .Returns(url);

            _mockConfiguration
                .Setup(x => x[It.Is<string>(s => s == "Pokedex:DescriptionLanguage")])
                .Returns("en");

            _mockApiClient
                .Setup(x => x.GetAsync<PokemonSpecies>(It.IsAny<string>()))
                .ReturnsAsync(pokemonSpecies);

            //Act
            var response = await _pokemonService.GetAsync(pokemonSpecies.Name);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(typeof(Pokemon), response.GetType());
            Assert.Equal(pokemonSpecies.Name, response.Name);
            Assert.Equal(pokemonSpecies.IsLegendary, response.IsLegendary);
            Assert.Equal(pokemonSpecies.Habitat.Name, response.Habitat);
            Assert.Equal(expectedDescription, response.Description);
        }

        [Fact]
        public async Task GIVEN_null_response_WHEN_Called_THEN_Throw_Exception()
        {
            //Arrange
            var url = "http://somthing.com/";
            var pokemonName = "mewtwo";

            _mockConfiguration
                .Setup(x => x[It.Is<string>(s => s == "Pokedex:PokeBaseUrl")])
                .Returns(url);

            _mockApiClient
                .Setup(x => x.GetAsync<PokemonSpecies>(It.IsAny<string>()))
                .ReturnsAsync((PokemonSpecies)null);

            //Act

            //Assert
            await Assert.ThrowsAsync<NullResponseException>(()
                => _pokemonService.GetAsync(pokemonName));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GIVEN_Invalid_name_WHEN_Called_THEN_Throw_Exception(string pokemonName)
        {
            //Arrange
            //Act
            //Assert
            await Assert.ThrowsAsync<EntryNameNullException>(()
                => _pokemonService.GetAsync(pokemonName));
        }

        private PokemonSpecies MockPokemonSpecies()
        {
            return new PokemonSpecies()
            {
                Name = "mewtwo",
                Habitat = new Habitat()
                {
                    Name = "rare",
                    Url = "https://pokeapi.co/api/v2/pokemon-habitat/5/"
                },
                IsLegendary = true,
                FlavorTextEntries = new List<FlavorTextEntry>()
                {
                    new FlavorTextEntry()
                    {
                        FlavorText = "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments.",
                        Language = new Language()
                        {
                            Name = "en",
                            Url = "https://pokeapi.co/api/v2/language/9/"
                        },
                        Version= new Version()
                        {
                            Name = "red",
                            Url="https://pokeapi.co/api/v2/version/1/"
                        }
                    },
                    new FlavorTextEntry()
                    {
                        FlavorText = "Il est le fruit de nombreuses expériences génétiques\nhorribles et malsaines.",
                        Language = new Language()
                        {
                            Name = "fr",
                            Url = "https://pokeapi.co/api/v2/language/5/"
                        },
                        Version= new Version()
                        {
                            Name = "x",
                            Url="https://pokeapi.co/api/v2/version/23/"
                        }
                    }
                }
            };
        }

    }
}
