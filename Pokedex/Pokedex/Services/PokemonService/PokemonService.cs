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
        private PokemonBuilder.PokemonBuilder _pokemonBuilder;

        public PokemonService(IApiClient apiClient, IConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _pokemonBuilder = new PokemonBuilder.PokemonBuilder();
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

        private string GetPokeBaseUrl(string name)
        {
            return _configuration["Pokedex:PokeBaseUrl"].Replace("{NAME}", name);
        }
    }
}
