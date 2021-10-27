using Microsoft.Extensions.Configuration;
using Pokedex.Exceptions;
using Pokedex.Services.ApiClient;
using Pokedex.Services.PokemonBuilder;
using Pokedex.Services.PokemonService.Contracts;
using Pokedex.Services.PokemonService.Exceptions;
using System.Linq;
using System.Threading.Tasks;

namespace Pokedex.Services.PokemonService
{
    public class PokemonService : IPokemonService
    {
        private readonly IApiClient _apiClient;
        private IConfiguration _configuration;
        private IPokemonBuilder _pokemonBuilder;

        public PokemonService(IApiClient apiClient, IConfiguration configuration, IPokemonBuilder pokemonBuilder)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _pokemonBuilder = pokemonBuilder;
        }

        public async Task<Pokemon> GetAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new EntryNameNullException();
            }

            var response = await _apiClient.GetAsync<PokemonSpecies>(GetPokeBaseUrl(name));

            return _pokemonBuilder.Init(response)
                .WithName()
                .WithHabitat()
                .WithIsLegendary()
                .WithDescription(_configuration["Pokedex:DescriptionLanguage"])
                .Build();
        }

        public async Task<Pokemon> GetWithTranslationAsync(string name)
        {
            var pokemon = await GetAsync(name);
            return await TranslateDescription(pokemon);

        }
        private async Task<Pokemon> TranslateDescription(Pokemon pokemon)
        {
            try
            {
                if (pokemon.Habitat == "cave" || (pokemon.IsLegendary.HasValue && pokemon.IsLegendary.Value))
                {
                    pokemon.Description = await _apiClient.GetAsync<string>(GetYodaBaseUrl(pokemon.Description));
                }
                else
                {
                    pokemon.Description = await _apiClient.GetAsync<string>(GetShakespeareBaseUrl(pokemon.Description)); ;

                }
            }
            catch
            {
            }
            return pokemon;
        }

        private string GetPokeBaseUrl(string name)
        {
            return _configuration["Pokedex:PokeBaseUrl"].Replace("{NAME}", name);
        }

        private string GetYodaBaseUrl(string description)
        {
            return _configuration["Pokedex:YodaUrl"].Replace("{DESCRIPTION}", description);
        }

        private string GetShakespeareBaseUrl(string description)
        {
            return _configuration["Pokedex:ShakespeareUrl"].Replace("{DESCRIPTION}", description);
        }
    }
}
