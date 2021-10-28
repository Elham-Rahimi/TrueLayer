using Microsoft.Extensions.Configuration;
using Moq;
using Pokedex.Services.ApiClient;
using Pokedex.Services.PokemonBuilder;
using Pokedex.Services.PokemonBuilder.Contracts;
using Pokedex.Services.PokemonBuilder.Exceptions;
using Pokedex.Services.PokemonService.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Pokedex.Test.Services
{
    public class PokemonBuilderTest
    {
        private Mock<IApiClient> _mockApiClient;
        private IConfiguration _configuration;
        private PokemonBuilder _pokemonBuilder;

        public PokemonBuilderTest()
        {
            _configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();
            _mockApiClient = new Mock<IApiClient>();
            _pokemonBuilder = new PokemonBuilder(_mockApiClient.Object, _configuration);
        }

        [Theory]
        [InlineData("mewtwo", "rare", true, "description")]
        [InlineData("mewtwo", null, true, "description")]
        [InlineData("mewtwo", "rare", null, "description")]
        [InlineData("mewtwo", "rare", true, null)]
        public async Task GIVEN_Valid_Input_WHEN_Build_THEN_Return_Proper_Pokemon(
            string name,
            string habitat,
            bool? isLegendary,
            string description)
        {
            //Arrang
            var species = MockPokemonSpecies(name , habitat, isLegendary, description);
            var pokeBaseUrl = _configuration["Pokedex:PokeBaseUrl"].Replace("{NAME}", species.Name);

            _mockApiClient
               .Setup(x => x.GetAsync<PokemonSpecies>(pokeBaseUrl))
               .ReturnsAsync(species);

            //Act
            var pokemon = (await _pokemonBuilder.Init(name))
                .WithHabitat().WithName().WithIsLegendary()
                .WithDescription()
                .Build();

            //Assert
            Assert.NotNull(pokemon);
            Assert.IsType<Pokemon>(pokemon);
            Assert.Equal(name, pokemon.Name);
            Assert.Equal(isLegendary, pokemon.IsLegendary);
            Assert.Equal(habitat, pokemon.Habitat);
            Assert.Equal(description, pokemon.Description);
        }

        [Theory]
        [InlineData("mewtwo", "cave", true, "description")]
        [InlineData("mewtwo", "rare", true, "description")]
        [InlineData("mewtwo", "cave", false, "description")]
        public async Task GIVEN_Input_Satisfy_IsYoda_WHEN_BuildTranslatedAsync_THEN_Return_Proper_Pokemon(
            string name,
            string habitat,
            bool? isLegendary,
            string description)
        {
            //Arrang
            var species = MockPokemonSpecies(name, habitat, isLegendary, description);
            var pokeBaseUrl = _configuration["Pokedex:PokeBaseUrl"].Replace("{NAME}", species.Name);
            var yodaURl = _configuration["Pokedex:YodaUrl"].Replace("{DESCRIPTION}", "desc");
            var expectedTranslationResponse = new TranslationResponse()
            {
                Contents = new Contents()
                {
                    Text = description,
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
            var pokemon = await (await _pokemonBuilder.Init(name))
                .WithHabitat().WithName().WithIsLegendary()
                .WithDescription()
                .BuildTranslatedAsync();

            //Assert
            Assert.NotNull(pokemon);
            Assert.IsType<Pokemon>(pokemon);
            Assert.Equal(name, pokemon.Name);
            Assert.Equal(isLegendary, pokemon.IsLegendary);
            Assert.Equal(habitat, pokemon.Habitat);
            Assert.Equal(expectedTranslationResponse.Contents.Translated, pokemon.Description);
        }

        [Theory]
        [InlineData("mewtwo", "rare", false, "description")]
        [InlineData("mewtwo", "forset", false, "description")]
        [InlineData("mewtwo", null, false, "description")]
        [InlineData("mewtwo", "forset", null, "description")]
        [InlineData("mewtwo", null, null, "description")]
        public async Task GIVEN_Input_Not_Satisfy_IsYoda_WHEN_BuildTranslatedAsync_THEN_Return_Proper_Pokemon(
            string name,
            string? habitat,
            bool? isLegendary,
            string description)
        {
            //Arrang
            var species = MockPokemonSpecies(name, habitat, isLegendary, description);
            var pokeBaseUrl = _configuration["Pokedex:PokeBaseUrl"].Replace("{NAME}", species.Name);
            var ShakespearURl = _configuration["Pokedex:ShakespeareUrl"].Replace("{DESCRIPTION}", "desc"); ;
            var expectedTranslationResponse = new TranslationResponse()
            {
                Contents = new Contents()
                {

                    Text = description,
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
            var pokemon = await (await _pokemonBuilder.Init(name))
                .WithHabitat().WithName().WithIsLegendary()
                .WithDescription()
                .BuildTranslatedAsync();

            //Assert
            Assert.NotNull(pokemon);
            Assert.IsType<Pokemon>(pokemon);
            Assert.Equal(name, pokemon.Name);
            Assert.Equal(isLegendary, pokemon.IsLegendary);
            Assert.Equal(habitat, pokemon.Habitat);
            Assert.Equal(expectedTranslationResponse.Contents.Translated, pokemon.Description);
        }

        [Fact]
        public async Task GIVEN_Null_Name_WHEN_Build_THEN_throw_Exception()
        {
            //Arrang

            //Act
            //Assert
            await Assert.ThrowsAsync<PokemonNullNameException>(
                 async () => (await _pokemonBuilder.Init(null))
                .WithHabitat().WithName().WithIsLegendary()
                .WithDescription()
                .Build());
        }

        [Fact]
        public async Task GIVEN_Null_Name_WHEN_BuildTranslatedAsync_THEN_throw_Exception()
        {
            //Arrang

            //Act
            //Assert
            await Assert.ThrowsAsync<PokemonNullNameException>(
                 async () => await (await _pokemonBuilder.Init(null))
                .WithHabitat().WithName().WithIsLegendary()
                .WithDescription()
                .BuildTranslatedAsync());
        }

        private PokemonSpecies MockPokemonSpecies(
            string name,
            string? habitat,
            bool? isLegendary,
            string description)
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
