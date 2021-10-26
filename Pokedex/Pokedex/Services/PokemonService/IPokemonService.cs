using Pokedex.Services.PokemonBuilder;
using System.Threading.Tasks;

namespace Pokedex.Services.PokemonService
{
    public interface IPokemonService
    {
        Task<Pokemon> GetAsync(string name);
    }
}
