using Pokedex.Services.PokemonBuilder.Exceptions;
using Pokedex.Services.PokemonService.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pokedex.Services.PokemonBuilder
{
    public class PokemonBuilder : IPokemonBuilder
    {
        private Pokemon _pokemon;
        private PokemonSpecies _species;

        public Pokemon Build()
        {
            return _pokemon;
        }

        public PokemonBuilder Init(PokemonSpecies species)
        {
            _pokemon = new Pokemon();
            _species = species;
            return this;
        }

        public PokemonBuilder WithName()
        {
            if(string.IsNullOrEmpty(_species.Name))
            {
                throw new PokemonNullNameException();
            }
            _pokemon.Name = _species.Name;
            return this;
        }

        public PokemonBuilder WithHabitat()
        {
            _pokemon.Habitat = _species?.Habitat?.Name;
            return this;
        }

        public PokemonBuilder WithIsLegendary()
        {
            _pokemon.IsLegendary = _species?.IsLegendary;
            return this;
        }

        public PokemonBuilder WithDescription(string languageKey)
        {
            var description = _species.FlavorTextEntries?
                .FirstOrDefault(e => e.Language?.Name?.ToLowerInvariant() == languageKey)
                ?.FlavorText;
            description = CleanText(description);
            _pokemon.Description = description;
            return this;
        }

        private string CleanText(string text)
        {
            return text?.Replace("\n", " ")?.Replace("\f", " ");   
        }

    }
}
