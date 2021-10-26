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

        [Fact]
        public void GIVEN_Valid_species_WHEN_Build_THEN_Return_Pokemon()
        {
            //Arrang
            var species = MockValisPokemonSpecies(false, false, false, false, false);
            var languageKey = "en";
            var expectedDescription = species.FlavorTextEntries
                        .FirstOrDefault(s => s.Language.Name.ToLower() == languageKey)?.FlavorText;

            //Act
            var pokemon = _pokemonBuilder.Init(species)
                .WithHabitat().WithName().WithIsLegendary()
                .WithDescription(languageKey)
                .Build();

            //Assert
            Assert.NotNull(pokemon);
            Assert.IsType<Pokemon>(pokemon);
            Assert.Equal(species.Name, pokemon.Name);
            Assert.Equal(species.IsLegendary, pokemon.IsLegendary);
            Assert.Equal(species.Habitat.Name, pokemon.Habitat);
            Assert.Equal(expectedDescription, pokemon.Description);
        }

        [Fact]
        public void GIVEN_null_name_WHEN_Build_THEN_throw_Exception()
        {
            //Arrang
            var species = MockValisPokemonSpecies(isNameNull: true,false,false,false,false);
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

        [Fact]
        public void GIVEN_null_description_WHEN_Build_THEN_throw_Exception()
        {
            //Arrang
            var species = MockValisPokemonSpecies(false, false, false, isDescriptionNull: true, false);
            var languageKey = "en";
            var expectedDescription = species.FlavorTextEntries?
                        .FirstOrDefault(s => s.Language.Name.ToLower() == languageKey)?.FlavorText;

            //Act
            //Assert
            Assert.Throws<PokemonNullDescriptionException>(
                () => _pokemonBuilder.Init(species)
                .WithHabitat().WithName().WithIsLegendary()
                .WithDescription(languageKey)
                .Build());
        }

        [Fact]
        public void GIVEN_no_en_description_WHEN_Build_THEN_throw_Exception()
        {
            //Arrang
            var species = MockValisPokemonSpecies(false, false, false, false, isEnDescriptionNull: true);
            var languageKey = "en";
            var expectedDescription = species.FlavorTextEntries?
                        .FirstOrDefault(s => s.Language?.Name?.ToLower() == languageKey)?.FlavorText;

            //Act
            //Assert
            Assert.Throws<PokemonNullDescriptionException>(
                () => _pokemonBuilder.Init(species)
                .WithHabitat().WithName().WithIsLegendary()
                .WithDescription(languageKey)
                .Build());
        }

        [Fact]
        public void GIVEN_null_habitat_WHEN_Build_THEN_return_pokemon()
        {
            //Arrang
            var species = MockValisPokemonSpecies(false, isHabitatNull: true, false, false, false);
            var languageKey = "en";
            var expectedDescription = species.FlavorTextEntries
                        .FirstOrDefault(s => s.Language.Name.ToLower() == languageKey)?.FlavorText;

            //Act
            var pokemon = _pokemonBuilder.Init(species)
                .WithHabitat().WithName().WithIsLegendary()
                .WithDescription(languageKey)
                .Build();

            //Assert
            Assert.NotNull(pokemon);
            Assert.IsType<Pokemon>(pokemon);
            Assert.Equal(species.Name, pokemon.Name);
            Assert.Equal(species.IsLegendary, pokemon.IsLegendary);
            Assert.Null(pokemon.Habitat);
            Assert.Equal(expectedDescription, pokemon.Description);
        }

        [Fact]
        public void GIVEN_null_legendary_WHEN_Build_THEN_return_pokemon()
        {
            //Arrang
            var species = MockValisPokemonSpecies(false, false, isLegendaryNull: true, false, false);
            var languageKey = "en";
            var expectedDescription = species.FlavorTextEntries
                        .FirstOrDefault(s => s.Language.Name.ToLower() == languageKey)?.FlavorText;

            //Act
            var pokemon = _pokemonBuilder.Init(species)
                .WithHabitat().WithName().WithIsLegendary()
                .WithDescription(languageKey)
                .Build();

            //Assert
            Assert.NotNull(pokemon);
            Assert.IsType<Pokemon>(pokemon);
            Assert.Equal(species.Name, pokemon.Name);
            Assert.Equal(species.Habitat.Name, pokemon.Habitat);
            Assert.Null(pokemon.IsLegendary);
            Assert.Equal(expectedDescription, pokemon.Description);
        }

        private PokemonSpecies MockValisPokemonSpecies(
            bool isNameNull,
            bool isHabitatNull,
            bool isLegendaryNull,
            bool isDescriptionNull,
            bool isEnDescriptionNull)
        {
            return new PokemonSpecies()
            {
                Name = isNameNull ? null : "mewtwo",
                Habitat = new Habitat()
                {
                    Name = isHabitatNull ? null : "rare",
                    Url = "https://pokeapi.co/api/v2/pokemon-habitat/5/"
                },
                IsLegendary = isLegendaryNull ? null : true,
                FlavorTextEntries = isDescriptionNull 
                ? null 
                :new List<FlavorTextEntry>()
                {
                    new FlavorTextEntry()
                    {
                        FlavorText = "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments.",
                        Language = new Language()
                        {
                            Name =isEnDescriptionNull? null: "en",
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
