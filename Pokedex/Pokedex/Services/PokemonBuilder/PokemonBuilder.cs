using Microsoft.Extensions.Configuration;
using Pokedex.Services.ApiClient;
using Pokedex.Services.PokemonBuilder.Contracts;
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
        private readonly IApiClient _apiClient;
        private IConfiguration _configuration;
        private Pokemon _pokemon;
        private PokemonSpecies _species;

        public PokemonBuilder()
        {

        }
        public PokemonBuilder(IApiClient apiClient, IConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
        }
        public Pokemon Build()
        {
            return _pokemon;
        }

        public async Task<PokemonBuilder> Init(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new PokemonNullNameException();
            }
            _species = await _apiClient.GetAsync<PokemonSpecies>(GetPokeBaseUrl(name));
            _pokemon = new Pokemon();
            return this;
        }

        public PokemonBuilder WithName()
        {
            _pokemon.Name = _species.Name;
            return this;
        }

        public async Task<Pokemon> BuildTranslatedAsync()
        {
            try
            {
                if (_pokemon.IsYoda())
                {
                    await WithYodaDescription();
                }
                else
                {
                    await WithShakespeareDescription();
                }
            }
            catch { }

            return _pokemon;
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

        public PokemonBuilder WithDescription()
        {
            var languageKey = _configuration["Pokedex:DescriptionLanguage"];
            var description = _species.FlavorTextEntries?
                .FirstOrDefault(e => e.Language?.Name?.ToLowerInvariant() == languageKey)
                ?.FlavorText;
            description = CleanText(description);
            _pokemon.Description = description;
            return this;
        }

        private async Task WithYodaDescription()
        {
            var response = await _apiClient.GetAsync<TranslationResponse>(GetYodaBaseUrl(_pokemon.Description));
            _pokemon.Description = response.Contents.Translated;
        }

        private async Task WithShakespeareDescription()
        {
            var response = await _apiClient.GetAsync<TranslationResponse>(GetShakespeareBaseUrl(_pokemon.Description));
            _pokemon.Description = response.Contents.Translated;
        }

        private string CleanText(string text)
        {
            return text?.Replace("\n", " ")?.Replace("\f", " ");
        }

        private string GetYodaBaseUrl(string description)
        {
            return _configuration["Pokedex:YodaUrl"].Replace("{DESCRIPTION}", description);
        }

        private string GetShakespeareBaseUrl(string description)
        {
            return _configuration["Pokedex:ShakespeareUrl"].Replace("{DESCRIPTION}", description);
        }

        private string GetPokeBaseUrl(string name)
        {
            return _configuration["Pokedex:PokeBaseUrl"].Replace("{NAME}", name);
        }

    }
}
