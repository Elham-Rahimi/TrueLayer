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
using Pokedex.Services.ApiClient.Exceptions;
using Pokedex.Services.PokemonBuilder.Exceptions;
using Pokedex.Services.PokemonBuilder.Contracts;

namespace Pokedex.Test.Services
{
    public class PokemonServiceTest
    {
        private Mock<IApiClient> _mockApiClient;
        private IPokemonBuilder _pokemonBuilder;
        private PokemonService _pokemonService;
        private IConfiguration _configuration;

        public PokemonServiceTest()
        {
            _configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();
            _mockApiClient = new Mock<IApiClient>();
            _pokemonBuilder = new PokemonBuilder(_mockApiClient.Object, _configuration);
            _pokemonService = new PokemonService(_pokemonBuilder);
        }

        [Fact]
        public async Task GIVEN_Valid_Name_WHEN_GetAsync_Called_THEN_Return_Proper_Result()
        {
            //Arrange
            var species = MockPokemonSpecies();
            var url = _configuration["Pokedex:PokeBaseUrl"].Replace("{NAME}", species.Name);
            var expectedDescription = species.FlavorTextEntries
                        .FirstOrDefault(s => s.Language.Name.ToLower() == "en")?.FlavorText;

            _mockApiClient
                .Setup(x => x.GetAsync<PokemonSpecies>(url))
                .ReturnsAsync(species);

            //Act
            var result = await _pokemonService.GetAsync(species.Name);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(typeof(Pokemon), result.GetType());
            Assert.Equal(species.Name, result.Name);
            Assert.Equal(species.IsLegendary, result.IsLegendary);
            Assert.Equal(species.Habitat.Name, result.Habitat);
            Assert.Equal(expectedDescription, result.Description);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GIVEN_Invalid_Name_WHEN_GetAsync_Called_THEN_Throw_Exception(string pokemonName)
        {
            //Arrange
            //Act
            //Assert
            await Assert.ThrowsAsync<PokemonNullNameException>(()
                => _pokemonService.GetAsync(pokemonName));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GIVEN_Invalid_Name_WHEN_GetWithTranslationAsync_Called_THEN_Throw_Exception(string pokemonName)
        {
            //Arrange
            //Act
            //Assert
            await Assert.ThrowsAsync<PokemonNullNameException>(()
                => _pokemonService.GetWithTranslationAsync(pokemonName));
        }

        [Fact]
        public async Task GIVEN_Valid_Name_For_NoneLegendary_With_Successful_Translation_WHEN_GetWithTranslationAsync_Called_THEN_Return_Proper_Result()
        {
            //Arrange
            var species = MockPokemonSpecies(description:"desc");
            var pokeBaseUrl = _configuration["Pokedex:PokeBaseUrl"].Replace("{NAME}", species.Name);
            var ShakespearURl = _configuration["Pokedex:ShakespeareUrl"].Replace("{DESCRIPTION}", "desc"); ;
            var expectedDescription = "Shakespeare_translated";
            var expectedTranslationResponse = new TranslationResponse()
            {
                Contents = new Contents()
                {
                    Text = "desc",
                    Translated = "Shakespeare_translated",
                    Translation = "Shakespeare"
                },
                Success = new Success()
                {
                    Total = 1
                }

            };

            _mockApiClient
                .Setup(x => x.GetAsync<PokemonSpecies>(pokeBaseUrl))
                .ReturnsAsync(species);

            _mockApiClient
                .Setup(x => x.GetAsync<TranslationResponse>(ShakespearURl))
                .ReturnsAsync(expectedTranslationResponse);

            //Act
            var result = await _pokemonService.GetWithTranslationAsync(species.Name);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(typeof(Pokemon), result.GetType());
            Assert.Equal(species.Name, result.Name);
            Assert.Equal(species.IsLegendary, result.IsLegendary);
            Assert.Equal(species.Habitat.Name, result.Habitat);
            Assert.Equal(expectedTranslationResponse.Contents.Translated, result.Description);
        }

        [Fact]
        public async Task GIVEN_Valid_Name_For_Cave_Legendary_With_Successful_Translation_WHEN_GetWithTranslationAsync_Called_THEN_Return_Proper_Result()
        {
            //Arrange
            var species = MockPokemonSpecies(isLegendary:true,habitat:"cave",description:"desc");
            var pokeBaseUrl = _configuration["Pokedex:PokeBaseUrl"].Replace("{NAME}", species.Name);
            var yodaURl = _configuration["Pokedex:YodaUrl"].Replace("{DESCRIPTION}", "desc"); ;
            var expectedTranslationResponse = new TranslationResponse()
            {
                Contents = new Contents()
                {
                    Text = "desc",
                    Translated = "Yoda_translated",
                    Translation = "Yoda"
                },
                Success = new Success()
                {
                    Total = 1
                }

            };

            _mockApiClient
                .Setup(x => x.GetAsync<PokemonSpecies>(pokeBaseUrl))
                .ReturnsAsync(species);

            _mockApiClient
                .Setup(x => x.GetAsync<TranslationResponse>(yodaURl))
                .ReturnsAsync(expectedTranslationResponse);

            //Act
            var result = await _pokemonService.GetWithTranslationAsync(species.Name);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(typeof(Pokemon), result.GetType());
            Assert.Equal(species.Name, result.Name);
            Assert.Equal(species.IsLegendary, result.IsLegendary);
            Assert.Equal(species.Habitat.Name, result.Habitat);
            Assert.Equal(expectedTranslationResponse.Contents.Translated, result.Description);
        }

        [Fact]
        public async Task GIVEN_Valid_Name_For_Just_Legendary_With_Successful_Translation_WHEN_GetWithTranslationAsync_Called_THEN_Return_Proper_Result()
        {
            //Arrange
            var species = MockPokemonSpecies(isLegendary: true,description:"desc");
            var pokeBaseUrl = _configuration["Pokedex:PokeBaseUrl"].Replace("{NAME}", species.Name);
            var yodaURl = _configuration["Pokedex:YodaUrl"].Replace("{DESCRIPTION}", "desc"); ;
            var expectedTranslationResponse = new TranslationResponse()
            {
                Contents = new Contents()
                {
                    Text = "desc",
                    Translated = "Yoda_translated",
                    Translation = "Yoda"
                },
                Success = new Success()
                {
                    Total = 1
                }

            };

            _mockApiClient
                .Setup(x => x.GetAsync<PokemonSpecies>(pokeBaseUrl))
                .ReturnsAsync(species);

            _mockApiClient
                .Setup(x => x.GetAsync<TranslationResponse>(yodaURl))
                .ReturnsAsync(expectedTranslationResponse);

            //Act
            var result = await _pokemonService.GetWithTranslationAsync(species.Name);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(typeof(Pokemon), result.GetType());
            Assert.Equal(species.Name, result.Name);
            Assert.Equal(species.IsLegendary, result.IsLegendary);
            Assert.Equal(species.Habitat.Name, result.Habitat);
            Assert.Equal(expectedTranslationResponse.Contents.Translated, result.Description);
        }

        [Fact]
        public async Task GIVEN_Valid_Name_For_Just_Cave_With_Successful_Translation_WHEN_GetWithTranslationAsync_Called_THEN_Return_Proper_Result()
        {
            //Arrange
            var species = MockPokemonSpecies(habitat: "cave",description:"desc");
            var pokeBaseUrl = _configuration["Pokedex:PokeBaseUrl"].Replace("{NAME}", species.Name);
            var yodaURl = _configuration["Pokedex:YodaUrl"].Replace("{DESCRIPTION}", "desc"); ;
            var expectedTranslationResponse = new TranslationResponse()
            {
                Contents = new Contents()
                {
                    Text = "desc",
                    Translated = "Yoda_translated",
                    Translation = "Yoda"
                },
                Success = new Success()
                {
                    Total = 1
                }

            };

            _mockApiClient
                .Setup(x => x.GetAsync<PokemonSpecies>(pokeBaseUrl))
                .ReturnsAsync(species);

            _mockApiClient
                .Setup(x => x.GetAsync<TranslationResponse>(yodaURl))
                .ReturnsAsync(expectedTranslationResponse);

            //Act
            var result = await _pokemonService.GetWithTranslationAsync(species.Name);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(typeof(Pokemon), result.GetType());
            Assert.Equal(species.Name, result.Name);
            Assert.Equal(species.IsLegendary, result.IsLegendary);
            Assert.Equal(species.Habitat.Name, result.Habitat);
            Assert.Equal(expectedTranslationResponse.Contents.Translated, result.Description);
        }

        [Fact]
        public async Task GIVEN_Valid_Name_With_Failure_Translation_WHEN_GetWithTranslationAsync_Called_THEN_Return_Proper_Result()
        {
            //Arrange
            var species = MockPokemonSpecies(description:"desc");
            var pokeBaseUrl = _configuration["Pokedex:PokeBaseUrl"].Replace("{NAME}", species.Name);

            _mockApiClient
                .Setup(x => x.GetAsync<PokemonSpecies>(pokeBaseUrl))
                .ReturnsAsync(species);

            _mockApiClient
                .Setup(x => x.GetAsync<string>(It.IsAny<string>()))
                .Throws<ApiClientNotFoundException>();

            //Act
            var result = await _pokemonService.GetWithTranslationAsync(species.Name);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(typeof(Pokemon), result.GetType());
            Assert.Equal(species.Name, result.Name);
            Assert.Equal(species.IsLegendary, result.IsLegendary);
            Assert.Equal(species.Habitat.Name, result.Habitat);
            Assert.Equal("desc", result.Description);
        }
        private PokemonSpecies MockPokemonSpecies(
             string name = "mewtwo",
             string habitat = "forest",
             bool? isLegendary = false,
             string description = "desc")
        {
            return new PokemonSpecies()
            {
                Name = name,
                Habitat = new Habitat()
                {
                    Name = habitat,
                    Url = "https://pokeapi.co/api/v2/pokemon-habitat/5/"
                },
                IsLegendary = isLegendary,
                FlavorTextEntries = new List<FlavorTextEntry>()
                {
                    new FlavorTextEntry()
                    {
                        FlavorText = description,
                        Language = new Language()
                        {
                            Name ="en",
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
