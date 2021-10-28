using Pokedex.Services.PokemonService.Contracts;
using System.Threading.Tasks;

namespace Pokedex.Services.PokemonBuilder
{
    public interface IPokemonBuilder
    {
        Pokemon Build();
        Task<PokemonBuilder> Init(string name);
        PokemonBuilder WithName();
        PokemonBuilder WithHabitat();
        PokemonBuilder WithIsLegendary();
        PokemonBuilder WithDescription();
        Task<Pokemon> BuildTranslatedAsync();
    }
}
