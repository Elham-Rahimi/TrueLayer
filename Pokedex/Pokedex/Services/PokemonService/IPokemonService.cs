using Pokedex.Services.PokemonService.Contracts;
using System.Threading.Tasks;

namespace Pokedex.Services.PokemonService
{
    public interface IPokemonService
    {
        Task<Pokemon> GetAsync(string name);
    }
}
