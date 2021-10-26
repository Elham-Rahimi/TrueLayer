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
        private PokemonService _pokemonService;
        private IConfiguration _configuration;

        public PokemonServiceTest()
        {
            _configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();
            _mockApiClient = new Mock<IApiClient>();
            _pokemonService = new PokemonService(_mockApiClient.Object, _configuration);
        }

        [Fact]
        public async Task GIVEN_Valid_Name_WHEN_Called_THEN_Return_Proper_Result()
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
        public async Task GIVEN_Invalid_Name_WHEN_Called_THEN_Throw_Exception(string pokemonName)
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
