using Microsoft.Extensions.Configuration;
using Pokedex.Exceptions;
using Pokedex.Services.ApiClient;
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

        public PokemonService(IApiClient apiClient, IConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
        }

        public async Task<Pokemon> GetAsync(string name)
        {
            if(string.IsNullOrEmpty(name))
            {
                throw new PokemonNameNullException();
            }

            var response = await _apiClient.GetAsync<PokemonSpecies>(GetPokeBaseUrl(name));
            if(response == null)
            {
                throw new NullResponseException();
            }

            var description = response.FlavorTextEntries?
                .FirstOrDefault(e => e.Language?.Name?.ToLowerInvariant() == "en")?.FlavorText;

            return new Pokemon()
            {
                Description = description,
                Habitat = response.Habitat.Name,
                Name = response.Name,
                IsLegendary = response.IsLegendary
            };
        }

        private string GetPokeBaseUrl(string name)
        {
            return _configuration["Pokedex:PokeBaseUrl"].Replace("{NAME}" , name);
        }
    }
}
