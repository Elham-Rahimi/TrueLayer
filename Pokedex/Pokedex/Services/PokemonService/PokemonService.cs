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
        private IPokemonBuilder _pokemonBuilder;

        public PokemonService(IPokemonBuilder pokemonBuilder)
        {
            _pokemonBuilder = pokemonBuilder;
        }

        public async Task<Pokemon> GetAsync(string name)
        {
            return (await _pokemonBuilder.Init(name))
                .WithName()
                .WithHabitat()
                .WithIsLegendary()
                .WithDescription()
                .Build();
        }

        public async Task<Pokemon> GetWithTranslationAsync(string name)
        {
            return await (await _pokemonBuilder.Init(name))
                .WithName()
                .WithHabitat()
                .WithIsLegendary()
                .WithDescription()
                .BuildTranslatedAsync();
        }
    }
}
