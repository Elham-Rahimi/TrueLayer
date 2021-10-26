using Pokedex.Services.PokemonBuilder;
using Pokedex.Services.PokemonBuilder.Exceptions;
using Pokedex.Services.PokemonService.Contracts;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Pokedex.Test.Services
{
    public class PokemonBuilderTest
    {
        private PokemonBuilder _pokemonBuilder;

        public PokemonBuilderTest()
        {
            _pokemonBuilder = new PokemonBuilder();
        }

        [Theory]
        [InlineData("mewtwo", "rare", true, "description")]
        [InlineData("mewtwo", null, true, "description")]
        [InlineData("mewtwo", "rare", null, "description")]
        [InlineData("mewtwo", "rare", true, null)]
        public void GIVEN_Valid_Input_WHEN_Build_THEN_Return_Proper_Pokemon(
            string name,
            string habitat,
            bool? isLegendary,
            string description)
        {
            //Arrang
            var species = MockPokemonSpecies(name, habitat, isLegendary, description);
            var languageKey = "en";

            //Act
            var pokemon = _pokemonBuilder.Init(species)
                .WithHabitat().WithName().WithIsLegendary()
                .WithDescription(languageKey)
                .Build();

            //Assert
            Assert.NotNull(pokemon);
            Assert.IsType<Pokemon>(pokemon);
            Assert.Equal(name, pokemon.Name);
            Assert.Equal(isLegendary, pokemon.IsLegendary);
            Assert.Equal(habitat, pokemon.Habitat);
            Assert.Equal(description, pokemon.Description);
        }

        [Fact]
        public void GIVEN_Null_Name_WHEN_Build_THEN_throw_Exception()
        {
            //Arrang
            var species = MockPokemonSpecies(null, "rare", true, "description");
            var languageKey = "en";
            var expectedDescription = species.FlavorTextEntries?
                        .FirstOrDefault(s => s.Language?.Name?.ToLower() == languageKey)?.FlavorText;

            //Act
            //Assert
            Assert.Throws<PokemonNullNameException>(
                () => _pokemonBuilder.Init(species)
                .WithHabitat().WithName().WithIsLegendary()
                .WithDescription(languageKey)
                .Build());
        }

        private PokemonSpecies MockPokemonSpecies(
            string name,
            string habitat,
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
